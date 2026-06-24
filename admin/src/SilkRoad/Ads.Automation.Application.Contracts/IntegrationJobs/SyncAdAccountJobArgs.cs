namespace Ads.Automation.Application.Contracts.IntegrationJobs;

/// <summary>
/// 同步单个渠道下的广告账户任务参数
/// </summary>
[Ads.Automation.Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdAccountJobArgs")]
public class SyncAdAccountJobArgs
{
    /// <summary>
    /// 渠道Id
    /// </summary>
    public long ChannelId { get; set; }
}
