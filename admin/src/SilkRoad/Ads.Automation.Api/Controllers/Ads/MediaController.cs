using Ads.Automation.Application.Contracts.Entity.Campaign;
using Ads.Automation.Application.Contracts.Entity.Media;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 媒体实时查询控制器
/// 统一管理所有需要实时调用 Meta API 获取媒体数据的接口
/// </summary>
[Route("api/ads/media")]
[ApiController]
public class MediaController : ApiControllerBase
{
    private readonly IMediaAppService _mediaAppService;

    public MediaController(IMediaAppService mediaAppService)
    {
        _mediaAppService = mediaAppService;
    }

    /// <summary>
    /// 获取广告系列详情（实时调用 Meta API，校验账户一致性）
    /// </summary>
    /// <param name="campaignNo">广告系列编号（Meta Campaign ID）</param>
    /// <param name="accountNo">广告账户编号（用于校验）</param>
    [HttpGet("campaign/detail")]
    public async Task<IActionResult> GetCampaignDetailAsync(
        [FromQuery] string campaignNo,
        [FromQuery] string accountNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(campaignNo))
                return Fail("广告系列编号不能为空");

            if (string.IsNullOrWhiteSpace(accountNo))
                return Fail("广告账户编号不能为空");

            var input = new CampaignDetailInput
            {
                CampaignNo = campaignNo.Trim(),
                AccountNo = accountNo.Trim()
            };

            var result = await _mediaAppService.GetCampaignDetailAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 根据广告账户获取公共主页列表（实时调用 Meta API）
    /// </summary>
    /// <param name="accountNo">广告账户编号</param>
    [HttpGet("account/page")]
    public async Task<IActionResult> GetPagesByAccountAsync([FromQuery] string accountNo)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(accountNo))
                return Fail("广告账户编号不能为空");

            var input = new GetPagesByAccountInput { AccountNo = accountNo.Trim() };
            var result = await _mediaAppService.GetPagesByAccountAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 批量删除广告系列（实时调用 Meta API）
    /// </summary>
    [HttpPost("campaign/batch_delete")]
    public async Task<IActionResult> BatchDeleteCampaignsAsync([FromBody] BatchDeleteCampaignsInput input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input.AccountNo))
                return Fail("广告账户编号不能为空");

            if (input.CampaignNos == null || input.CampaignNos.Count == 0)
                return Fail("广告系列编号列表不能为空");

            var results = await _mediaAppService.BatchDeleteCampaignsAsync(input);
            return Success(results);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 批量删除广告组（实时调用 Meta API）
    /// </summary>
    [HttpPost("adset/batch_delete")]
    public async Task<IActionResult> BatchDeleteAdSetsAsync([FromBody] BatchDeleteAdSetsInput input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input.AccountNo))
                return Fail("广告账户编号不能为空");

            if (input.AdSetNos == null || input.AdSetNos.Count == 0)
                return Fail("广告组编号列表不能为空");

            var results = await _mediaAppService.BatchDeleteAdSetsAsync(input);
            return Success(results);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }


    }

    /// <summary>
    /// 获取区域/城市列表（实时调用 Meta API）
    /// </summary>
    [HttpPost("region/list")]
    public async Task<IActionResult> GetRegionListAsync([FromBody] GetRegionListInput input)
    {
        try
        {
            var result = await _mediaAppService.GetRegionListAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
