namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 广告复制状态
/// </summary>
public enum DuplicateState : byte
{
    /// <summary>
    /// 未开始（已提交，等待执行）
    /// </summary>
    PENDING = 0,

    /// <summary>
    /// 成功
    /// </summary>
    SUCCESS = 1,

    /// <summary>
    /// 失败
    /// </summary>
    FAILED = 2,

    /// <summary>
    /// 进行中
    /// </summary>
    IN_PROGRESS = 3
}
