namespace Ads.Automation.Domain.Shared.Enums.Publishing;

/// <summary>
/// 发布广告类型
/// </summary>
public enum AdsPublishingAdType
{
    /// <summary>
    /// 无
    /// </summary>
    NONE = 0,

    /// <summary>
    /// Meta 应用 / TikTok 应用推广 / Snapchat 应用推广
    /// </summary>
    APP = 1,

    /// <summary>
    /// Meta 落地页 / TikTok 网站转化量 / Snapchat 销售
    /// </summary>
    PIXEL = 2,

    /// <summary>
    /// 商品目录 / TikTok 商品销售
    /// </summary>
    PRODUCT_CATALOG = 3,

    /// <summary>
    /// Google 效果最大化
    /// </summary>
    PERFORMANCE_MAX = 4,

    /// <summary>
    /// Google 展示
    /// </summary>
    DISPLAY = 5,

    /// <summary>
    /// Google 应用（多渠道）
    /// </summary>
    MULTI_CHANNEL = 6,
}
