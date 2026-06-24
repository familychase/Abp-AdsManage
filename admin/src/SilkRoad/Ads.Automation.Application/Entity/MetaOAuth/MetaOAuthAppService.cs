using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.MetaOAuth;

/// <summary>
/// Meta OAuth 授权应用服务实现
/// 处理 OAuth 回调：从 auth_app 读取配置 → code 换 token → 验证有效性 → 去重检查 → 加密存储 → 创建同步调度
/// </summary>
public class MetaOAuthAppService : ApplicationService, IMetaOAuthAppService
{
    private readonly IBaseRepository<AdsChannel> _channelRepository;
    private readonly IBaseRepository<AuthApp> _authAppRepository;
    private readonly IBaseRepository<AdsSyncSchedule> _scheduleRepository;
    private readonly MetaTokenEncryptionService _encryptionService;
    private readonly UserInfoContext _userInfoContext;

    private static readonly Random _rng = new();

    public MetaOAuthAppService(
        IBaseRepository<AdsChannel> channelRepository,
        IBaseRepository<AuthApp> authAppRepository,
        IBaseRepository<AdsSyncSchedule> scheduleRepository,
        MetaTokenEncryptionService encryptionService, UserInfoContext userInfoContext)
    {
        _channelRepository = channelRepository;
        _authAppRepository = authAppRepository;
        _scheduleRepository = scheduleRepository;
        _encryptionService = encryptionService;
        _userInfoContext = userInfoContext;
    }

    /// <inheritdoc />
    public async Task<MetaOAuthCallbackResultDto> HandleCallbackAsync(MetaOAuthCallbackInput input)
    {
        // Step 0: 通过前端传入的 AppId 从 auth_app 表读取配置
        var authApp = await ResolveAuthAppAsync(input.AppId);

        // Step 1: 用授权码换取短期 Token
        Logger.LogInformation("Meta OAuth 回调：开始处理授权码交换，AppId={AppId}", authApp.AppId);
        var shortLivedToken = await MetaOpenApi.ExchangeAuthorizationCodeAsync(
            input.Code, authApp.AppId, authApp.AppSecreat, input.RedirectUri);

        if (shortLivedToken == null || string.IsNullOrEmpty(shortLivedToken.access_token))
        {
            Logger.LogError("Meta OAuth 回调：授权码交换失败，返回空 Token");
            throw new BusinessException("MetaOAuth:ExchangeCodeFailed", "授权码换取 Token 失败，请检查 auth_app 配置和 RedirectUri 是否正确");
        }

        // Step 2: 短期 Token 换长期 Token（fb_exchange_token）
        Logger.LogInformation("Meta OAuth 回调：开始换取长期 Token");
        var longLivedToken = await MetaOpenApi.GetAccessTokenAsync(
            shortLivedToken.access_token, authApp.AppId, authApp.AppSecreat);

        if (longLivedToken == null || string.IsNullOrEmpty(longLivedToken.access_token))
        {
            Logger.LogError("Meta OAuth 回调：换取长期 Token 失败");
            throw new BusinessException("MetaOAuth:ExchangeLongTokenFailed", "换取长期 Token 失败");
        }

        var accessToken = longLivedToken.access_token;
        var expiresIn = longLivedToken.expires_in;
        var expired = expiresIn > 0 ? DateTime.Now.AddSeconds(expiresIn) : DateTime.Now.AddMonths(3);

        // Step 3: 验证 Token 有效性（调用 /me 获取 Meta 用户信息）
        Logger.LogInformation("Meta OAuth 回调：验证 Token 有效性");
        var mediaUser = await ValidateTokenAsync(accessToken);

        var mediaUserId = mediaUser.id;
        var userName = mediaUser.name ?? "Meta User";
        var channelName = BuildChannelName(userName, mediaUser);

        // Step 4: 去重检查 — 按 AppId + 平台去重
        var existingChannel = await FindExistingChannelAsync(input.AppId);

        if (existingChannel != null)
        {
            // 已存在：更新 Token 信息
            Logger.LogInformation("Meta OAuth 回调：渠道已存在 ChannelId={ChannelId}，更新授权信息", existingChannel.Id);

            existingChannel.SetChannelName(channelName);
            existingChannel.SetAccessToken(_encryptionService.Encrypt(accessToken));
            existingChannel.SetRefreshToken(_encryptionService.Encrypt(shortLivedToken.access_token));
            existingChannel.SetExpired(expired);
            existingChannel.SetAppKey(authApp.AppId);
            existingChannel.SetAppSecret(authApp.AppSecreat);
            existingChannel.SetMediaUserId(mediaUserId);
            existingChannel.SetChannelState(ChannelStateType.ACTIVE);
            existingChannel.SetAuthMissingReason(null);
            existingChannel.LastModifierId = _userInfoContext.UserId;

            await _channelRepository.UpdateAsync(existingChannel);

            return new MetaOAuthCallbackResultDto
            {
                ChannelId = existingChannel.Id,
                ChannelName = existingChannel.ChannelName,
                Platform = existingChannel.Platform.ToString(),
                ChannelState = existingChannel.ChannelState.ToString(),
                MediaUserId = mediaUserId,
                Expired = expired,
                IsNew = false
            };
        }

        // 不存在：创建新渠道，加密存储 Token
        Logger.LogInformation("Meta OAuth 回调：创建新渠道，MediaUserId={MediaUserId}", mediaUserId);

        var encryptedAccessToken = _encryptionService.Encrypt(accessToken);
        var encryptedRefreshToken = _encryptionService.Encrypt(shortLivedToken.access_token);

        var channel = AdsChannel.Create(
            channelName: channelName,
            platform: PlatformType.META,
            channelState: ChannelStateType.ACTIVE,
            auditType: AuditType.NO_SETTING,
            managerId: null,
            isManager: false,
            appKey: authApp.AppId,
            appSecret: authApp.AppSecreat,
            accessToken: encryptedAccessToken,
            refreshToken: encryptedRefreshToken,
            expired: expired,
            mediaUserId: mediaUserId,
            creatorId: _userInfoContext.UserId);

        await _channelRepository.InsertAsync(channel);

        Logger.LogInformation("Meta OAuth 回调：新渠道创建成功 ChannelId={ChannelId}, ChannelName={ChannelName}",
            channel.Id, channel.ChannelName);

        // Step 5: 创建同步调度计划（同步账户 + 同步主页）
        await CreateSyncSchedulesAsync(channel.Id);

        return new MetaOAuthCallbackResultDto
        {
            ChannelId = channel.Id,
            ChannelName = channel.ChannelName,
            Platform = channel.Platform.ToString(),
            ChannelState = channel.ChannelState.ToString(),
            MediaUserId = mediaUserId,
            Expired = expired,
            IsNew = true
        };
    }

