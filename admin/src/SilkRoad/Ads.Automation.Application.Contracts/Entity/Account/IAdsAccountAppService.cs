namespace Ads.Automation.Application.Contracts.Entity.Account;

/// <summary>
/// 广告账户应用服务接口
/// </summary>
public interface IAdsAccountAppService : IApplicationService
{
    /// <summary>
    /// 获取账户信息
    /// </summary>
    Task<AdsAccountDto> GetAsync(long id);

    /// <summary>
    /// 获取账户列表
    /// </summary>
    Task<PagedResultDto<AdsAccountDto>> GetListAsync(GetAdsAccountListInput input);

    /// <summary>
    /// 创建账户
    /// </summary>
    Task<AdsAccountDto> CreateAsync(CreateUpdateAdsAccountDto input);

    /// <summary>
    /// 修改账户
    /// </summary>
    Task<AdsAccountDto> UpdateAsync(long id, CreateUpdateAdsAccountDto input);

    /// <summary>
    /// 删除账户
    /// </summary>
    Task DeleteAsync(long id);
}
