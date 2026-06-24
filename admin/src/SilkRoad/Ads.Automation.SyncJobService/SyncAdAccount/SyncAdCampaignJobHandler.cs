using Ads.Automation.Application.Entity.Campaign;
using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 广告结构（广告系列/广告组/广告）同步 RabbitMQ 消费者
/// 锁管理和重试由 <see cref="JobHandlerBase{SyncAdCampaignJobArgs}"/> 统一处理
/// </summary>
public class SyncAdCampaignJobHandler : JobHandlerBase<SyncAdCampaignJobArgs>
{
    public SyncAdCampaignJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<SyncAdCampaignJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
    }

    protected override string BuildLockKey(SyncAdCampaignJobArgs args)
        => $"campaign_sync:{args.AccountNo}";

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, SyncAdCampaignJobArgs args, CancellationToken ct)
    {
        var syncService = sp.GetRequiredService<IAdStructureSyncService>();
        var (campaignCount, adSetCount, adCount) = await syncService.SyncAsync(args.AccountNo, ct);

        var logger = sp.GetRequiredService<ILogger<SyncAdCampaignJobHandler>>();
        logger.LogInformation(
            "账户 {AccountNo} 广告结构同步完成：系列 {CampaignCount} 条，广告组 {AdSetCount} 条，广告 {AdCount} 条",
            args.AccountNo, campaignCount, adSetCount, adCount);
    }
}
