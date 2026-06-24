using Ads.Automation.Domain.Shared.Enums;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// 发布模板列表查询输入参数
/// </summary>
public class AdsPublishTemplateListInput : BasePagedAndSortedRequestDto
{
    /// <summary>模板名称（模糊匹配，同时解析模板 ID）</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>媒体平台</summary>
    [JsonPropertyName("platform")]
    public PlatformType? Platform { get; set; }

    /// <summary>应用 ID</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>像素 ID</summary>
    [JsonPropertyName("pixel_id")]
    public long? PixelId { get; set; }

    /// <summary>创建人 ID</summary>
    [JsonPropertyName("creator_id")]
    public long? CreatorId { get; set; }

    /// <summary>
    /// 时间范围
    /// </summary>
    public DateRange? DateRange { get; set; }
}
