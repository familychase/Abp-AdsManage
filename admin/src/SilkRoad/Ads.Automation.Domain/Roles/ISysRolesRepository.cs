namespace Ads.Automation.Domain.Roles;

public interface ISysRolesRepository : IBaseRepository<SysRoles>
{
    Task<List<SysRoles>> GetListAsync(
        string? filterText = null,
        bool? isActive = null,
        string sorting = "Name",
        int maxResultCount = 10,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string? filterText = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}
