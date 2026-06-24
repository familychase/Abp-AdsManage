using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// Meta 平台的广告组视图模型
/// </summary>
public class MetaAdsetViewModel
{
    /// <summary>命名规则（支持占位符）</summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>是否开启预算</summary>
    public bool IsOpenBudget { get; set; }

    /// <summary>
    /// 预算类型（DAILY_BUDGET / LIFETIME_BUDGET）
    /// 字典枚举：MetaBudgetType
    /// </summary>
    public string BudgetType { get; set; } = string.Empty;

    /// <summary>预算金额</summary>
    public float Budget { get; set; }

    /// <summary>
    /// 竞价策略
    /// 媒体字典：MetaBidStrategy
    /// </summary>
    public string? BidStrategy { get; set; }

    /// <summary>出价金额</summary>
    public float? BidAmount { get; set; }

    /// <summary>
    /// 投放节奏类型
    /// 枚举：MetaPacingType
    /// </summary>
    public string? PacingType { get; set; }

    /// <summary>是否开启花费上限</summary>
    public bool IsOpenSpendLimit { get; set; }

    /// <summary>最低花费上限</summary>
    public decimal? MinSpendLimit { get; set; }

    /// <summary>花费上限</summary>
    public decimal? SpendLimit { get; set; }

    /// <summary>
    /// 优化目标（如 OFFSITE_CONVERSIONS）
    /// 媒体字典：MetaOptimizationGoal
    /// </summary>
    public string OptimizationGoal { get; set; } = string.Empty;

    /// <summary>
    /// 应用事件类型
    /// 媒体字典：MetaCustomEventType
    /// </summary>
    public string AppEventType { get; set; } = null!;

    /// <summary>
    /// 像素事件
    /// 媒体字典：MetaPixelCodeStandardEvent
    /// </summary>
    public string? CustomEventType { get; set; }

    /// <summary>
    /// 事件类型（标准事件 / 自定义事件）
    /// 媒体字典：MetaAppEventType
    /// </summary>
    public string? EventType { get; set; }

    /// <summary>自定义事件 ID</summary>
    public long? CustomEventId { get; set; }

    /// <summary>
    /// 计费事件
    /// 媒体字典：MetaBillingEvent，默认IMPRESSIONS，展示数，当前只能选取展示数
    /// </summary>
    public string BillingEvent { get; set; } = string.Empty;

    ///// <summary>
    ///// 归因窗口
    ///// 系统字典：MetaAttributionSpec
    ///// </summary>
    //[JsonPropertyName("attribution_spec")]
    //public MetaAdsPublishAttributionSpecViewModel? AdsAttributionSpec { get; set; }

    /// <summary>投放开始时间</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>投放结束时间</summary>
    public string? EndTime { get; set; }

    /// <summary>是否开启分时段投放</summary>
    public bool OpenTimeSchedule { get; set; }

    /// <summary>
    /// 时区类型
    /// 媒体字典：MetaTimeScheduleZoneType
    /// </summary>
    public string TimeScheduleZoneType { get; set; } = string.Empty;

    /// <summary>分时段投放时间表</summary>
    public List<List<long>>? TimeSchedule { get; set; }

    /// <summary>ROAS 出价</summary>
    public float? RoasBid { get; set; }

    /// <summary>时区类型</summary>
    public string? TimeZoneType { get; set; }
}
