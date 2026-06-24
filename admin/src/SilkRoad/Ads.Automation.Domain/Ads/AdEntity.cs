namespace Ads.Automation.Domain.Ads;

/// <summary>
/// 广告实体
/// </summary>
public class AdEntity : AggregateRootEntity,IHasCreationTimeEntity, IHasModificationTimeEntity
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
    /// 广告系列Id
    /// </summary>
    public long CampaignId { get; set; }

    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = null!;

    /// <summary>
    /// 广告组Id
    /// </summary>
    public long AdSetId { get; set; }

    /// <summary>
    /// 广告组编号
    /// </summary>
    public string AdSetNo { get; set; } = null!;

    /// <summary>
    /// 广告编号
    /// </summary>
    public string AdNo { get; set; } = null!;

    /// <summary>
    /// 广告名称
    /// </summary>
    public string AdName { get; set; } = null!;

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string MediaState { get; set; } = null!;

    /// <summary>
    /// 广告创意编号
    /// </summary>
    public string CreativeNo { get; set; } = null!;

    /// <summary>
    /// 主页编号
    /// </summary>
    public string PageNo { get; set; } = null!;

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

    private AdEntity() { }

    internal AdEntity(long id)
        : base(id)
    {
    }

    /// <summary>
    /// 创建广告
    /// </summary>
    public static AdEntity Create(
        long accountId,
        string accountNo,
        long campaignId,
        string campaignNo,
        long adSetId,
        string adSetNo,
        string adNo,
        string adName,
        string mediaState,
        string creativeNo,
        string pageNo,
        DateTime mediaCreateTime)
    {
        return new AdEntity(IdGenerator.GetNextId())
        {
            AccountId = accountId,
            AccountNo = accountNo,
            CampaignId = campaignId,
            CampaignNo = campaignNo,
            AdSetId = adSetId,
            AdSetNo = adSetNo,
            AdNo = adNo,
            AdName = adName,
            MediaState = mediaState,
            CreativeNo = creativeNo,
            PageNo = pageNo,
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
