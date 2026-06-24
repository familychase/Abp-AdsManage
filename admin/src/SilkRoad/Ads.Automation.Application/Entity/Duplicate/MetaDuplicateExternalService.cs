using Ads.Automation.Domain.Duplicate;
using Ads.Automation.Domain.Shared.Localization;
using BusinessException = Volo.Abp.BusinessException;
using Microsoft.Extensions.Localization;

namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// Meta 跨账户广告复制服务
/// 处理跨账户广告复制：从源账户获取素材并上传到目标账户，再创建广告
/// </summary>
public class MetaDuplicateExternalService : ApplicationService
{
    private readonly IBaseRepository<AdsDuplicateLogging> _logRepository;
    private readonly IBaseRepository<AdsDuplicateDetail> _detailRepository;
    private readonly MetaErrorParser _errorParser;
    private readonly MetaApiRetryPolicy _retry;
    private readonly IMetaAuthorizationGateway _metaAuth;
    private readonly IStringLocalizer<AdsAutomationResource> _loc;

    private record ExternalPendingData(long sourceChannelId, long targetChannelId, string targetPageNo);

    public MetaDuplicateExternalService(
        IBaseRepository<AdsDuplicateLogging> logRepository,
        IBaseRepository<AdsDuplicateDetail> detailRepository,
        MetaErrorParser errorParser,
        MetaApiRetryPolicy retry,
        IMetaAuthorizationGateway metaAuth,
        IStringLocalizer<AdsAutomationResource> loc)
    {
        _logRepository = logRepository;
        _detailRepository = detailRepository;
        _errorParser = errorParser;
        _retry = retry;
        _metaAuth = metaAuth;
        _loc = loc;
    }

    // ============ 请求阶段 ============

    /// <summary>
    /// 跨账户复制 —— 创建 PENDING 记录，由 Job 稍后执行
    /// </summary>
    public async Task<AdsDuplicateLogging> ExternalDuplicateAsync(ExternalDuplicateInput input)
    {
        var extendedData = JsonSerializer.Serialize(new
        {
            sourceChannelId = input.SourceChannelId,
            targetChannelId = input.TargetChannelId,
            targetPageNo = input.TargetPageNo
        });

        var log = AdsDuplicateLogging.Create(
            DuplicateSource.ADS_MANAGEMENT, 0, false,
            input.AdObjectLevel, input.ObjectNo, input.SourceAccountNo,
            input.TargetAccountNo, input.TargetPageNo, DateTime.Now, input.UserId, extendedData,
            copyNumber: input.CopyNumber);

        await _logRepository.InsertAsync(log);
        return log;
    }

    // ============ Job 执行 ============

    /// <summary>
    /// 执行跨账户复制 —— 解析扩展数据，获取源/目标渠道令牌后执行复制
    /// </summary>
    public async Task ProcessExternalPendingAsync(AdsDuplicateLogging log)
    {
        var ext = JsonSerializer.Deserialize<ExternalPendingData>(log.ExtendedData)
            ?? throw new BusinessException(_loc["Business:ExtensionDataParseError"]);

        var srcIdentity = await GetAccessIdentityAsync(ext.sourceChannelId);
        var tgtIdentity = await GetAccessIdentityAsync(ext.targetChannelId);

        if (log.AdObjectLevel == AdObjectLevel.CAMPAIGN)
        {
            await ExternalCampaignDuplicateAsync(
                srcIdentity, log.AccountNo, tgtIdentity, log.DuplicateAccountNo,
                log.AdObjectNo, ext.targetPageNo, log);
        }
        else
        {
            await ExternalAdSetDuplicateAsync(
                srcIdentity, log.AccountNo, tgtIdentity, log.DuplicateAccountNo,
                log.AdObjectNo, ext.targetPageNo, log);
        }
    }

    /// <summary>
    /// 跨账户广告系列复制
    /// 从源账户获取 Campaign→AdSets→Ads，
    /// 先拷贝素材到目标账户，再在目标账户创建广告系列
    /// </summary>
    private async Task ExternalCampaignDuplicateAsync(
        AccessIdentity srcIdentity, string srcAccountNo,
        AccessIdentity tgtIdentity, string tgtAccountNo,
        string campaignNo, string targetPageNo, AdsDuplicateLogging log)
    {
        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(srcIdentity, campaignNo, CampaignFields),
            "跨账户读取广告系列")
            ?? throw new BusinessException(string.Format(_loc["Business:CampaignNotFound"], campaignNo));

