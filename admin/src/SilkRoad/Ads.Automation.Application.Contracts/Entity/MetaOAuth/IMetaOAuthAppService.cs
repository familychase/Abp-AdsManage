using Volo.Abp.Application.Services;

namespace Ads.Automation.Application.Contracts.Entity.MetaOAuth;

/// <summary>
/// Meta OAuth 授权应用服务接口
/// </summary>
public interface IMetaOAuthAppService : IApplicationService
{
    /// <summary>
    /// 处理 Meta OAuth 回调授权
    /// 使用授权码换取 Token，验证有效性，去重后加密存储至渠道
    /// </summary>
    /// <param name="input">OAuth 回调数据</param>
    /// <returns>授权结果</returns>
    Task<MetaOAuthCallbackResultDto> HandleCallbackAsync(MetaOAuthCallbackInput input);
}
