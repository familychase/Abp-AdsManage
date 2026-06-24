namespace Ads.Automation.Infrastructure.RabbitMq.Options;

/// <summary>
/// RabbitMQ 连接配置选项
/// </summary>
public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    /// <summary>主机地址</summary>
    public string Host { get; set; } = "localhost";

    /// <summary>端口</summary>
    public int Port { get; set; } = 5672;

    /// <summary>虚拟主机</summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>用户名</summary>
    public string UserName { get; set; } = "guest";

    /// <summary>密码</summary>
    public string Password { get; set; } = "guest";

    /// <summary>连接超时（毫秒）</summary>
    public int ConnectionTimeout { get; set; } = 5000;

    /// <summary>自动恢复网络连接</summary>
    public bool AutomaticRecoveryEnabled { get; set; } = true;

    /// <summary>网络恢复间隔（毫秒）</summary>
    public int NetworkRecoveryInterval { get; set; } = 5000;

    /// <summary>请求心跳超时（秒）</summary>
    public int RequestedHeartbeat { get; set; } = 60;

    /// <summary>最大 Channel 池大小</summary>
    public int MaxChannelPoolSize { get; set; } = 100;

    /// <summary>发布者确认模式</summary>
    public bool PublisherConfirms { get; set; } = true;

    /// <summary>消息持久化（默认）</summary>
    public bool Persistent { get; set; } = true;

    /// <summary>最大批量发布大小</summary>
    public int MaxBatchSize { get; set; } = 100;

    /// <summary>预取数量（消费者 QoS）</summary>
    public ushort PrefetchCount { get; set; } = 10;

    /// <summary>MessageBus 优雅关闭时排空 Channel 的超时时间（秒），默认 30</summary>
    public int MessageBusDrainTimeoutSeconds { get; set; } = 30;
}