        var adsets = await GetAdSetsByCampaignAsync(srcIdentity, campaignNo);
        var ads = (await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdsByCampaignAsync(srcIdentity, campaignNo, 500, AdFields),
            "跨账户读取广告系列下广告")).data?.ToList()
                  ?? new List<Ad>();

        await CopyMaterialsForAds(srcIdentity, srcAccountNo, tgtIdentity, tgtAccountNo, ads, log);

        var content = string.Empty;
        for (var i = 1; i <= log.CopyNumber; i++)
        {
            var (state, msg, campaignId) = await ExecuteExternalDuplicateAsync(
                tgtIdentity, tgtAccountNo, campaign, adsets, ads, i, targetPageNo);
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
    /// 跨账户广告组复制
    /// 从源账户获取 AdSet→Campaign→Ads，
    /// 先拷贝素材到目标账户，再在目标账户创建广告组
    /// </summary>
    private async Task ExternalAdSetDuplicateAsync(
        AccessIdentity srcIdentity, string srcAccountNo,
        AccessIdentity tgtIdentity, string tgtAccountNo,
        string adsetNo, string targetPageNo, AdsDuplicateLogging log)
    {
        var adset = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdSetAsync(srcIdentity, adsetNo, AdSetFields),
            "跨账户读取广告组")
            ?? throw new BusinessException(string.Format(_loc["Business:AdSetNotFound"], adsetNo));

        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(srcIdentity, adset.campaign_id!, CampaignFields),
            "跨账户读取广告系列")
            ?? throw new BusinessException(string.Format(_loc["Business:CampaignNotFound"], adset.campaign_id!));

        var campaignNo = adset.campaign_id!;
        var ads = (await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdsByCampaignAsync(srcIdentity, campaignNo, 500, AdFields),
            "跨账户读取广告组下广告")).data?.ToList()
                  ?? new List<Ad>();

        await CopyMaterialsForAds(srcIdentity, srcAccountNo, tgtIdentity, tgtAccountNo, ads, log);

        var content = string.Empty;
        for (var i = 1; i <= log.CopyNumber; i++)
        {
            var (state, msg, campaignId) = await ExecuteExternalDuplicateAsync(
                tgtIdentity, tgtAccountNo, campaign, new List<AdSet> { adset }, ads, i, targetPageNo);
            content += msg;

            // 记录每次迭代的明细（包含本迭代的创建内容）
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
    /// 跨账户单次执行 —— 在目标账户逐层创建 Campaign→AdSet→Ad
    /// </summary>
    private async Task<(DuplicateState State, string Content, string CampaignId)> ExecuteExternalDuplicateAsync(
        AccessIdentity identity, string accountNo,
        AdCampaign sourceCampaign, List<AdSet> sourceAdSets,
        List<Ad> sourceAds, int index, string targetPageNo)
    {
        var content = string.Empty;
        var campaignId = string.Empty;
        try
        {
            var builtCampaign = BuildCampaign(sourceCampaign, index);
            LogApiCall(accountNo, sourceCampaign.id!, "跨账户创建广告系列", builtCampaign);
            var campaignResult = await _retry.ExecuteAsync(
                () => MetaOpenApi.AdCampaignAddAsync(identity, $"act_{accountNo}", builtCampaign),
                "跨账户创建广告系列",
                accountNo);
            campaignId = campaignResult.id;
            LogApiResult(accountNo, sourceCampaign.id!, "广告系列", campaignId);
            content += string.Format(_loc["Duplicate:CampaignCreated"], campaignId, builtCampaign.name);

            foreach (var adset in sourceAdSets)
            {
                var builtAdSet = BuildAdSet(adset, index, builtCampaign.smart_promotion_type ?? string.Empty);
                builtAdSet.campaign_id = campaignResult.id;

                LogApiCall(accountNo, sourceCampaign.id!, "跨账户创建广告组", builtAdSet);
                var adsetResult = await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdSetAddAsync(identity, $"act_{accountNo}", builtAdSet),
                    "跨账户创建广告组",
                    accountNo);
                LogApiResult(accountNo, sourceCampaign.id!, "广告组", adsetResult.id);
                var newAdSetId = adsetResult.id;
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
                        "跨账户更新广告组花费限额",
                        accountNo);
                    LogApiResult(accountNo, sourceCampaign.id!, "广告组花费限额已更新", newAdSetId);
                }

                var adsetAds = sourceAds.Where(a => a.adset_id == adset.id).ToList();
                foreach (var ad in adsetAds)
                {
                    BuildAd(ad, builtAdSet, index, builtCampaign.smart_promotion_type ?? string.Empty, false, targetPageNo);
                    ad.adset_id = newAdSetId;

                    // 广告创意：首次迭代创建，后续迭代复用（跨迭代共享同一个 creative_id）
                    if (ad.creative_id == null && ad.creative != null)
                    {
                        var creativeParam = BuildCreativeAddParameter(ad.creative, ad.name);
                        LogApiCall(accountNo, sourceCampaign.id!, "跨账户创建广告创意", creativeParam);
                        var creativeResult = await _retry.ExecuteAsync(
                            () => MetaOpenApi.AdCreativesAddAsync(identity, $"act_{accountNo}", creativeParam),
                            "跨账户创建广告创意",
                            accountNo);
                        LogApiResult(accountNo, sourceCampaign.id!, "广告创意", creativeResult.id);
                        ad.creative_id = creativeResult.id;
                    }
                    var adForApi = new Ad
                    {
                        name = ad.name,
                        adset_id = ad.adset_id,
                        creative_id = ad.creative_id,
                        status = ad.status
                    };
                    LogApiCall(accountNo, sourceCampaign.id!, "跨账户创建广告", adForApi);
                    var adResult = await _retry.ExecuteAsync(
                        () => MetaOpenApi.AdAddAsync(identity, $"act_{accountNo}", adForApi),
                        "跨账户创建广告",
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
            Logger.LogError(ex, "Cross-account ad copy failed - Account:{AccountNo}, Object:{ObjectNo}, Error:{Error}",
                accountNo, sourceCampaign.id, ex.Message);
            content = errorMsg;
            return (DuplicateState.FAILED, content, campaignId);
        }
    }

    // ============ 素材跨账户拷贝 ============

    /// <summary>
    /// 跨账户拷贝广告素材
    /// 从源账户提取所有图片 hash 和视频 ID，
    /// 通过 Meta API 拷贝到目标账户，并替换广告中的素材引用
    /// </summary>
    private async Task CopyMaterialsForAds(
        AccessIdentity srcIdentity, string srcAccountNo,
        AccessIdentity tgtIdentity, string tgtAccountNo,
        List<Ad> ads, AdsDuplicateLogging log)
    {
        var (imageHashSet, videoIdSet) = ExtractMaterials(ads);
        var imageDic = new Dictionary<string, string>();
        var videoDic = new Dictionary<string, (string videoNo, string coverUrl)>();

        foreach (var imageHash in imageHashSet)
        {
            try
            {
                var result = await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdImageCopyAsync(tgtIdentity, $"act_{tgtAccountNo}", $"act_{srcAccountNo}", imageHash),
                    "跨账户复制图片");
                if (result.images?.Count > 0)
                    imageDic[imageHash] = result.images.First().Value.hash;
            }
            catch (Exception ex)
            {
                var errorMsg = _errorParser.ExtractErrorMessage(ex);
                Logger.LogWarning(ex, "复制图片素材失败 - Hash:{Hash}, 错误:{Error}", imageHash, ex.Message);
                log.SetErrorMessage(string.Format(_loc["Duplicate:ImageCopyFailed"], imageHash, errorMsg));
            }
        }

        foreach (var videoId in videoIdSet)
        {
            try
            {
                var videoInfo = await _retry.ExecuteAsync(
                    () => MetaOpenApi.GetAdVideoInfoAsync(srcIdentity, videoId, "id,source,title,thumbnails"),
                    "跨账户读取视频信息");
                if (videoInfo?.source == null)
                {
                    Logger.LogWarning("获取视频信息失败 - VideoId:{VideoId}", videoId);
                    continue;
                }

                var videoResult = await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdVideoAddAsync(tgtIdentity, $"act_{tgtAccountNo}", videoInfo.source, videoInfo.title ?? videoId),
                    "跨账户上传视频");

                var coverUrl = string.Empty;
                for (var i = 0; i < 10; i++)
                {
                    await Task.Delay(2 * 1000);
                    var thumbnails = await _retry.ExecuteAsync(
                        () => MetaOpenApi.GetAdVideoThumbnailsAsync(tgtIdentity, videoResult.id),
                        "获取视频缩略图");
                    if (thumbnails.data?.ToList()?.Count > 0)
                    {
                        var thumb = thumbnails.data?.ToList().FirstOrDefault(t => t.is_preferred == true) ?? thumbnails.data?.ToList()[0];
                        coverUrl = thumb.uri ?? string.Empty;
                        break;
                    }
                }

                videoDic[videoId] = (videoResult.id, coverUrl);
            }
            catch (Exception ex)
            {
                var errorMsg = _errorParser.ExtractErrorMessage(ex);
                Logger.LogWarning(ex, "复制视频素材失败 - VideoId:{VideoId}, 错误:{Error}", videoId, ex.Message);
                log.SetErrorMessage(string.Format(_loc["Duplicate:VideoCopyFailed"], videoId, errorMsg));
            }
        }

        foreach (var ad in ads) ReplaceAdMaterials(ad, imageDic, videoDic);
    }

    // ============ 辅助方法 ============

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
    private async Task<List<AdSet>> GetAdSetsByCampaignAsync(AccessIdentity identity, string campaignNo)
    {
        var result = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetAdSetsByCampaignAsync(identity, campaignNo, 500, AdSetFields),
            "跨账户读取广告组列表");
        return result.data?.ToList()?.ToList() ?? new List<AdSet>();
    }

    // ============ 媒体操作日志 ============

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
            accountNo, campaignNo, type, JsonSerializer.Serialize(parameter));
    }

    private void LogApiResult(string accountNo, string campaignNo, string type, string objectId)
    {
        Logger.LogInformation(
            "Meta API 结果 - 账户:{AccountNo} 来源:{CampaignNo} 类型:{Type} 对象ID:{ObjectId}",
            accountNo, campaignNo, type, objectId);
    }
}
