using Ads.Automation.Domain.Shared;
using Ads.Automation.Domain.Shared.Localization;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Creative;
using BusinessException = Volo.Abp.BusinessException;
using Microsoft.Extensions.Localization;

namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// Meta 广告复制核心服务 —— 内部复制 + Job 调度
/// 外部复制逻辑已拆分到 MetaDuplicateExternalService
/// </summary>
public class MetaDuplicateService : ApplicationService
{
    private readonly IBaseRepository<AdsDuplicateLogging> _logRepository;
    private readonly IBaseRepository<AdsDuplicateDetail> _detailRepository;
    private readonly MetaErrorParser _errorParser;
    private readonly MetaDuplicateExternalService _externalService;
    private readonly MetaApiRetryPolicy _retry;
    private readonly IMetaAuthorizationGateway _metaAuth;
    private readonly IStringLocalizer<AdsAutomationResource> _loc;

    private record InternalPendingData(long channelId);

    public MetaDuplicateService(
        IBaseRepository<AdsDuplicateLogging> logRepository,
        IBaseRepository<AdsDuplicateDetail> detailRepository,
        MetaErrorParser errorParser,
        MetaDuplicateExternalService externalService,
        MetaApiRetryPolicy retry,
        IMetaAuthorizationGateway metaAuth,
        IStringLocalizer<AdsAutomationResource> loc)
    {
        _logRepository = logRepository;
        _detailRepository = detailRepository;
        _errorParser = errorParser;
        _externalService = externalService;
        _retry = retry;
        _metaAuth = metaAuth;
        _loc = loc;
    }

    // ============ 请求阶段：只建 PENDING 记录 ============

    /// <summary>
    /// 账户内复制 —— 创建 PENDING 记录，由 Job 稍后执行
    /// </summary>
    public async Task<AdsDuplicateLogging> InternalDuplicateAsync(InternalDuplicateInput input)
    {
        var scheduleTime = await CalculateDelayAsync(input.AccountNo);

        var extendedData = JsonSerializer.Serialize(new
        {
            channelId = input.ChannelId
        });

        var log = AdsDuplicateLogging.Create(
            DuplicateSource.ADS_MANAGEMENT, 0, true,
            input.AdObjectLevel, input.ObjectNo, input.AccountNo,
            input.AccountNo, string.Empty, scheduleTime, input.UserId, extendedData,
            copyNumber: input.CopyNumber);

        await _logRepository.InsertAsync(log);
        return log;
    }

    /// <summary>
    /// 广告系列复制提交 —— 校验 CampaignNo 有效性后创建 PENDING 记录
    /// </summary>
    public Task<AdsDuplicateLogging> SubmitCampaignAsync(CampaignSubmitInput input)
    {
        return SubmitInternalAsync(
            input, AdObjectLevel.CAMPAIGN, input.CampaignNo,
            "Business:CampaignNoInvalid",
            async identity =>
            {
                var campaign = await _retry.ExecuteAsync(
                    () => MetaOpenApi.GetCampaignAsync(identity, input.CampaignNo, "id"),
                    "提交校验-读取广告系列",
                    input.AccountNo);
                return campaign != null;
            });
    }

    /// <summary>
    /// 广告组复制提交 —— 校验 AdSetNo 有效性后创建 PENDING 记录
    /// </summary>
    public Task<AdsDuplicateLogging> SubmitAdSetAsync(AdSetSubmitInput input)
    {
        return SubmitInternalAsync(
            input, AdObjectLevel.AD_SET, input.AdSetNo,
            "Business:AdSetNoInvalid",
            async identity =>
            {
                var adSet = await _retry.ExecuteAsync(
                    () => MetaOpenApi.GetAdSetAsync(identity, input.AdSetNo, "id"),
                    "提交校验-读取广告组",
                    input.AccountNo);
                return adSet != null;
            });
    }

