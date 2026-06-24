namespace Ads.Automation.Application.Contracts.Entity.Account;

/// <summary>
/// 创建/修改广告账户 Dto
/// </summary>
public class CreateUpdateAdsAccountDto
{
    /// <summary>
    /// 账户编号
    /// </summary>
    public string? AccountNo { get; set; }

    /// <summary>
    /// 账户名称
    /// </summary>
    [Required]
    public string? AccountName { get; set; }

    /// <summary>
    /// 账户状态（0=NONE, 1=ACTIVE, 2=DISABLED, 3=PENDING）
    /// </summary>
    [Required]
    public AdAccountState AccountState { get; set; }

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string? MediaState { get; set; }

    /// <summary>
    /// 余额
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// UTC 时区偏移
    /// </summary>
    public string? UtcTimezoneOffset { get; set; }

    /// <summary>
    /// 媒体平台
    /// </summary>
    [Required]
    public PlatformType Platform { get; set; }

    /// <summary>
    /// 所有者 ID
    /// </summary>
    [Required]
    public long OwnerId { get; set; }

    /// <summary>
    /// 所有者团队 ID
    /// </summary>
    [Required]
    public long OwnerTeamId { get; set; }

    /// <summary>
    /// 是否为经理级账户
    /// </summary>
    public bool IsManager { get; set; }

    /// <summary>
    /// 币种
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// 是否受限
    /// </summary>
    public bool IsLimit { get; set; }

    /// <summary>
    /// 媒体禁用原因
    /// </summary>
    public string? MediaDisableReason { get; set; }

    /// <summary>
    /// 媒体创建时间
    /// </summary>
    public DateTime MediaCreatedTime { get; set; }

    /// <summary>
    /// 账户运行时长
    /// </summary>
    public decimal AccountRunningTime { get; set; }
}
