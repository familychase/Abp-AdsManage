using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.IntegrationJobs
{
    /// <summary>
    /// 同步单个账号的主页信息
    /// </summary>
    [Domain.Shared.Attributes.MessageRoute(Exchange = "ads.automation.jobs", RoutingKey = "SyncAdPageAccountJobArgs")]
    public class SyncAdPageAccountJobArgs
    {
        /// <summary>
        /// 账户Id
        /// </summary>
        public long AccountId { get; set; }
    }
}
