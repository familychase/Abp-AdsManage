using Ads.Automation.Application.Contracts.Entity.Publishing.Template;
using Ads.Automation.Domain.Shared.Enums.Publishing;
using Ads.Automation.Domain.Shared.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Validation;

namespace Ads.Automation.Application.Validators.Publishing;

/// <summary>
/// AdsPublishTemplateViewModel 的参数校验器
/// META Pixel 深度校验来源：
///   BI4Sight MetaPublishDataViewModelValidator.VerifyMetaViewModel() (lines 72-647)
/// </summary>
public class AdsPublishTemplateViewModelValidator : IObjectValidationContributor, ITransientDependency
{
    #region Constants (来自参考项目)

    private const string AdStructureVariable = "#AD_STRUCTURE#";
    private const string MaterialNameVariable = "#MATERIAL_NAME#";

    private const string BuyingTypeAuction = "AUCTION";
    private const string EventTypeStandard = "STANDARD_EVENTS";
    private const string EventTypeCustom = "CUSTOM_EVENTS";
    private const string BudgetTypeDaily = "DAILY_BUDGET";
    private const string BidStrategyLowestCostWithCap = "LOWEST_COST_WITH_BID_CAP";
    private const string BidStrategyCostCap = "COST_CAP";

    private const string PositionFeed = "feed";
    private const string PositionProfileFeed = "profile_feed";
    private const string PositionMessengerHome = "messenger_home";

    #endregion

    private readonly IStringLocalizer<AdsAutomationResource> _language;

    public AdsPublishTemplateViewModelValidator(IStringLocalizer<AdsAutomationResource> language)
    {
        _language = language;
    }

    public Task AddErrorsAsync(ObjectValidationContext context)
    {
        if (context.ValidatingObject is not AdsPublishTemplateViewModel model)
            return Task.CompletedTask;

        ValidateModel(model);
        return Task.CompletedTask;
    }

    // ================================================================
    // 第 1 层：模板级别校验
    // ================================================================

