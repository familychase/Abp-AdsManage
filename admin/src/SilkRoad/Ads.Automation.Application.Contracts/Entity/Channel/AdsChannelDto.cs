namespace Ads.Automation.Application.Contracts.Entity.Channel;

/// <summary>
/// 广告渠道 Dto
/// </summary>
public class AdsChannelDto
{
    public long Id { get; set; }

    /// <summary>
    /// 渠道名称
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

    /// <summary>
    /// 授权类型
    /// </summary>
    public AuditType AuditType { get; set; }

    /// <summary>
    /// 媒体经理级账户（Meta=经理账户, Google=MCC ID）
    /// </summary>
    public string? ManagerId { get; set; }

    /// <summary>
    /// 是否为经理级账户
    /// </summary>
    public bool IsManager { get; set; }

    /// <summary>
    /// App Key
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// App Secret
    /// </summary>
    public string? AppSecret { get; set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime? Expired { get; set; }

    /// <summary>
    /// 媒体身份标识
    /// </summary>
    public string? MediaUserId { get; set; }

    /// <summary>
    /// 授权丢失原因
    /// </summary>
    public string? AuthMissingReason { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    public long CreatorId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public long? LastModifierId { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}
