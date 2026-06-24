namespace Ads.Automation.Application.Contracts.Entity.Ads;

/// <summary>
/// 获取广告列表入参
/// </summary>
public class GetAdListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 广告账户Ids
    /// </summary>
    public List<long>? AccountIds { get; set; }

    /// <summary>
    /// 广告系列Ids
    /// </summary>
    public List<long>? CampaignIds { get; set; }

    /// <summary>
    /// 广告组Ids
    /// </summary>
    public List<long>? AdSetIds { get; set; }

    /// <summary>
    /// 广告编号（精确匹配）
    /// </summary>
    public string? AdNo { get; set; }

    /// <summary>
    /// 媒体平台（精确匹配）
    /// </summary>
    public PlatformType? Platform { get; set; }

    /// <summary>
    /// 广告名称（模糊搜索）
    /// </summary>
    public string? AdName { get; set; }
}
