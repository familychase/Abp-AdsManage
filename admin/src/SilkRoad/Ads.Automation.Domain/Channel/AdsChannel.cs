namespace Ads.Automation.Domain.Channel;

/// <summary>
/// 广告渠道实体
/// </summary>
public class AdsChannel : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
{
    /// <summary>
    /// 渠道名称
    /// </summary>
    public string ChannelName { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 授权状态
    /// </summary>
    public ChannelStateType ChannelState { get; private set; }

    /// <summary>
    /// 授权类型
    /// </summary>
    public AuditType AuditType { get; private set; }

    /// <summary>
    /// 媒体经理级账户（Meta=经理账户, Google=MCC ID）
    /// </summary>
    public string? ManagerId { get; private set; }

    /// <summary>
    /// 是否为经理级账户
    /// </summary>
    public bool IsManager { get; private set; }

    /// <summary>
    /// App Key
    /// </summary>
    public string? AppKey { get; private set; }

    /// <summary>
    /// App Secret
    /// </summary>
    public string? AppSecret { get; private set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? AccessToken { get; private set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; private set; }

    /// <summary>
    /// 失效时间
    /// </summary>
    public DateTime? Expired { get; private set; }

    /// <summary>
    /// 媒体身份标识
    /// </summary>
    public string? MediaUserId { get; private set; }

    /// <summary>
    /// 授权丢失原因
    /// </summary>
    public string? AuthMissingReason { get; private set; }

    // ===== 审计字段 =====

    public long CreatorId { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;

    public long? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? DeleterId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public bool IsDeleted { get; set; }

    // ===== 构造函数 =====

    private AdsChannel() { }

    public static AdsChannel Create(
        string channelName,
        PlatformType platform,
        ChannelStateType channelState,
        AuditType auditType,
        string? managerId = null,
        bool isManager = false,
        string? appKey = null,
        string? appSecret = null,
        string? accessToken = null,
        string? refreshToken = null,
        DateTime? expired = null,
        string? mediaUserId = null,
        long creatorId = 0)
    {
        return new AdsChannel(
            IdGenerator.GetNextId(),
            channelName,
            platform,
            channelState,
            auditType,
            managerId,
            isManager,
            appKey,
            appSecret,
            accessToken,
            refreshToken,
            expired,
            mediaUserId,
            creatorId);
    }

    internal AdsChannel(
        long id,
        string channelName,
        PlatformType platform,
        ChannelStateType channelState,
        AuditType auditType,
        string? managerId = null,
        bool isManager = false,
        string? appKey = null,
        string? appSecret = null,
        string? accessToken = null,
        string? refreshToken = null,
        DateTime? expired = null,
        string? mediaUserId = null,
        long creatorId = 0)
        : base(id)
    {
        ChannelName = channelName;
        Platform = platform;
        ChannelState = channelState;
        AuditType = auditType;
        ManagerId = managerId;
        IsManager = isManager;
        AppKey = appKey;
        AppSecret = appSecret;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Expired = expired;
        MediaUserId = mediaUserId;
        CreatorId = creatorId;
        LastModificationTime = DateTime.Now;
        LastModifierId = creatorId;
    }

    // ===== 更新方法 =====

    public void SetChannelName(string channelName) => ChannelName = channelName;
    public void SetPlatform(PlatformType platform) => Platform = platform;
    public void SetChannelState(ChannelStateType channelState) => ChannelState = channelState;
    public void SetAuditType(AuditType auditType) => AuditType = auditType;
    public void SetManagerId(string? managerId) => ManagerId = managerId;
    public void SetIsManager(bool isManager) => IsManager = isManager;
    public void SetAppKey(string? appKey) => AppKey = appKey;
    public void SetAppSecret(string? appSecret) => AppSecret = appSecret;
    public void SetAccessToken(string? accessToken) => AccessToken = accessToken;
    public void SetRefreshToken(string? refreshToken) => RefreshToken = refreshToken;
    public void SetExpired(DateTime? expired) => Expired = expired;
    public void SetMediaUserId(string? mediaUserId) => MediaUserId = mediaUserId;
    public void SetAuthMissingReason(string? reason) => AuthMissingReason = reason;
}
