namespace Ads.Automation.EntityFrameworkCore.Log;

public class EFCoreSysLogRepository : EFCoreBaseRepository<SysLog>, ISysLogRepository
{
    public EFCoreSysLogRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo)
        : base(dbContextProvider, userInfo)
    {
    }
}
