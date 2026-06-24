using Ads.Automation.Domain.Publishing.BusinessModel.Meta;
using Ads.Automation.Domain.Publishing.ValueObjects;
using Ads.Automation.Domain.Shared.Enums.Publishing;
using System.Text.Json;

namespace Ads.Automation.Domain.Publishing;

/// <summary>
/// Entity - 广告发布模板
/// </summary>
public sealed class AdsPublishTemplate : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
{
    /// <summary>
    /// 版本号（原版本版本号默认为0，当前版本号为1）
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 模板名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 发布广告类型
    /// </summary>
    public AdsPublishingAdType PublishingAdType { get; set; }

    /// <summary>
    /// 发布广告类型所依据的对象Id（application/pixel/catalog）
    /// </summary>
    public long ResourceId { get; set; }

    /// <summary>
    /// 来源网站内容（网址）
    /// </summary>
    public string ResourceContent { get; set; } = null!;

    /// <summary>
    /// 发布统计信息
    /// </summary>
    public AdsPublishingTemplateStats Statistics { get; set; } = null!;

    /// <summary>
    /// 批量发布选项
    /// </summary>
    public AdsBatchPublishingOptions BatchPublishingOptions { get; set; } = null!;

    /// <summary>
    /// 发布模板内容（JSON）
    /// </summary>
    public string TemplateContent { get; set; } = null!;

    /// <summary>
    /// 末次发布时间
    /// </summary>
    public DateTime LastPublishTime { get; set; }

