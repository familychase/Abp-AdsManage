namespace Ads.Automation.Application.Contracts.Entity.SyncSchedule;

/// <summary>
/// 获取同步调度计划列表 Input
/// </summary>
public class GetAdsSyncScheduleListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 任务名称/资源ID过滤
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 动作类型过滤
    /// </summary>
    public ActionType? ActionType { get; set; }

    /// <summary>
    /// 平台过滤
    /// </summary>
    public PlatformType? Platform { get; set; }

    /// <summary>
    /// 资源类型过滤
    /// </summary>
    public ResourceType? ResourceType { get; set; }

    /// <summary>
    /// 资源主体标识精确匹配
    /// </summary>
    public string? ResourceId { get; set; }
}
