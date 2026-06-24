namespace Ads.Automation.EntityFrameworkCore.Entity.ApiUsage;

public class EFCoreAdsMetaApiUsageRepository : EFCoreBaseRepository<AdsMetaApiUsage>, IAdsMetaApiUsageRepository
{
    public EFCoreAdsMetaApiUsageRepository(IDbContextProvider<AdsAutomationDbContext> dbContextProvider, UserInfoContext userInfo)
        : base(dbContextProvider, userInfo) { }

    public async Task<AdsMetaApiUsage?> FindBySlotAsync(string accountNo, DateTime timeSlot)
    {
        var db = await GetDbContextAsync();
        return await db.Set<AdsMetaApiUsage>()
            .FirstOrDefaultAsync(u => u.AccountNo == accountNo && u.TimeSlot == timeSlot);
    }
}
