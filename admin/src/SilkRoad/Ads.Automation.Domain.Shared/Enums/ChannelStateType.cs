namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 渠道授权状态
/// </summary>
public enum ChannelStateType : byte
{
    /// <summary>
    /// 无
    /// </summary>
    NONE = 0,

    /// <summary>
    /// 已授权
    /// </summary>
    ACTIVE = 1,

    /// <summary>
    /// 授权丢失
    /// </summary>
    MISSING = 2,
}
