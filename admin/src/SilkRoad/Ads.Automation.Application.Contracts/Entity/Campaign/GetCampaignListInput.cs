namespace Ads.Automation.Application.Contracts.Entity.Campaign;

/// <summary>
/// 广告系列列表查询入参
/// </summary>
public class GetCampaignListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 广告账户Ids
    /// </summary>
    public List<long>? AccountIds { get; set; }

    /// <summary>
    /// 广告系列编号（精确匹配）
    /// </summary>
    public string? CampaignNo { get; set; }

    /// <summary>
    /// 媒体平台（精确匹配）
    /// </summary>
    public PlatformType? Platform { get; set; }

    /// <summary>
    /// 广告系列名称（模糊搜索）
    /// </summary>
    public string? CampaignName { get; set; }
}
