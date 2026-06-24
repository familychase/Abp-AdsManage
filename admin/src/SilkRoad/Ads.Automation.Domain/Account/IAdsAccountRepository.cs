namespace Ads.Automation.Domain.Account;

public interface IAdsAccountRepository : IBaseRepository<AdsAccount>
{
    Task<List<AdsAccount>> GetListAsync(
        string? filterText = null,
        PlatformType? platform = null,
        byte? accountState = null,
        string sorting = "AccountName",
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        PlatformType? platform = null,
        byte? accountState = null,
        CancellationToken cancellationToken = default);
}