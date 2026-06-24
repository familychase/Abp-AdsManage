using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Domain.Reporting
{
    /// <summary>
    /// 广告系列报表实体
    /// </summary>
    public class AdCampaignBaseRpt : Entity<long>
    {
        /// <summary>
        /// 广告账户Id
        /// </summary>
        public long AccountId { get; set; }

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
        /// 购物次数
        /// </summary>
        public decimal Purchase { get; set; }

        /// <summary>
        /// 购物价值
        /// </summary>
        public decimal PurchaseValue { get; set; }

        /// <summary>
        /// CPC
        /// </summary>
        public decimal CPC { get; set; }

        /// <summary>
        /// CPCO
        /// </summary>
        public decimal CPCO { get; set; }

    }
}