    /// <summary>
    /// 提交公共逻辑：校验对象有效性 → 计算延迟 → 创建 PENDING 记录
    /// </summary>
    private async Task<AdsDuplicateLogging> SubmitInternalAsync(
        DuplicateSubmitInput input,
        AdObjectLevel objectLevel,
        string objectNo,
        string invalidKey,
        Func<AccessIdentity, Task<bool>> validateAsync)
    {
        var identity = await GetAccessIdentityAsync(input.ChannelId);

        var valid = await validateAsync(identity);
        if (!valid)
            throw new BusinessException(string.Format(_loc[invalidKey], objectNo));

        var scheduleTime = await CalculateDelayAsync(input.AccountNo);

        var extendedData = JsonSerializer.Serialize(new
        {
            channelId = input.ChannelId,
            targetPageNo = input.PageNo
        });

        var log = AdsDuplicateLogging.Create(
            DuplicateSource.ADS_MANAGEMENT, 0, true,
            objectLevel, objectNo, input.AccountNo,
            input.AccountNo, input.PageNo, scheduleTime, 0L, extendedData, input.CopyNumber);

        await _logRepository.InsertAsync(log);
        return log;
    }

    // ============ Job 阶段：消费 PENDING 记录 ============

    // ============ 内部复制 ============

    /// <summary>
    /// 解析 PENDING 记录的扩展数据，获取渠道访问令牌后执行内部复制
    /// </summary>
    public async Task ProcessInternalPendingAsync(AdsDuplicateLogging log)
    {
        var ext = JsonSerializer.Deserialize<InternalPendingData>(log.ExtendedData)
            ?? throw new BusinessException(_loc["Business:ExtensionDataParseError"]);

        var identity = await GetAccessIdentityAsync(ext.channelId);

        if (log.AdObjectLevel == AdObjectLevel.CAMPAIGN)
        {
            await InternalCampaignDuplicateAsync(identity, log.AccountNo, log.AdObjectNo, log.CopyNumber, log);
        }
        else
        {
            await InternalAdSetDuplicateAsync(identity, log.AccountNo, log.AdObjectNo, log.CopyNumber, log);
        }
    }

    /// <summary>
    /// 同账户广告系列复制
    /// 获取源 Campaign→AdSets→Ads，预构建 creative 素材结构后逐次迭代创建
    /// </summary>
    private async Task InternalCampaignDuplicateAsync(
        AccessIdentity identity, string accountNo, string campaignNo, long copyNumber, AdsDuplicateLogging log)
    {
        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(identity, campaignNo, CampaignFields),
            "读取广告系列",
            accountNo)
            ?? throw new BusinessException(string.Format(_loc["Business:CampaignNotFound"], campaignNo));

