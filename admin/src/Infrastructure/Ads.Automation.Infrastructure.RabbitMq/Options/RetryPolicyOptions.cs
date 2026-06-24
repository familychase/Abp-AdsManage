namespace Ads.Automation.Infrastructure.RabbitMq.Options;

/// <summary>
/// 重试策略配置选项
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>最大重试次数</summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>初始重试延迟（毫秒）</summary>
    public int InitialRetryDelayMs { get; set; } = 1000;

    /// <summary>退避乘数（指数退避）</summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>最大重试延迟（毫秒）</summary>
    public int MaxRetryDelayMs { get; set; } = 30000;

    /// <summary>是否启用死信队列</summary>
    public bool UseDeadLetterQueue { get; set; } = true;

    /// <summary>死信交换机名称</summary>
    public string DeadLetterExchange { get; set; } = "dlx.default";

    /// <summary>死信队列名称</summary>
    public string DeadLetterQueue { get; set; } = "dlq.default";
}
