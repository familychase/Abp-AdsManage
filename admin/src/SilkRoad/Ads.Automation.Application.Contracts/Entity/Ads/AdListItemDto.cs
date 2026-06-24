namespace Ads.Automation.Application.Contracts.Entity.Ads;

/// <summary>
/// 广告列表项
/// </summary>
public class AdListItemDto
{
    /// <summary>
    /// 广告Id
    /// </summary>
    public long AdId { get; set; }

    /// <summary>
    /// 广告编号
    /// </summary>
    public string AdNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告名称
    /// </summary>
    public string AdName { get; set; } = string.Empty;

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string MediaState { get; set; } = string.Empty;

    /// <summary>
    /// 广告创意编号
    /// </summary>
    public string CreativeNo { get; set; } = string.Empty;

    /// <summary>
    /// 主页编号
    /// </summary>
    public string PageNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告组编号
    /// </summary>
    public string AdSetNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告账户编号
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 媒体创建时间
    /// </summary>
    public DateTime MediaCreateTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}
