namespace Ads.Automation.Application.Contracts.Entity.SyncSchedule;

/// <summary>
/// 创建/修改同步调度计划 Dto
/// </summary>
public class CreateUpdateAdsSyncScheduleDto
{
    /// <summary>
    /// 动作类型
    /// </summary>
    [Required]
    public ActionType ActionType { get; set; }

    /// <summary>
    /// 资源主体标识
    /// </summary>
    [Required]
    [StringLength(256)]
    public string ResourceId { get; set; } = string.Empty;

    /// <summary>
    /// 资源主体类型
    /// </summary>
    [Required]
    public ResourceType ResourceType { get; set; }

    /// <summary>
    /// 媒体平台
    /// </summary>
    [Required]
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Required]
    [StringLength(256)]
    public string JobName { get; set; } = string.Empty;

    /// <summary>
    /// 扩展数据
    /// </summary>
    public string? ExtendingData { get; set; }

    /// <summary>
    /// 广告层级
    /// </summary>
    public int Level { get; set; } = 1;

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
    [Required]
    public DateTime NextPublishTime { get; set; }
}
