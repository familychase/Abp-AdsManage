using Ads.Automation.Domain.Shared.Enums;
using Ads.Automation.Domain.Shared.Enums.Publishing;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// 广告发布模板的视图模型
/// </summary>
public class AdsPublishTemplateViewModel
{
    /// <summary>版本号</summary>
    public int? Version { get; set; }

    /// <summary>模板 ID</summary>
    public long TemplateId { get; set; }

    /// <summary>模板名称</summary>
    public string TemplateName { get; set; } = null!;

    /// <summary>媒体平台</summary>
    public PlatformType Platform { get; set; }

    /// <summary>发布广告类型</summary>
    public AdsPublishingAdType PublishingAdType { get; set; }

    /// <summary>应用 ID</summary>
    public long? ApplicationId { get; set; }

    /// <summary>像素 ID</summary>
    public long? PixelId { get; set; }

    /// <summary>商品目录 ID</summary>
    public long? ProductCatalogId { get; set; }

    /// <summary>来源网站内容</summary>
    public string ResourceContent { get; set; } = null!;

    /// <summary>批量发布类型</summary>
    public AdsBatchPublishingType PublishingType { get; set; }

    /// <summary>最大发布条数</summary>
    public int MaxPublishCount { get; set; }

    /// <summary>是否平均发布</summary>
    public bool PublishAverage { get; set; }

    /// <summary>Meta 平台专属发布数据</summary>
    public MetaPublishDataViewModel? MetaPublishingData { get; set; }
}
