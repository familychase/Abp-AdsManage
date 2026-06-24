namespace Ads.Automation.Infrastructure.RabbitMq.Interfaces;

/// <summary>
/// 消息发布者接口 —— 可控推流
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// 发布单条消息到指定交换机和路由键
    /// </summary>
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布单条消息（可选配置）
    /// </summary>
    Task PublishAsync<T>(string exchange, string routingKey, T message, PublishOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量发布消息
    /// </summary>
    Task PublishBatchAsync<T>(string exchange, string routingKey, IEnumerable<T> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布延迟消息（通过 TTL + DLX 实现）
    /// </summary>
    Task PublishWithDelayAsync<T>(string exchange, string routingKey, T message, int delayMs, CancellationToken cancellationToken = default);

    /// <summary>暂停推流</summary>
    void Pause();

    /// <summary>恢复推流</summary>
    void Resume();

    /// <summary>是否处于暂停状态</summary>
    bool IsPaused { get; }
}
