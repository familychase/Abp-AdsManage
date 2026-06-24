using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.AdSets
{
    public class GetAdSetListInput : BasePagedAndSortedRequestDto
    {
        /// <summary>
        /// 广告账户Ids
        /// </summary>
        public List<long>? AccountIds { get; set; }

        /// <summary>
        /// 广告系列Ids
        /// </summary>
        public List<long>? CampaignIds { get; set; }

        /// <summary>
        /// 广告账户Id（精确匹配）
        /// </summary>
        public long? AccountId { get; set; }

        /// <summary>
        /// <summary>
        /// 广告组编号（精确匹配）
        /// </summary>
        public string? AdSetNo { get; set; }

        /// 媒体平台（精确匹配）
        /// </summary>
        public PlatformType? Platform { get; set; }

        /// <summary>
        /// 广告组名称（模糊搜索）
        /// </summary>
        public string? AdSetName { get; set; }
    }
}
