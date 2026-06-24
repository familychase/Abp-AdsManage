namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 字典项值类型（单位）
/// </summary>
public enum DictItemValueType : byte
{
    /// <summary>
    /// 未设置
    /// </summary>
    NONE = 0,

    /// <summary>
    /// 秒（s）
    /// </summary>
    TIME_SECOND = 1,

    /// <summary>
    /// 百分比（%）
    /// </summary>
    PERCENT = 2,

    /// <summary>
    /// 货币符号（$）
    /// </summary>
    CURRENCY = 3,

    /// <summary>
    /// 次数
    /// </summary>
    NUMBER = 4
}
