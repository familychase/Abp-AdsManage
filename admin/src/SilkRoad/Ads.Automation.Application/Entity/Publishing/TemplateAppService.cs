using Ads.Automation.Application.Contracts.Entity.Publishing.Template;
using Ads.Automation.Domain.Publishing;
using Ads.Automation.Domain.Publishing.BusinessModel.Meta;
using Ads.Automation.Domain.Publishing.ValueObjects;
using Ads.Automation.Domain.Shared.Enums.Publishing;
using Ads.Automation.Domain.Shared.Localization;
using Microsoft.Extensions.Localization;

namespace Ads.Automation.Application.Entity.Publishing;

/// <summary>
/// 发布模板应用服务实现
/// 参考 BI4Sight CreateAdsPublishingTemplateCommand.Handler 的 META 分支处理逻辑
/// </summary>
public class TemplateAppService : ApplicationService, ITemplateAppService
{
    private readonly IBaseRepository<AdsPublishTemplate> _templateRepository;
    private readonly IBaseRepository<AdsPage> _pageRepository;
    private readonly IStringLocalizer<AdsAutomationResource> _language;

    public TemplateAppService(IBaseRepository<AdsPublishTemplate> templateRepository, IBaseRepository<AdsPage> pageRepository, IStringLocalizer<AdsAutomationResource> language)
    {
        _templateRepository = templateRepository;
        _pageRepository = pageRepository;
        _language = language;
    }

    /// <summary>
    /// 新增发布模板
    /// 核心处理流程（META 平台）：
    ///   1. 校验 META 平台数据完整性
    ///   2. ViewModel → Domain BO 映射
    ///   4. 设置模板名称规则（占位符替换）
    ///   5. 校验 Facebook Page 是否存在
    ///   8. 持久化到数据库
    /// </summary>
    /// <inheritdoc />
    public async Task<long> CreateAsync(AdsPublishTemplateViewModel input)
    {
        // ========== 第1步：校验 META 平台数据完整性 ==========
        if (input.Platform == PlatformType.META && input.MetaPublishingData == null)
            throw new UserFriendlyException(_language["Template:MetaDataRequired"]);

        var metaModel = input.MetaPublishingData!;

        // ========== 第2步：ViewModel → Domain BO 映射（AutoMapper） ==========
        var metaData = ObjectMapper.Map<MetaPublishDataViewModel, MetaPublishDataBo>(metaModel);

        // ========== 第4步：设置模板名称规则（META 平台占位符替换） ==========
        // 模板名称中的占位符：#PUBLISHER# #PLACEMENT_TYPE# #CONVERSION_TYPE# #APP_TYPE# #GENDER#
        // 生成替换后的模板名称
        var templateName = input.TemplateName;
        if (templateName.Contains("#GENDER#"))
        {
            var genderText = metaData.AudienceData.Gender switch
            {
                1 => "男",
                2 => "女",
                _ => "不限"
            };
            templateName = templateName.Replace("#GENDER#", genderText);
        }
        if (templateName.Contains("#APP_TYPE#"))
        {
            var appType = metaData.AudienceData.AppType ?? string.Empty;
            var appTypeText = appType.Equals("IOS", StringComparison.OrdinalIgnoreCase) ? "ios"
                : appType.Equals("ANDROID", StringComparison.OrdinalIgnoreCase) ? "and"
                : "and/ios";
            templateName = templateName.Replace("#APP_TYPE#", appTypeText);
        }
        if (templateName.Contains("#CONVERSION_TYPE#"))
        {
            templateName = templateName.Replace("#CONVERSION_TYPE#", metaData.AdsetData.OptimizationGoal);
        }
        if (templateName.Contains("#PLACEMENT_TYPE#"))
        {
            var placementType = metaData.AudienceData.SubdivisionPositionV1 != null
                && metaData.AudienceData.SubdivisionPositionV1.Any()
                ? "设置细分定位（兴趣与行为）"
                : "不设置细分定位";
            templateName = templateName.Replace("#PLACEMENT_TYPE#", placementType);
        }

        // ========== 第5步：校验 Facebook Page 是否存在 ==========
        if (!await _pageRepository.AnyAsync(c => c.PageNo == metaModel.AdData.PageNo))
            throw new Domain.Shared.Common.BusinessException(string.Format(_language["Template:InvalidPageNo"], metaModel.AdData.PageNo), 400);

        // ========== 第8步：解析 ResourceId ==========
        // 优先级：ApplicationId → PixelId → ProductCatalogId
        var resourceId = input.ApplicationId
            ?? input.PixelId
            ?? 0;

        // ========== 第9步：构建批量发布选项 ==========
        var batchOptions = new AdsBatchPublishingOptions(
            input.PublishingType,
            input.MaxPublishCount,
            input.PublishAverage);

        // ========== 第10步：创建并持久化实体 ==========
        // AdsPublishTemplate.Create() 工厂方法内部会调用 IdGenerator.GetNextId() 生成 Snowflake ID，
        // 并设置 Version（META 平台默认为 1）、CreationTime 等基础属性
        var template = AdsPublishTemplate.Create(
            templateName,
            input.Platform,
            input.PublishingAdType,
            resourceId,
            input.ResourceContent ?? string.Empty,
            batchOptions);

        template.SetTemplateContent(metaData);

        // 名称长度校验（数据库字段限制 100 字符）
        if (template.Name.Length > 100)
            template.NameSubstring();

        await _templateRepository.InsertAsync(template);

        return template.Id;
    }

