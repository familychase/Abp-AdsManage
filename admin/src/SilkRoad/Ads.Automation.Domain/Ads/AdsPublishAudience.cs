using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Domain.Ads
{
    /// <summary>
    /// 受众信息 国家信息
    /// </summary>
    public class AdsPublishAudience : AggregateRootEntity
    {
        /// <summary>
        /// 媒体平台
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// 发布受众类型
        /// </summary>
        public AdsPublishingAudienceType Type { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 名称 - English
        /// </summary>
        public string Name_EN { get; set; } = null!;

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; } = null!;
    }
}