    private void ValidateModel(AdsPublishTemplateViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.TemplateName))
            throw Error("Template:NameRequired");

        if (model.Platform == PlatformType.NONE)
            throw Error("Template:PlatformRequired");

        if (model.Platform == PlatformType.META && model.MetaPublishingData == null)
            throw Error("Template:MetaDataRequired");

        if (model.Platform == PlatformType.META)
        {
            ValidateMetaDeep(model.MetaPublishingData!, model.PublishingAdType);
            ValidateMetaPixelResource(model);
        }
    }

    // ================================================================
    // 第 2 层：META 深度校验（来源：MetaPublishDataViewModelValidator.VerifyMetaViewModel）
    // ================================================================

    private void ValidateMetaDeep(MetaPublishDataViewModel meta, AdsPublishingAdType publishingAdType)
    {
        var campaign = meta.CampaignData;
        var adset = meta.AdsetData;
        var ad = meta.AdData;
        var audience = meta.AudienceData;

        // ---- 1. 基础空值校验 ----
        if (campaign == null) throw Error("Template:Validate:CampaignRequired");
        if (adset == null) throw Error("Template:Validate:AdsetRequired");
        if (ad == null) throw Error("Template:Validate:AdRequired");
        if (audience == null) throw Error("Template:Validate:AudienceRequired");

        // ---- 2. 命名规则校验 ----
        ValidateNameRules(campaign, adset, ad);

        // ---- 3. 广告系列基本校验 ----
        if (campaign.BuyingType != BuyingTypeAuction)
            throw Error("Template:Validate:BuyingType");

        // ---- 4. 预算校验 ----
        var isAdvanced = campaign.SmartPromotionType == AdsPublishingSmartPromotionType.SMART_APP_PROMOTION
                      || campaign.SmartPromotionType == AdsPublishingSmartPromotionType.AUTOMATED_SHOPPING_ADS;
        ValidateBudget(campaign, adset, isAdvanced);

        // ---- 5. Pixel 推广目标分发 ----
        if (publishingAdType == AdsPublishingAdType.PIXEL)
        {
            // 进阶赋能不可用商品目录
            if (campaign.SmartPromotionType != AdsPublishingSmartPromotionType.GUIDED_CREATION
                && campaign.IsProductCatalog)
            {
                throw Error("Template:Validate:AdvancedNoCatalog");
            }

            if (campaign.IsProductCatalog && campaign.SmartPromotionType == AdsPublishingSmartPromotionType.GUIDED_CREATION)
            {
                // Pixel + 商品目录 → 不做 Pixel 特有校验（目录校验待后续补充）
                ValidateAd(ad, campaign, publishingAdType);
                ValidateAudience(audience, campaign);
                return;
            }

            // Pixel + 标准（非目录、非应用转化）
            ValidatePixelStandard(campaign, adset, ad, audience, publishingAdType, isAdvanced);
            return;
        }

        // ---- 6. 通用：广告校验 + 受众校验 ----
        ValidateAd(ad, campaign, publishingAdType);
        ValidateAudience(audience, campaign);
    }

    // ================================================================
    // 命名规则校验
    // ================================================================

    private void ValidateNameRules(MetaCampaignViewModel campaign, MetaAdsetViewModel adset, MetaAdViewModel ad)
    {
        if (string.IsNullOrWhiteSpace(campaign.NameRule))
            throw Error("Template:Validate:CampaignNameRuleRequired");
        if (string.IsNullOrWhiteSpace(adset.NameRule))
            throw Error("Template:Validate:AdsetNameRuleRequired");
        if (string.IsNullOrWhiteSpace(ad.NameRule))
            throw Error("Template:Validate:AdNameRuleRequired");

        var allNameRule = $"{campaign.NameRule}{adset.NameRule}{ad.NameRule}";

        // #AD_STRUCTURE# 校验：三个命名规则必须同时包含且各只出现一次
        if (allNameRule.Contains(AdStructureVariable))
        {
            if (campaign.NameRule.Split(AdStructureVariable).Length != 2
                || adset.NameRule.Split(AdStructureVariable).Length != 2
                || ad.NameRule.Split(AdStructureVariable).Length != 2)
            {
                throw Error("Template:Validate:AdStructureOnce");
            }
            if (string.IsNullOrEmpty(campaign.AdStructSplit))
                throw Error("Template:Validate:AdStructSplitRequired");
        }

        // #MATERIAL_NAME# 校验：只能在广告级别使用
        if (allNameRule.Contains(MaterialNameVariable))
        {
            if (campaign.NameRule.Contains(MaterialNameVariable) || adset.NameRule.Contains(MaterialNameVariable))
                throw Error("Template:Validate:MaterialNameAdOnly");
        }
    }

    // ================================================================
    // 预算校验（来源：VerifyBudget）
    // ================================================================

    private void ValidateBudget(MetaCampaignViewModel campaign, MetaAdsetViewModel adset, bool isAdvanced)
    {
        if (isAdvanced && campaign.IsOpenBudget)
            throw Error("Template:Validate:AdvancedNoCampaignBudget");

        string? bidStrategy = null;

        if (campaign.IsOpenBudget)
        {
            // 广告系列预算
            if (string.IsNullOrEmpty(campaign.BudgetType))
                throw Error("Template:Validate:BudgetTypeRequired");
            if (adset.IsOpenBudget)
                throw Error("Template:Validate:BudgetConflict");
            bidStrategy = campaign.BidStrategy;
            VerifyBudgetValue(campaign.BudgetType, campaign.Budget);

            // 花费上下限
            if (adset.IsOpenSpendLimit && adset.MinSpendLimit > 0 && adset.SpendLimit > 0
                && adset.MinSpendLimit >= adset.SpendLimit)
            {
                throw Error("Template:Validate:SpendLimitRange");
            }
        }
        else
        {
            // 广告组预算
            if (!adset.IsOpenBudget)
                throw Error("Template:Validate:AdsetBudgetRequired");
            if (string.IsNullOrEmpty(adset.BudgetType))
                throw Error("Template:Validate:BudgetTypeRequired");
            bidStrategy = adset.BidStrategy;
            VerifyBudgetValue(adset.BudgetType, adset.Budget);
        }

        // 竞价策略校验
        if (!isAdvanced && string.IsNullOrEmpty(bidStrategy) && adset.BidAmount > 0)
            throw Error("Template:Validate:BidStrategyRequired");
        if (!isAdvanced && (bidStrategy == BidStrategyLowestCostWithCap || bidStrategy == BidStrategyCostCap))
        {
            if (adset.BidAmount == null || adset.BidAmount < 0.01f)
                throw Error("Template:Validate:BidAmountRequired");
        }

        void VerifyBudgetValue(string? budgetType, float budget)
        {
            if (budget < 1)
                throw Error(budgetType == BudgetTypeDaily
                    ? "Template:Validate:DailyBudgetMin"
                    : "Template:Validate:TotalBudgetMin");
        }
    }

    // ================================================================
    // Pixel 标准路径校验
    // ================================================================

    private void ValidatePixelStandard(
        MetaCampaignViewModel campaign, MetaAdsetViewModel adset, MetaAdViewModel ad,
        MetaAudienceViewModel audience, AdsPublishingAdType publishingAdType, bool isAdvanced)
    {
        // 转化事件校验
        if ((adset.EventType == EventTypeStandard && string.IsNullOrEmpty(adset.CustomEventType))
            || (adset.EventType == EventTypeCustom && (adset.CustomEventId == null || adset.CustomEventId == 0)))
        {
            throw Error("Template:Validate:EventRequired");
        }

        // 非进阶赋能时的手动版位操作系统校验
        if (!isAdvanced && audience.IsManualPosition == true && !string.IsNullOrEmpty(audience.AppType))
        {
            if (string.IsNullOrEmpty(audience.AppMinSystemVer))
                throw Error("Template:Validate:AppMinSystemVerRequired");
            if (!string.IsNullOrEmpty(audience.AppMaxSystemVer)
                && float.TryParse(audience.AppMinSystemVer, out var minVer)
                && float.TryParse(audience.AppMaxSystemVer, out var maxVer)
                && minVer >= maxVer)
            {
                throw Error("Template:Validate:AppVersionRange");
            }
        }

        ValidateAd(ad, campaign, publishingAdType);
        ValidateAudience(audience, campaign);
    }

    // ================================================================
    // 广告创意校验（来源：VerifyAd）
    // ================================================================

    private void ValidateAd(MetaAdViewModel ad, MetaCampaignViewModel campaign, AdsPublishingAdType publishingAdType)
    {
        if (ad.CallActionTypes == null || ad.CallActionTypes.Count == 0)
            throw Error("Template:Validate:CallActionRequired");
        if (ad.CallActionTypes.Distinct().Count() != ad.CallActionTypes.Count)
            throw Error("Template:Validate:CallActionDuplicate");
        if (ad.CallActionTypes.Count > 5)
            throw Error("Template:Validate:CallActionMax");

        // Pixel + 非商品目录 → 网站地址必填且需 http/https 开头
        if (publishingAdType == AdsPublishingAdType.PIXEL && !campaign.IsProductCatalog)
        {
            if (string.IsNullOrEmpty(ad.WebUrl))
                throw Error("Template:Validate:WebUrlRequired");
            if (!ad.WebUrl!.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                && !ad.WebUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                throw Error("Template:Validate:WebUrlProtocol");
            }
            // 追踪域名格式校验
            if (!string.IsNullOrEmpty(ad.TrackPixelUrl)
                && !Regex.IsMatch(ad.TrackPixelUrl, @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$"))
            {
                throw Error("Template:Validate:TrackPixelUrlFormat", ad.TrackPixelUrl);
            }
        }
    }

    // ================================================================
    // 受众校验（来源：VerifyAudiencePositions）
    // ================================================================

    private void ValidateAudience(MetaAudienceViewModel audience, MetaCampaignViewModel campaign)
    {
        // 进阶赋能不校验版位
        if (campaign.SmartPromotionType != AdsPublishingSmartPromotionType.GUIDED_CREATION)
            return;

        // 手动版位校验
        if (audience.IsManualPosition == true)
        {
            var pos = audience.PositionPublisher;
            if (pos == null
                || (pos.Facebook == null || pos.Facebook.Count == 0)
                && (pos.Instagram == null || pos.Instagram.Count == 0)
                && (pos.AudienceNetwork == null || pos.AudienceNetwork.Count == 0)
                && (pos.Messenger == null || pos.Messenger.Count == 0))
            {
                throw Error("Template:Validate:PositionRequired");
            }

            // profile_feed 需要同时选 feed
            if (pos.Facebook != null
                && pos.Facebook.Any(p => p == PositionProfileFeed)
                && pos.Facebook.All(p => p != PositionFeed))
            {
                throw Error("Template:Validate:ProfileFeedNeedFeed");
            }

            // messenger_home 需要同时选 feed
            if (pos.Messenger != null
                && pos.Messenger.Any(p => p == PositionMessengerHome)
                && (pos.Facebook == null || pos.Facebook.All(p => p != PositionFeed)))
            {
                throw Error("Template:Validate:MessengerHomeNeedFeed");
            }
        }

        // 年龄校验
        if (audience.AgeMax.HasValue && audience.AgeMin > audience.AgeMax)
            throw Error("Template:Validate:AgeRange");
        if (!audience.IsAdvancedAudience)
        {
            if (audience.AgeMin < 13)
                throw Error("Template:Validate:AgeMinTooLow");
            if (audience.AgeMax > 65)
                throw Error("Template:Validate:AgeMaxTooHigh");
        }

        // 地区类型
        if (!string.IsNullOrEmpty(audience.LocationType) && audience.LocationType != "RECENT_HOME")
            throw Error("Template:Validate:LocationType");

        // 设备包含/排除互斥
        if (audience.AppDevices != null && audience.AppExcludedDevices != null)
        {
            var overlap = audience.AppExcludedDevices.Where(d => audience.AppDevices!.Contains(d)).ToList();
            if (overlap.Count > 0)
                throw Error("Template:Validate:DeviceOverlap", string.Join(",", overlap));
        }
    }

    // ================================================================
    // META Pixel 资源关联校验（来源：AdsPublishingTemplateViewModelValidator lines 98-131）
    // ================================================================

    private void ValidateMetaPixelResource(AdsPublishTemplateViewModel model)
    {
        if (model.Platform != PlatformType.META || model.PublishingAdType != AdsPublishingAdType.PIXEL)
            return;

        var campaign = model.MetaPublishingData!.CampaignData;

        // 1. Pixel + 应用转化 → ApplicationId
        if (campaign.ConversionType == MetaPixelConversionType.APPLICATION
            && (model.ApplicationId == null || model.ApplicationId == 0))
        {
            throw Error("Template:ApplicationIdRequired");
        }

        // 2. Pixel + GUIDED_CREATION + 非APPLICATION + 非ProductCatalog → PixelId
        if (campaign.SmartPromotionType == AdsPublishingSmartPromotionType.GUIDED_CREATION
            && campaign.ConversionType != MetaPixelConversionType.APPLICATION
            && !campaign.IsProductCatalog
            && (model.PixelId == null || model.PixelId == 0))
        {
            throw Error("Template:PixelIdRequired");
        }

        // 3. Pixel + GUIDED_CREATION + ProductCatalog → ProductCatalogId
        if (campaign.SmartPromotionType == AdsPublishingSmartPromotionType.GUIDED_CREATION
            && campaign.IsProductCatalog
            && (model.ProductCatalogId == null || model.ProductCatalogId == 0))
        {
            throw Error("Template:ProductCatalogIdRequired");
        }

        // 4. Pixel + AUTOMATED_SHOPPING_ADS → PixelId
        if (campaign.SmartPromotionType == AdsPublishingSmartPromotionType.AUTOMATED_SHOPPING_ADS
            && (model.PixelId == null || model.PixelId == 0))
        {
            throw Error("Template:PixelIdRequired");
        }
    }

    // ================================================================
    // Helper：统一抛出 BusinessException（带本地化消息）
    // ================================================================

    private Domain.Shared.Common.BusinessException Error(string key, params object[] args)
    {
        var message = args.Length > 0
            ? string.Format(_language[key], args)
            : _language[key];
        return new Domain.Shared.Common.BusinessException(message, 400);
    }
}
