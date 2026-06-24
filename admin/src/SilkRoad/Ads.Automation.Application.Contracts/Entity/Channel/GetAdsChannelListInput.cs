namespace Ads.Automation.Application.Contracts.Entity.Channel;

/// <summary>
/// 获取广告渠道列表 Input
/// </summary>
public class GetAdsChannelListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 渠道名称过滤
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 平台过滤
    /// </summary>
    public PlatformType? Platform { get; set; }

    /// <summary>
    /// 授权状态过滤
    /// </summary>
    public ChannelStateType? ChannelState { get; set; }
}
