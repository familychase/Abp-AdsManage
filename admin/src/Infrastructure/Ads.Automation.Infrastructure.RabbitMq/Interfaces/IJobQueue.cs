namespace Ads.Automation.Infrastructure.RabbitMq.Interfaces;

/// <summary>
/// 任务队列 —— 业务侧只需 Enqueue，路由由消息类型的 [MessageRoute] 特性自动推断
/// </summary>
public interface IJobQueue
{
    /// <summary>入队单个任务（瞬时返回，底层走 MessageBus 后台队列）</summary>
    void Enqueue<T>(T job);

    /// <summary>批量入队</summary>
    void EnqueueBatch<T>(IEnumerable<T> jobs);
}
