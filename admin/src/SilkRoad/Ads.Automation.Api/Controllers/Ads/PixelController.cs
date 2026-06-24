using Ads.Automation.Application.Contracts.Entity.Pixel;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 像素控制器
/// </summary>
[Route("api/ads/pixel")]
[ApiController]
public class PixelController : ApiControllerBase
{
    private readonly IPixelAppService _pixelAppService;

    public PixelController(IPixelAppService pixelAppService)
    {
        _pixelAppService = pixelAppService;
    }

    /// <summary>
    /// 分页查询像素列表（含关联账户信息）
    /// </summary>
    /// <param name="input">查询条件</param>
    /// <returns>分页结果</returns>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetPixelListInput input)
    {
        return Success(await _pixelAppService.GetListAsync(input));
    }
}
