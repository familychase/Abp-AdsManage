using MetaCampaign = Ads.Automation.Infrastructure.SDK.Models.Meta.Domain.MetaDomain.AdCampaign;
using MetaAdSet = Ads.Automation.Infrastructure.SDK.Models.Meta.Domain.MetaDomain.AdSet;
using MetaAd = Ads.Automation.Infrastructure.SDK.Models.Meta.Domain.MetaDomain.Ad;

namespace Ads.Automation.Application.Entity.Campaign;

/// <summary>
/// 广告结构同步服务
/// </summary>
public interface IAdStructureSyncService
{
    /// <summary>
    /// 执行广告结构同步（广告系列/广告组/广告）
    /// </summary>
    /// <param name="accountNo">广告账户编号</param>
    /// <param name="cancellationToken"></param>
    /// <returns>(系列数量, 广告组数量, 广告数量)</returns>
    Task<(int CampaignCount, int AdSetCount, int AdCount)> SyncAsync(
        string accountNo, CancellationToken cancellationToken);
}
