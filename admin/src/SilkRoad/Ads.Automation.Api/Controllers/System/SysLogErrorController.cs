using Ads.Automation.Application.Contracts.Log;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.System;

/// <summary>
/// 错误日志查询控制器
/// </summary>
[Route("api/system/log_error")]
[ApiController]
public class SysLogErrorController : ApiControllerBase
{
    private readonly ISysLogErrorAppService _sysLogErrorAppService;

    public SysLogErrorController(ISysLogErrorAppService sysLogErrorAppService)
    {
        _sysLogErrorAppService = sysLogErrorAppService;
    }

    /// <summary>
    /// 分页查询错误日志（按 ID 倒序，Message / Exception 模糊匹配）
    /// </summary>
    /// <param name="input">查询条件</param>
    /// <returns>分页结果</returns>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetSysLogErrorListInput input)
    {
        return Success(await _sysLogErrorAppService.GetListAsync(input));
    }
}
