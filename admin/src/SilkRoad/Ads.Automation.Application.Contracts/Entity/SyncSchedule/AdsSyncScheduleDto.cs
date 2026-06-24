namespace Ads.Automation.Application.Contracts.Entity.SyncSchedule;

/// <summary>
/// 同步调度计划 Dto
/// </summary>
public class AdsSyncScheduleDto
{
    public long Id { get; set; }

    /// <summary>
    /// 动作类型
    /// </summary>
    public ActionType ActionType { get; set; }

    /// <summary>
    /// 资源主体标识
    /// </summary>
    public string ResourceId { get; set; } = default!;

    /// <summary>
    /// 资源主体类型
    /// </summary>
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; set; } = default!;

    /// <summary>
    /// 任务名称（国际化显示）
    /// </summary>
    public string JobNameDisplay { get; set; } = string.Empty;

    /// <summary>
    /// 扩展数据
    /// </summary>
    public string? ExtendingData { get; set; }

    /// <summary>
    /// 广告层级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 是否为受众报告
    /// </summary>
    public bool IsAudience { get; set; }

    /// <summary>
    /// 关联数据
    /// </summary>
    public string? LinkDate { get; set; }

    /// <summary>
    /// 下次发布时间
    /// </summary>
    public DateTime NextPublishTime { get; set; }
}
