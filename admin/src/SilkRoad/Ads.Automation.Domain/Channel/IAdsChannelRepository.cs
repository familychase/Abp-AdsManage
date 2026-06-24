namespace Ads.Automation.Domain.Channel;

public interface IAdsChannelRepository : IBaseRepository<AdsChannel>
{
    Task<List<AdsChannel>> GetListAsync(
        string? filterText = null,
        PlatformType? platform = null,
        ChannelStateType? channelState = null,
        string sorting = "ChannelName",
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        PlatformType? platform = null,
        ChannelStateType? channelState = null,
        CancellationToken cancellationToken = default);
}
