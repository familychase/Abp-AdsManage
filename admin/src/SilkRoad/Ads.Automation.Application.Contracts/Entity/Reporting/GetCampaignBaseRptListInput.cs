using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Reporting
{
    public class GetCampaignBaseRptListInput: BasePagedAndSortedRequestDto
    {
        /// <summary>
        /// 广告账户
        /// </summary>
        public string? AccountNo { get; set; }
        /// <summary>
        /// 系列编号
        /// </summary>
        public string? CampaignNo { get; set; }
        /// <summary>
        /// 系列名称
        /// </summary>
        public string? CampaignName { get; set; }
        /// <summary>
        /// 时间范围
        /// </summary>
        public DateRange? DateRange { get; set; }
    }
}
