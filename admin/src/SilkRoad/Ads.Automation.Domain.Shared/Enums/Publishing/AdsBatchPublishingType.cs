namespace Ads.Automation.Domain.Shared.Enums.Publishing;

/// <summary>
/// 批量发布模式
/// </summary>
public enum AdsBatchPublishingType
{
    /// <summary>无</summary>
    NONE = 0,
    /// <summary>平均 - 所有账户发布总数</summary>
    AVG = 1,
    /// <summary>全量 - 单个账户发布结构</summary>
    ALL = 2,
}
