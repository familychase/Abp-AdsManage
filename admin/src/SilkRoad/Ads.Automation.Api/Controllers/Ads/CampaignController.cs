using Ads.Automation.Application.Contracts.Entity.Campaign;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 广告系列控制器（读本地数据）
/// </summary>
[Route("api/ads/campaign")]
[ApiController]
public class CampaignController : ApiControllerBase
{
    private readonly ICampaignAppService _campaignAppService;

    public CampaignController(ICampaignAppService campaignAppService)
    {
        _campaignAppService = campaignAppService;
    }

    /// <summary>
    /// 获取广告系列列表（读取本地数据）
    /// </summary>
    /// <param name="input">查询条件</param>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetCampaignListInput input)
    {
        try
        {
            var result = await _campaignAppService.GetListAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
