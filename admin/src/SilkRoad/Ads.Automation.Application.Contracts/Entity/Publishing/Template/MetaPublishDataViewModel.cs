using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// Meta 平台的发布数据视图模型
/// </summary>
public sealed class MetaPublishDataViewModel
{
    /// <summary>广告系列数据</summary>
    [JsonPropertyName("campaign_data")]
    public MetaCampaignViewModel CampaignData { get; set; } = null!;

    /// <summary>广告组数据</summary>
    [JsonPropertyName("adset_data")]
    public MetaAdsetViewModel AdsetData { get; set; } = null!;

    /// <summary>广告创意数据</summary>
    [JsonPropertyName("ad_data")]
    public MetaAdViewModel AdData { get; set; } = null!;

    /// <summary>受众数据</summary>
    [JsonPropertyName("audience_data")]
    public MetaAudienceViewModel AudienceData { get; set; } = null!;

    /// <summary>账户参数列表</summary>
    [JsonPropertyName("account_data")]
    public List<MetaPublishAccountDataViewModel>? AccountData { get; set; }
}
