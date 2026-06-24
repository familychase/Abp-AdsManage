using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Pages
{
    public class PagesDto
    {
        /// <summary>
        /// Meta 主页编号
        /// </summary>
        public string PageNo { get; set; } = string.Empty;

        /// <summary>
        /// 主页名称
        /// </summary>
        public string PageName { get; set; } = string.Empty;

        /// <summary>
        /// 主页分类
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 主页访问令牌
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// 关联的广告账户号
        /// </summary>
        public string AccountNo { get; set; } = string.Empty;

        /// <summary>
        /// 媒体平台
        /// </summary>
        public PlatformType Platform { get; set; }

        /// <summary>
        /// 最后同步时间
        /// </summary>
        public string? LastSyncTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreationTime { get; set; } = string.Empty;

    }
}
