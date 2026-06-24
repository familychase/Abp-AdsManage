using Ads.Automation.Application.Contracts.Entity.Ads;
using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Ads
{
    /// <summary>
    /// 广告控制器
    /// </summary>
    [Route("api/ads/ad")]
    [ApiController]
    public class AdController : ApiControllerBase
    {
        private readonly IAdAppService _adAppService;

        public AdController(IAdAppService adAppService)
        {
            _adAppService = adAppService;
        }

        /// <summary>
        /// 获取广告组列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] GetAdListInput input)
        {
            try
            {
                var result = await _adAppService.GetListAsync(input);
                return Success(result);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