    /// <summary>
    /// 授权成功后创建同步调度记录：同步账户（6h）+ 同步主页（3h），各加 1h 随机延迟避免限流
    /// </summary>
    private async Task CreateSyncSchedulesAsync(long channelId)
    {
        var resourceId = channelId.ToString();

        // 同步账户：6 小时后首次同步，随机 0~3600s 避免限流
        var accountNextTime = DateTime.Now
            .AddHours(6)
            .AddSeconds(_rng.Next(0, 3600));
        var accountSchedule = AdsSyncSchedule.Create(
            actionType: ActionType.AUTOMATIC,
            resourceId: resourceId,
            resourceType: ResourceType.CHANNEL,
            platform: PlatformType.META,
            jobName: "SyncAdAccountJobArgs",
            nextPublishTime: accountNextTime);
        await _scheduleRepository.InsertAsync(accountSchedule);

        // 同步主页：3 小时后首次同步，随机 0~3600s 避免限流
        var pageNextTime = DateTime.Now
            .AddHours(3)
            .AddSeconds(_rng.Next(0, 3600));
        var pageSchedule = AdsSyncSchedule.Create(
            actionType: ActionType.AUTOMATIC,
            resourceId: resourceId,
            resourceType: ResourceType.CHANNEL,
            platform: PlatformType.META,
            jobName: "SyncAdPageJobArgs",
            nextPublishTime: pageNextTime);
        await _scheduleRepository.InsertAsync(pageSchedule);

        Logger.LogInformation("Meta OAuth：渠道 {ChannelId} 同步调度已创建（账户={AccountTime}, 主页={PageTime}）",
            channelId, accountNextTime, pageNextTime);
    }

    /// <summary>
    /// 通过 AppId（Meta 分配的 App ID 字符串）从 auth_app 表读取配置
    /// </summary>
    private async Task<AuthApp> ResolveAuthAppAsync(string appId)
    {
        var query = await _authAppRepository.GetQueryableAsync();
        var authApp = query.FirstOrDefault(a =>
            a.Platform == PlatformType.META && a.AppId == appId)
            ?? throw new BusinessException("MetaOAuth:NoAuthApp",
                $"未找到 AppId={appId} 的 Meta 授权应用配置，请先在 auth_app 表中添加");

        return authApp;
    }

    /// <summary>
    /// 验证 Token 有效性并获取用户信息
    /// </summary>
    private async Task<User> ValidateTokenAsync(string accessToken)
    {
        try
        {
            var user = await MetaOpenApi.GetUserInfo(accessToken, "id,name,email");
            if (user == null || string.IsNullOrEmpty(user.id))
            {
                throw new BusinessException("MetaOAuth:TokenInvalid", "Token 无效：无法获取 Meta 用户信息");
            }
            return user;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Meta OAuth 回调：验证 Token 失败");
            throw new BusinessException("MetaOAuth:TokenValidationFailed", $"Token 验证失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 按 AppId + 平台查找已存在的渠道（去重，AppId 存储在 AppKey 字段）
    /// </summary>
    private async Task<AdsChannel?> FindExistingChannelAsync(string appId)
    {
        var query = await _channelRepository.GetQueryableAsync();
        return query.FirstOrDefault(c =>
            c.Platform == PlatformType.META &&
            c.AppKey == appId &&
            !c.IsDeleted);
    }

    /// <summary>
    /// 构建渠道名称：优先使用前端传入名称，否则用 Meta 用户名，传入 Email 则组合为 name(email)
    /// </summary>
    private static string BuildChannelName(string userName, MetaDomain.User user)
    {
        if (user == null)
            return "";

        if (!string.IsNullOrWhiteSpace(user.email))
            return $"{userName}({user.email})";

        return userName;
    }
}
