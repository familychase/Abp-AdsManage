namespace Ads.Automation.Domain.ApiUsage;

public interface IAdsMetaApiUsageRepository : IBaseRepository<AdsMetaApiUsage>
{
    Task<AdsMetaApiUsage?> FindBySlotAsync(string accountNo, DateTime timeSlot);
}
