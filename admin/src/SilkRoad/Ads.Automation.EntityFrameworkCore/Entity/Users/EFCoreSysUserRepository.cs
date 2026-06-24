using Microsoft.EntityFrameworkCore;

namespace Ads.Automation.EntityFrameworkCore.Entity.Users
{
    public class EFCoreSysUserRepository : EFCoreBaseRepository<SysUser>, ISysUserRepository
    {
        public EFCoreSysUserRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo) : base(dbContextProvider, userInfo)
        {
        }

        public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AnyAsync(user => user.UserCode == code, GetCancellationToken(cancellationToken));
        }

        public async Task<SysUser?> FindByCodeAsync(string code, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(user => user.UserCode == code, GetCancellationToken(cancellationToken));
        }

        public async Task<List<SysUser>> GetListAsync(string? filterText = null, bool? isActive = null, string sorting = "UserName", int maxResultCount = 10, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await CreateFilteredQueryAsync(filterText, isActive);

            return await query
                .OrderBy(user => user.UserName)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(string? filterText = null, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var query = await CreateFilteredQueryAsync(filterText, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        private async Task<IQueryable<SysUser>> CreateFilteredQueryAsync(string? filterText, bool? isActive)
        {
            var dbSet = await GetDbSetAsync();

            return dbSet
                .WhereIf(!filterText.IsNullOrWhiteSpace(),
                    user => user.UserName.Contains(filterText!) ||
                            user.UserCode.Contains(filterText!) ||
                            (user.AliasName != null && user.AliasName.Contains(filterText!)));
        }
    }
}
