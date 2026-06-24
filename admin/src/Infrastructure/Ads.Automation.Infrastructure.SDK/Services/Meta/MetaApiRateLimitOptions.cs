namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API 限流配置
/// 基于 Redis 令牌桶，在读/写操作前预检，避免打到 Meta 后才收到 80004 限流错误
/// </summary>
public class MetaApiRateLimitOptions
{
    /// <summary>读操作令牌桶容量（GET 请求每次消耗 1 token）</summary>
    public int ReadBucketCapacity { get; set; } = 15;

    /// <summary>读操作令牌补充速率（每秒补充 token 数）</summary>
    public double ReadRefillRatePerSecond { get; set; } = 10;

    /// <summary>写操作令牌桶容量（POST/PUT/DELETE 每次消耗 1 token）</summary>
    public int WriteBucketCapacity { get; set; } = 5;

    /// <summary>写操作令牌补充速率（每秒补充 token 数）</summary>
    public double WriteRefillRatePerSecond { get; set; } = 3;

    /// <summary>限流等待最大超时（秒），超时后直接抛 RateLimitException</summary>
    public int MaxWaitSeconds { get; set; } = 30;

    /// <summary>是否启用限流</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Redis key 前缀</summary>
    public string KeyPrefix { get; set; } = "ratelimit:meta";
}
