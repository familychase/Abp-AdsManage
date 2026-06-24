using Ads.Automation.Domain.Ads;

namespace Ads.Automation.Application.Entity.Campaign;

/// <summary>
/// 广告结构同步服务实现
/// </summary>
public class AdStructureSyncService : IAdStructureSyncService, ITransientDependency
{
    private readonly IBaseRepository<AdsAccount> _accountRepo;
    private readonly IBaseRepository<AdCampaignEntity> _campaignRepo;
    private readonly IBaseRepository<AdSetEntity> _adSetRepo;
    private readonly IBaseRepository<AdEntity> _adRepo;
    private readonly IBaseRepository<AdsChannelAdAccount> _channelAdAccountRepo;
    private readonly IAsyncQueryableExecuter _asyncExecuter;
    private readonly IMetaAuthorizationGateway _metaAuth;
    private readonly IUnitOfWorkManager _uowManager;
    private readonly MetaApiRetryPolicy _retry;
    private readonly ILogger<AdStructureSyncService> _logger;

    public AdStructureSyncService(
        IBaseRepository<AdsAccount> accountRepo,
        IBaseRepository<AdCampaignEntity> campaignRepo,
        IBaseRepository<AdSetEntity> adSetRepo,
        IBaseRepository<AdEntity> adRepo,
        IBaseRepository<AdsChannelAdAccount> channelAdAccountRepo,
        IAsyncQueryableExecuter asyncExecuter,
        IMetaAuthorizationGateway metaAuth,
        IUnitOfWorkManager uowManager,
        MetaApiRetryPolicy retry,
        ILogger<AdStructureSyncService> logger)
    {
        _accountRepo = accountRepo;
        _campaignRepo = campaignRepo;
        _adSetRepo = adSetRepo;
        _adRepo = adRepo;
        _channelAdAccountRepo = channelAdAccountRepo;
        _asyncExecuter = asyncExecuter;
        _metaAuth = metaAuth;
        _uowManager = uowManager;
        _retry = retry;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(int CampaignCount, int AdSetCount, int AdCount)> SyncAsync(
        string accountNo, CancellationToken cancellationToken)
    {
        using var uow = _uowManager.Begin(requiresNew: true);

        var (account, identity) = await PrepareAsync(accountNo, cancellationToken);

        _logger.LogInformation("开始同步账户 {AccountNo} 的广告结构", accountNo);

        var campaignMap = await SyncCampaignsAsync(identity, account, accountNo, cancellationToken);
        var adSetMap = await SyncAdSetsAsync(identity, account, accountNo, campaignMap, cancellationToken);
        var adCount = await SyncAdsAsync(identity, account, accountNo, campaignMap, adSetMap, cancellationToken);

        await uow.CompleteAsync();

        _logger.LogInformation(
            "账户 {AccountNo} 广告结构同步完成：系列 {CampaignCount} 条，广告组 {AdSetCount} 条，广告 {AdCount} 条",
            accountNo, campaignMap.Count, adSetMap.Count, adCount);

        return (campaignMap.Count, adSetMap.Count, adCount);
    }

    // ==================== 准备工作 ====================

    private async Task<(AdsAccount Account, AccessIdentity Identity)> PrepareAsync(
        string accountNo, CancellationToken cancellationToken)
    {
        var accountQuery = await _accountRepo.GetQueryableAsync();
        var account = await _asyncExecuter.FirstOrDefaultAsync(
            accountQuery.Where(a => a.AccountNo == accountNo), cancellationToken)
            ?? throw new InvalidOperationException($"账户 {accountNo} 不存在");

        var channelAdAccountQuery = await _channelAdAccountRepo.GetQueryableAsync();
        var relation = await _asyncExecuter.FirstOrDefaultAsync(
            channelAdAccountQuery.Where(r => r.AccountNo == accountNo), cancellationToken)
            ?? throw new InvalidOperationException($"账户 {accountNo} 无关联渠道");

        var identity = await _metaAuth.GetAccessIdentityAsync(relation.ChannelId);
        return (account, identity);
    }

    // ==================== Phase 1: 同步广告系列 ====================

    private async Task<Dictionary<string, AdCampaignEntity>> SyncCampaignsAsync(
        AccessIdentity identity, AdsAccount account, string accountNo, CancellationToken cancellationToken)
    {
        var campaignMap = new Dictionary<string, AdCampaignEntity>();

        var allMetaCampaigns = await FetchAllPagesAsync<AdCampaign>(
            () => MetaOpenApi.GetCampaignsAsync(identity, $"act_{accountNo}", 500, MetaConst.CampaignFields),
            identity, "同步广告系列");

        if (allMetaCampaigns.Count == 0)
            return campaignMap;

        var campaignNos = allMetaCampaigns.Select(c => c.id!).Distinct().ToList();
        var campaignQuery = await _campaignRepo.GetQueryableAsync();
        var existingCampaigns = await _asyncExecuter.ToListAsync(
            campaignQuery.Where(c => c.AccountId == account.Id && campaignNos.Contains(c.CampaignNo)),
            cancellationToken);
        var existingCampaignMap = existingCampaigns.ToDictionary(c => c.CampaignNo);

        foreach (var metaCampaign in allMetaCampaigns)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var campaignNo = metaCampaign.id ?? string.Empty;
            if (string.IsNullOrEmpty(campaignNo))
                continue;

            var (budgetType, budget) = ParseBudget(metaCampaign.daily_budget, metaCampaign.lifetime_budget);

            if (existingCampaignMap.TryGetValue(campaignNo, out var existing))
            {
                existing.CampaignName = metaCampaign.name ?? string.Empty;
                existing.MediaState = metaCampaign.status ?? string.Empty;
                existing.Objective = metaCampaign.objective ?? string.Empty;
                existing.MediaCreateTime = metaCampaign.created_time ?? DateTime.MinValue;
                existing.BudgetType = budgetType;
                existing.Budget = budget;
                existing.LastModificationTime = DateTime.Now;
                await _campaignRepo.UpdateAsync(existing, cancellationToken: cancellationToken);
                campaignMap[campaignNo] = existing;
            }
            else
            {
                var campaign = AdCampaignEntity.Create(
                    accountId: account.Id,
                    accountNo: account.AccountNo ?? string.Empty,
                    campaignNo: campaignNo,
                    campaignName: metaCampaign.name ?? string.Empty,
                    mediaState: metaCampaign.status ?? string.Empty,
                    budgetType: budgetType,
                    budget: budget,
                    objective: metaCampaign.objective ?? string.Empty,
                    mediaCreateTime: metaCampaign.created_time ?? DateTime.MinValue);
                await _campaignRepo.InsertAsync(campaign, cancellationToken: cancellationToken);
                campaignMap[campaignNo] = campaign;
            }
        }

        return campaignMap;
    }

