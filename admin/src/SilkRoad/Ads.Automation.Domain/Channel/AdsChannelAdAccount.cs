using Volo.Abp.Domain.Entities;

namespace Ads.Automation.Domain.Channel;

/// <summary>
/// 渠道-账户关联实体（多对多关系）
/// </summary>
public class AdsChannelAdAccount : Entity<long>
{
    /// <summary>
    /// 账户 ID
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 渠道 ID
    /// </summary>
    public long ChannelId { get; set; }

    /// <summary>
    /// 媒体账户编号（渠道内的账户标识）
    /// </summary>
    public string AccountNo { get; set; } = string.Empty;
}
