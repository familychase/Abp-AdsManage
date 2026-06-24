namespace Ads.Automation.Domain.Ads;

/// <summary>
/// 广告组实体
/// </summary>
public class AdSetEntity : AggregateRootEntity,IHasCreationTimeEntity, IHasModificationTimeEntity
{
    /// <summary>
    /// 广告账户Id
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 广告账户编号.
    /// </summary>
    public string AccountNo { get; set; } = null!;

    /// <summary>
    /// 广告系列Id
    /// </summary>
    public long CampaignId { get; set; }

    /// <summary>
    /// 广告系列编号.
    /// </summary>
    public string CampaignNo { get; set; } = null!;

    /// <summary>
    /// 广告组编号
    /// </summary>
    public string AdSetNo { get; set; } = null!;

    /// <summary>
    /// 广告组名称
    /// </summary>
    public string AdSetName { get; set; } = null!;

    /// <summary>
    /// 媒体方状态.
    /// </summary>
    public string MediaState { get; set; } = null!;

    /// <summary>
    /// 预算
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// 预算类型.
    /// </summary>
    public string BudgetType { get; set; } = null!;

    /// <summary>
    /// 媒体创建时间
    /// </summary>
    public DateTime MediaCreateTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }

    // ===== 构造函数 =====

    private AdSetEntity() { }

    internal AdSetEntity(long id)
        : base(id)
    {
    }

    /// <summary>
    /// 创建广告组
    /// </summary>
    public static AdSetEntity Create(
        long accountId,
        string accountNo,
        long campaignId,
        string campaignNo,
        string adSetNo,
        string adSetName,
        string mediaState,
        string budgetType,
        decimal budget,
        DateTime mediaCreateTime)
    {
        return new AdSetEntity(IdGenerator.GetNextId())
        {
            AccountId = accountId,
            AccountNo = accountNo,
            CampaignId = campaignId,
            CampaignNo = campaignNo,
            AdSetNo = adSetNo,
            AdSetName = adSetName,
            MediaState = mediaState,
            BudgetType = budgetType,
            Budget = budget,
            MediaCreateTime = mediaCreateTime,
            CreationTime = DateTime.Now,
            LastModificationTime = DateTime.Now,
        };
    }

    [NotMapped]
    public long CreatorId { get; set; }
    [NotMapped]
    public long? LastModifierId { get; set; }
}
