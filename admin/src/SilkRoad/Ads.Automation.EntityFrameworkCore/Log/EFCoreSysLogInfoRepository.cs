namespace Ads.Automation.EntityFrameworkCore.Log;

public class EFCoreSysLogInfoRepository : EFCoreBaseRepository<SysLogInfo>, ISysLogInfoRepository
{
    public EFCoreSysLogInfoRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo)
        : base(dbContextProvider, userInfo)
    {
    }
}
