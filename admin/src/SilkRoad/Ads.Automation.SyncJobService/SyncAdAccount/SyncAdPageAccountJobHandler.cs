using Ads.Automation.Application.Entity.MetaOAuth;
using Ads.Automation.Domain.Account;
using Ads.Automation.Domain.Channel;
using Ads.Automation.Domain.Page;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 同步单个账号下的个号主页 RabbitMQ 消费者 —— 收到消息后按 AccountId 拉取 Meta 公共主页并同步到 ads_page 表
/// </summary>
public class SyncAdPageAccountJobHandler : RabbitMqConsumerBase<SyncAdPageAccountJobArgs>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedLock _distributedLock;
    private readonly ILogger<SyncAdPageAccountJobHandler> _logger;
    private readonly MetaApiRetryPolicy _retry;

    /// <summary>
    /// 主页同步锁过期时间（10分钟，防止死锁后无法继续）
    /// </summary>
    private static readonly TimeSpan LockExpiration = TimeSpan.FromMinutes(10);

    public SyncAdPageAccountJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<SyncAdPageAccountJobHandler> logger,
        MetaApiRetryPolicy retry)
        : base(consumer, logger)
    {
        _scopeFactory = scopeFactory;
        _distributedLock = distributedLock;
        _logger = logger;
        _retry = retry;
    }

    /// <summary>
    /// 处理单条主页同步消息
    /// </summary>
    protected override async Task<bool> HandleAsync(SyncAdPageAccountJobArgs args, CancellationToken cancellationToken)
    {
        var lockKey = $"page_account_sync:{args.AccountId}";
        if (!await _distributedLock.AcquireAsync(lockKey, LockExpiration))
        {
            _logger.LogInformation("账户 {AccountId} 主页同步任务正在执行中，跳过本次消费", args.AccountId);
            return true; // ACK：锁冲突不是业务错误，正常跳过
        }

        var totalCount = 0;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            using var uow = uowManager.Begin(requiresNew: true);

            var accountRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsAccount>>();
            var channelAdAccountRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsChannelAdAccount>>();
            var channelRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsChannel>>();
            var pageRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsPage>>();
            var asyncExecuter = scope.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();
            var metaAuth = scope.ServiceProvider.GetRequiredService<IMetaAuthorizationGateway>();

            var account = await accountRepo.GetAsync(args.AccountId, cancellationToken: cancellationToken);

            _logger.LogInformation("开始同步账户 {AccountId} 的主页", account.Id);

            // 通过 AdsChannelAdAccount 关联表查找账户所属的渠道
            var channelAdAccountQuery = await channelAdAccountRepo.GetQueryableAsync();
            var channelRelations = await asyncExecuter.ToListAsync(
                channelAdAccountQuery.Where(r => r.AccountId == account.Id));

            var channelRelation = channelRelations.FirstOrDefault();
            if (channelRelation == null)
            {
                _logger.LogWarning("账户 {AccountId} 未关联任何渠道，跳过主页同步", account.Id);
                await uow.CompleteAsync();
                return true;
            }

            var channel = await channelRepo.GetAsync(channelRelation.ChannelId, cancellationToken: cancellationToken);

            var identity = await metaAuth.GetAccessIdentityAsync(channel.Id);
            cancellationToken.ThrowIfCancellationRequested();

            var accountPageRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsAccountPages>>();
            var pageQuery = await pageRepo.GetQueryableAsync();
            string? after = null;

            // 记录本次同步的所有主页 PageNo → AdsPage 映射，用于后续关联关系同步
            var pageByPageNo = new Dictionary<string, AdsPage>();

            // 游标分页循环，直到全部拉取完成
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pages = await _retry.ExecuteAsync(() => MetaOpenApi.GetUserPagesAsync(identity, 500, MetaConst.PageFields, after), "主页同步");
                if (pages?.data == null || pages.data.Count == 0)
                    break;

                var pageIds = pages.data.Where(a => a != null).Select(a => a.id).Distinct().ToList();
                var existingPages = await asyncExecuter.ToListAsync(
                    pageQuery.Where(e => pageIds.Contains(e.PageNo)));

                // Dictionary 索引化，O(1) 查找替代 O(n) 遍历
                var pageMap = existingPages.ToDictionary(e => e.PageNo);

                foreach (var metaPage in pages.data)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (pageMap.TryGetValue(metaPage.id, out var existing))
                    {
                        // 更新已有主页
                        existing.SetPageName(metaPage.name ?? string.Empty);
                        existing.SetCategory(metaPage.category);
                        existing.SetLastSyncTime(DateTime.Now);
                        await pageRepo.UpdateAsync(existing);

                        pageByPageNo[metaPage.id] = existing;
                    }
                    else
                    {
                        // 创建新主页
                        var page = AdsPage.Create(
                            metaPage.id,
                            metaPage.name ?? string.Empty,
                            metaPage.category,
                            channel.ManagerId ?? string.Empty,
                            channel.Platform);

                        await pageRepo.InsertAsync(page);
                        pageByPageNo[metaPage.id] = page;
                    }

                    totalCount++;
                }

                // 检查是否还有下一页
                var next = pages.paging?.cursors?.after;
                if (string.IsNullOrEmpty(next))
                    break;

                after = next;
            }

            // ===== 同步账号-主页关联关系 =====

            var accountPageQuery = await accountPageRepo.GetQueryableAsync();
            var existingRelations = await asyncExecuter.ToListAsync(
                accountPageQuery.Where(r => r.AccountId == account.Id));
            var relationMap = existingRelations.ToDictionary(r => r.PageId);

            // 1. 新增不存在的关联关系
            var insertedRelationCount = 0;
            var syncedPageIds = new HashSet<long>(pageByPageNo.Values.Select(p => p.Id));
            foreach (var pageNo in pageByPageNo.Keys)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var pageId = pageByPageNo[pageNo].Id;
                if (!relationMap.ContainsKey(pageId))
                {
                    var relation = AdsAccountPages.Create(account.Id, pageId, account.AccountNo ?? string.Empty);
                    await accountPageRepo.InsertAsync(relation);
                    insertedRelationCount++;
                }
            }

            // 2. 删除不再存在的关联关系
            var relationsToDelete = existingRelations
                .Where(r => !syncedPageIds.Contains(r.PageId)).ToList();
            foreach (var relation in relationsToDelete)
            {
                await accountPageRepo.DeleteAsync(relation);
            }

            await uow.CompleteAsync();

            _logger.LogInformation(
                "账户 {AccountId} 主页同步完成，共 {Count} 条，新增关联 {InsertedCount} 条，移除关联 {RemovedCount} 条",
                account.Id, totalCount, insertedRelationCount, relationsToDelete.Count);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "账户 {AccountId} 主页同步失败，已同步 {Count} 条",
                args.AccountId, totalCount);
            return false; // NACK → 进入重试队列
        }
        finally
        {
            await _distributedLock.ReleaseAsync(lockKey);
        }

        return true;
    }
}
