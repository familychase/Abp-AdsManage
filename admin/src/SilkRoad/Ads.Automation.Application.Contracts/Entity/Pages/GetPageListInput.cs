using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Pages
{
    public class GetPageListInput : BasePagedAndSortedRequestDto
    {
        /// <summary>
        /// 主页名称/编号过滤
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// 个号（渠道 ChannelId）筛选
        /// </summary>
        public long? ChannelId { get; set; }
    }
}
