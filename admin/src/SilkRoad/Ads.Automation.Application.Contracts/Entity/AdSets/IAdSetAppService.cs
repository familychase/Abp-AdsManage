using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.AdSets
{
    /// <summary>
    /// 广告组应用服务接口
    /// </summary>
    public interface IAdSetAppService : IApplicationService
    {
        /// <summary>
        /// 获取广告组列表（读取本地数据），支持分页
        /// </summary>
        Task<PagedResultDto<AdSetListItemDto>> GetListAsync(GetAdSetListInput input);
    }
}
