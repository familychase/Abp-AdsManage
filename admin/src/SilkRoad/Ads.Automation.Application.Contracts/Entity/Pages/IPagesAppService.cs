using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Contracts.Entity.Pages
{
    /// <summary>
    /// 主页信息Interface
    /// </summary>
    public interface IPagesAppService : IApplicationService
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        Task<PagedResultDto<PagesDto>> GetListAsync(GetPageListInput input);
    }
}
