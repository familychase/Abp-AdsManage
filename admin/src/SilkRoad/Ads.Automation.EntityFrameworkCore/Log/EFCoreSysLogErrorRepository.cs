namespace Ads.Automation.EntityFrameworkCore.Log;

public class EFCoreSysLogErrorRepository : EFCoreBaseRepository<SysLogError>, ISysLogErrorRepository
{
    public EFCoreSysLogErrorRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo)
        : base(dbContextProvider, userInfo)
    {
    }
}