    // ==================== Phase 2: 同步广告组 ====================

    private async Task<Dictionary<string, AdSetEntity>> SyncAdSetsAsync(
        AccessIdentity identity, AdsAccount account, string accountNo,
        Dictionary<string, AdCampaignEntity> campaignMap, CancellationToken cancellationToken)
    {
        var adSetMap = new Dictionary<string, AdSetEntity>();

        var allMetaAdSets = await FetchAllPagesAsync<MetaDomain.AdSet>(
            () => MetaOpenApi.GetAdSetsAsync(identity, $"act_{accountNo}", 500, MetaConst.AdSetFields),
            identity, "同步广告组");

        if (allMetaAdSets.Count == 0)
            return adSetMap;

        var adSetNos = allMetaAdSets.Select(s => s.id).Distinct().ToList();
        var adSetQuery = await _adSetRepo.GetQueryableAsync();
        var existingAdSets = await _asyncExecuter.ToListAsync(
            adSetQuery.Where(s => s.AccountId == account.Id && adSetNos.Contains(s.AdSetNo)),
            cancellationToken);
        var existingAdSetMap = existingAdSets.ToDictionary(s => s.AdSetNo);

        foreach (var metaAdSet in allMetaAdSets)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var adSetNo = metaAdSet.id;
            if (string.IsNullOrEmpty(adSetNo))
                continue;

            var campaignNo = metaAdSet.campaign_id ?? string.Empty;
            if (!campaignMap.TryGetValue(campaignNo, out var parentCampaign))
                continue;

            var (budgetType, budget) = ParseBudget(metaAdSet.daily_budget, metaAdSet.lifetime_budget);

            if (existingAdSetMap.TryGetValue(adSetNo, out var existingAdSet))
            {
                existingAdSet.AdSetName = metaAdSet.name ?? string.Empty;
                existingAdSet.MediaState = metaAdSet.status ?? string.Empty;
                existingAdSet.CampaignNo = campaignNo;
                existingAdSet.CampaignId = parentCampaign.Id;
                existingAdSet.MediaCreateTime = metaAdSet.created_time ?? DateTime.MinValue;
                existingAdSet.BudgetType = budgetType;
                existingAdSet.Budget = budget;
                existingAdSet.LastModificationTime = DateTime.Now;
                await _adSetRepo.UpdateAsync(existingAdSet, cancellationToken: cancellationToken);
                adSetMap[adSetNo] = existingAdSet;
            }
            else
            {
                var adSet = AdSetEntity.Create(
                    accountId: account.Id,
                    accountNo: account.AccountNo ?? string.Empty,
                    campaignId: parentCampaign.Id,
                    campaignNo: campaignNo,
                    adSetNo: adSetNo,
                    adSetName: metaAdSet.name ?? string.Empty,
                    mediaState: metaAdSet.status ?? string.Empty,
                    budgetType: budgetType,
                    budget: budget,
                    mediaCreateTime: metaAdSet.created_time ?? DateTime.MinValue);
                await _adSetRepo.InsertAsync(adSet, cancellationToken: cancellationToken);
                adSetMap[adSetNo] = adSet;
            }
        }

