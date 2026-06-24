namespace Ads.Automation.Infrastructure.RabbitMq.Interfaces;

/// <summary>
/// 消息消费者接口 —— 可靠消费与重试
/// </summary>
public interface IMessageConsumer : IDisposable
{
    /// <summary>
    /// 订阅队列消息（内部先声明队列 → 绑定到交换机 → 开始消费）
    /// </summary>
    /// <param name="queue">队列名称</param>
    /// <param name="exchange">交换机名称</param>
    /// <param name="routingKey">路由键</param>
    /// <param name="handler">消息处理委托</param>
    /// <param name="autoAck">是否自动确认（默认手动确认以保证可靠性）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消费者标签</returns>
    Task<string> SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, CancellationToken, Task<bool>> handler, bool autoAck = false, CancellationToken cancellationToken = default);

    /// <summary>取消订阅</summary>
    Task UnsubscribeAsync(string consumerTag);

    /// <summary>手动确认消息</summary>
    void Acknowledge(ulong deliveryTag, bool multiple = false);

    /// <summary>拒绝消息（可重入队）</summary>
    void Reject(ulong deliveryTag, bool requeue = false);

    /// <summary>拒绝多消息（批量）</summary>
    void Nack(ulong deliveryTag, bool multiple = false, bool requeue = false);

    /// <summary>
    /// 声明队列（如不存在则创建，带死信配置）
    /// </summary>
    QueueDeclareOk QueueDeclare(string queue, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null);

    /// <summary>
    /// 声明交换机
    /// </summary>
    void ExchangeDeclare(string exchange, string type, bool durable = true, bool autoDelete = false, IDictionary<string, object?>? arguments = null);

    /// <summary>
    /// 绑定队列到交换机
    /// </summary>
    void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object?>? arguments = null);

    /// <summary>
    /// 获取队列中的消息数量
    /// </summary>
    uint GetMessageCount(string queue);
}
