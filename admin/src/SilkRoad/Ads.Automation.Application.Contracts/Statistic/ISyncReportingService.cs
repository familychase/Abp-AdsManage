using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Statistic
{
    public interface ISyncReportingService : IApplicationService
    {
        /// <summary>
        /// 媒体平台
        /// </summary>
        public PlatformType Platform { get; }

        /// <summary>
        /// 校验账户是否为活跃账户
        /// </summary>
        /// <param name="accountNo">账户编号</param>
        /// <param name="range">时间范围</param>
        /// <param name="tenantIds">需要推送的租户ids</param>
        /// <returns></returns>
        Task<bool> SyncGetRecentCost(string accountNo, DateRange range);

        /// <summary>
        /// 同步广告账户指定区间范围的基础报表数据
        /// </summary>
        /// <param name="accountNo">账户编号</param>
        /// <param name="range">时间范围</param>
        /// <returns></returns>
        Task SyncAdBaseReporting(string accountNo, DateRange range);

    }
}
