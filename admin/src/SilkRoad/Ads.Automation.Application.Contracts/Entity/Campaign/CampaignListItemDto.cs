namespace Ads.Automation.Application.Contracts.Entity.Campaign;

/// <summary>
/// 广告系列列表项
/// </summary>
public class CampaignListItemDto
{
    /// <summary>
    /// 系列Id
    /// </summary>
    public long CampaignId { get; set; }

    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列名称
    /// </summary>
    public string CampaignName { get; set; } = string.Empty;

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
    /// 推广目标
    /// </summary>
    public string Objective { get; set; } = string.Empty;

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
