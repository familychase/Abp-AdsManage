using Volo.Abp.Auditing;

namespace Ads.Automation.Domain.Ads;

/// <summary>
/// 广告系列实体
/// </summary>
public class AdCampaignEntity : AggregateRootEntity,IHasCreationTimeEntity, IHasModificationTimeEntity
{
    /// <summary>
    /// 广告账户Id
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 广告账户编号
    /// </summary>
    public string AccountNo { get; set; } = null!;

    /// <summary>
    /// 系列编号
    /// </summary>
    public string CampaignNo { get; set; } = null!;

    /// <summary>
    /// 系列名称
    /// </summary>
    public string CampaignName { get; set; } = null!;

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string MediaState { get; set; } = null!;

    /// <summary>
    /// 预算类型
    /// </summary>
    public string BudgetType { get; set; } = null!;

    /// <summary>
    /// 预算每日上限
    /// </summary>
    public decimal Budget { get; set; }

    /// <summary>
    /// 推广目标
    /// </summary>
    public string Objective { get; set; } = null!;

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

    private AdCampaignEntity() { }

    internal AdCampaignEntity(long id)
        : base(id)
    {
    }

    /// <summary>
    /// 创建广告系列
    /// </summary>
    public static AdCampaignEntity Create(
        long accountId,
        string accountNo,
        string campaignNo,
        string campaignName,
        string mediaState,
        string budgetType,
        decimal budget,
        string objective,
        DateTime mediaCreateTime)
    {
        return new AdCampaignEntity(IdGenerator.GetNextId())
        {
            AccountId = accountId,
            AccountNo = accountNo,
            CampaignNo = campaignNo,
            CampaignName = campaignName,
            MediaState = mediaState,
            BudgetType = budgetType,
            Budget = budget,
            Objective = objective,
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
