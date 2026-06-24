using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Reporting
{
    /// <summary>
    /// 广告系列报表实体
    /// </summary>
    public class CampaignBaseRptDto
    {
        public long Id { get; set; }

        /// <summary>
        /// 账户Id
        /// </summary>
        public long AccountId { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>
        /// 账户编号
        /// </summary>
        public string AccountNo { get; set; } = string.Empty;

        /// <summary>
        /// 广告系列编号
        /// </summary>
        public string CampaignNo { get; set; } = string.Empty;

        /// <summary>
        /// 广告系列名称
        /// </summary>
        public string CampaignName { get; set; } = string.Empty;

        /// <summary>
        /// 报表日期
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// 媒体平台类型
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// 花费
        /// </summary>
        public decimal Spend { get; set; }

        /// <summary>
        /// 点击数
        /// </summary>
        public long Clicks { get; set; }

        /// <summary>
        /// 转化数
        /// </summary>
        public long Converts { get; set; }

        /// <summary>
        /// 展示数
        /// </summary>
        public long Impressions { get; set; }
        
        /// <summary>
        /// CPC
        /// </summary>
        public decimal CPC { get; set; }

    }
}
