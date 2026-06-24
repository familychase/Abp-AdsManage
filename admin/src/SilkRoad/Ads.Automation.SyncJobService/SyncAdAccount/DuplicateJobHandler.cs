using Ads.Automation.Application.Contracts.Entity.Duplicate;
using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 复制任务 RabbitMQ 消费者
/// 锁保护和重试由 <see cref="JobHandlerBase{DuplicateTaskMessage}"/> 统一处理
/// </summary>
public class DuplicateJobHandler : JobHandlerBase<DuplicateTaskMessage>
{
    public DuplicateJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<DuplicateJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
    }

    protected override string BuildLockKey(DuplicateTaskMessage message)
        => $"duplicate:{message.LogId}";

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, DuplicateTaskMessage message, CancellationToken ct)
    {
        var processor = sp.GetRequiredService<IDuplicateBatchProcessor>();
        await processor.ProcessSingleAsync(message.LogId);
    }
}
