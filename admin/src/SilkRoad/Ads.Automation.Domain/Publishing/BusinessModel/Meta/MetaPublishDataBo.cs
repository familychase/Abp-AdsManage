using Ads.Automation.Domain.Publishing.BusinessModel.MediaAudience;
using Ads.Automation.Domain.Publishing.BusinessModel.CrossPublishing.Meta;
using Ads.Automation.Domain.Shared.Enums.Publishing;

namespace Ads.Automation.Domain.Publishing.BusinessModel.Meta;

/// <summary>
/// Meta 平台的发布对象视图模型（桩类，属性按需补充）
/// </summary>
public sealed class MetaPublishDataBo
{
    /// <summary>版本号（原版本默认为 0，当前版本号为 1）</summary>
    public int? Version { get; set; }

    /// <summary>广告系列数据</summary>
    public MetaCampaignBo CampaignData { get; set; } = null!;

    /// <summary>广告组数据</summary>
    public MetaAdsetBo AdsetData { get; set; } = null!;

    /// <summary>广告创意数据</summary>
    public MetaAdBo AdData { get; set; } = null!;

    /// <summary>受众数据</summary>
    public MetaAudienceBo AudienceData { get; set; } = null!;

    /// <summary>账户参数列表</summary>
    public List<MetaPublishAccountData> AccountData { get; set; } = null!;

    /// <summary>广告系列状态</summary>
    public bool CampaignStatus { get; set; } = true;
}

/// <summary>
/// Meta 发布账户数据
/// </summary>
public class MetaPublishAccountData
{
    public MetaPublishAccountData() { }

    public MetaPublishAccountData(long accountId, string? pageNo)
    {
        AccountId = accountId;
        PageNo = pageNo;
    }

    /// <summary>广告账户 ID</summary>
    public long AccountId { get; set; }

    /// <summary>主页编号（Facebook Page ID）</summary>
    public string? PageNo { get; set; }
}

/// <summary>
/// Meta 广告系列 BO
/// </summary>
public class MetaCampaignBo
{
    /// <summary>系列名称规则</summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>广告结构拆分标识</summary>
    public string AdStructSplit { get; set; } = string.Empty;

    /// <summary>购买类型</summary>
    public string BuyingType { get; set; } = string.Empty;

    /// <summary>是否开启预算控制</summary>
    public bool IsOpenBudget { get; set; }

    /// <summary>预算类型</summary>
    public string BudgetType { get; set; } = string.Empty;

    /// <summary>预算金额</summary>
    public decimal Budget { get; set; }

    /// <summary>出价策略</summary>
    public string? BidStrategy { get; set; }

    /// <summary>投放节奏类型</summary>
    public string? PacingType { get; set; }

    /// <summary>是否开启 iOS14</summary>
    public bool? OpenIOS14 { get; set; }

    /// <summary>进阶赋能类型</summary>
    public AdsPublishingSmartPromotionType SmartPromotionType { get; set; }

    /// <summary>像素转化设置类型</summary>
    public MetaPixelConversionType? ConversionType { get; set; }

    /// <summary>是否开启特殊广告类别</summary>
    public bool IsOpenSpecialAd { get; set; }

    /// <summary>特殊广告类别列表</summary>
    public List<MetaSpecialAdStatementCategories>? SpecialAdCategories { get; set; }

    /// <summary>国家代码列表</summary>
    public List<MetaCrossPublishDataAreaGroupAreaBo>? CountryCodes { get; set; }
}

/// <summary>
/// Meta 广告组 BO
/// </summary>
public class MetaAdsetBo
{
    /// <summary>广告组名称规则</summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>是否开启预算控制</summary>
    public bool IsOpenBudget { get; set; }

    /// <summary>预算类型</summary>
    public string BudgetType { get; set; } = string.Empty;

    /// <summary>预算金额</summary>
    public decimal Budget { get; set; }

    /// <summary>出价策略</summary>
    public string? BidStrategy { get; set; }

