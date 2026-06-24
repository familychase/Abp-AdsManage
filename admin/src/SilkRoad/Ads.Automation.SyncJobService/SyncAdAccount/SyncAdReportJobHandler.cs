using Ads.Automation.Application.Contracts.Statistic;
using Ads.Automation.Application.Statistic;
using Ads.Automation.Domain.Shared.Models;
using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 广告报表同步 RabbitMQ 消费者
/// 核心流程：活跃检查（7天消耗缓存） → 分布式锁 → 按天同步基础报表
/// </summary>
public class SyncAdReportJobHandler : JobHandlerBase<SyncAdReportJobArgs>
{
    private readonly ICacheService _cacheService;

    private static readonly TimeSpan ActiveCacheDuration = TimeSpan.FromDays(1);
    private static readonly TimeSpan InactiveCacheBaseDuration = TimeSpan.FromHours(2);

    public SyncAdReportJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ICacheService cacheService,
        ILogger<SyncAdReportJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
        _cacheService = cacheService;
    }

    protected override string BuildLockKey(SyncAdReportJobArgs message)
    {
        var reportDate = DateTime.TryParse(message.ReportDate, out var date) ? date : DateTime.Today;
        return $"report_sync:{message.AccountNo}:{reportDate:yyyyMMdd}";
    }

    /// <summary>锁前检查：近7天无消耗的账户跳过同步（带缓存）</summary>
    protected override async Task<bool> BeforeLockAsync(SyncAdReportJobArgs message, CancellationToken ct)
    {
        return await CheckAccountActiveAsync(message.AccountNo);
    }

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, SyncAdReportJobArgs message, CancellationToken ct)
    {
        var accountNo = message.AccountNo;
        var reportDate = DateTime.TryParse(message.ReportDate, out var date) ? date : DateTime.Today;

        var reportingFactory = sp.GetRequiredService<SyncReportingFactory>();
        var reportingService = reportingFactory.Get(PlatformType.META);

        var syncRange = new DateRange { Start = reportDate, Stop = reportDate };
        await reportingService.SyncAdBaseReporting(accountNo, syncRange);

        var logger = sp.GetRequiredService<ILogger<SyncAdReportJobHandler>>();
        logger.LogInformation("账户 {AccountNo} 报表同步完成（日期={ReportDate}）",
            accountNo, reportDate.ToString("yyyy-MM-dd"));
    }

    // ==================== 账户活跃检查 ====================

    private async Task<bool> CheckAccountActiveAsync(string accountNo)
    {
        var activeCacheKey = $"account:active:{PlatformType.META}:{accountNo}";
        var inactiveLockKey = $"account:inactive:{PlatformType.META}:{accountNo}";

        var cached = await _cacheService.GetAsync<bool?>(activeCacheKey);
        if (cached != null)
            return cached.Value;

        // 无缓存 → 查询近30天消耗（独立 Scope）
        using var scope = ScopeFactory.CreateScope();
        var reportingFactory = scope.ServiceProvider.GetRequiredService<SyncReportingFactory>();
        var reportingService = reportingFactory.Get(PlatformType.META);

        var recentRange = new DateRange { Start = DateTime.Now.AddDays(-30), Stop = DateTime.Now };
        var isActive = await reportingService.SyncGetRecentCost(accountNo, recentRange);

        if (isActive)
        {
            await _cacheService.SetAsync(activeCacheKey, true, ActiveCacheDuration);
            await _cacheService.RemoveAsync(inactiveLockKey);
        }
        else
        {
            var jitter = TimeSpan.FromMinutes(Random.Shared.Next(0, 30));
            await _cacheService.SetAsync(activeCacheKey, false, InactiveCacheBaseDuration + jitter);
            await _cacheService.SetAsync(inactiveLockKey, "1", TimeSpan.FromDays(1));
        }

        return isActive;
    }
}