    /// <summary>
    /// 是否被删除
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除人
    /// </summary>
    public long? DeleterId { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletionTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
    /// <summary>
    /// 创建人id
    /// </summary>
    public long CreatorId { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long? LastModifierId { get; set; }

    /// <summary>
    /// META 发布模板版本号常量
    /// </summary>
    private const int MetaPublishTemplateVersion = 1;

    // ===== 构造函数 =====

    private AdsPublishTemplate() { }

    internal AdsPublishTemplate(long id) : base(id) { }

    /// <summary>
    /// 创建广告发布模板
    /// </summary>
    public static AdsPublishTemplate Create(
        string name,
        PlatformType platform,
        AdsPublishingAdType publishingAdType,
        long resourceId,
        string resourceContent,
        AdsBatchPublishingOptions? batchPublishingOptions = null)
    {
        var template = new AdsPublishTemplate(IdGenerator.GetNextId())
        {
            Name = name,
            Platform = platform,
            PublishingAdType = publishingAdType,
            ResourceId = resourceId,
            ResourceContent = resourceContent ?? string.Empty,
            BatchPublishingOptions = batchPublishingOptions ?? new AdsBatchPublishingOptions(AdsBatchPublishingType.NONE, 0, false),
            CreationTime = DateTime.Now,
            LastModificationTime = DateTime.Now,
        };
        template.Create();
        return template;
    }

    /// <summary>
    /// 创建模型调用（初始化默认值）
    /// </summary>
    public void Create()
    {
        DeletionTime = DateTime.Now;
        LastPublishTime = DateTime.Now;
        Statistics = new AdsPublishingTemplateStats(0, 0);
        Version = Platform == PlatformType.META ? MetaPublishTemplateVersion : 0;
        ResourceContent = ResourceContent ?? string.Empty;
    }

    /// <summary>
    /// 还原模板状态
    /// </summary>
    public void RestoreTemplateState()
    {
        IsDeleted = false;
    }

    /// <summary>
    /// 名称长度超过数据库长度截断
    /// </summary>
    public void NameSubstring()
    {
        if (Name.Length > 100)
            Name = Name[..100];
    }

    /// <summary>
    /// 获取模板内容
    /// </summary>
    public T? GetTemplateContent<T>() where T : class
    {
        return JsonSerializer.Deserialize<T>(TemplateContent);
    }

    /// <summary>
    /// 设置模板内容
    /// </summary>
    public void SetTemplateContent<T>(T content) where T : class
    {
        TemplateContent = JsonSerializer.Serialize(content);
    }

    /// <summary>
    /// 模板复制
    /// </summary>
    public void ReproductionTemplate()
    {
        Id = 0;
        Name = $"{Name}（复制）";
    }

    /// <summary>
    /// 获取 Meta 模板内容（含版本兼容逻辑）
    /// </summary>
    public MetaPublishDataBo? GetMetaTemplateContent()
    {
        if (Platform != PlatformType.META) return null;

        var model = JsonSerializer.Deserialize<MetaPublishDataBo>(TemplateContent);

        switch (Version)
        {
            case 0:
                {
                    model!.AdsetData.AdsAttributionSpec = new BusinessModel.Meta.MetaAdsPublishAttributionSpecBo();

                    if (model.AdsetData.AttributionSpec.NotNullOrEmpty())
                    {
                        switch (model.AdsetData.AttributionSpec)
                        {
                            case "CLICK_THROUGH_ONE_DAY":
                                model.AdsetData.AdsAttributionSpec.ClickThrough = "1";
                                break;
                            case "CLICK_THROUGH_SEVEN_DAY":
                                model.AdsetData.AdsAttributionSpec.ClickThrough = "7";
                                break;
                            case "CLICK_OR_VIEW_THROUGH_ONE_DAY":
                                model.AdsetData.AdsAttributionSpec.ClickThrough = "1";
                                model.AdsetData.AdsAttributionSpec.ViewThrough = "1";
                                break;
                            case "CLICK_THROUGH_SEVEN_DAY_OR_VIEW_THROUGH_ONE_DAY":
                                model.AdsetData.AdsAttributionSpec.ClickThrough = "7";
                                model.AdsetData.AdsAttributionSpec.ViewThrough = "1";
                                break;
                        }
                    }

                    var subdivisionPositionV1 = new List<BusinessModel.MediaAudience.MetaAdsPublishSubdivisionPositionBo>();

                    model!.AudienceData.SubdivisionPositionV1 = new List<List<BusinessModel.MediaAudience.MetaAdsPublishSubdivisionPositionBo>> { subdivisionPositionV1 };

                    // 设置版位
                    model.AudienceData.PositionPublisher = new BusinessModel.MediaAudience.MetaAdsPublishDataPositionChildrenBo();
                    model.AudienceData.PositionPublisher.Facebook = model.AudienceData.FacebookPositions ?? new List<string>();
                    model.AudienceData.PositionPublisher.Instagram = model.AudienceData.InstagramPositions ?? new List<string>();
                    model.AudienceData.PositionPublisher.AudienceNetwork = model.AudienceData.AudienceNetworkPositions ?? new List<string>();
                    model.AudienceData.PositionPublisher.Messenger = model.AudienceData.MessengerPositions ?? new List<string>();

                    model.AudienceData.Areas = new BusinessModel.MediaAudience.MetaAdsPublishDataAreaGroupAreaBo();

                    // 设置地区
                    if (model.AudienceData.CountryGroups.NotNullOrEmpty())
                    {
                        model.AudienceData.Areas.Includes.AddRange(model.AudienceData.CountryGroups!.Select(s =>
                            new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo
                            {
                                Key = s,
                                Type = "country_group"
                            }));
                    }

                    if (model.AudienceData.ExcludedCountryGroups.NotNullOrEmpty())
                    {
                        model.AudienceData.Areas.Excludes.AddRange(model.AudienceData.ExcludedCountryGroups!.Select(s =>
                            new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo
                            {
                                Key = s,
                                Type = "country_group"
                            }));
                    }

                    if (model.AudienceData.Countries.NotNullOrEmpty())
                    {
                        model.AudienceData.Countries!.ForEach(country =>
                        {
                            country.Countries.ForEach(_country =>
                            {
                                if (string.Equals(_country.Type, "COUNTRY", StringComparison.OrdinalIgnoreCase) && !_country.IsExcluded)
                                    model.AudienceData.Areas.Includes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "country" });
                                if (string.Equals(_country.Type, "REGION", StringComparison.OrdinalIgnoreCase) && !_country.IsExcluded)
                                    model.AudienceData.Areas.Includes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "region" });
                                if (string.Equals(_country.Type, "CITY", StringComparison.OrdinalIgnoreCase) && !_country.IsExcluded)
                                    model.AudienceData.Areas.Includes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "city" });
                                if (string.Equals(_country.Type, "COUNTRY", StringComparison.OrdinalIgnoreCase) && _country.IsExcluded)
                                    model.AudienceData.Areas.Excludes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "country" });
                                if (string.Equals(_country.Type, "REGION", StringComparison.OrdinalIgnoreCase) && _country.IsExcluded)
                                    model.AudienceData.Areas.Excludes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "region" });
                                if (string.Equals(_country.Type, "CITY", StringComparison.OrdinalIgnoreCase) && _country.IsExcluded)
                                    model.AudienceData.Areas.Excludes.Add(new BusinessModel.CrossPublishing.Meta.MetaCrossPublishDataAreaGroupAreaBo { Key = _country.Code, Type = "city" });
                            });
                        });
                    }
                }
                break;
        }

        return model;
    }
}

/// <summary>
/// 模板名称替换模型
/// </summary>
public sealed class TemplateNameRulesDto
{
    public TemplateNameRulesDto(string publisherName, string placementType, string conversionType,
        string appType, string gender, List<string> regionCodes, List<string>? langCodes)
    {
        PublisherName = publisherName;
        PlacementType = placementType;
        ConversionType = conversionType;
        AppType = appType;
        Gender = gender;
        RegionCodes = regionCodes.Select(s => s.ToLower()).ToList();
        LangCodes = langCodes?.Select(s => s.ToLower()).ToList();
    }

    /// <summary>发布人姓名</summary>
    public string PublisherName { get; set; } = null!;

    /// <summary>投放方式 - 设置细分定位（兴趣与行为）</summary>
    public string PlacementType { get; set; }

    /// <summary>转化方式 - 优化目标</summary>
    public string ConversionType { get; set; } = null!;

    /// <summary>应用平台（IOS，ANDROID，All）</summary>
    public string AppType { get; set; } = null!;

    /// <summary>性别</summary>
    public string Gender { get; set; } = null!;

    /// <summary>国家地区</summary>
    public List<string> RegionCodes { get; set; } = null!;

    /// <summary>语言</summary>
    public List<string>? LangCodes { get; set; } = null!;
}
