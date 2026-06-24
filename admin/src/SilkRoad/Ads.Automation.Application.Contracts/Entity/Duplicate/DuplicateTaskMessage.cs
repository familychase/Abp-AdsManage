namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 复制任务消息 —— 生产者写入 DB 后入队，消费者收到后按 LogId 执行复制
/// </summary>
[Domain.Shared.Attributes.MessageRoute(Exchange = Exchange, RoutingKey = RoutingKey)]
public class DuplicateTaskMessage
{
    /// <summary>交换机名称（生产者 + 消费者共享常量）</summary>
    public const string Exchange = "ads.automation.duplicate";

    /// <summary>RoutingKey（生产者 + 消费者共享常量）</summary>
    public const string RoutingKey = "duplicate-task";

    /// <summary>队列名称（消费者共享常量）</summary>
    public const string Queue = "duplicate-task";

    /// <summary>复制日志记录 ID</summary>
    public long LogId { get; set; }
}
