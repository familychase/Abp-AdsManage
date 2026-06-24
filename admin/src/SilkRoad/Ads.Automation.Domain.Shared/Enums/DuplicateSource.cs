namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 广告复制来源
/// </summary>
public enum DuplicateSource : byte
{
    /// <summary>
    /// 广告管理
    /// </summary>
    ADS_MANAGEMENT = 0,

    /// <summary>
    /// 广告策略
    /// </summary>
    STRATEGY = 1,

    /// <summary>
    /// 小程序
    /// </summary>
    MINI_PROGRAM = 2

}
