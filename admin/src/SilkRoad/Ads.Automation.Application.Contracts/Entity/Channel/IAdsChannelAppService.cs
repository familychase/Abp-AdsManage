namespace Ads.Automation.Application.Contracts.Entity.Channel;

/// <summary>
/// 广告渠道应用服务接口
/// </summary>
public interface IAdsChannelAppService : IApplicationService
{
    /// <summary>
    /// 获取渠道信息
    /// </summary>
    Task<AdsChannelDto> GetAsync(long id);

    /// <summary>
    /// 获取渠道列表
    /// </summary>
    Task<PagedResultDto<AdsChannelListDto>> GetListAsync(GetAdsChannelListInput input);

    /// <summary>
    /// 删除渠道
    /// </summary>
    Task DeleteAsync(long id);
}
