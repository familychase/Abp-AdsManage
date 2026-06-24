namespace Ads.Automation.Domain.Shared.Enums.Publishing;

/// <summary>
/// 广告发布子类型（各平台细分）
/// </summary>
public enum AdsPublishingAdSubType
{
    /// <summary>普通广告</summary>
    STANDARD = 0,

    /// <summary>进阶赋能</summary>
    ADVANCE_ENERGIZE = 1,

    /// <summary>目录广告</summary>
    PRODUCT_CATALOG = 2,

    /// <summary>落地页</summary>
    LANDING_PAGE = 3,

    /// <summary>直跳商店</summary>
    JUMP_SHOP = 4,

    /// <summary>像素发布应用普通广告</summary>
    PIXEL_APPLICATION = 5,

    /// <summary>应用安装</summary>
    APP_INSTALL = 6,

    /// <summary>应用互动</summary>
    APP_INTERACT = 7,

    /// <summary>应用再营销</summary>
    APP_RETARGETING = 8,

    /// <summary>视频购物 - 商店</summary>
    VIDEO_PRODUCT = 9,

    /// <summary>视频购物 - 橱窗</summary>
    VIDEO_SHOWCASE = 10,

    /// <summary>直播购物</summary>
    LIVE = 11,

    /// <summary>直播购物 - 商店</summary>
    LIVE_STORE = 12,

    /// <summary>商品购物</summary>
    PRODUCT_SHOPPING = 13,

    /// <summary>老 Smart+ 推广系列</summary>
    WEB_SMART_SERIES = 14,

    /// <summary>新 Smart+ 推广系列</summary>
    SMART_WEB_CONVERSION = 15,

    /// <summary>Snapchat 应用安装</summary>
    SNAPCHAT_APP_INSTALL = 16,

    /// <summary>Snapchat 应用互动</summary>
    SNAPCHAT_APP_REENGAGEMENT = 17,

    /// <summary>Snapchat 销售额 - 网站</summary>
    SNAPCHAT_SALES = 18,

    /// <summary>Snapchat 销售额 - 应用</summary>
    SNAPCHAT_SALES_APP = 19,

    /// <summary>TikTok 应用小程序</summary>
    TIKTOK_APP_MINIS = 22,
}
