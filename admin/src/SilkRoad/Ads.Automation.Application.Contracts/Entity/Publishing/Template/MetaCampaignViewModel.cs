using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// Meta 平台的广告系列视图模型
/// </summary>
public class MetaCampaignViewModel
{
    /// <summary>
    /// 命名规则（支持占位符，如 #YEAR##MONTH##DAY#）
    /// </summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>广告结构命名分隔符</summary>
    public string? AdStructSplit { get; set; }

    /// <summary>购买类型，默认 AUCTION</summary>
    public string BuyingType { get; set; } = string.Empty;

    /// <summary>是否开启预算</summary>
    public bool IsOpenBudget { get; set; }

    /// <summary>预算类型（DAILY_BUDGET / LIFETIME_BUDGET）</summary>
    public string BudgetType { get; set; } = string.Empty;

    /// <summary>预算金额</summary>
    public float Budget { get; set; }

    /// <summary>竞价策略</summary>
    public string? BidStrategy { get; set; }

    /// <summary>投放节奏类型</summary>
    public string? PacingType { get; set; }

    /// <summary>是否开启 iOS 14+ 广告系列</summary>
    public bool? OpenIOS14 { get; set; }

    /// <summary>进阶赋能类型（智能推广类型）</summary>
    public AdsPublishingSmartPromotionType SmartPromotionType { get; set; }

    /// <summary>像素转化设置类型</summary>
    public MetaPixelConversionType? ConversionType { get; set; }

    /// <summary>是否为商品目录广告</summary>
    public bool IsProductCatalog { get; set; }

    /// <summary>是否开启特殊广告声明</summary>
    public bool IsOpenSpecialAd { get; set; }

    /// <summary>特殊广告类别列表</summary>
    public List<MetaSpecialAdStatementCategories>? SpecialAdCategories { get; set; }

    /// <summary>国家代码列表</summary>
    public List<MetaCrossPublishDataAreaGroupAreaBo>? CountryCodes { get; set; }

    /// <summary>章节 ID（动态参数）</summary>
    public string ChapterId { get; set; } = null!;

    /// <summary>章节名称（动态参数）</summary>
    public string ChapterName { get; set; } = null!;

    /// <summary>自定义备注（动态参数）</summary>
    public string CustomRemarks { get; set; } = string.Empty;

    /// <summary>自定义备注 2</summary>
    public string CustomRemarks2 { get; set; } = string.Empty;

    /// <summary>自定义备注 3</summary>
    public string CustomRemarks3 { get; set; } = string.Empty;

    /// <summary>自定义备注 4</summary>
    public string? CustomRemarks4 { get; set; }
}
