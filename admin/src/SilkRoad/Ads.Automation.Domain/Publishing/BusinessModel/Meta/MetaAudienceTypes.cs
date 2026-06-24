namespace Ads.Automation.Domain.Publishing.BusinessModel.Meta;

/// <summary>
/// Meta 国家/区域受众模型
/// </summary>
public class MetaAudienceCountryBo
{
    /// <summary>地域类型（COUNTRY / REGION / CITY）</summary>
    public string? Type { get; set; }

    /// <summary>区域代码</summary>
    public string Code { get; set; } = null!;

    /// <summary>是否排除</summary>
    public bool IsExcluded { get; set; }

    /// <summary>子级国家列表（国家组展开）</summary>
    public List<MetaAudienceCountryBo>? Countries { get; set; }
}

/// <summary>
/// Meta Facebook 库存计划枚举
/// </summary>
public enum MetaFacebookInventoryPlan { NONE = 0 }

/// <summary>
/// Meta Audience Network 库存计划枚举
/// </summary>
public enum MetaAudienceNetworkInventoryPlan { NONE = 0 }

/// <summary>
/// Meta 发布设备平台枚举
/// </summary>
public enum MetaAdsPublishDevicePlatform { NONE = 0 }

/// <summary>
/// Meta 特殊广告陈述类别枚举
/// </summary>
public enum MetaSpecialAdStatementCategories { NONE = 0 }
