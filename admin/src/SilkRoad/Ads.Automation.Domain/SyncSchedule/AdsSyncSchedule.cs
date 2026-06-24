namespace Ads.Automation.Domain.SyncSchedule;

/// <summary>
/// 同步数据调度计划实体
/// </summary>
public class AdsSyncSchedule : AggregateRootEntity
{
    /// <summary>
    /// 动作类型
    /// </summary>
    public ActionType ActionType { get; private set; }

    /// <summary>
    /// 资源主体标识（渠道Id/广告账户编号等）
    /// </summary>
    public string ResourceId { get; private set; } = string.Empty;

    /// <summary>
    /// 资源主体类型
    /// </summary>
    public ResourceType ResourceType { get; private set; }

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string JobName { get; private set; } = string.Empty;

    /// <summary>
    /// 扩展数据
    /// </summary>
    public string? ExtendingData { get; private set; }

    /// <summary>
    /// 广告层级
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// 是否为受众报告
    /// </summary>
    public bool IsAudience { get; private set; }

    /// <summary>
    /// 关联数据（如手动任务ID）
    /// </summary>
    public string? LinkDate { get; private set; }

    /// <summary>
    /// 下次发布时间
    /// </summary>
    public DateTime NextPublishTime { get; private set; }

    // ===== 构造函数 =====

    private AdsSyncSchedule() { }

    public static AdsSyncSchedule Create(
        ActionType actionType,
        string resourceId,
        ResourceType resourceType,
        PlatformType platform,
        string jobName,
        string? extendingData = null,
        int level = 1,
        bool isAudience = false,
        string? linkDate = null,
        DateTime? nextPublishTime = null)
    {
        return new AdsSyncSchedule(
            IdGenerator.GetNextId(),
            actionType,
            resourceId,
            resourceType,
            platform,
            jobName,
            extendingData,
            level,
            isAudience,
            linkDate,
            nextPublishTime ?? DateTime.Now);
    }

    internal AdsSyncSchedule(
        long id,
        ActionType actionType,
        string resourceId,
        ResourceType resourceType,
        PlatformType platform,
        string jobName,
        string? extendingData = null,
        int level = 1,
        bool isAudience = false,
        string? linkDate = null,
        DateTime? nextPublishTime = null)
        : base(id)
    {
        ActionType = actionType;
        ResourceId = resourceId;
        ResourceType = resourceType;
        Platform = platform;
        JobName = jobName;
        ExtendingData = extendingData;
        Level = level;
        IsAudience = isAudience;
        LinkDate = linkDate;
        NextPublishTime = nextPublishTime ?? DateTime.Now;
    }

    // ===== 更新方法 =====

    public void SetActionType(ActionType actionType) => ActionType = actionType;
    public void SetResourceId(string resourceId) => ResourceId = resourceId;
    public void SetResourceType(ResourceType resourceType) => ResourceType = resourceType;
    public void SetPlatform(PlatformType platform) => Platform = platform;
    public void SetJobName(string jobName) => JobName = jobName;
    public void SetExtendingData(string? data) => ExtendingData = data;
    public void SetLevel(int level) => Level = level;
    public void SetIsAudience(bool isAudience) => IsAudience = isAudience;
    public void SetLinkDate(string? linkDate) => LinkDate = linkDate;
    public void SetNextPublishTime(DateTime time) => NextPublishTime = time;
}
