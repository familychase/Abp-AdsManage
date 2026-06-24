namespace Ads.Automation.Application.Contracts.Entity.Channel;

/// <summary>
/// 广告渠道列表 Dto
/// </summary>
public class AdsChannelListDto
{
    /// <summary>
    /// 个号 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 个号名称
    /// </summary>
    public string ChannelName { get; set; } = default!;

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 授权状态
    /// </summary>
    public ChannelStateType ChannelState { get; set; }
}
