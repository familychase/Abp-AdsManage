namespace Ads.Automation.Domain.Duplicate;

/// <summary>
/// 复制明细实体 —— 记录每次复制迭代的结果
/// 每次迭代创建一个新的广告对象（广告系列/广告组）后，记录其结果（成功/失败）
/// 用于后续补充、删除等精细化管理
/// </summary>
public class AdsDuplicateDetail : AggregateRootEntity
{
    /// <summary>
    /// 关联的复制日志 ID
    /// </summary>
    public long LogId { get; private set; }

    /// <summary>
    /// 迭代序号（第几次复制，1..CopyNumber）
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// 广告层级：广告系列(CAMPAIGN) 或 广告组(AD_SET)
    /// </summary>
    public AdObjectLevel AdObjectLevel { get; private set; }

    /// <summary>
    /// 新创建的广告对象编号（如 Meta Campaign ID 或 AdSet ID），为空表示创建失败
    /// </summary>
    public string AdObjectNo { get; private set; } = string.Empty;

    /// <summary>
    /// 本轮状态（SUCCESS / FAILED）
    /// </summary>
    public DuplicateState State { get; private set; }

    /// <summary>
    /// 错误信息（失败时记录）
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 创建内容详情（如创建广告系列:123, 创建广告组:456, 创建广告:789 等）
    /// </summary>
    public string? Content { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; private set; } = DateTime.Now;

    // ===== 构造函数 =====

    private AdsDuplicateDetail() { }

    internal AdsDuplicateDetail(long id, long logId, int index,
        AdObjectLevel adObjectLevel, string adObjectNo,
        DuplicateState state, string? errorMessage, string? content)
        : base(id)
    {
        LogId = logId;
        Index = index;
        AdObjectLevel = adObjectLevel;
        AdObjectNo = adObjectNo;
        State = state;
        ErrorMessage = errorMessage;
        Content = content;
        CreateTime = DateTime.Now;
    }

    public static AdsDuplicateDetail Create(
        long logId, int index, AdObjectLevel adObjectLevel, string adObjectNo,
        DuplicateState state, string? errorMessage = null, string? content = null)
    {
        return new AdsDuplicateDetail(
            IdGenerator.GetNextId(),
            logId,
            index,
            adObjectLevel,
            adObjectNo,
            state,
            errorMessage,
            content);
    }

    // ===== 更新方法 =====

    public void SetState(DuplicateState state) => State = state;
    public void SetErrorMessage(string message) => ErrorMessage = message;
    public void SetAdObjectNo(string adObjectNo) => AdObjectNo = adObjectNo;
}