        var adsets = await GetAdSetsByCampaignAsync(identity, campaignNo, accountNo);
        var ads = (await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdsByCampaignAsync(identity, campaignNo, 500, AdFields),
            "读取广告系列下广告",
            accountNo)).data?.ToList()
                  ?? new List<Ad>();

        // 第一次循环构建 creative 素材结构，后续直接复用
        var builtAds = BuildAdsOnce(ads, builtCampaign: campaign!, builtAdset: null!, index: 1,
            smartPromotionType: string.Empty, isInternal: true, targetPageId: log.PageNo);
        var builtAdsByAdSetId = builtAds.GroupBy(a => a.adset_id).ToDictionary(g => g.Key, g => g.ToList());

        var content = string.Empty;
        for (var i = 1; i <= copyNumber; i++)
        {
            var (state, msg, campaignId) = await ExecuteInternalDuplicateIterationAsync(
                identity, accountNo, campaign, adsets, builtAdsByAdSetId, i, AdObjectLevel.CAMPAIGN);
            content += msg;

            // 记录每次迭代的明细（包含本迭代的创建内容）
            await SaveDetailAsync(log.Id, i, AdObjectLevel.CAMPAIGN, campaignId, state,
                state == DuplicateState.FAILED ? msg : null,
                state == DuplicateState.FAILED ? null : msg);

            if (state != DuplicateState.SUCCESS)
            {
                log.SetState(DuplicateState.FAILED);
                return;
            }
        }

        log.SetState(DuplicateState.SUCCESS);
    }

    /// <summary>
    /// 同账户广告组复制
    /// 获取源 AdSet→Campaign→Ads，预构建 creative 素材结构后逐次迭代创建
    /// </summary>
    private async Task InternalAdSetDuplicateAsync(
        AccessIdentity identity, string accountNo, string adsetNo, long copyNumber, AdsDuplicateLogging log)
    {
        var adset = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdSetAsync(identity, adsetNo, AdSetFields),
            "读取广告组",
            accountNo)
            ?? throw new BusinessException(string.Format(_loc["Business:AdSetNotFound"], adsetNo));

        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(identity, adset.campaign_id!, CampaignFields),
            "读取广告系列",
            accountNo)
            ?? throw new BusinessException(string.Format(_loc["Business:CampaignNotFound"], adset.campaign_id!));

        var campaignNo = adset.campaign_id!;
        var ads = (await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdsByCampaignAsync(identity, campaignNo, 500, AdFields),
            "读取广告组下广告",
            accountNo)).data?.ToList()
                  ?? new List<Ad>();

        var builtAds = BuildAdsOnce(ads, builtCampaign: campaign!, builtAdset: adset!, index: 1,
            smartPromotionType: string.Empty, isInternal: true, targetPageId: log.PageNo);
        var builtAdsByAdSetId = builtAds.GroupBy(a => a.adset_id).ToDictionary(g => g.Key, g => g.ToList());

        var content = string.Empty;
        for (var i = 1; i <= copyNumber; i++)
        {
            var (state, msg, campaignId) = await ExecuteInternalDuplicateIterationAsync(
                identity, accountNo, campaign, new List<AdSet> { adset }, builtAdsByAdSetId, i, AdObjectLevel.AD_SET);
            content += msg;

            await SaveDetailAsync(log.Id, i, AdObjectLevel.AD_SET, campaignId, state,
                state == DuplicateState.FAILED ? msg : null,
                state == DuplicateState.FAILED ? null : msg);

            if (state != DuplicateState.SUCCESS)
            {
                log.SetState(DuplicateState.FAILED);
                return;
            }
        }

        log.SetState(DuplicateState.SUCCESS);
    }

    /// <summary>
    /// 同账户单次迭代执行
    /// 素材只第一次构建，后续循环只重置 id/name 等标识字段，creative 结构直接复用
    /// </summary>
    private async Task<(DuplicateState State, string Content, string CampaignId)> ExecuteInternalDuplicateIterationAsync(
        AccessIdentity identity, string accountNo,
        AdCampaign sourceCampaign, List<AdSet> sourceAdSets,
        Dictionary<string, List<Ad>> prebuiltAdsByAdSetId, int index, AdObjectLevel objectLevel)
    {
        var content = string.Empty;
        var campaignId = sourceCampaign.id;
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null!
        };

        try
        {
            var campaignJson = JsonSerializer.Serialize(sourceCampaign, serializerOptions);
            var builtCampaign = JsonSerializer.Deserialize<AdCampaign>(campaignJson, serializerOptions)!;
            builtCampaign.id = null!;
            builtCampaign.account_id = null!;
            builtCampaign.name = MetaDuplicateHelper.BuildCopyName(builtCampaign.name, index);
            builtCampaign.special_ad_categories = new List<string>();
            builtCampaign.status = "ACTIVE";

            if (objectLevel == AdObjectLevel.CAMPAIGN)
            {
                LogApiCall(accountNo, sourceCampaign.id!, "创建广告系列", builtCampaign);
                var campaignResult = await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdCampaignAddAsync(identity, $"act_{accountNo}", builtCampaign),
                    "创建广告系列",
                    accountNo);
                campaignId = campaignResult.id;
                LogApiResult(accountNo, sourceCampaign.id!, "广告系列", campaignId);
                content += string.Format(_loc["Duplicate:CampaignCreated"], campaignId, builtCampaign.name);
            }

            var smartPromotionType = sourceCampaign.smart_promotion_type ?? string.Empty;

            foreach (var adset in sourceAdSets)
            {
                var adsetJson = JsonSerializer.Serialize(adset, serializerOptions);
                var builtAdSet = JsonSerializer.Deserialize<AdSet>(adsetJson, serializerOptions)!;
                builtAdSet.name = MetaDuplicateHelper.BuildCopyName(builtAdSet.name, index);
                builtAdSet.status = "ACTIVE";
                if (builtAdSet.start_time.HasValue && builtAdSet.start_time < DateTime.UtcNow)
                    builtAdSet.start_time = DateTime.UtcNow;
                builtAdSet.SetParameterIsNull(smartPromotionType);
                builtAdSet.campaign_id = campaignId;

                LogApiCall(accountNo, sourceCampaign.id!, "创建广告组", builtAdSet);
                var adsetResult = await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdSetAddAsync(identity, $"act_{accountNo}", builtAdSet),
                    "创建广告组",
                    accountNo);
                var newAdSetId = adsetResult.id;
                LogApiResult(accountNo, sourceCampaign.id!, "广告组", newAdSetId);
                content += string.Format(_loc["Duplicate:AdSetCreated"], newAdSetId);

                // 创建广告组后手动更新花费上下限（Meta API 创建时不支持直接设置 spend_cap）
                var hasSpendCap = !string.IsNullOrEmpty(builtAdSet.daily_spend_cap?.ToString())
                    || !string.IsNullOrEmpty(builtAdSet.lifetime_spend_cap?.ToString())
                    || !string.IsNullOrEmpty(builtAdSet.daily_min_spend_target?.ToString())
                    || !string.IsNullOrEmpty(builtAdSet.lifetime_min_spend_target?.ToString());
                if (hasSpendCap)
                {
                    var spendUpdate = new AdSet
                    {
                        daily_spend_cap = builtAdSet.daily_spend_cap,
                        lifetime_spend_cap = builtAdSet.lifetime_spend_cap,
                        daily_min_spend_target = builtAdSet.daily_min_spend_target,
                        lifetime_min_spend_target = builtAdSet.lifetime_min_spend_target
                    };
                    await _retry.ExecuteAsync(
                        () => MetaOpenApi.AdSetUpdateAsync(identity, newAdSetId, spendUpdate),
                        "更新广告组花费限额",
                        accountNo);
                    LogApiResult(accountNo, sourceCampaign.id!, "广告组花费限额已更新", newAdSetId);
                }

                if (!prebuiltAdsByAdSetId.TryGetValue(adset.id!, out var adsetAds))
                    continue;

                foreach (var ad in adsetAds)
                {
                    ad.id = null!;
                    ad.name = MetaDuplicateHelper.BuildCopyName(ad.name, index);
                    ad.adset_id = newAdSetId;

                    // 广告创意：首次迭代创建，后续迭代复用（跨迭代共享同一个 creative_id）
                    if (ad.creative_id == null && ad.creative != null)
                    {
                        ad.creative.id = null!;
                        var creativeParam = BuildCreativeAddParameter(ad.creative, ad.name);
                        LogApiCall(accountNo, sourceCampaign.id!, "创建广告创意", creativeParam);
                        var creativeResult = await _retry.ExecuteAsync(
                            () => MetaOpenApi.AdCreativesAddAsync(identity, $"act_{accountNo}", creativeParam),
                            "创建广告创意",
                            accountNo);
                        LogApiResult(accountNo, sourceCampaign.id!, "广告创意", creativeResult.id);
                        ad.creative_id = creativeResult.id;
                        
                        await Task.Delay(TimeSpan.FromSeconds(10)); // Meta API 对同一广告账户的修改操作有速率限制，间隔至少 10 秒;
                    }
                    
                    // 构建最小 Ad 对象，只传 creative_id 引用已创建的创意
                    var adForApi = new Ad
                    {
                        name = ad.name,
                        adset_id = ad.adset_id,
                        status = ad.status,
                        creative = new MetaCreativeDto()
                        {
                            creative_id = ad.creative_id!
                        }
                    };
                    LogApiCall(accountNo, sourceCampaign.id!, "创建广告", adForApi);
                    var adResult = await _retry.ExecuteAsync(
                        () => MetaOpenApi.AdAddAsync(identity, $"act_{accountNo}", adForApi),
                        "创建广告",
                        accountNo);
                    LogApiResult(accountNo, sourceCampaign.id!, "广告", adResult.id);
                    content += string.Format(_loc["Duplicate:AdCreated"], adResult.id);
                }
            }

            return (DuplicateState.SUCCESS, content, campaignId);
        }
        catch (Exception ex)
        {
            var errorMsg = _errorParser.ExtractErrorMessage(ex);
            Logger.LogError(ex, "Internal ad copy failed - Account:{AccountNo}, Object:{ObjectNo}, Error:{Error}",
                accountNo, sourceCampaign.id, ex.Message);
            content = errorMsg;
            return (DuplicateState.FAILED, content, campaignId);
        }
    }

    // ============ 辅助方法 ============

    /// <summary>
    /// 计算复制延迟时间，同一账户相邻复制间隔至少 30 分钟
    /// </summary>
    private async Task<DateTime> CalculateDelayAsync(string accountNo)
    {
        var query = await _logRepository.GetQueryableAsync();
        var hasPending = query.Any(l =>
            l.AccountNo == accountNo &&
            (l.State == DuplicateState.PENDING || l.State == DuplicateState.IN_PROGRESS));

        // 同账户已有未执行/执行中任务 → 延迟半小时避免冲突
        return hasPending ? DateTime.Now.AddMinutes(DuplicateIntervalMinutes) : DateTime.Now;
    }

    /// <summary>
    /// 根据渠道 ID 构建 Meta API 访问身份（通过授权网关自动解密 Token）
    /// </summary>
    private async Task<AccessIdentity> GetAccessIdentityAsync(long channelId)
    {
        return await _metaAuth.GetAccessIdentityAsync(channelId);
    }

    /// <summary>
    /// 获取 Campaign 下的所有 AdSet
    /// </summary>
    private async Task<List<AdSet>> GetAdSetsByCampaignAsync(AccessIdentity identity, string campaignNo, string accountNo)
    {
        var result = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdSetsByCampaignAsync(identity, campaignNo, 500, AdSetFields),
            "读取广告组列表",
            accountNo);
        return result.data?.ToList() ?? new List<AdSet>();
    }

    
    /// <summary>
    /// 保存复制明细记录
    /// </summary>
    private async Task SaveDetailAsync(long logId, int index, AdObjectLevel objectLevel, string adObjectNo,
        DuplicateState state, string? errorMessage, string? content = null)
    {
        var detail = AdsDuplicateDetail.Create(logId, index, objectLevel, adObjectNo, state, errorMessage, content);
        await _detailRepository.InsertAsync(detail);
    }

    private void LogApiCall(string accountNo, string campaignNo, string type, object parameter)
    {
        Logger.LogInformation(
            "Meta API 调用 - 账户:{AccountNo} 来源:{CampaignNo} 类型:{Type} 参数:{Param}",
            accountNo, campaignNo, type, parameter.ToJsonIgnoreNullValue());
    }

    private void LogApiResult(string accountNo, string campaignNo, string type, string objectId)
    {
        Logger.LogInformation(
            "Meta API 结果 - 账户:{AccountNo} 来源:{CampaignNo} 类型:{Type} 对象ID:{ObjectId}",
            accountNo, campaignNo, type, objectId);
    }
}
