namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 媒体平台类型
/// </summary>
public enum PlatformType : byte
{
    /// <summary>
    /// 无
    /// </summary>
    NONE = 0,

    /// <summary>
    /// Google Ads
    /// </summary>
    GOOGLE = 1,

    /// <summary>
    /// Meta (Facebook/Instagram)
    /// </summary>
    META = 2,

    /// <summary>
    /// TikTok
    /// </summary>
    TIKTOK = 3
}
