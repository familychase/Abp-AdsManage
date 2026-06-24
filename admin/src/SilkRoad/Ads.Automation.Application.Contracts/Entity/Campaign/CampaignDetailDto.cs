namespace Ads.Automation.Application.Contracts.Entity.Campaign;

/// <summary>
/// 广告系列详情出参
/// </summary>
public class CampaignDetailDto
{
    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列名称
    /// </summary>
    public string CampaignName { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列状态（ACTIVE / PAUSED / DELETED / ARCHIVED）
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列所属的实际账户编号
    /// </summary>
    public string ActualAccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 传入的账户与广告系列实际账户是否一致
    /// </summary>
    public bool IsAccountMatched { get; set; }

    /// <summary>
    /// 账户不一致时的提示信息（一致时为 null）
    /// </summary>
    public string? AccountMismatchMessage { get; set; }

    /// <summary>
    /// 推广目标（Meta API objective 字段，如 OUTCOME_TRAFFIC、OUTCOME_CONVERSIONS 等）
    /// </summary>
    public string Objective { get; set; } = string.Empty;

    /// <summary>
    /// 日预算（Meta API daily_budget 换算为美元，如 1000 → 10.00）
    /// </summary>
    public decimal? DailyBudget { get; set; }

    /// <summary>
    /// 总预算（Meta API lifetime_budget 换算为美元，如 50000 → 500.00）
    /// </summary>
    public decimal? LifetimeBudget { get; set; }
}