        return adSetMap;
    }

    // ==================== Phase 3: 同步广告 ====================

    private async Task<int> SyncAdsAsync(
        AccessIdentity identity, AdsAccount account, string accountNo,
        Dictionary<string, AdCampaignEntity> campaignMap, Dictionary<string, AdSetEntity> adSetMap,
        CancellationToken cancellationToken)
    {
        var count = 0;

        var allMetaAds = await FetchAllPagesAsync<MetaDomain.Ad>(
            () => MetaOpenApi.GetAdsAsync(identity, $"act_{accountNo}", 500, MetaConst.AdFields),
            identity, "同步广告");

        if (allMetaAds.Count == 0)
            return count;

        var adNos = allMetaAds.Select(a => a.id).Distinct().ToList();
        var adQuery = await _adRepo.GetQueryableAsync();
        var existingAds = await _asyncExecuter.ToListAsync(
            adQuery.Where(a => a.AccountId == account.Id && adNos.Contains(a.AdNo)),
            cancellationToken);
        var existingAdMap = existingAds.ToDictionary(a => a.AdNo);

        foreach (var metaAd in allMetaAds)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var adNo = metaAd.id;
            if (string.IsNullOrEmpty(adNo))
                continue;

            var campaignNo = metaAd.campaign_id ?? string.Empty;
            var adSetNo = metaAd.adset_id ?? string.Empty;
            if (!campaignMap.TryGetValue(campaignNo, out var parentCampaign))
                continue;
            if (!adSetMap.TryGetValue(adSetNo, out var parentAdSet))
                continue;

            if (existingAdMap.TryGetValue(adNo, out var existingAd))
            {
                existingAd.AdName = metaAd.name ?? string.Empty;
                existingAd.MediaState = metaAd.status;
                existingAd.CampaignNo = campaignNo;
                existingAd.CampaignId = parentCampaign.Id;
                existingAd.AdSetNo = adSetNo;
                existingAd.AdSetId = parentAdSet.Id;
                existingAd.CreativeNo = metaAd.creative?.id ?? string.Empty;
                existingAd.PageNo = metaAd.creative?.object_story_id ?? string.Empty;
                existingAd.MediaCreateTime = metaAd.created_time ?? DateTime.MinValue;
                existingAd.LastModificationTime = DateTime.Now;
                await _adRepo.UpdateAsync(existingAd, cancellationToken: cancellationToken);
            }
            else
            {
                var ad = AdEntity.Create(
                    accountId: account.Id,
                    accountNo: account.AccountNo ?? string.Empty,
                    campaignId: parentCampaign.Id,
                    campaignNo: campaignNo,
                    adSetId: parentAdSet.Id,
                    adSetNo: adSetNo,
                    adNo: adNo,
                    adName: metaAd.name ?? string.Empty,
                    mediaState: metaAd.status,
                    creativeNo: metaAd.creative?.id ?? string.Empty,
                    pageNo: metaAd.creative?.object_story_id ?? string.Empty,
                    mediaCreateTime: metaAd.created_time ?? DateTime.MinValue);
                await _adRepo.InsertAsync(ad, cancellationToken: cancellationToken);
            }

            count++;
        }

        return count;
    }

    // ==================== 通用辅助方法 ====================

    private async Task<List<T>> FetchAllPagesAsync<T>(
        Func<Task<MetaPagedDto<T>>> fetchFirstPage,
        AccessIdentity identity,
        string operationName)
    {
        var result = new List<T>();

        var firstPage = await _retry.ExecuteAsync(fetchFirstPage, operationName);
        if (firstPage?.data?.Count > 0)
        {
            result.AddRange(firstPage.data);
            if (!string.IsNullOrEmpty(firstPage.paging?.next))
            {
                var rest = await MetaOpenApi.PagingAllAsync<T>(identity, firstPage.paging.next);
                result.AddRange(rest);
            }
        }

        return result;
    }

    private static (string BudgetType, decimal Budget) ParseBudget(string? dailyBudget, string? lifetimeBudget)
    {
        if (!string.IsNullOrEmpty(dailyBudget) && decimal.TryParse(dailyBudget, out var daily) && daily > 0)
            return ("daily_budget", daily);

        if (!string.IsNullOrEmpty(lifetimeBudget) && decimal.TryParse(lifetimeBudget, out var lifetime) && lifetime > 0)
            return ("lifetime_budget", lifetime);

        return (string.Empty, 0);
    }
}
