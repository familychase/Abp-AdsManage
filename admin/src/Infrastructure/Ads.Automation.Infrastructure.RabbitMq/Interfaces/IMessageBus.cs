namespace Ads.Automation.Infrastructure.RabbitMq.Interfaces;

/// <summary>
/// 消息总线 —— 简化推送入口，只需传入消息类即可异步发送
/// </summary>
public interface IMessageBus
{
    /// <summary>
    /// 异步发布消息（后台发送，不阻塞调用方）
    /// </summary>
    /// <typeparam name="T">消息类型（根据类型自动推断 Exchange/RoutingKey）</typeparam>
    /// <param name="message">消息体</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 延迟发布消息（后台发送）
    /// </summary>
    /// <param name="delayMs">延迟毫秒</param>
    Task PublishWithDelayAsync<T>(T message, int delayMs, CancellationToken cancellationToken = default);
}
