using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : AbpControllerBase
{
    protected IActionResult Success<T>(T data)
    {
        return Ok(WebApiResponse<T>.Success(data));
    }

    protected IActionResult Fail(string message, int code = 400)
    {
        return Ok(WebApiResponse<object>.Fail(message, code));
    }
}
