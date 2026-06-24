namespace Ads.Automation.Application.Contracts.Entity.Account;

/// <summary>
/// 获取广告账户列表 Input
/// </summary>
public class GetAdsAccountListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 账户名称/编号模糊过滤（同时匹配编号和名称）
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 账户编号筛选（精确或包含匹配）
    /// </summary>
    public string? AccountNo { get; set; }

    /// <summary>
    /// 账户名称筛选（模糊匹配）
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// 平台过滤
    /// </summary>
    public PlatformType? Platform { get; set; }

    /// <summary>
    /// 账户状态过滤
    /// </summary>
    public AdAccountState? AccountState { get; set; }

    /// <summary>
    /// 授权渠道ID过滤（通过 ads_channel_adaccounts 关联表筛选）
    /// </summary>
    public long? ChannelId { get; set; }
}
