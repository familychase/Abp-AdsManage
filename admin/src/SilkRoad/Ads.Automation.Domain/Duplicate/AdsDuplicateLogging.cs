using Volo.Abp.Auditing;

namespace Ads.Automation.Domain.Duplicate;

/// <summary>
/// 广告复制日志实体
/// </summary>
public class AdsDuplicateLogging : AggregateRootEntity,IHasCreationTimeEntity
{
    /// <summary>
    /// 复制来源
    /// </summary>
    public DuplicateSource DuplicateSource { get; private set; }

    /// <summary>
    /// 来源ID（广告策略等，默认 0）
    /// </summary>
    public long ResourceId { get; private set; }

    /// <summary>
    /// 是否为账户内复制
    /// </summary>
    public bool IsInternal { get; private set; }

    /// <summary>
    /// 广告对象层级
    /// </summary>
    public AdObjectLevel AdObjectLevel { get; private set; }

    /// <summary>
    /// 广告对象编号（媒体编号，CampaignNo 或 AdSetNo）
    /// </summary>
    public string AdObjectNo { get; private set; } = string.Empty;

    /// <summary>
    /// 账户编号
    /// </summary>
    public string AccountNo { get; private set; } = string.Empty;

    /// <summary>
    /// 目标账户号（跨账户复制时的目标账户）
    /// </summary>
    public string DuplicateAccountNo { get; private set; } = string.Empty;

    /// <summary>
    /// 公共主页编号
    /// </summary>
    public string PageNo { get; private set; } = string.Empty;

    /// <summary>
    /// 复制状态
    /// </summary>
    public DuplicateState State { get; private set; }

    /// <summary>
    /// 复制结果内容
    /// </summary>
    public string DuplicateContent { get; private set; } = string.Empty;

    /// <summary>
    /// 计划执行时间（延迟复制时使用）
    /// </summary>
    public DateTime ScheduleTime { get; private set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// 扩展数据（JSON，存储 Job 执行所需参数如 ChannelId、TargetPageNo 等）
    /// </summary>
    public string ExtendedData { get; private set; } = string.Empty;

    /// <summary>
    /// 复制数量
    /// </summary>
    public long CopyNumber { get; private set; }

    /// <summary>
    /// 错误信息（媒体返回错误或代码异常详情）
    /// </summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    // ===== 构造函数 =====

    private AdsDuplicateLogging() { }

    public static AdsDuplicateLogging Create(
        DuplicateSource duplicateSource,
        long resourceId,
        bool isInternal,
        AdObjectLevel adObjectLevel,
        string adObjectNo,
        string accountNo,
        string duplicateAccountNo,
        string pageNo,
        DateTime scheduleTime,
        long createBy,
        string extendedData = "",
        long copyNumber = 1)
    {
        return new AdsDuplicateLogging(
            IdGenerator.GetNextId(),
            duplicateSource,
            resourceId,
            isInternal,
            adObjectLevel,
            adObjectNo,
            accountNo,
            duplicateAccountNo,
            pageNo,
            DuplicateState.PENDING,
            string.Empty,
            scheduleTime,
            DateTime.Now,
            createBy,
            extendedData,
            copyNumber);
    }

    internal AdsDuplicateLogging(
        long id,
        DuplicateSource duplicateSource,
        long resourceId,
        bool isInternal,
        AdObjectLevel adObjectLevel,
        string adObjectNo,
        string accountNo,
        string duplicateAccountNo,
        string pageNo,
        DuplicateState state,
        string duplicateContent,
        DateTime scheduleTime,
        DateTime createTime,
        long createBy,
        string extendedData,
        long copyNumber = 1)
        : base(id)
    {
        DuplicateSource = duplicateSource;
        ResourceId = resourceId;
        IsInternal = isInternal;
        AdObjectLevel = adObjectLevel;
        AdObjectNo = adObjectNo;
        AccountNo = accountNo;
        DuplicateAccountNo = duplicateAccountNo;
        PageNo = pageNo;
        State = state;
        DuplicateContent = duplicateContent;
        ScheduleTime = scheduleTime;
        CreationTime = createTime;
        CreatorId = createBy;
        ExtendedData = extendedData;
        CopyNumber = copyNumber;
        ErrorMessage = string.Empty;
    }

    // ===== 更新方法 =====

    public void SetState(DuplicateState state) => State = state;

    public void SetDuplicateContent(string content) => DuplicateContent = content;

    public void SetDuplicateAccountNo(string accountNo) => DuplicateAccountNo = accountNo;

    public void SetPageNo(string pageNo) => PageNo = pageNo;

    public void SetScheduleTime(DateTime scheduleTime) => ScheduleTime = scheduleTime;

    public void SetEndTime(DateTime? endTime) => EndTime = endTime;

    public void SetExtendedData(string extendedData) => ExtendedData = extendedData;

    /// <summary>
    /// 设置错误信息
    /// </summary>
    public void SetErrorMessage(string errorMessage)
    {
        ErrorMessage = string.IsNullOrEmpty(ErrorMessage)
            ? errorMessage
            : $"{ErrorMessage}; {errorMessage}";
    }

    public DateTime CreationTime { get; }
    public long CreatorId { get; set; }
}
