using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Reporting
{
    public interface IAdCampaignBaseRptAppService : IApplicationService
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        Task<PagedResultDto<CampaignBaseRptDto>> GetListAsync(GetCampaignBaseRptListInput input);
    }
}
