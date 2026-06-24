using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Domain.Channel
{
    /// <summary>
    /// 渠道主页关联表实体类
    /// </summary>
    public class AdsChannelPage : Entity<long>
    {
        /// <summary>
        /// 渠道Id
        /// </summary>
        public long ChannelId { get; set; }

        /// <summary>
        /// 主页Id
        /// </summary>
        public long PageId { get; set; }

        /// <summary>
        /// 主页编号
        /// </summary>
        public string PageNo { get; set; } = string.Empty;
    }
}
