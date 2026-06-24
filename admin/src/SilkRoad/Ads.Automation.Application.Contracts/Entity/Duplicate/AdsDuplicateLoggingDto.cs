using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 广告复制日志 DTO
/// </summary>
public class AdsDuplicateLoggingDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 复制来源
    /// </summary>
    [JsonIgnore]
    public DuplicateSource DuplicateSource { get; set; }

    /// <summary>
    /// 来源ID
    /// </summary>
    [JsonIgnore]
    public long ResourceId { get; set; }

    /// <summary>
    /// 是否为账户内复制
    /// </summary>
    public bool IsInternal { get; set; }

    /// <summary>
    /// 广告对象层级
    /// </summary>
    public AdObjectLevel AdObjectLevel { get; set; }

    /// <summary>
    /// 广告对象编号（媒体编号）
    /// </summary>
    public string AdObjectNo { get; set; } = string.Empty;

    /// <summary>
    /// 账户编号
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 目标账户号
    /// </summary>
    [JsonIgnore]
    public string DuplicateAccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 公共主页编号
    /// </summary>
    public string PageNo { get; set; } = string.Empty;

    /// <summary>
    /// 复制状态
    /// </summary>
    public DuplicateState State { get; set; }

    /// <summary>
    /// 复制结果内容
    /// </summary>
    public string DuplicateContent { get; set; } = string.Empty;

    /// <summary>
    /// 计划执行时间（北京时间，格式 yyyy-MM-dd HH:mm:ss）
    /// </summary>
    public string ScheduleTime { get; set; } = string.Empty;

    /// <summary>
    /// 结束时间（北京时间，格式 yyyy-MM-dd HH:mm:ss）
    /// </summary>
    public string? EndTime { get; set; }

    /// <summary>
    /// 创建时间（北京时间，格式 yyyy-MM-dd HH:mm:ss）
    /// </summary>
    public string CreationTime { get; set; } = string.Empty;

    /// <summary>
    /// 创建人
    /// </summary>
    public long CreatorId { get; set; }

    /// <summary>
    /// 扩展数据
    /// </summary>
    public string ExtendedData { get; set; } = string.Empty;

    /// <summary>
    /// 复制数量
    /// </summary>
    public long CopyNumber { get; set; }

    /// <summary>
    /// 错误信息（媒体返回错误或代码异常详情）
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
