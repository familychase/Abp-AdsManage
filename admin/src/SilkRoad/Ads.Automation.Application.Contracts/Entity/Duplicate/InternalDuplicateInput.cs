namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 账户内复制入参
/// </summary>
public class InternalDuplicateInput
{
    /// <summary>
    /// 广告对象层级（CAMPAIGN | AD_SET）
    /// </summary>
    [Required]
    public AdObjectLevel AdObjectLevel { get; set; }

    /// <summary>
    /// 渠道ID（获取授权）
    /// </summary>
    [Required]
    public long ChannelId { get; set; }

    /// <summary>
    /// 广告账户号
    /// </summary>
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 媒体编号（CampaignNo 或 AdSetNo）
    /// </summary>
    [Required]
    public string ObjectNo { get; set; } = string.Empty;

    /// <summary>
    /// 复制份数
    /// </summary>
    [Required]
    [Range(1, 100)]
    public long CopyNumber { get; set; } = 1;

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }
}
