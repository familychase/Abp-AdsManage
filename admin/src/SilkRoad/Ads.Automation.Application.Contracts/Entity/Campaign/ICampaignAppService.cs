using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ads.Automation.Application.Contracts.Entity.Campaign;

/// <summary>
/// 广告系列应用服务接口
/// </summary>
public interface ICampaignAppService : IApplicationService
{
    /// <summary>
    /// 获取广告系列详情，校验账户一致性
    /// </summary>
    Task<CampaignDetailDto> GetDetailAsync(CampaignDetailInput input);

    /// <summary>
    /// 获取广告系列列表（读取本地数据），支持分页
    /// </summary>
    Task<PagedResultDto<CampaignListItemDto>> GetListAsync(GetCampaignListInput input);
}
