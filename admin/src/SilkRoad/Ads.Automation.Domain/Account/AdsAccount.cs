namespace Ads.Automation.Domain.Account;

/// <summary>
/// 广告账户实体
/// </summary>
public class AdsAccount : AggregateRootEntity
{
    /// <summary>
    /// 账户编号
    /// </summary>
    public string? AccountNo { get; private set; }

    /// <summary>
    /// 账户名称
    /// </summary>
    public string? AccountName { get; private set; }

    /// <summary>
    /// 账户状态（0=NONE, 1=ACTIVE, 2=DISABLED, 3=PENDING）
    /// </summary>
    public AdAccountState AccountState { get; private set; }

    /// <summary>
    /// 媒体状态
    /// </summary>
    public string? MediaState { get; private set; }

    /// <summary>
    /// 余额
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// 时区
    /// </summary>
    public string? Timezone { get; private set; }

    /// <summary>
    /// UTC 时区偏移
    /// </summary>
    public string? UtcTimezoneOffset { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime LastModificationTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 所有者 ID
    /// </summary>
    public long OwnerId { get; private set; }

    /// <summary>
    /// 所有者团队 ID
    /// </summary>
    public long OwnerTeamId { get; private set; }

    /// <summary>
    /// 是否为经理级账户
    /// </summary>
    public bool IsManager { get; private set; }

    /// <summary>
    /// 币种
    /// </summary>
    public string? Currency { get; private set; }

    /// <summary>
    /// 是否受限
    /// </summary>
    public bool IsLimit { get; private set; }

    /// <summary>
    /// 媒体禁用原因
    /// </summary>
    public string? MediaDisableReason { get; private set; }

    /// <summary>
    /// 媒体创建时间
    /// </summary>
    public DateTime MediaCreatedTime { get; private set; }

    /// <summary>
    /// 账户运行时长
    /// </summary>
    public decimal AccountRunningTime { get; private set; }

    // ===== 构造函数 =====

    private AdsAccount() { }

    public static AdsAccount Create(
        string? accountNo,
        string? accountName,
        AdAccountState accountState,
        string? mediaState,
        decimal balance,
        string? timezone,
        string? utcTimezoneOffset,
        PlatformType platform,
        long ownerId,
        long ownerTeamId,
        bool isManager,
        string? currency,
        bool isLimit,
        string? mediaDisableReason,
        DateTime mediaCreatedTime,
        decimal accountRunningTime)
    {
        return new AdsAccount(
            IdGenerator.GetNextId(),
            accountNo,
            accountName,
            accountState,
            mediaState,
            balance,
            timezone,
            utcTimezoneOffset,
            platform,
            ownerId,
            ownerTeamId,
            isManager,
            currency,
            isLimit,
            mediaDisableReason,
            mediaCreatedTime,
            accountRunningTime);
    }

    internal AdsAccount(
        long id,
        string? accountNo,
        string? accountName,
        AdAccountState accountState,
        string? mediaState,
        decimal balance,
        string? timezone,
        string? utcTimezoneOffset,
        PlatformType platform,
        long ownerId,
        long ownerTeamId,
        bool isManager,
        string? currency,
        bool isLimit,
        string? mediaDisableReason,
        DateTime mediaCreatedTime,
        decimal accountRunningTime)
        : base(id)
    {
        AccountNo = accountNo;
        AccountName = accountName;
        AccountState = accountState;
        MediaState = mediaState;
        Balance = balance;
        Timezone = timezone;
        UtcTimezoneOffset = utcTimezoneOffset;
        Platform = platform;
        OwnerId = ownerId;
        OwnerTeamId = ownerTeamId;
        IsManager = isManager;
        Currency = currency;
        IsLimit = isLimit;
        MediaDisableReason = mediaDisableReason;
        MediaCreatedTime = mediaCreatedTime;
        AccountRunningTime = accountRunningTime;
    }

    // ===== 更新方法 =====

    public void SetAccountNo(string? accountNo) => AccountNo = accountNo;
    public void SetAccountName(string? accountName) => AccountName = accountName;
    public void SetAccountState(AdAccountState accountState) => AccountState = accountState;
    public void SetMediaState(string? mediaState) => MediaState = mediaState;
    public void SetBalance(decimal balance) => Balance = balance;
    public void SetTimezone(string? timezone) => Timezone = timezone;
    public void SetUtcTimezoneOffset(string? utcTimezoneOffset) => UtcTimezoneOffset = utcTimezoneOffset;
    public void SetPlatform(PlatformType platform) => Platform = platform;
    public void SetOwnerId(long ownerId) => OwnerId = ownerId;
    public void SetOwnerTeamId(long ownerTeamId) => OwnerTeamId = ownerTeamId;
    public void SetIsManager(bool isManager) => IsManager = isManager;
    public void SetCurrency(string? currency) => Currency = currency;
    public void SetIsLimit(bool isLimit) => IsLimit = isLimit;
    public void SetMediaDisableReason(string? reason) => MediaDisableReason = reason;
    public void SetMediaCreatedTime(DateTime time) => MediaCreatedTime = time;
    public void SetAccountRunningTime(decimal time) => AccountRunningTime = time;
}
