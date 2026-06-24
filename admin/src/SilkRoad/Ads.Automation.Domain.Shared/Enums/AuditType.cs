namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 授权类型
/// </summary>
public enum AuditType : byte
{
    /// <summary>
    /// 未设置
    /// </summary>
    NO_SETTING = 1,

    /// <summary>
    /// 全部账户
    /// </summary>
    ALL_ACCOUNT = 2,

    /// <summary>
    /// 当前账户
    /// </summary>
    CURRENT_ACCOUNT = 3,
}
