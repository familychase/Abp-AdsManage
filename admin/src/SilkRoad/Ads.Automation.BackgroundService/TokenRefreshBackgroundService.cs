namespace Ads.Automation.BackgroundService;

/// <summary>
/// Meta Token 刷新定时任务
/// 定时扫描即将过期的 Meta 渠道 Token 并调用 Meta API 刷新
/// 支持分布式锁，按渠道独立加锁
/// </summary>
public class TokenRefreshBackgroundService : BaseBackgroundTaskService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedLock _distributedLock;

    /// <summary>
    /// Token 刷新锁过期时间（10分钟，防止死锁后无法继续）
    /// </summary>
    private static readonly TimeSpan LockExpiration = TimeSpan.FromMinutes(10);

    public TokenRefreshBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<TokenRefreshBackgroundService> logger,
        IDistributedLock distributedLock)
        : base(logger)
    {
        _scopeFactory = scopeFactory;
        _distributedLock = distributedLock;
    }

    /// <inheritdoc />
    protected override TimeSpan Interval => TimeSpan.FromHours(12);

    /// <inheritdoc />
    protected override async Task InternalExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

        using var uow = uowManager.Begin(requiresNew: true);

        var channelRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<AdsChannel>>();
        var asyncExecuter = scope.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();
        var metaAuth = scope.ServiceProvider.GetRequiredService<IMetaAuthorizationGateway>();

        var channelQuery = await channelRepo.GetQueryableAsync();
        var channels = await asyncExecuter.ToListAsync(
            channelQuery.Where(c => c.Platform == PlatformType.META
                && c.AccessToken != null
                && c.ChannelState == ChannelStateType.ACTIVE));

        foreach (var channel in channels)
        {
            await RefreshChannelTokenAsync(channel, channelRepo, metaAuth, stoppingToken);
        }

        await uow.CompleteAsync();
    }

    /// <summary>
    /// 刷新单个渠道的 Meta Token
    /// </summary>
    private async Task RefreshChannelTokenAsync(
        AdsChannel channel, IBaseRepository<AdsChannel> channelRepo, IMetaAuthorizationGateway metaAuth, CancellationToken ct)
    {
        // 按渠道ID获取分布式锁
        var lockKey = $"token_refresh:{channel.Id}";
        if (!await _distributedLock.AcquireAsync(lockKey, LockExpiration))
        {
            Logger.LogInformation("渠道 {ChannelId} Token 刷新任务正在执行中，跳过", channel.Id);
            return;
        }

        // // 检查是否在刷新阈值之外（token 还很新鲜，不需要刷新）
        // if (channel.Expired.HasValue &&
        //     channel.Expired.Value > DateTime.Now.Add(RefreshThreshold))
        // {
        //     return;
        // }

        // 跳过没有凭证信息的渠道（解密后校验）
        var decryptedToken = metaAuth.DecryptToken(channel.AccessToken ?? string.Empty);
        if (string.IsNullOrEmpty(decryptedToken) ||
            string.IsNullOrEmpty(channel.AppKey) ||
            string.IsNullOrEmpty(channel.AppSecret))
        {
            channel.SetChannelState(ChannelStateType.MISSING);
            await channelRepo.UpdateAsync(channel);
            return;
        }

        try
        {
            // 先检查授权是否有效（使用解密后的 Token）
            var authState = await CheckAuthorizationActiveAsync(decryptedToken);
            if (!authState.Active)
            {
                channel.SetChannelState(ChannelStateType.MISSING);
                await channelRepo.UpdateAsync(channel);
                return;
            }


            Logger.LogInformation("开始刷新渠道 {ChannelId} 的 Meta Token，当前过期时间 {Expired}",
                channel.Id, channel.Expired);

            // 调用 Meta API 交换长期 Token（使用解密后的 Token）
            var newToken = await MetaOpenApi.GetAccessTokenAsync(
                decryptedToken,
                channel.AppKey,
                channel.AppSecret);

            if (newToken == null || string.IsNullOrEmpty(newToken.access_token))
            {
                channel.SetChannelState(ChannelStateType.MISSING);
                await channelRepo.UpdateAsync(channel);
                Logger.LogWarning("渠道 {ChannelId} Token 刷新失败，API 返回空 Token", channel.Id);
                return;
            }

            // 更新渠道 Token 信息（加密后存储）
            channel.SetAccessToken(metaAuth.EncryptToken(newToken.access_token));
            channel.SetExpired(DateTime.Now.AddSeconds(newToken.expires_in));

            await channelRepo.UpdateAsync(channel);

            Logger.LogInformation("渠道 {ChannelId} Meta Token 刷新成功，新过期时间 {NewExpired}",
                channel.Id, channel.Expired);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.LogError(ex, "渠道 {ChannelId} Meta Token 刷新失败", channel.Id);
            channel.SetChannelState(ChannelStateType.MISSING);
            await channelRepo.UpdateAsync(channel);
        }
        finally
        {
            await _distributedLock.ReleaseAsync(lockKey);
        }
    }

    public async Task<CheckAuthorizationStateReply> CheckAuthorizationActiveAsync(string accessToken)
    {
        var reply = new CheckAuthorizationStateReply()
        {
            Active = true
        };

        try
        {
            _ = await MetaOpenApi.GetUserInfo(accessToken, string.Empty);
        }
        catch (HttpResponseException ex)
        {
            // 未授权
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                reply.Active = false;
                reply.ErrorMessage = ex.Message;
            }

            // 用户失效了
            if (ex.Message.Contains("API access disrupted. Go to the App Dashboard and complete Data Use Checkup"))
            {
                reply.Active = false;
                reply.ErrorMessage = ex.Message;
            }

            Logger.LogError(ex, $"CheckAuthorizationActiveAsync Response excute error accessToken:{accessToken}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"CheckAuthorizationActiveAsync excute error accessToken:{accessToken}");
        }

        return reply;
    }

}

public class CheckAuthorizationStateReply
{
    /// <summary>
    /// 是否授权有效
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// 错误信息（如果有）
    /// </summary>
    public string ErrorMessage { get; set; }
}
