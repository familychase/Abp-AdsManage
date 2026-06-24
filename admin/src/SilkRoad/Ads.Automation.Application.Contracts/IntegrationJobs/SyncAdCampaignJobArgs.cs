namespace Ads.Automation.Application.Contracts.IntegrationJobs;

/// <summary>
/// 同步单个广告账户下的广告结构（广告系列/广告组/广告）任务参数
/// </summary>
[Ads.Automation.Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdCampaignJobArgs")]
public class SyncAdCampaignJobArgs
{
    /// <summary>
    /// 广告账户编号（Meta account_id）
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;
}
