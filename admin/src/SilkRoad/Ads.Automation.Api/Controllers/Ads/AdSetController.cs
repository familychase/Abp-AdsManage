using Ads.Automation.Application.Contracts.Entity.AdSets;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads
{
    /// <summary>
    /// 广告组控制器
    /// </summary>
    [Route("api/ads/adset")]
    [ApiController]
    public class AdSetController : ApiControllerBase
    {
        private readonly IAdSetAppService _adSetAppService;

        public AdSetController(IAdSetAppService adSetAppService)
        {
            _adSetAppService = adSetAppService;
        }

        /// <summary>
        /// 获取广告组列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] GetAdSetListInput input)
        {
            try
            {
                var result = await _adSetAppService.GetListAsync(input);
                return Success(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
