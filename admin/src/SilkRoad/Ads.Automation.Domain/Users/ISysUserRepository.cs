
namespace Ads.Automation.Domain.Users
{
    public interface ISysUserRepository : IBaseRepository<SysUser>
    {
        Task<SysUser?> FindByCodeAsync(string code, bool includeDetails = true, CancellationToken cancellationToken = default);

        Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

        Task<List<SysUser>> GetListAsync(string? filterText = null, bool? isActive = null, string sorting = "Name", int maxResultCount = 10, int skipCount = 0, CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(string? filterText = null, bool? isActive = null, CancellationToken cancellationToken = default);
    }
}
