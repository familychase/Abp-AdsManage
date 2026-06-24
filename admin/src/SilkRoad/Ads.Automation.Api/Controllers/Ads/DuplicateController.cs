using Ads.Automation.Application.Contracts.Entity.Duplicate;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 广告复制控制器
/// </summary>
[Route("api/ads/duplicate")]
[ApiController]
public class DuplicateController : ApiControllerBase
{
    private readonly IDuplicateAppService _duplicateAppService;

    public DuplicateController(IDuplicateAppService duplicateAppService)
    {
        _duplicateAppService = duplicateAppService;
    }

    /// <summary>
    /// 广告系列复制提交
    /// 校验广告系列编号有效性后创建待执行记录
    /// </summary>
    [HttpPost("campaign/internal")]
    public async Task<IActionResult> SubmitCampaignAsync([FromBody] CampaignSubmitInput input)
    {
        try
        {
            var result = await _duplicateAppService.SubmitCampaignAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 广告组复制提交
    /// 校验广告组编号有效性后创建待执行记录
    /// </summary>
    [HttpPost("adset/internal")]
    public async Task<IActionResult> SubmitAdSetAsync([FromBody] AdSetSubmitInput input)
    {
        try
        {
            var result = await _duplicateAppService.SubmitAdSetAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 获取广告系列复制日志列表（分页）
    /// </summary>
    [HttpGet("campaign/logging/list")]
    public async Task<IActionResult> GetCampaignListAsync([FromQuery] GetDuplicateLoggingListInput input)
    {
        try
        {
            var result = await _duplicateAppService.GetCampaignListAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 获取广告组复制日志列表（分页）
    /// </summary>
    [HttpGet("adset/logging/list")]
    public async Task<IActionResult> GetAdSetListAsync([FromQuery] GetDuplicateLoggingListInput input)
    {
        try
        {
            var result = await _duplicateAppService.GetAdSetListAsync(input);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }

    /// <summary>
    /// 获取复制明细列表（按日志ID查询全部明细）
    /// </summary>
    [HttpGet("detail/{logId:long}")]
    public async Task<IActionResult> GetDetailListAsync(long logId)
    {
        try
        {
            var result = await _duplicateAppService.GetDetailListAsync(logId);
            return Success(result);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
