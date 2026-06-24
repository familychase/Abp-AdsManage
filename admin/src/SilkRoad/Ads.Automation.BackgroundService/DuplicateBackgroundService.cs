using Ads.Automation.Application.Contracts.Entity.Duplicate;
using Ads.Automation.Domain.Duplicate;
using Ads.Automation.Infrastructure.RabbitMq.Interfaces;

namespace Ads.Automation.BackgroundService;

/// <summary>
/// 复制任务调度器 —— 每 1 分钟扫描 PENDING 记录，推入 RabbitMQ
/// 推送前的限流/延迟由 DB 中的 ScheduleTime 和 slot 控制
/// 实际执行由 SyncJobService 的 DuplicateJobHandler 消费 MQ 完成
/// </summary>
public class DuplicateBackgroundService : BaseBackgroundTaskService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IJobQueue _queue;

    private const int MaxConcurrent = 200;

    public DuplicateBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<DuplicateBackgroundService> logger,
        IJobQueue queue)
        : base(logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
    }

    protected override TimeSpan Interval => TimeSpan.FromSeconds(15);

    protected override async Task InternalExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        using var uow = uowManager.Begin(requiresNew: true);

        var repo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsDuplicateLogging>>();

        var query = await repo.GetQueryableAsync();
        var inProgressCount = await query.CountAsync(l => l.State == DuplicateState.IN_PROGRESS, stoppingToken);
        var availableSlots = MaxConcurrent - inProgressCount;

        if (availableSlots <= 0)
            return;

        var now = DateTime.Now;
        var pending = await query
            .Where(l => l.State == DuplicateState.PENDING && l.ScheduleTime <= now)
            .Take(availableSlots)
            .ToListAsync(stoppingToken);

        if (pending.Count == 0)
            return;

        // 批量标记 IN_PROGRESS
        foreach (var log in pending)
        {
            log.SetState(DuplicateState.IN_PROGRESS);
            await repo.UpdateAsync(log, autoSave: false);
        }

        // 推入 MQ，由 SyncJobService 消费执行
        foreach (var log in pending)
        {
            _queue.Enqueue(new DuplicateTaskMessage { LogId = log.Id });
        }

        await uow.CompleteAsync();

        Logger.LogInformation("复制任务调度完成: 推送 {Count} 条到 MQ（可用槽位 {Slots}）",
            pending.Count, availableSlots);
    }
}
