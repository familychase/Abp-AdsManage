namespace Ads.Automation.Application.Contracts.Entity.Channel;

/// <summary>
/// 创建/修改广告渠道 Dto
/// </summary>
public class CreateUpdateAdsChannelDto
{
    /// <summary>
    /// 渠道名称
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ChannelName { get; set; } = string.Empty;

    /// <summary>
    /// 媒体平台
    /// </summary>
    [Required]
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 授权状态
    /// </summary>
    [Required]
    public ChannelStateType ChannelState { get; set; }

    /// <summary>
    /// 授权类型（1=未设置, 2=全部账户, 3=当前账户）
    /// </summary>
    public AuditType AuditType { get; set; } = AuditType.NO_SETTING;

    /// <summary>
    /// 媒体经理级账户（Meta=经理账户, Google=MCC ID）
    /// </summary>
    [StringLength(200)]
    public string? ManagerId { get; set; }

    /// <summary>
    /// 是否为经理级账户
    /// </summary>
    public bool IsManager { get; set; }

    /// <summary>
    /// App Key
    /// </summary>
    [StringLength(500)]
    public string? AppKey { get; set; }

    /// <summary>
    /// App Secret
    /// </summary>
    [StringLength(500)]
    public string? AppSecret { get; set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    [StringLength(2000)]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [StringLength(2000)]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime? Expired { get; set; }

    /// <summary>
    /// 媒体身份标识
    /// </summary>
    [StringLength(200)]
    public string? MediaUserId { get; set; }
}