    /// <summary>
    /// 编辑发布模板
    /// 核心处理流程：
    ///   1. 查找模板并校验存在性
    ///   2. META 平台数据校验 → ViewModel → BO 映射
    ///   3. META 特殊处理：ProductCatalog → Pixel 转换
    ///   4. 覆盖模板属性（Name、Platform、ResourceId、BatchOptions 等）
    ///   5. 设置模板内容 → 持久化
    ///   6. META：保存受益人/付费人信息
    /// </summary>
    /// <inheritdoc />
    public async Task<long> UpdateAsync(AdsPublishTemplateViewModel input)
    {
        // ========== 第1步：查找模板并校验 ==========
        if (input.TemplateId <= 0)
            throw new UserFriendlyException(_language["Template:IdRequired"]);

        var template = await _templateRepository.FindAsync(input.TemplateId);
        if (template == null)
            throw new Domain.Shared.Common.BusinessException(_language["Template:NotFound"], 404);

        // ========== 第2步：META 平台数据校验与映射 ==========
        if (input.Platform == PlatformType.META && input.MetaPublishingData == null)
            throw new UserFriendlyException(_language["Template:MetaDataRequired"]);

        var metaModel = input.MetaPublishingData!;
        var metaData = ObjectMapper.Map<MetaPublishDataViewModel, MetaPublishDataBo>(metaModel);

        // ========== 第3步：META 特殊处理（参考参考项目第125行） ==========
        // META 平台 + 商品目录类型 → 强制转换为像素类型
        if (input.Platform == PlatformType.META && input.PublishingAdType == AdsPublishingAdType.PRODUCT_CATALOG)
            input.PublishingAdType = AdsPublishingAdType.PIXEL;

        // ========== 第4步：校验 Facebook Page ==========
        if (!await _pageRepository.AnyAsync(c => c.PageNo == metaModel.AdData.PageNo))
            throw new Domain.Shared.Common.BusinessException(string.Format(_language["Template:InvalidPageNo"], metaModel.AdData.PageNo), 400);

        // ========== 第5步：覆盖模板属性 ==========
        template.Name = input.TemplateName;
        template.Platform = input.Platform;
        template.PublishingAdType = input.PublishingAdType;
        template.ResourceContent = input.ResourceContent ?? string.Empty;
        template.ResourceId = input.ApplicationId ?? input.PixelId ?? 0;
        template.Version = 1; // META 当前版本号

        template.BatchPublishingOptions = new AdsBatchPublishingOptions(
            input.PublishingType,
            input.MaxPublishCount,
            input.PublishAverage);

        // ========== 第6步：设置模板内容 ==========
        template.SetTemplateContent(metaData);

        // 名称长度校验
        if (template.Name.Length > 100)
            template.NameSubstring();

        // ========== 第7步：持久化 ==========
        await _templateRepository.UpdateAsync(template);

        // ========== 第8步：META 保存受益人/付费人（占位，待实现）= ==========
        // 参考项目：await _basicContentService.SaveMetaPublishAsync(input.MetaPublishingData);
        // 当前版本暂不实现，后续集成 AdsBasicContentService

        return template.Id;
    }

    /// <summary>
    /// 删除发布模板（软删除）
    /// 参考 BI4Sight RemoveAdsPublishingTemplateCommand.Handler
    /// </summary>
    /// <inheritdoc />
    public async Task DeleteAsync(long templateId)
    {
        var template = await _templateRepository.FindAsync(templateId);
        if (template == null)
            throw new Domain.Shared.Common.BusinessException(_language["Template:NotFound"], 404);

        await _templateRepository.DeleteAsync(template);
    }

    /// <summary>
    /// 获取发布模板分页列表
    /// 参考 BI4Sight GetAdsPublishingTemplateListQuerier.Handler
    /// 支持按名称（模糊）、平台、应用、像素、创建人筛选，
    /// 结果按创建时间倒序排列
    /// </summary>
    /// <inheritdoc />
    public async Task<PagedResultDto<AdsPublishTemplateListDto>> GetListAsync(AdsPublishTemplateListInput input)
    {
        // ========== 构建查询条件 ==========
        var query = (await _templateRepository.GetQueryableAsync()).AsNoTracking();

        // 名称模糊搜索（同时支持模板 ID 精确解析）
        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            if (long.TryParse(input.Name, out var templateId) && templateId > 0)
            {
                // 纯数字输入 → 精确匹配模板 ID
                query = query.Where(t => t.Id == templateId);
            }
            else
            {
                query = query.Where(t => t.Name.Contains(input.Name));
            }
        }

        // 平台过滤
        if (input.Platform.HasValue)
            query = query.Where(t => t.Platform == input.Platform.Value);

        // 应用 ID 过滤
        if (input.ApplicationId.HasValue)
            query = query.Where(t => t.ResourceId == input.ApplicationId.Value);

        // 像素 ID 过滤
        if (input.PixelId.HasValue)
            query = query.Where(t => t.ResourceId == input.PixelId.Value);

        // 创建人过滤
        if (input.CreatorId.HasValue)
            query = query.Where(t => t.CreatorId == input.CreatorId.Value);

        if (input.DateRange.IsNotNull())
            query = query.Where(t => t.CreationTime >= input.DateRange!.Start && t.CreationTime < input.DateRange.Stop);

        // ========== 获取总数 ==========
        var totalCount = await query.CountAsync();

        // ========== 分页查询（按创建时间倒序） ==========
        var entities = await query
            .OrderByDescending(t => t.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        // ========== Entity → DTO 映射 ==========
        var dtos = ObjectMapper.Map<List<AdsPublishTemplate>, List<AdsPublishTemplateListDto>>(entities);

        return new PagedResultDto<AdsPublishTemplateListDto>(totalCount, dtos);
    }

}
