namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// 任务队列实现 —— Enqueue 立即返回，底层走 MessageBus 后台异步推送
/// </summary>
public class JobQueue : IJobQueue
{
    private readonly IMessageBus _bus;
    private readonly ILogger<JobQueue> _logger;

    public JobQueue(IMessageBus bus, ILogger<JobQueue> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    /// <inheritdoc/>
    public void Enqueue<T>(T job)
    {
        _ = PublishAndLogAsync(job);
    }

    /// <inheritdoc/>
    public void EnqueueBatch<T>(IEnumerable<T> jobs)
    {
        foreach (var job in jobs)
            _ = PublishAndLogAsync(job);
    }

    private async Task PublishAndLogAsync<T>(T job)
    {
        try
        {
            await _bus.PublishAsync(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[JobQueue] 消息发布失败: Type={JobType}", typeof(T).Name);
        }
    }
}
