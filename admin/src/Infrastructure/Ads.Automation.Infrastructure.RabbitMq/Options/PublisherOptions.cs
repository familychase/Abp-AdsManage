namespace Ads.Automation.Infrastructure.RabbitMq.Options;

/// <summary>
/// 单次发布可选配置
/// </summary>
public class PublishOptions
{
    /// <summary>消息持久化</summary>
    public bool? Persistent { get; set; }

    /// <summary>消息 TTL（毫秒）</summary>
    public int? Ttl { get; set; }

    /// <summary>消息优先级 (0-9)</summary>
    public byte? Priority { get; set; }

    /// <summary>消息 ID</summary>
    public string? MessageId { get; set; }

    /// <summary>关联 ID</summary>
    public string? CorrelationId { get; set; }

    /// <summary>消息头</summary>
    public Dictionary<string, object?>? Headers { get; set; }

    /// <summary>重试次数覆盖（null 使用全局默认）</summary>
    public int? MaxRetryCount { get; set; }

    /// <summary>创建基础属性</summary>
    internal IBasicProperties CreateProperties(IModel channel)
    {
        var props = channel.CreateBasicProperties();

        if (Persistent.HasValue)
            props.Persistent = Persistent.Value;

        if (Ttl.HasValue)
            props.Expiration = Ttl.Value.ToString();

        if (Priority.HasValue)
            props.Priority = Priority.Value;

        if (!string.IsNullOrEmpty(MessageId))
            props.MessageId = MessageId;

        if (!string.IsNullOrEmpty(CorrelationId))
            props.CorrelationId = CorrelationId;

        if (Headers is { Count: > 0 })
        {
            props.Headers = new Dictionary<string, object?>(Headers);
        }

        return props;
    }
}
