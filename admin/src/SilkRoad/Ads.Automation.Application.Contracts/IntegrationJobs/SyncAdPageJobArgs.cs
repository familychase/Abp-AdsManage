using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.IntegrationJobs
{
    /// <summary>
    /// 同步单个渠道账户下的个号主页任务参数
    /// </summary>
    [Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdPageJobArgs")]
    public class SyncAdPageJobArgs
    {
        /// <summary>
        /// 渠道Id
        /// </summary>
        public long ChannelId { get; set; }
    }
}
