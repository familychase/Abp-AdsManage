namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 复制提交入参基类
/// </summary>
public abstract class DuplicateSubmitInput
{
    /// <summary>
    /// 渠道ID（授权个号）
    /// </summary>
    [Required]
    public long ChannelId { get; set; }

    /// <summary>
    /// 广告账户号
    /// </summary>
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 公共主页编号
    /// </summary>
    [Required]
    public string PageNo { get; set; } = string.Empty;

    /// <summary>
    /// 复制份数（默认 1）
    /// </summary>
    [Range(1, 250)]
    public long CopyNumber { get; set; } = 1;
}

/// <summary>
/// 广告系列复制提交入参
/// </summary>
public class CampaignSubmitInput : DuplicateSubmitInput
{
    /// <summary>
    /// 广告系列编号
    /// </summary>
    [Required]
    public string CampaignNo { get; set; } = string.Empty;
}

/// <summary>
/// 广告组复制提交入参
/// </summary>
public class AdSetSubmitInput : DuplicateSubmitInput
{
    /// <summary>
    /// 广告组编号
    /// </summary>
    [Required]
    public string AdSetNo { get; set; } = string.Empty;
}
