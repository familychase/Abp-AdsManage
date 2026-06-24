using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Ads
{
    public interface IAdAppService : IApplicationService
    {
        /// <summary>
        /// 获取广告列表（读取本地数据），支持分页
        /// </summary>
        Task<PagedResultDto<AdListItemDto>> GetListAsync(GetAdListInput input);
    }
}
