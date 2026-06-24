using Ads.Automation.Application.Contracts.Entity.Publishing.Template;

namespace Ads.Automation.Application.Contracts.Entity.Publishing;

/// <summary>
/// 发布模板服务接口
/// </summary>
public interface ITemplateAppService : IApplicationService
{
    /// <summary>
    /// 新增发布模板
    /// </summary>
    /// <param name="input">模板视图模型</param>
    /// <returns>新创建的模板 ID</returns>
    Task<long> CreateAsync(AdsPublishTemplateViewModel input);

    /// <summary>
    /// 编辑发布模板
    /// 参考 BI4Sight ChangeAdsPublishingTemplateCommand.Handler
    /// 核心流程：查找模板 → 平台校验 → 属性覆盖 → 更新模板内容 → 持久化
    /// </summary>
    /// <param name="input">模板视图模型（含 TemplateId）</param>
    /// <returns>更新后的模板 ID</returns>
    Task<long> UpdateAsync(AdsPublishTemplateViewModel input);

    /// <summary>
    /// 删除发布模板（软删除）
    /// 参考 BI4Sight RemoveAdsPublishingTemplateCommand.Handler
    /// </summary>
    /// <param name="templateId">模板 ID</param>
    Task DeleteAsync(long templateId);

    /// <summary>
    /// 获取发布模板分页列表
    /// 参考 BI4Sight GetAdsPublishingTemplateListQuerier.Handler
    /// 支持按名称、平台、应用、像素、创建人筛选
    /// </summary>
    /// <param name="input">分页查询参数</param>
    /// <returns>分页模板列表</returns>
    Task<PagedResultDto<AdsPublishTemplateListDto>> GetListAsync(AdsPublishTemplateListInput input);
}
