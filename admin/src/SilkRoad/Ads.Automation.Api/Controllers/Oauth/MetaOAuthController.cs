using Ads.Automation.Application.Contracts.Entity.MetaOAuth;

namespace Ads.Automation.Api.Controllers.Oauth;

/// <summary>
/// Meta OAuth 授权控制器
/// </summary>
[Route("api/ads/meta_oauth")]
[ApiController]
public class MetaOAuthController : ApiControllerBase
{
    private readonly IMetaOAuthAppService _metaOAuthAppService;

    public MetaOAuthController(IMetaOAuthAppService metaOAuthAppService)
    {
        _metaOAuthAppService = metaOAuthAppService;
    }

    /// <summary>
    /// Meta OAuth 授权回调处理
    /// 接收前端传来的授权码，换取 Token → 验证有效性 → 去重 → 加密存储
    /// </summary>
    /// <param name="input">OAuth 回调数据（code, appId, appSecret, redirectUri）</param>
    [HttpPost("callback")]
    public async Task<IActionResult> HandleCallbackAsync([FromBody] MetaOAuthCallbackInput input)
    {
        var result = await _metaOAuthAppService.HandleCallbackAsync(input);
        return Success(result);
    }
}
