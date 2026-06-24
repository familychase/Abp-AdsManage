using Ads.Automation.Domain.Publishing.BusinessModel.MediaAudience;
using Ads.Automation.Domain.Publishing.BusinessModel.Meta;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// Meta 平台的受众视图模型
/// </summary>
public class MetaAudienceViewModel
{
    /// <summary>是否手动版位</summary>
    public bool? IsManualPosition { get; set; }

    /// <summary>发布版位列表</summary>
    public List<string>? PublisherPositions { get; set; }

    /// <summary>Facebook 具体版位</summary>
    public List<string>? FacebookPositions { get; set; }

    /// <summary>Instagram 具体版位</summary>
    public List<string>? InstagramPositions { get; set; }

    /// <summary>Audience Network 具体版位</summary>
    public List<string>? AudienceNetworkPositions { get; set; }

    /// <summary>Messenger 具体版位</summary>
    public List<string>? MessengerPositions { get; set; }

    /// <summary>性别（0=所有，1=男性，2=女性）</summary>
    public int Gender { get; set; }

    /// <summary>最大年龄（上限 65）</summary>
    public int? AgeMax { get; set; }

    /// <summary>最小年龄（下限 13，默认 18）</summary>
    public int AgeMin { get; set; }

    /// <summary>包含的国家组列表</summary>
    public List<string>? CountryGroups { get; set; }

    /// <summary>排除的国家组列表</summary>
    public List<string>? ExcludedCountryGroups { get; set; }

    /// <summary>国家/区域列表</summary>
    public List<MetaAudienceCountryBo>? Countries { get; set; }

    ///// <summary>兴趣列表</summary>
    //public List<MetaAudienceInterestBo>? Interests { get; set; }

    ///// <summary>搜索的兴趣列表</summary>
    //public List<MetaAudienceInterestSearchBo>? SearchInterests { get; set; }

    /// <summary>语言列表</summary>
    public List<string>? Languages { get; set; }

    /// <summary>地区人群类型，默认为 HOME</summary>
    public string LocationType { get; set; } = null!;

    /// <summary>是否开启细分定位扩展</summary>
    public bool IsOpenTargetExtend { get; set; }

    /// <summary>应用平台（IOS / ANDROID）</summary>
    public string AppType { get; set; } = string.Empty;

    /// <summary>应用最低系统版本</summary>
    public string AppMinSystemVer { get; set; } = string.Empty;

    /// <summary>应用最高系统版本</summary>
    public string AppMaxSystemVer { get; set; } = string.Empty;

    /// <summary>包含的设备型号列表</summary>
    public List<string>? AppDevices { get; set; }

    /// <summary>排除的设备型号列表</summary>
    public List<string>? AppExcludedDevices { get; set; }

    /// <summary>是否仅 Wifi 连接</summary>
    public bool? OpenWirelessCarrier { get; set; }

    /// <summary>账户自定义受众列表</summary>
    public List<MediaPublishAccountAudienceBo>? Audiences { get; set; }

    /// <summary>排除可跳过的广告</summary>
    public bool? ExcludedAd { get; set; }

    /// <summary>Facebook 库存计划</summary>
    public MetaFacebookInventoryPlan FacebookInventoryPlan { get; set; }

    /// <summary>Audience Network 库存计划</summary>
    public MetaAudienceNetworkInventoryPlan NetworkInventoryPlan { get; set; }

    /// <summary>内容类型排除条件</summary>
    public bool ContentExclusionCondition { get; set; }

    /// <summary>细分定位（V1 版本，支持进一步限定）</summary>
    public List<List<MetaAdsPublishSubdivisionPositionBo>>? SubdivisionPositionV1 { get; set; }

    /// <summary>子版位</summary>
    public MetaAdsPublishDataPositionChildrenBo? PositionPublisher { get; set; } = null!;

    /// <summary>包含/排除地区列表</summary>
    public MetaAdsPublishDataAreaGroupAreaBo? Areas { get; set; } = null!;

    /// <summary>是否为进阶赋能受众</summary>
    public bool IsAdvancedAudience { get; set; }

    /// <summary>排除的细分定位</summary>
    public List<MetaAdsPublishSubdivisionPositionBo>? ExcludeSubdivisionPosition { get; set; }

    /// <summary>受众建议</summary>
    public bool? IsAudienceSuggest { get; set; }

    /// <summary>最小年龄限制</summary>
    public int? AgeMinLimit { get; set; }
}

/// <summary>
/// Meta 发布账户数据视图模型
/// </summary>
public class MetaPublishAccountDataViewModel
{
    /// <summary>广告账户 ID</summary>
    public long AccountId { get; set; }

    /// <summary>主页编号（Facebook Page ID）</summary>
    public string? PageNo { get; set; }
}