    /// <summary>出价金额</summary>
    public float? BidAmount { get; set; }

    /// <summary>投放节奏类型</summary>
    public string? PacingType { get; set; }

    /// <summary>是否开启花费上限</summary>
    public bool IsOpenSpendLimit { get; set; }

    /// <summary>最低花费上限</summary>
    public decimal? MinSpendLimit { get; set; }

    /// <summary>花费上限</summary>
    public decimal? SpendLimit { get; set; }

    /// <summary>优化目标（如 OFFSITE_CONVERSIONS、VALUE）</summary>
    public string OptimizationGoal { get; set; } = string.Empty;

    /// <summary>应用事件类型</summary>
    public string AppEventType { get; set; } = null!;

    /// <summary>自定义事件类型</summary>
    public string CustomEventType { get; set; } = string.Empty;

    /// <summary>事件类型</summary>
    public string? EventType { get; set; }

    /// <summary>自定义事件 ID</summary>
    public long? CustomEventId { get; set; }

    /// <summary>计费事件类型</summary>
    public string BillingEvent { get; set; } = string.Empty;

    /// <summary>归因窗口字符串（V0 版本）</summary>
    public string AttributionSpec { get; set; } = string.Empty;

    /// <summary>归因窗口模型对象（V1 版本）</summary>
    public MetaAdsPublishAttributionSpecBo? AdsAttributionSpec { get; set; }

    /// <summary>投放开始时间</summary>
    public string StartTime { get; set; } = string.Empty;

    /// <summary>投放结束时间</summary>
    public string EndTime { get; set; } = string.Empty;

    /// <summary>是否开启分时段投放</summary>
    public bool OpenTimeSchedule { get; set; }

    /// <summary>时区类型</summary>
    public string TimeScheduleZoneType { get; set; } = string.Empty;

    /// <summary>分时段投放时间表</summary>
    public List<List<long>>? TimeSchedule { get; set; }

    /// <summary>ROAS 出价</summary>
    public float? RoasBid { get; set; }

    /// <summary>商品受众类型</summary>
    public string? ProductAudienceType { get; set; }

    /// <summary>广告系列归因</summary>
    public string? CampaignAttribution { get; set; }

    /// <summary>时区类型</summary>
    public string? TimeZoneType { get; set; }

    /// <summary>归因类型（默认 / 增量）</summary>
    public MetaAdsPublishAttributionType AdsAttributionType { get; set; }
}

/// <summary>
/// Meta 广告创意 BO
/// </summary>
public class MetaAdBo
{
    /// <summary>
    /// 名称规则（支持占位符替换，如 #YEAR##MONTH##DAY#）
    /// </summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>
    /// 行动号召类型列表（MetaCallActionType，动态广告可为多个）
    /// </summary>
    public List<string> CallActionTypes { get; set; } = null!;

    /// <summary>
    /// 主页 ID（Facebook Page ID）
    /// </summary>
    public string PageNo { get; set; } = null!;

    /// <summary>
    /// 应用深度链接
    /// </summary>
    public string AppDeepLink { get; set; } = string.Empty;

    /// <summary>
    /// 使用文案中的深度链接
    /// </summary>
    public bool UseLetterDeepLink { get; set; }

    /// <summary>
    /// 网站地址（落地页 URL）
    /// </summary>
    public string WebUrl { get; set; } = null!;

    /// <summary>
    /// 显示地址（非必填）
    /// </summary>
    public string DisplayUrl { get; set; } = null!;

    /// <summary>
    /// 落地页追踪像素编号（仅投放应用时可填）
    /// </summary>
    public string TrackPixelNo { get; set; } = null!;

    /// <summary>
    /// 网域追踪地址（仅为落地页时可填）
    /// </summary>
    public string TrackPixelUrl { get; set; } = null!;

    ///// <summary>
    ///// 发布网关使用类型
    ///// </summary>
    //public AdsPublishUseGateWayType AdsPublishUseGateWayType { get; set; }

