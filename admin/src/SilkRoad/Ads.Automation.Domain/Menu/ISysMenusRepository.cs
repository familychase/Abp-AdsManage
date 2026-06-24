namespace Ads.Automation.Domain.Menu
{
    public interface ISysMenusRepository : IBaseRepository<SysMenus>
    {

        Task<List<SysMenus>> GetListAsync(
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
}
