namespace Ads.Automation.Domain.Account;

/// <summary>
/// 广告账户与主页关联表
/// </summary>
public class AdsAccountPages : AggregateRootEntity
{
    /// <summary>
    /// 广告账户Id
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 主页Id
    /// </summary>
    public long PageId { get; set; }

    /// <summary>
    /// 广告账户编号
    /// </summary>
    public string AccountNo { get; set; } = null!;

    // ===== 构造函数 =====

    private AdsAccountPages() { }

    public static AdsAccountPages Create(long accountId, long pageId, string accountNo)
    {
        return new AdsAccountPages
        {
            AccountId = accountId,
            PageId = pageId,
            AccountNo = accountNo
        };
    }
}
