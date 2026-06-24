namespace Ads.Automation.Application.Contracts.IntegrationJobs;

/// <summary>
/// 同步单个广告账户下的像素任务参数
/// </summary>
[Ads.Automation.Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdPixelJobArgs")]
public class SyncAdPixelJobArgs
{
    /// <summary>
    /// 广告账户编号（Meta account_id）
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;
}
