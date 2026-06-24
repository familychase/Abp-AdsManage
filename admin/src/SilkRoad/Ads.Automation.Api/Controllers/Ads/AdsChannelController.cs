using Ads.Automation.Application.Contracts.Entity.Channel;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 广告渠道控制器
/// </summary>
[Route("api/ads/channel")]
[ApiController]
public class AdsChannelController : ApiControllerBase
{
    private readonly IAdsChannelAppService _channelAppService;

    public AdsChannelController(IAdsChannelAppService channelAppService)
    {
        _channelAppService = channelAppService;
    }

    /// <summary>
    /// 获取渠道信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _channelAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取渠道列表
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetAdsChannelListInput input)
    {
        return Success(await _channelAppService.GetListAsync(input));
    }

    /// <summary>
    /// 删除渠道
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _channelAppService.DeleteAsync(id);
        return Success<object?>(null);
    }
}
