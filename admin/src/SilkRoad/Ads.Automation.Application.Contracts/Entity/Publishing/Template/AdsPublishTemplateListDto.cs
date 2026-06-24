using Ads.Automation.Domain.Shared.Enums;
using Ads.Automation.Domain.Shared.Enums.Publishing;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// 发布模板列表项 DTO
/// </summary>
public class AdsPublishTemplateListDto
{
    /// <summary>模板 ID</summary>
    [JsonPropertyName("template_id")]
    public long Id { get; set; }

    /// <summary>版本号（V{N}）</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>模板名称</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>媒体平台</summary>
    [JsonPropertyName("platform")]
    public PlatformType Platform { get; set; }

    /// <summary>发布广告类型</summary>
    [JsonPropertyName("publishing_ad_type")]
    public AdsPublishingAdType PublishingAdType { get; set; }

    /// <summary>关联的资源 ID（应用/像素）</summary>
    [JsonPropertyName("resource_id")]
    public long ResourceId { get; set; }

    /// <summary>累计发布次数</summary>
    [JsonPropertyName("publish_ad_count")]
    public int PublishAdCount { get; set; }

    /// <summary>末次发布时间</summary>
    [JsonPropertyName("last_publish_time")]
    public DateTime? LastPublishTime { get; set; }

    /// <summary>创建时间</summary>
    [JsonPropertyName("create_time")]
    public DateTime CreationTime { get; set; }

    /// <summary>最后修改时间</summary>
    [JsonPropertyName("last_modification_time")]
    public DateTime? LastModificationTime { get; set; }

    /// <summary>创建人 ID</summary>
    [JsonPropertyName("creator_id")]
    public long CreatorId { get; set; }

    /// <summary>删除时间</summary>
    [JsonPropertyName("delete_time")]
    public DateTime? DeletionTime { get; set; }
}
