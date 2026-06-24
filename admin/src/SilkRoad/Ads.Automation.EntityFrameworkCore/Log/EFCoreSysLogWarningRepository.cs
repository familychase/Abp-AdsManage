namespace Ads.Automation.EntityFrameworkCore.Log;

public class EFCoreSysLogWarningRepository : EFCoreBaseRepository<SysLogWarning>, ISysLogWarningRepository
{
    public EFCoreSysLogWarningRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo)
        : base(dbContextProvider, userInfo)
    {
    }
}