    /// <summary>
    /// 是否使用公共主页而不是应用名称作为广告发布身份
    /// </summary>
    public bool? UsePageIdentity { get; set; }

    /// <summary>
    /// 是否开启进阶赋能素材（默认 false）
    /// </summary>
    public bool IsOpenAdvancedMaterial { get; set; }

    /// <summary>
    /// 是否为多广告组广告
    /// </summary>
    public bool MultiAds { get; set; }

    ///// <summary>
    ///// 单图片进阶赋能素材选项
    ///// </summary>
    //public AdvancedMaterialTypeBo? ImageOption { get; set; }

    ///// <summary>
    ///// 单视频进阶赋能素材选项
    ///// </summary>
    //public AdvancedMaterialTypeBo? VideoOption { get; set; }

    ///// <summary>
    ///// 轮播进阶赋能素材选项
    ///// </summary>
    //public AdvancedMaterialTypeBo? CarouselOption { get; set; }

    /// <summary>
    /// 自定义商品页面 ID
    /// </summary>
    public string? AppProductPageId { get; set; }

    /// <summary>
    /// 是否开启再营销
    /// </summary>
    public bool OpenRemarketing { get; set; }

    /// <summary>
    /// 网站地址（未替换参数的原始地址，供后端调用）
    /// </summary>
    public string? OriginWebUrl { get; set; }
}

/// <summary>
/// 进阶赋能素材类型 — 控制各素材美化开关
/// </summary>
public class AdvancedMaterialTypeBo
{
    public AdvancedMaterialTypeBo() { }

    /// <summary>
    /// 根据子项自动设置标准美化总开关
    /// </summary>
    /// <param name="isStandardEnhancements">是否标准美化；传 null 时由子项自动推导</param>
    public void SetStandardEnhancements(bool? isStandardEnhancements = null!)
    {
        if (isStandardEnhancements.IsNotNull())
        {
            IsStandardEnhancements = isStandardEnhancements!.Value;
        }
        else
        {
            IsStandardEnhancements = ProductExtensions
                                     || ImageTouchups
                                     || TextOptimizations
                                     || InlineComment
                                     || ImageTemplates
                                     || ImageUncrop
                                     || CvTransformation
                                     || ImageEnhancement
                                     || ProfileCard
                                     || DescriptionAutomation
                                     || MediaOrder;
        }
    }

    /// <summary>是否开启标准美化（总开关）</summary>
    public bool IsStandardEnhancements { get; set; }

    /// <summary>添加目录中的商品</summary>
    public bool ProductExtensions { get; set; }

    /// <summary>调整图像素材</summary>
    public bool ImageTouchups { get; set; }

    /// <summary>美化文字</summary>
    public bool TextOptimizations { get; set; }

    /// <summary>突显重要评论</summary>
    public bool InlineComment { get; set; }

    /// <summary>添加图片模板</summary>
    public bool ImageTemplates { get; set; }

    /// <summary>扩展图片</summary>
    public bool ImageUncrop { get; set; }

    /// <summary>添加 3D 动画</summary>
    public bool CvTransformation { get; set; }

    /// <summary>添加图片滤镜</summary>
    public bool ImageEnhancement { get; set; }

    /// <summary>添加资料图卡</summary>
    public bool ProfileCard { get; set; }

    /// <summary>定制说明</summary>
    public bool DescriptionAutomation { get; set; }

    /// <summary>突显卡片</summary>
    public bool MediaOrder { get; set; }
}

/// <summary>
/// Meta 受众 BO
/// </summary>
public class MetaAudienceBo
{
    /// <summary>是否手动版位</summary>
    public bool IsManualPosition { get; set; }

    /// <summary>发布版位列表</summary>
    public List<string>? PublisherPositions { get; set; }

    /// <summary>Facebook 具体版位</summary>
    public List<string>? FacebookPositions { get; set; }

