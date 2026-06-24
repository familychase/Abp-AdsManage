using Ads.Automation.Domain.Account;
using Ads.Automation.Domain.Channel;
using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.MetaOAuth;

/// <summary>
/// 媒体授权服务实现 —— 封装渠道查找 + Token 解密，统一入口
/// </summary>
public class MediaAuthService : IMediaAuthService, ITransientDependency
{
    private readonly IMetaAuthorizationGateway _gateway;
    private readonly IBaseRepository<AdsChannel> _channelRepository;
    private readonly IBaseRepository<AdsAccount> _accountRepository;
    private readonly IBaseRepository<AdsChannelAdAccount> _channelAccountRepository;

    public MediaAuthService(
        IMetaAuthorizationGateway gateway,
        IBaseRepository<AdsChannel> channelRepository,
        IBaseRepository<AdsAccount> accountRepository,
        IBaseRepository<AdsChannelAdAccount> channelAccountRepository)
    {
        _gateway = gateway;
        _channelRepository = channelRepository;
        _accountRepository = accountRepository;
        _channelAccountRepository = channelAccountRepository;
    }

    /// <inheritdoc />
    public async Task<AccessIdentity> GetByChannelIdAsync(long channelId)
    {
        return await _gateway.GetAccessIdentityAsync(channelId);
    }

    /// <inheritdoc />
    public async Task<AccessIdentity> GetByAccountNoAsync(string accountNo)
    {
        var channelQuery = await _channelRepository.GetQueryableAsync();
        var normalized = NormalizeAccountNo(accountNo);
        var accountQuery = await _accountRepository.GetQueryableAsync();
        var linkQuery = await _channelAccountRepository.GetQueryableAsync();

        var channelIds = from link in linkQuery
                         join acc in accountQuery on link.AccountId equals acc.Id
                         where acc.AccountNo == normalized || acc.AccountNo == $"act_{normalized}"
                         select link.ChannelId;

        var channel = channelQuery
            .Where(c => channelIds.Contains(c.Id))
            .FirstOrDefault()
            ?? throw new BusinessException("No available channel found");

        return await _gateway.GetAccessIdentityAsync(channel.Id);
    }

    /// <inheritdoc />
    public async Task<AccessIdentity> GetFromAnyChannelAsync()
    {
        var channelQuery = await _channelRepository.GetQueryableAsync();
        var channel = channelQuery.FirstOrDefault()
            ?? throw new BusinessException("No available channel found");

        return await _gateway.GetAccessIdentityAsync(channel.Id);
    }

    private static string NormalizeAccountNo(string accountNo)
    {
        var normalized = accountNo.Trim();
        if (normalized.StartsWith("act_", StringComparison.OrdinalIgnoreCase))
            normalized = normalized[4..];
        return normalized;
    }
}
