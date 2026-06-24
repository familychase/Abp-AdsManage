namespace Ads.Automation.Application.Contracts.Entity.AdSets;

/// <summary>
/// 广告组列表项
/// </summary>
public class AdSetListItemDto
{
    /// <summary>
    /// 广告组Id
    /// </summary>
    public long AdSetId { get; set; }

    /// <summary>
    /// 广告组编号
    /// </summary>
    public string AdSetNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告组名称
    /// </summary>
    public string AdSetName { get; set; } = string.Empty;

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string MediaState { get; set; } = string.Empty;

    /// <summary>
    /// 预算类型
    /// </summary>
    public string BudgetType { get; set; } = string.Empty;

    /// <summary>
    /// 预算
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = string.Empty;

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
