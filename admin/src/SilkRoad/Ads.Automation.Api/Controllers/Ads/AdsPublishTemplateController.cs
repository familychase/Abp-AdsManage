using Ads.Automation.Application.Contracts.Entity.Publishing;
using Ads.Automation.Application.Contracts.Entity.Publishing.Template;

namespace Ads.Automation.Api.Controllers.Ads
{

    [Route("api/ads/publish/template")]
    [ApiController]
    public class AdsPublishTemplateController : ApiControllerBase
    {
        private readonly ITemplateAppService _templateAppService;

        public AdsPublishTemplateController(ITemplateAppService templateAppService)
        {
            _templateAppService = templateAppService;
        }

        /// <summary>
        /// 新增发布模板
        /// 参考 BI4Sight AdsPublishingTemplateController.AdsPublishingTemplateAddAsync
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] AdsPublishTemplateViewModel input)
        {
            return Success(await _templateAppService.CreateAsync(input));
        }

        /// <summary>
        /// 编辑发布模板
        /// 参考 BI4Sight AdsPublishingTemplateController.AdsPublishingTemplateEditAsync
        /// 核心流程：查找模板 → META 数据校验 → 属性覆盖 → 设置模板内容 → 持久化
        /// </summary>
        [HttpPost("edit")]
        public async Task<IActionResult> EditAsync([FromBody] AdsPublishTemplateViewModel input)
        {
            return Success(await _templateAppService.UpdateAsync(input));
        }

        /// <summary>
        /// 删除发布模板（软删除）
        /// 参考 BI4Sight AdsPublishingTemplateController.RemoveAdsPublishingTemplateAsync
        /// </summary>
        [HttpPost("del")]
        public async Task<IActionResult> DeleteAsync([FromQuery] long templateId)
        {
            await _templateAppService.DeleteAsync(templateId);
            return Success<object?>(null);
        }

        /// <summary>
        /// 获取发布模板分页列表
        /// 参考 BI4Sight AdsPublishingTemplateController.GetAdsPublishingTemplateListValidAsync
        /// 支持按名称（模糊+ID解析）、平台、应用、像素、创建人筛选
        /// </summary>
        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] AdsPublishTemplateListInput input)
        {
            return Success(await _templateAppService.GetListAsync(input));
        }
    }
}
