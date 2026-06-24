namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 跨账户复制入参
/// </summary>
public class ExternalDuplicateInput
{
    /// <summary>
    /// 广告对象层级（CAMPAIGN | AD_SET）
    /// </summary>
    [Required]
    public AdObjectLevel AdObjectLevel { get; set; }

    /// <summary>
    /// 源渠道ID
    /// </summary>
    [Required]
    public long SourceChannelId { get; set; }

    /// <summary>
    /// 源广告账户号
    /// </summary>
    [Required]
    public string SourceAccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 媒体编号（CampaignNo 或 AdSetNo）
    /// </summary>
    [Required]
    public string ObjectNo { get; set; } = string.Empty;

    /// <summary>
    /// 目标渠道ID
    /// </summary>
    [Required]
    public long TargetChannelId { get; set; }

    /// <summary>
    /// 目标广告账户号
    /// </summary>
    [Required]
    public string TargetAccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 目标公共主页编号（跨账户复制时替换源 Page）
    /// </summary>
    [Required]
    public string TargetPageNo { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 复制数量
    /// </summary>
    public long CopyNumber { get; set; } = 1;
}
