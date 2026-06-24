using Ads.Automation.Application.Contracts.Entity.Campaign;

namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 媒体实时查询应用服务接口
/// 统一管理所有实时调用 Meta API 的查询操作
/// </summary>
public interface IMediaAppService : IApplicationService
{
    /// <summary>
    /// 获取广告系列详情（实时调用 Meta API，校验账户一致性）
    /// </summary>
    Task<CampaignDetailDto> GetCampaignDetailAsync(CampaignDetailInput input);

    /// <summary>
    /// 根据广告账户获取公共主页列表（实时调用 Meta API）
    /// </summary>
    Task<PagedResultDto<MediaPageDto>> GetPagesByAccountAsync(GetPagesByAccountInput input);

    /// <summary>
    /// 批量删除广告系列（实时调用 Meta API）
    /// </summary>
    Task<List<BatchDeleteCampaignResultDto>> BatchDeleteCampaignsAsync(BatchDeleteCampaignsInput input);

    /// <summary>
    /// 批量删除广告组（实时调用 Meta API）
    /// </summary>
    Task<List<BatchDeleteAdSetResultDto>> BatchDeleteAdSetsAsync(BatchDeleteAdSetsInput input);

    /// <summary>
    /// 获取区域/城市列表（实时调用 Meta API）
    /// </summary>
    Task<List<MediaRegionDto>> GetRegionListAsync(GetRegionListInput input);
}
