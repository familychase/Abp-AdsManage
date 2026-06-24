namespace Ads.Automation.Domain.SyncSchedule;

public interface IAdsSyncScheduleRepository : IBaseRepository<AdsSyncSchedule>
{
    Task<List<AdsSyncSchedule>> GetListAsync(
        string? filterText = null,
        ActionType? actionType = null,
        PlatformType? platform = null,
        string sorting = "Id",
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        ActionType? actionType = null,
        PlatformType? platform = null,
        CancellationToken cancellationToken = default);
}
