using Ads.Automation.Domain.Publishing.BusinessModel.CrossPublishing.Meta;
using Ads.Automation.Domain.Shared.Enums.Publishing;

namespace Ads.Automation.Domain.Publishing.BusinessModel.MediaAudience;

/// <summary>
/// 媒体发布账户受众视图模型
/// </summary>
public sealed record MediaPublishAccountAudienceBo
{
    /// <summary>广告账户 ID</summary>
    public long AccountId { get; set; }

    /// <summary>包含的自定义受众 ID 列表</summary>
    public List<long>? AudienceIds { get; set; }

    /// <summary>排除的自定义受众 ID 列表</summary>
    public List<long>? ExcludedAudienceIds { get; set; }
}

/// <summary>
/// Meta 细分定位（人口数据统计、兴趣、行为）
/// </summary>
public class MetaAdsPublishSubdivisionPositionBo
{
    /// <summary>细分定位类型</summary>
    public MetaSubdivisionPositionType Type { get; set; }

    /// <summary>定位路径（层级路径）</summary>
    public List<string> Path { get; set; } = null!;

    /// <summary>定位名称（显示名）</summary>
    public string Name { get; set; } = null!;

    /// <summary>定位值（投放 Key）</summary>
    public string Value { get; set; } = null!;
}

/// <summary>
/// Meta 子版位模板（继承跨平台子版位基类）
/// </summary>
public class MetaAdsPublishDataPositionChildrenBo : MetaCrossPublishDataPositionChildrenBo { }

/// <summary>
/// Meta 包含与排除地区的视图模型
/// </summary>
public class MetaAdsPublishDataAreaGroupAreaBo : MetaCrossPublishDataAreaGroupAreaViewBo
{
    public MetaAdsPublishDataAreaGroupAreaBo()
    {
        Includes ??= new List<MetaCrossPublishDataAreaGroupAreaBo>();
        Excludes ??= new List<MetaCrossPublishDataAreaGroupAreaBo>();
    }
}