    /// <summary>Instagram 具体版位</summary>
    public List<string>? InstagramPositions { get; set; }

    /// <summary>Audience Network 具体版位</summary>
    public List<string>? AudienceNetworkPositions { get; set; }

    /// <summary>Messenger 具体版位</summary>
    public List<string>? MessengerPositions { get; set; }

    /// <summary>性别（0=所有，1=男性，2=女性）</summary>
    public int Gender { get; set; }

    /// <summary>最大年龄（上限 65）</summary>
    public int? AgeMax { get; set; }

    /// <summary>最小年龄（下限 13，默认 18）</summary>
    public int AgeMin { get; set; }

    /// <summary>包含的国家组列表</summary>
    public List<string>? CountryGroups { get; set; }

    /// <summary>排除的国家组列表</summary>
    public List<string>? ExcludedCountryGroups { get; set; }

    /// <summary>国家/区域列表</summary>
    public List<MetaAudienceCountryBo>? Countries { get; set; }

    /// <summary>语言列表</summary>
    public List<string>? Languages { get; set; }

    /// <summary>地区人群类型（默认为 HOME）</summary>
    public string LocationType { get; set; } = null!;

    /// <summary>是否开启细分定位扩展</summary>
    public bool IsOpenTargetExtend { get; set; }

    /// <summary>应用平台（IOS / ANDROID）</summary>
    public string AppType { get; set; } = string.Empty;

    /// <summary>应用最低系统版本</summary>
    public string AppMinSystemVer { get; set; } = string.Empty;

    /// <summary>应用最高系统版本</summary>
    public string AppMaxSystemVer { get; set; } = string.Empty;

    /// <summary>包含的设备型号列表</summary>
    public List<string>? AppDevices { get; set; }

    /// <summary>排除的设备型号列表</summary>
    public List<string>? AppExcludedDevices { get; set; }

    /// <summary>是否开启仅 Wifi 连接</summary>
    public bool? OpenWirelessCarrier { get; set; }

    /// <summary>账户自定义受众列表</summary>
    public List<MediaPublishAccountAudienceBo>? Audiences { get; set; }

    /// <summary>是否排除可跳过的广告</summary>
    public bool? ExcludedAd { get; set; }

    /// <summary>Facebook 库存计划</summary>
    public MetaFacebookInventoryPlan FacebookInventoryPlan { get; set; }

    /// <summary>Audience Network 库存计划</summary>
    public MetaAudienceNetworkInventoryPlan NetworkInventoryPlan { get; set; }

    /// <summary>内容类型排除条件</summary>
    public bool ContentExclusionCondition { get; set; }

    /// <summary>细分定位（V1 版本，支持进一步限定）</summary>
    public List<List<MetaAdsPublishSubdivisionPositionBo>>? SubdivisionPositionV1 { get; set; }

    /// <summary>子版位</summary>
    public MetaAdsPublishDataPositionChildrenBo? PositionPublisher { get; set; } = null!;

    /// <summary>包含/排除地区列表</summary>
    public MetaAdsPublishDataAreaGroupAreaBo? Areas { get; set; } = null!;

    /// <summary>是否为进阶赋能受众</summary>
    public bool IsAdvancedAudience { get; set; }

    /// <summary>排除的细分定位</summary>
    public List<MetaAdsPublishSubdivisionPositionBo>? ExcludeSubdivisionPosition { get; set; }

    /// <summary>是否使用受众建议</summary>
    public bool? IsAudienceSuggest { get; set; }

    /// <summary>最小年龄限制</summary>
    public int? AgeMinLimit { get; set; }

    /// <summary>广告 RMT 参数</summary>
    [NotMapped]
    public string AdRmt { get; set; } = string.Empty;

    /// <summary>设备类型</summary>
    public MetaAdsPublishDevicePlatform? DeviceType { get; set; }

    /// <summary>包含的区域组 ID</summary>
    public long? IncludeRegionGroup { get; set; }
}
