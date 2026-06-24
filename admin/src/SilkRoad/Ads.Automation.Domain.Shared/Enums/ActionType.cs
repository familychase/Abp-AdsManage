namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 同步动作类型
/// </summary>
public enum ActionType : byte
{
    /// <summary>
    /// 无
    /// </summary>
    NONE = 0,

    /// <summary>
    /// 自动同步
    /// </summary>
    AUTOMATIC = 1,

    /// <summary>
    /// 手动同步
    /// </summary>
    MANUAL = 2,
}
