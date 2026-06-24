using Ads.Automation.Application.Entity.MetaOAuth;
using Ads.Automation.Domain.Channel;
using Ads.Automation.Domain.Page;
using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 公共主页同步 RabbitMQ 消费者 —— 按 ChannelId 拉取 Meta 公共主页并同步到 ads_page 表
/// </summary>
public class SyncAdPageJobHandler : JobHandlerBase<SyncAdPageJobArgs>
{
    public SyncAdPageJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<SyncAdPageJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
    }

    protected override string BuildLockKey(SyncAdPageJobArgs args)
        => $"page_sync:{args.ChannelId}";

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, SyncAdPageJobArgs args, CancellationToken ct)
    {
        var uowManager = sp.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin(requiresNew: true);

        var channelRepo = sp.GetRequiredService<IBaseRepository<AdsChannel>>();
        var pageRepo = sp.GetRequiredService<IBaseRepository<AdsPage>>();
        var channelPageRepo = sp.GetRequiredService<IBaseRepository<AdsChannelPage>>();
        var asyncExecuter = sp.GetRequiredService<IAsyncQueryableExecuter>();
        var metaAuth = sp.GetRequiredService<IMetaAuthorizationGateway>();
        var retry = sp.GetRequiredService<MetaApiRetryPolicy>();
        var logger = sp.GetRequiredService<ILogger<SyncAdPageJobHandler>>();

        var channel = await channelRepo.GetAsync(args.ChannelId, cancellationToken: ct);
        logger.LogInformation("开始同步渠道 {ChannelId} 的主页", channel.Id);

        var identity = await metaAuth.GetAccessIdentityAsync(channel.Id);
        ct.ThrowIfCancellationRequested();

        var pageQuery = await pageRepo.GetQueryableAsync();
        string? after = null;
        var pageByPageNo = new Dictionary<string, AdsPage>();
        var totalCount = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var pages = await retry.ExecuteAsync(
                () => MetaOpenApi.GetUserPagesAsync(identity, 500, MetaConst.PageFields, after), "主页同步");
            if (pages?.data == null || pages.data.Count == 0) break;

            var pageIds = pages.data.Where(a => a != null).Select(a => a.id).Distinct().ToList();
            var existingPages = await asyncExecuter.ToListAsync(
                pageQuery.Where(e => pageIds.Contains(e.PageNo)));
            var pageMap = existingPages.ToDictionary(e => e.PageNo);

            foreach (var metaPage in pages.data)
            {
                ct.ThrowIfCancellationRequested();

                if (pageMap.TryGetValue(metaPage.id, out var existing))
                {
                    existing.SetPageName(metaPage.name ?? string.Empty);
                    existing.SetCategory(metaPage.category);
                    existing.SetLastSyncTime(DateTime.Now);
                    await pageRepo.UpdateAsync(existing);
                    pageByPageNo[metaPage.id] = existing;
                }
                else
                {
                    var page = AdsPage.Create(metaPage.id, metaPage.name ?? string.Empty,
                        metaPage.category, channel.ManagerId ?? string.Empty, channel.Platform);
                    await pageRepo.InsertAsync(page);
                    pageByPageNo[metaPage.id] = page;
                }

                totalCount++;
            }

            var next = pages.paging?.cursors?.after;
            if (string.IsNullOrEmpty(next)) break;
            after = next;
        }

        // ===== 维护渠道-主页关联关系 =====
        var channelPageQuery = await channelPageRepo.GetQueryableAsync();
        var existingRelations = await asyncExecuter.ToListAsync(
            channelPageQuery.Where(r => r.ChannelId == channel.Id));
        var relationMap = existingRelations.ToDictionary(r => r.PageNo);

        var syncedPageNos = new HashSet<string>(pageByPageNo.Keys);
        var insertedRelationCount = 0;

        foreach (var pageNo in syncedPageNos)
        {
            ct.ThrowIfCancellationRequested();
            if (!relationMap.ContainsKey(pageNo))
            {
                await channelPageRepo.InsertAsync(new AdsChannelPage
                {
                    ChannelId = channel.Id,
                    PageId = pageByPageNo[pageNo].Id,
                    PageNo = pageNo
                });
                insertedRelationCount++;
            }
        }

        var relationsToDelete = existingRelations
            .Where(r => !syncedPageNos.Contains(r.PageNo)).ToList();
        foreach (var r in relationsToDelete)
            await channelPageRepo.DeleteAsync(r);

        await uow.CompleteAsync();

        logger.LogInformation(
            "渠道 {ChannelId} 主页同步完成，共 {Count} 条，新增关联 {InsertedCount} 条，移除关联 {RemovedCount} 条",
            channel.Id, totalCount, insertedRelationCount, relationsToDelete.Count);
    }
}
