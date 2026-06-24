namespace Ads.Automation.Application.Contracts.Entity.Pixel;

/// <summary>
/// 像素应用服务接口
/// </summary>
public interface IPixelAppService : IApplicationService
{
    /// <summary>
    /// 分页查询像素列表（含关联账户信息）
    /// </summary>
    Task<PagedResultDto<PixelDto>> GetListAsync(GetPixelListInput input);
}
