namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API 重试配置
/// </summary>
public class MetaApiRetryOptions
{
    /// <summary>
    /// 初始重试间隔（秒），默认 2
    /// </summary>
    public int InitialRetryDelaySeconds { get; set; } = 2;

    /// <summary>
    /// 退避倍数，默认 2.0（2s → 4s → 8s → 16s）
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// 最大重试间隔（秒），默认 60
    /// </summary>
    public int MaxRetryDelaySeconds { get; set; } = 60;

    /// <summary>
    /// 最大重试次数，默认 4
    /// </summary>
    public int MaxRetries { get; set; } = 4;

    /// <summary>
    /// 瞬时错误码集合
    /// </summary>
    public HashSet<int> TransientErrorCodes { get; set; } = [1, 2, 4, 17, 32, 613, 80004];

    /// <summary>
    /// 瞬时错误子码集合（用于细化 code=100 等通用错误码中可重试的子类型）
    /// 1772103: "Missing Instagram account" — 间歇性发生，重试可恢复
    /// </summary>
    public HashSet<int> TransientErrorSubcodes { get; set; } = [1772103];
}
