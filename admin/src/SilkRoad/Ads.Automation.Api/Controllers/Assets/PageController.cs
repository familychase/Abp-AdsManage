
using Ads.Automation.Application.Contracts.Entity.Pages;

namespace Ads.Automation.Api.Controllers.Assets
{
    [Route("api/ads/assets/page")]
    [ApiController]
    public class PageController : ApiControllerBase
    {
        private readonly IPagesAppService _pagesService;

        public PageController(IPagesAppService pagesService)
        {
            _pagesService = pagesService;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetPageListAsync([FromBody] GetPageListInput input)
        {
            var result = await _pagesService.GetListAsync(input);
            return Success(result);
        }
    }
}
