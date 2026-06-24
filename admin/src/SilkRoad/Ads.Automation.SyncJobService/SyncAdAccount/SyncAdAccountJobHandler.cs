using Ads.Automation.Application.Entity.MetaOAuth;
using Ads.Automation.Domain.SyncSchedule;
using Ads.Automation.Domain.Shared.Enums;
using Ads.Automation.Infrastructure.Caching.Interfaces;
using static Ads.Automation.Infrastructure.SDK.Models.Meta.Domain.MetaDomain;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 广告账户同步 RabbitMQ 消费者 —— 按 ChannelId 拉取 Meta 广告账户并同步到 DB
/// 同步完成后为每个新增账户插入同步数据计划（像素/广告结构/报表/受众包）
/// </summary>
public class SyncAdAccountJobHandler : JobHandlerBase<SyncAdAccountJobArgs>
{
    private const int FirstRunJitterMaxSeconds = 120;

    public SyncAdAccountJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<SyncAdAccountJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
    }

    protected override TimeSpan LockExpiration => TimeSpan.FromMinutes(10);

    protected override string BuildLockKey(SyncAdAccountJobArgs args)
        => $"account_sync:{args.ChannelId}";

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, SyncAdAccountJobArgs args, CancellationToken ct)
    {
        var uowManager = sp.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin(requiresNew: true);

        var channelRepo = sp.GetRequiredService<IBaseRepository<AdsChannel>>();
        var accountRepo = sp.GetRequiredService<IBaseRepository<AdsAccount>>();
        var channelAdAccountRepo = sp.GetRequiredService<IBaseRepository<AdsChannelAdAccount>>();
        var asyncExecuter = sp.GetRequiredService<IAsyncQueryableExecuter>();
        var metaAuth = sp.GetRequiredService<IMetaAuthorizationGateway>();
        var retry = sp.GetRequiredService<MetaApiRetryPolicy>();
        var logger = sp.GetRequiredService<ILogger<SyncAdAccountJobHandler>>();

        var channel = await channelRepo.GetAsync(args.ChannelId, cancellationToken: ct);
        logger.LogInformation("开始同步渠道 {ChannelId} 的广告账户", channel.Id);

        var identity = await metaAuth.GetAccessIdentityAsync(channel.Id);
        ct.ThrowIfCancellationRequested();

        var bmAccounts = new List<AdAccount>();
        var result = await retry.ExecuteAsync(
            () => MetaOpenApi.GetAdAccountsAsync(identity, 500, MetaConst.AccountFields), "广告账户同步");

        if (result?.data != null && result.data.Count > 0)
        {
            bmAccounts.AddRange(result.data);
            if (result.paging != null && !result.paging.next.IsNullOrWhiteSpace())
            {
                var restAccounts = await MetaOpenApi.PagingAllAsync<AdAccount>(identity, result.paging.next);
                bmAccounts.AddRange(restAccounts);
            }
        }

        if (bmAccounts.Count == 0)
        {
            logger.LogInformation("渠道 {ChannelId} 无广告账户数据", channel.Id);
            await uow.CompleteAsync();
            return;
        }

        // 查询已存在的账户和关联关系
        var accountIds = bmAccounts.Select(a => a.account_id).Distinct().ToList();
        var accountQuery = await accountRepo.GetQueryableAsync();
        var existingAccounts = await asyncExecuter.ToListAsync(
            accountQuery.Where(e => accountIds.Contains(e.AccountNo)));
        var accountMap = existingAccounts
            .GroupBy(a => a.AccountNo).ToDictionary(g => g.Key, g => g.Last());

        var channelAdAccountQuery = await channelAdAccountRepo.GetQueryableAsync();
        var existingRelations = await asyncExecuter.ToListAsync(
            channelAdAccountQuery.Where(r => r.ChannelId == channel.Id));
        var relationMap = existingRelations
            .GroupBy(r => r.AccountNo).ToDictionary(g => g.Key, g => g.Last());

        var accountByAccountNo = new Dictionary<string, AdsAccount>();
        var newAccountNos = new List<string>();
        var totalCount = 0;

        foreach (var metaAccount in bmAccounts)
        {
            ct.ThrowIfCancellationRequested();

            if (accountMap.TryGetValue(metaAccount.account_id, out var existing))
            {
                existing.SetAccountName(metaAccount.name ?? string.Empty);
                existing.SetAccountState(metaAccount.account_status == 1
                    ? AdAccountState.NORMAL : AdAccountState.ABNORMAL);
                existing.SetBalance(decimal.TryParse(metaAccount.balance, out var balance) ? balance : 0);
                existing.SetTimezone(metaAccount.timezone_name);
                existing.SetUtcTimezoneOffset(metaAccount.timezone_offset_hours_utc.ToString());
                existing.SetCurrency(metaAccount.currency);
                existing.SetMediaDisableReason(metaAccount.disable_reason.ToString());
                existing.SetMediaCreatedTime(metaAccount.created_time.IsNullOrEmpty()
                    ? DateTime.MinValue : DateTime.Parse(metaAccount.created_time));
                existing.SetAccountRunningTime(metaAccount.age ?? 0);
                existing.LastModificationTime = DateTime.Now;
                await accountRepo.UpdateAsync(existing);
                accountByAccountNo[metaAccount.account_id] = existing;
            }
            else
            {
                var account = AdsAccount.Create(
                    metaAccount.account_id, metaAccount.name ?? string.Empty,
                    metaAccount.account_status == 1 ? AdAccountState.NORMAL : AdAccountState.ABNORMAL,
                    metaAccount.account_status.ToString(),
                    decimal.TryParse(metaAccount.balance, out var newBalance) ? newBalance : 0,
                    metaAccount.timezone_name, metaAccount.timezone_offset_hours_utc.ToString(),
                    channel.Platform, channel.CreatorId, 0, channel.IsManager,
                    metaAccount.currency, false, metaAccount.disable_reason.ToString(),
                    metaAccount.created_time.IsNullOrEmpty()
                        ? DateTime.MinValue : DateTime.Parse(metaAccount.created_time),
                    metaAccount.age ?? 0);

                await accountRepo.InsertAsync(account);
                accountByAccountNo[metaAccount.account_id] = account;
                newAccountNos.Add(metaAccount.account_id);
            }

            totalCount++;
        }

        // ===== 渠道-账户关联关系维护 =====
        var syncedAccountNos = new HashSet<string>(bmAccounts.Select(a => a.account_id));
        var correctAccountIdByAccountNo = accountByAccountNo.ToDictionary(kv => kv.Key, kv => kv.Value.Id);

        // 1. 删除 AccountId 不匹配的旧关联
        var staleRelations = existingRelations
            .Where(r => syncedAccountNos.Contains(r.AccountNo)
                     && (!correctAccountIdByAccountNo.TryGetValue(r.AccountNo, out var correctId)
                         || r.AccountId != correctId)).ToList();
        foreach (var relation in staleRelations)
            await channelAdAccountRepo.DeleteAsync(relation);

        // 2. 新建缺失的关联
        var existingAccountNosWithCorrectId = existingRelations
            .Where(r => syncedAccountNos.Contains(r.AccountNo)
                     && correctAccountIdByAccountNo.TryGetValue(r.AccountNo, out var cid)
                     && r.AccountId == cid)
            .Select(r => r.AccountNo).ToHashSet();

        var insertedRelationCount = 0;
        foreach (var metaAccount in bmAccounts)
        {
            ct.ThrowIfCancellationRequested();
            if (!existingAccountNosWithCorrectId.Contains(metaAccount.account_id))
            {
                await channelAdAccountRepo.InsertAsync(new AdsChannelAdAccount
                {
                    AccountId = correctAccountIdByAccountNo[metaAccount.account_id],
                    ChannelId = channel.Id,
                    AccountNo = metaAccount.account_id
                });
                insertedRelationCount++;
            }
        }

        // 3. 删除不再存在的关联
        var relationsToDelete = existingRelations
            .Where(r => !syncedAccountNos.Contains(r.AccountNo)).ToList();
        foreach (var relation in relationsToDelete)
            await channelAdAccountRepo.DeleteAsync(relation);

        await uow.CompleteAsync();

        logger.LogInformation(
            "渠道 {ChannelId} 广告账户同步完成，共 {Count} 条，新增关联 {InsertedCount} 条，清理旧关联 {StaleCount} 条，移除关联 {RemovedCount} 条",
            channel.Id, totalCount, insertedRelationCount, staleRelations.Count, relationsToDelete.Count);

        // ===== 仅为新增账户插入同步数据计划 =====
        if (newAccountNos.Count > 0)
        {
            await InsertSyncSchedulesAsync(sp, channel, newAccountNos, ct, logger);
        }
    }

    // ==================== 同步数据计划插入 ====================

    private static async Task InsertSyncSchedulesAsync(
        IServiceProvider sp,
        AdsChannel channel,
        List<string> accountNos,
        CancellationToken ct,
        ILogger logger)
    {
        var uowManager = sp.GetRequiredService<IUnitOfWorkManager>();
        var scheduleRepo = sp.GetRequiredService<IBaseRepository<AdsSyncSchedule>>();

        using var uow = uowManager.Begin(requiresNew: true);

        var now = DateTime.Now;
        var insertedCount = 0;

        foreach (var accountNo in accountNos)
        {
            ct.ThrowIfCancellationRequested();

            // 同步像素：6 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdPixelJobArgs), 21600, now));
            insertedCount++;

            // 同步广告结构：3 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdCampaignJobArgs), 10800, now));
            insertedCount++;

            // 同步报表 - 昨天：24 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdReportJobArgs), 86400, now, linkDate: "yesterday"));
            insertedCount++;

            // 同步报表 - 当日：10 分钟间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdReportJobArgs), 600, now, linkDate: "today"));
            insertedCount++;

            // 同步受众包：12 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdAudienceJobArgs), 43200, now));
            insertedCount++;

            // 同步渠道主页 - 6 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdPageJobArgs), 21600, now));
            insertedCount++;

            // 同步账户主页 - 6 小时间隔
            await scheduleRepo.InsertAsync(BuildSchedule(
                channel.Platform, accountNo, nameof(SyncAdPageAccountJobArgs), 21600, now));
            insertedCount++;
        }

        await uow.CompleteAsync();
        logger.LogInformation("渠道 {ChannelId} 新增 {Count} 个账户，已插入 {InsertedCount} 条同步数据计划",
            channel.Id, accountNos.Count, insertedCount);
    }

    private static AdsSyncSchedule BuildSchedule(
        PlatformType platform, string resourceId, string jobName,
        int intervalSeconds, DateTime now, string? linkDate = null)
    {
        var jitter = TimeSpan.FromSeconds(Random.Shared.Next(FirstRunJitterMaxSeconds));
        var nextTime = now.AddSeconds(10).Add(jitter);

        return AdsSyncSchedule.Create(
            actionType: ActionType.AUTOMATIC,
            resourceId: resourceId,
            resourceType: ResourceType.AD_ACCOUNT,
            platform: platform,
            jobName: jobName,
            extendingData: $"{{\"IntervalSeconds\": {intervalSeconds}}}",
            linkDate: linkDate,
            nextPublishTime: nextTime);
    }
}
