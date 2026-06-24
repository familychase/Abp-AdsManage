using Microsoft.AspNetCore.Mvc;

namespace Ads.Automation.Api.Controllers.Reports
{
    /// <summary>
    /// 报表控制器
    /// </summary>
    [Route("api/ads/reports/campaign")]
    [ApiController]
    public class AdsCampaignRptController : ApiControllerBase
    {
        private readonly IAdCampaignBaseRptAppService _campaignRptService;

        public AdsCampaignRptController(IAdCampaignBaseRptAppService campaignRptService)
        {
            _campaignRptService = campaignRptService;
        }

        /// <summary>
        /// 获取广告系列报表列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] GetCampaignBaseRptListInput input)
        {
            return Success(await _campaignRptService.GetListAsync(input));
        }
    }
}
