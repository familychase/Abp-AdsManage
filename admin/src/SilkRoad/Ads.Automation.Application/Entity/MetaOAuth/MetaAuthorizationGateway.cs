namespace Ads.Automation.Application.Entity.MetaOAuth;

/// <summary>
/// Meta 授权网关实现
/// 统一的 Meta API 调用入口，自动解密 Token、构建 AccessIdentity、验证授权有效性
/// </summary>
public class MetaAuthorizationGateway : IMetaAuthorizationGateway, ITransientDependency
{
    private readonly IBaseRepository<AdsChannel> _channelRepository;
    private readonly MetaTokenEncryptionService _encryptionService;

    public MetaAuthorizationGateway(
        IBaseRepository<AdsChannel> channelRepository,
        MetaTokenEncryptionService encryptionService)
    {
        _channelRepository = channelRepository;
        _encryptionService = encryptionService;
    }

    /// <inheritdoc />
    public async Task<AccessIdentity> GetAccessIdentityAsync(long channelId)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == channelId);
        return BuildAccessIdentity(channel);
    }

    /// <inheritdoc />
    public async Task<T> ExecuteAsync<T>(long channelId, Func<AccessIdentity, Task<T>> action)
    {
        var identity = await GetAccessIdentityAsync(channelId);
        return await action(identity);
    }

    /// <inheritdoc />
    public async Task<bool> IsAuthorizationActiveAsync(long channelId)
    {
        try
        {
            var identity = await GetAccessIdentityAsync(channelId);
            if (string.IsNullOrEmpty(identity.AccessToken))
                return false;

            _ = await MetaOpenApi.GetUserInfo(identity.AccessToken, string.Empty);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public string DecryptToken(string encryptedToken)
    {
        return _encryptionService.Decrypt(encryptedToken);
    }

    /// <inheritdoc />
    public string EncryptToken(string plainToken)
    {
        return _encryptionService.Encrypt(plainToken);
    }

    /// <summary>
    /// 从 AdsChannel 构建解密后的 AccessIdentity
    /// </summary>
    private AccessIdentity BuildAccessIdentity(AdsChannel channel)
    {
        var model =  new AccessIdentity
        {
            AppKey = channel.AppKey ?? string.Empty,
            AppSecret = channel.AppSecret ?? string.Empty,
            ManagerId = channel.ManagerId ?? string.Empty,
            MediaIdentity = channel.MediaUserId ?? string.Empty,
            AccessToken = _encryptionService.Decrypt(channel.AccessToken ?? string.Empty),
            Expired = channel.Expired ?? DateTime.MinValue,
        };

        if (channel.Platform != PlatformType.META)
        {
            model.RefreshToken = !string.IsNullOrEmpty(channel.RefreshToken)
                ? _encryptionService.Decrypt(channel.RefreshToken)
                : string.Empty;
        }

        return model;
    }
}
