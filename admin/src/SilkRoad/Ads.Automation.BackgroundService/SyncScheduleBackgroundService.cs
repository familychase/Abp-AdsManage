using System.Text.Json;
using Ads.Automation.Application.Contracts.IntegrationJobs;
using Ads.Automation.Domain.SyncSchedule;
using Ads.Automation.Infrastructure.RabbitMq.Interfaces;

namespace Ads.Automation.BackgroundService;

/// <summary>
/// 同步计划调度器 —— 每 1 分钟扫描 ads_sync_schedule 表，
/// 将 NextPublishTime 到期的记录按 jobName 推送到 MQ
/// </summary>
public class SyncScheduleBackgroundService : BaseBackgroundTaskService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IJobQueue _jobQueue;

    private const int BatchSize = 200;
    private const int JitterMaxSeconds = 300;
    private static readonly TimeSpan DefaultInterval = TimeSpan.FromHours(1);

    public SyncScheduleBackgroundService(
        IServiceScopeFactory scopeFactory,
        IJobQueue jobQueue,
        ILogger<SyncScheduleBackgroundService> logger)
        : base(logger)
    {
        _scopeFactory = scopeFactory;
        _jobQueue = jobQueue;
    }

    protected override TimeSpan Interval => TimeSpan.FromMinutes(1);

    protected override async Task InternalExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var sp = scope.ServiceProvider;
        var uowManager = sp.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin(requiresNew: true);

        var repo = sp.GetRequiredService<IBaseRepository<AdsSyncSchedule>>();
        var query = await repo.GetQueryableAsync();
        var now = DateTime.Now;

        var pending = await query
            .Where(s => s.NextPublishTime <= now)
            .Take(BatchSize)
            .ToListAsync(stoppingToken);

        if (pending.Count == 0)
            return;

        foreach (var schedule in pending)
        {
            var pushed = false;

            switch (schedule.JobName)
            {
                case nameof(SyncAdAccountJobArgs):
                    _jobQueue.Enqueue(new SyncAdAccountJobArgs { ChannelId = long.Parse(schedule.ResourceId) });
                    pushed = true;
                    break;

                case nameof(SyncAdPageJobArgs):
                    _jobQueue.Enqueue(new SyncAdPageJobArgs { ChannelId = long.Parse(schedule.ResourceId) });
                    pushed = true;
                    break;

                case nameof(SyncAdPixelJobArgs):
                    _jobQueue.Enqueue(new SyncAdPixelJobArgs { AccountNo = schedule.ResourceId });
                    pushed = true;
                    break;

                case nameof(SyncAdCampaignJobArgs):
                    _jobQueue.Enqueue(new SyncAdCampaignJobArgs { AccountNo = schedule.ResourceId });
                    pushed = true;
                    break;

                case nameof(SyncAdReportJobArgs):
                    _jobQueue.Enqueue(new SyncAdReportJobArgs
                    {
                        AccountNo = schedule.ResourceId,
                        DataType = schedule.LinkDate,
                        ReportDate = SyncAdReportJobArgs.Resolve(schedule.LinkDate)
                    });
                    pushed = true;
                    break;

                case nameof(SyncAdAudienceJobArgs):
                    _jobQueue.Enqueue(new SyncAdAudienceJobArgs { AccountNo = schedule.ResourceId });
                    pushed = true;
                    break;

                default:
                    Logger.LogWarning("同步计划 {ScheduleId} 的 JobName={JobName} 未匹配，跳过",
                        schedule.Id, schedule.JobName);
                    break;
            }

            if (pushed)
            {
                var interval = ParseInterval(schedule.ExtendingData);
                schedule.SetNextPublishTime(now + interval);
                await repo.UpdateAsync(schedule);
            }
        }

        await uow.CompleteAsync();

        Logger.LogInformation("同步计划调度完成: 推送 {Count} 条", pending.Count);
    }

    private static TimeSpan ParseInterval(string? extendingData)
    {
        TimeSpan baseInterval;

        if (string.IsNullOrWhiteSpace(extendingData))
        {
            baseInterval = DefaultInterval;
        }
        else
        {
            try
            {
                using var doc = JsonDocument.Parse(extendingData);
                if (doc.RootElement.TryGetProperty("IntervalSeconds", out var prop) && prop.TryGetInt32(out var seconds))
                    baseInterval = TimeSpan.FromSeconds(seconds);
                else
                    baseInterval = DefaultInterval;
            }
            catch (JsonException)
            {
                baseInterval = DefaultInterval;
            }
        }

        var jitter = TimeSpan.FromSeconds(Random.Shared.Next(JitterMaxSeconds));
        return baseInterval + jitter;
    }
}
