using Ads.Automation.Domain.Ads;
using Ads.Automation.Domain.Reporting;
using Ads.Automation.Domain.Shared.Models;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Input;
using Volo.Abp.Domain.Entities;

namespace Ads.Automation.Application.Statistic;

/// <summary>
/// Meta 拉取报表数据服务
/// 参照 huntmobi BI4Sight MetaSyncReportingService 的 SyncAdBaseReporting 实现
/// </summary>
public class MetaSyncReportingService : ApplicationService, ISyncReportingService
{
    private readonly IMetaAuthorizationGateway _metaAuthorizationGateway;
    private readonly IBaseRepository<AdsChannelAdAccount> _channelAdAccountRepository;
    private readonly IBaseRepository<AdCampaignBaseRpt> _campaignBaseRptRepository;
    private readonly ILogger<MetaSyncReportingService> _logger;

    public MetaSyncReportingService(
        IMetaAuthorizationGateway metaAuthorizationGateway,
        IBaseRepository<AdsChannelAdAccount> channelAdAccountRepository,
        IBaseRepository<AdCampaignBaseRpt> campaignBaseRptRepository,
        ILogger<MetaSyncReportingService> logger)
    {
        _metaAuthorizationGateway = metaAuthorizationGateway;
        _channelAdAccountRepository = channelAdAccountRepository;
        _campaignBaseRptRepository = campaignBaseRptRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public PlatformType Platform => PlatformType.META;

    /// <inheritdoc />
    public async Task<bool> SyncGetRecentCost(string accountNo, DateRange range)
    {
        var channelId = await ResolveChannelIdAsync(accountNo);
        if (channelId == null)
        {
            _logger.LogWarning("SyncGetRecentCost：未找到账户 {AccountNo} 对应的渠道", accountNo);
            return false;
        }

        try
        {
            var callResult = await _metaAuthorizationGateway.ExecuteAsync(channelId.Value,
                async identity =>
                {
                    var parameter = new MetaInput.ReportingQuery
                    {
                        account_id = $"act_{accountNo}",
                        fields = "account_id,spend",
                        limit = 10
                    };
                    parameter.SetTotalTimeRanges(range.Start, range.Stop);

                    return await GetReportingAsync(identity, parameter);
                });

            var hasData = !callResult.IsNullOrEmpty();
            _logger.LogInformation("SyncGetRecentCost {Platform} {AccountNo} 结果：{HasData}",
                Platform, accountNo, hasData);
            return hasData;
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("SyncGetRecentCost：账户 {AccountNo} 关联的渠道已不存在，跳过", accountNo);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task SyncAdBaseReporting(string accountNo, DateRange range)
    {
        var channelId = await ResolveChannelIdAsync(accountNo);
        if (channelId == null)
        {
            _logger.LogWarning("SyncAdBaseReporting：未找到账户 {AccountNo} 对应的渠道", accountNo);
            return;
        }

        List<Reporting> callResult;
        var startTime = DateTime.Now;
        try
        {

            callResult = await _metaAuthorizationGateway.ExecuteAsync(channelId.Value,
                async identity =>
                {
                    var parameter = new MetaInput.ReportingQuery
                    {
                        account_id = $"act_{accountNo}",
                        fields = "account_id,campaign_id,campaign_name,impressions,spend,clicks,date_start,conversions,conversion_values",
                        limit = 200,
                        level = "campaign"
                    };

                    parameter.SetTimeRanges(range.Start, range.Stop);
                    parameter.SetAttributionWindows();

                    return await GetReportingAsync(identity, parameter, true);
                });
        }
        catch (EntityNotFoundException)
        {
            _logger.LogWarning("SyncAdBaseReporting：账户 {AccountNo} 关联的渠道已不存在，跳过", accountNo);
            return;
        }

        if (callResult.IsNullOrEmpty()) return;

        // Step 1：解析本地 AccountId
        var accountMappingQuery = await _channelAdAccountRepository.GetQueryableAsync();
        var accountMapping = accountMappingQuery.FirstOrDefault(m => m.AccountNo == accountNo);
        if (accountMapping == null)
        {
            _logger.LogWarning("SyncAdBaseReporting：未找到 AccountNo={AccountNo} 对应的账户映射", accountNo);
            return;
        }
        var accountId = accountMapping.AccountId;

        // Step 2：按 (campaign_id, date_start) 分组聚合
        var campaignGroups = callResult
            .Where(r => !string.IsNullOrEmpty(r.campaign_id))
            .GroupBy(r => new { r.campaign_id, r.campaign_name, r.date_start })
            .Select(g => new
            {
                CampaignNo = g.Key.campaign_id!,
                CampaignName = g.Key.campaign_name!,
                ReportDate = g.Key.date_start,
                Spend = g.Sum(r => decimal.TryParse(r.spend, out var v) ? v : 0),
                Clicks = g.Sum(r => int.TryParse(r.clicks, out var v) ? v : 0),
                Impressions = g.Sum(r => long.TryParse(r.impressions, out var v) ? v : 0),
                Converts = (int)g.Sum(r => r.BiConverts)
            })
            .ToList();

        // Step 3：按 (AccountId, CampaignNo, ReportDate) 比对，新增或编辑
        var rptQuery = await _campaignBaseRptRepository.GetQueryableAsync();
        foreach (var group in campaignGroups)
        {
            var flag = DateTime.TryParse(group.ReportDate, out var dtime);
            var existing = rptQuery.FirstOrDefault(r =>
                r.AccountId == accountId && r.CampaignNo == group.CampaignNo && r.ReportDate == dtime);

            if (existing != null)
            {
                existing.CampaignName = group.CampaignName;
                existing.ReportDate = dtime;
                existing.Spend = group.Spend;
                existing.Clicks = group.Clicks;
                existing.Impressions = group.Impressions;
                existing.Converts = group.Converts;
                existing.Purchase = 0;
                existing.PurchaseValue = 0;
                await _campaignBaseRptRepository.UpdateAsync(existing);
            }
            else
            {
                var newRpt = new AdCampaignBaseRpt
                {
                    AccountId = accountId,
                    CampaignNo = group.CampaignNo,
                    CampaignName = group.CampaignName,
                    ReportDate = dtime,
                    Platform = PlatformType.META,
                    Spend = group.Spend,
                    Clicks = group.Clicks,
                    Impressions = group.Impressions,
                    Converts = group.Converts,
                    Purchase = 0,
                    PurchaseValue = 0,
                    CPC = group.Clicks > 0 ? group.Spend * decimal.One / group.Clicks : decimal.Zero,
                    CPCO = 0
                };
                await _campaignBaseRptRepository.InsertAsync(newRpt);
            }
        }

        var elapsed = DateTime.Now.Subtract(startTime).TotalSeconds;
        _logger.LogInformation(
            "SyncAdBaseReporting {Seconds}s {Platform} {AccountNo} {Range} — " +
            "rpt:{ReportCount},spend:{Spend},upsert:{UpsertCount}",
            elapsed, Platform, accountNo, range.ToShortString(),
            campaignGroups.Count, campaignGroups.Sum(g => g.Spend), campaignGroups.Count);

    }

    #region Private Methods

    /// <summary>
    /// 通过 accountNo 查找关联的 ChannelId
    /// </summary>
    private async Task<long?> ResolveChannelIdAsync(string accountNo)
    {
        var query = await _channelAdAccountRepository.GetQueryableAsync();
        var mapping = query.FirstOrDefault(m => m.AccountNo == accountNo);
        return mapping?.ChannelId;
    }

    /// <summary>
    /// 获取报告（含分页和转化数）
    /// </summary>
    private async Task<List<Reporting>> GetReportingAsync(
        AccessIdentity identity,
        MetaInput.ReportingQuery parameter,
        bool isSync = false)
    {
        var reports = new List<Reporting>();

        var paged = await MetaOpenApi.GetReportingAsync(identity, parameter, isSync);
        if (paged?.data == null) return reports;

        reports.AddRange(paged.data);

        // 获取第二页后的所有数据
        if (!string.IsNullOrWhiteSpace(paged.paging?.next))
        {
            var reportingList = await MetaOpenApi.PagingAllAsync<Reporting>(
                identity, paged.paging.next);
            reports.AddRange(reportingList);
        }

        // 获取报表的转化数
        await GetReportingConvertsAsync(identity, parameter.account_id, reports);

        return reports;
    }

    /// <summary>
    /// 获取报表的转化数（从广告组设置读取转化事件）
    /// </summary>
    private async Task GetReportingConvertsAsync(
        AccessIdentity identity,
        string accountId,
        List<Reporting> reports)
    {
        var adsetNos = reports
            .Where(c => !string.IsNullOrEmpty(c.adset_id))
            .Select(s => s.adset_id!)
            .Distinct()
            .ToList();

        if (adsetNos.IsNullOrEmpty()) return;

        try
        {
            var adsetEventDic = await GetAdsetConversionEventDictionaryAsync(identity, accountId, adsetNos);
            foreach (var item in reports)
            {
                if (item.adset_id == null || !adsetEventDic.TryGetValue(item.adset_id, out var eventName))
                    continue;

                eventName = eventName.TrimEnd('s');

                if (item.actions is { Count: > 0 })
                {
                    var action = item.actions!.FirstOrDefault(c => c.action_type.Contains(eventName));
                    if (action != null)
                    {
                        item.BiConverts = decimal.TryParse(action.value, out var v) ? v : 0;
                        continue;
                    }
                }

                if (item.conversions is { Count: > 0 })
                {
                    var action = item.conversions!.FirstOrDefault(c => c.action_type.Contains(eventName));
                    if (action != null)
                    {
                        item.BiConverts = decimal.TryParse(action.value, out var v) ? v : 0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetReportingConvertsAsync accountId:{AccountId}，adsetNos:{@AdsetNos}",
                accountId, adsetNos);
        }
    }

    /// <summary>
    /// 获取广告组投放的转化事件字典
    /// </summary>
    private async Task<Dictionary<string, string>> GetAdsetConversionEventDictionaryAsync(
        AccessIdentity identity,
        string accountId,
        List<string> adsetNos)
    {
        var dictionary = new Dictionary<string, string>();

        var adsetList = new List<AdSet>();
        var pageSize = 100;
        var pageTotal = adsetNos.Count / pageSize + 1;

        for (var page = 1; page <= pageTotal; page++)
        {
            var list = adsetNos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var adSets = await MetaOpenApi.GetAdSetsAsync(identity, accountId, 100,
                "id,optimization_goal,promoted_object", list);
            if (adSets?.data != null)
                adsetList.AddRange(adSets.data);
        }

        foreach (var adset in adsetList)
        {
            if (dictionary.ContainsKey(adset.id)) continue;

            var promotedObject = adset.promoted_object;
            var conversionEvent = promotedObject?.custom_event_type ?? string.Empty;

            if (promotedObject != null)
            {
                // 投放的是应用，投放自定义事件
                if (!string.IsNullOrEmpty(promotedObject.application_id)
                    && !string.IsNullOrEmpty(promotedObject.custom_event_str))
                {
                    conversionEvent = promotedObject.custom_event_str!;
                }
                // 投放的是像素，像素可以投放两种自定义事件
                else if (!string.IsNullOrEmpty(promotedObject.pixel_id)
                    && (!string.IsNullOrEmpty(promotedObject.custom_event_str)
                        || !string.IsNullOrEmpty(promotedObject.custom_conversion_id)))
                {
                    conversionEvent = promotedObject.custom_conversion_id
                                      ?? promotedObject.custom_event_str;
                }
            }

            // 如果未读取到对应的转化事件，则读取广告组设置的优化目标
            conversionEvent = !string.IsNullOrEmpty(conversionEvent)
                ? conversionEvent
                : adset.optimization_goal ?? "OTHER";
            conversionEvent = conversionEvent.ToLower();

            dictionary.TryAdd(adset.id, conversionEvent);
        }

        return dictionary;
    }

    #endregion
}
