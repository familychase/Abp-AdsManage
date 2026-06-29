// ══════════════════════════════════════════
//  模板管理 — TypeScript 类型定义
//  参照 Swagger AdsPublishTemplate 接口模型
// ══════════════════════════════════════════

import type { PlatformType } from '@/api/ads/channel/types'

// ── 查询参数 ──────────────────────────────

/** 模板列表查询参数（对应 AdsPublishTemplateListInput） */
export interface GetTemplateListInput {
  page: number
  pageSize: number
  sorting?: string
  name?: string
  platform?: PlatformType | string
  application_id?: number
  pixel_id?: number
  creator_id?: number
  dateRange?: {
    start: string
    stop: string
  }
}

/** 有效模板列表查询（发布选模板用） */
export interface GetValidTemplateListInput {
  page?: number
  pageSize?: number
  validType?: 'NORMAL'
  platform?: PlatformType
}

// ── 枚举 ──────────────────────────────────

export type TemplateGroupType = 'common' | 'rarely' | 'removed'
export type TemplateStatus = 'DRAFT' | 'PUBLISHED' | 'FAILED' | 'ARCHIVED'

/** 发布方式（对应 AdsBatchPublishingType） */
export type PublishingType = 'NONE' | 'AVG' | 'ALL'

/** 广告类型（对应 AdsPublishingAdType） */
export type PublishingAdType =
  | 'NONE'
  | 'APP'
  | 'PIXEL'
  | 'PRODUCT_CATALOG'
  | 'PERFORMANCE_MAX'
  | 'DISPLAY'
  | 'MULTI_CHANNEL'

/** 智能推广类型（对应 AdsPublishingSmartPromotionType） */
export type SmartPromotionType =
  | 'GUIDED_CREATION'
  | 'SMART_APP_PROMOTION'
  | 'AUTOMATED_SHOPPING_ADS'

/** 转化类型（对应 MetaPixelConversionType） */
export type PixelConversionType = 'NONE' | 'WEBSITE' | 'APPLICATION'

export type CreativePattern = 'NONE' | 'IDENTITY_VIDEO' | 'SMART_CREATIVE_AD'
export type TimeZoneType = 'START_NOW' | 'START_AT'

// ══════════════════════════════════════════
//  发布数据（平台特定 — 对应 Swagger MetaPublishDataViewModel）
// ══════════════════════════════════════════

/** 广告系列数据（对应 MetaCampaignViewModel） */
export interface CampaignData {
  campaign_name_rule?: string
  /** 广告结构命名分隔符 */
  ad_struct_split?: string
  buying_type?: string
  /** 是否开启特殊广告类别 */
  is_open_special_ad?: boolean
  /** 特殊广告类别（多选） */
  special_ad_categories?: string[]
  /** 国家代码（特殊广告类别关联） */
  country_codes?: string[]
  /** 是否开启 IOS 14+ */
  open_ios14?: boolean
  budget_type?: string
  budget?: number
  /** 是否开启广告系列预算 */
  is_open_budget?: boolean
  bid_strategy?: string
  pacing_type?: string
  /** 智能推广类型 */
  smart_promotion_type?: SmartPromotionType
  /** 转化类型 */
  conversion_type?: PixelConversionType
  /** 是否为商品目录广告 */
  is_product_catalog?: boolean
  objective_type?: string
  campaign_type?: string
  spc_type?: string
  app_promotion_type?: string
}

/** 归因规格 */
export interface AttributionSpec {
  attribution_type?: string
  click_through?: string
  engaged_video_view?: string
  view_through?: string
}

/** 地区条目 */
export interface AreaItem {
  type?: string
  name?: string
  code?: string
  country_code?: string
  region_code?: string
  is_excluded?: boolean
}

/** 地区配置 */
export interface AreaEntry {
  country_name?: string
  country_code?: string
  countries?: AreaItem[]
}

/** 地区受众结构 */
export interface LocationAreas {
  includes?: AreaEntry[]
  excludes?: AreaEntry[]
}

/** 产品受众条目 */
export interface ProductAudienceItem {
  is_excluded?: boolean
  day?: number
  event_type?: string
  product_set_id?: string
}

export interface ProductAudience {
  product_audience_type?: string
  product_set_id?: string
  day?: number
  audiences?: ProductAudienceItem[]
}

export interface ExcludedProductAudience {
  excluded_product_audience_type?: string
  day?: number
  audiences?: ProductAudienceItem[]
}

/** 广告组数据（对应 MetaAdsetViewModel） */
export interface AdSetData {
  adset_name_rule?: string
  /** 是否设置花费上下限 */
  is_open_spend_limit?: boolean
  /** 花费下限 */
  min_spend_limit?: number
  /** 花费上限 */
  spend_limit?: number
  budget_type?: string
  budget?: number
  optimization_goal?: string
  /** 广告归因方法 */
  campaign_attribution?: string
  /** 是否开启广告组预算 */
  is_open_budget?: boolean
  bid_strategy?: string
  pacing_type?: string
  /** 事件类型 */
  event_type?: 'STANDARD_EVENTS' | 'SPECIAL_EVENTS' | string
  /** 应用事件类型 */
  app_event_type?: string
  /** 自定义事件 ID */
  custom_event_id?: string | number
  /** 转化事件（Pixel 用） */
  custom_event_type?: string
  /** 竞价金额 */
  bid_amount?: number
  /** 计费方式 */
  billing_event?: string
  /** 归因设置 */
  attribution_spec?: AttributionSpec
  start_time?: string
  end_time?: string
  time_zone_type?: TimeZoneType
  time_schedule_zone_type?: string
  placements?: string[]
  shopping_ads_type?: string
  // ── 时间表 ──
  /** 是否开启时间表 */
  open_time_schedule?: boolean
  /** 时间表数据 */
  time_schedule?: number[][]
  /** ROAS 竞价 */
  roas_bid?: number
  /** 时区类型 */
  time_zone_type_v2?: string
  // ── 金融/受益方 ──
  is_taiwan_finserv?: boolean
  is_unified_beneficiary?: boolean
  unified_beneficiary?: string
  unified_payor?: string
  beneficiary?: string
  payor?: string
  singapore_beneficiary?: string
  singapore_payor?: string
  taiwan_beneficiary?: string
  taiwan_payor?: string
  australia_beneficiary?: string
  australia_payor?: string
}

/** 广告数据（对应 MetaAdViewModel） */
export interface AdData {
  ad_name_rule?: string
  /** 行动号召（多选） */
  call_action_types?: string
  /** 应用深度链接 */
  app_deep_link?: string
  /** 应用产品页面 ID */
  app_product_page_id?: string
  /** 落地页 URL */
  web_url?: string
  /** 显示地址 */
  display_url?: string
  /** 以公共主页身份发布 */
  use_page_identity?: boolean
  /** 网关类型 */
  use_gateway_type?: 'USE' | 'NO_USE'
  /** 再营销 */
  open_remarketing?: boolean
  /** 多广告主广告 */
  multi_ads?: boolean
  /** 网域追踪地址 */
  track_pixel_url?: string
  /** 落地页追踪 Pixel */
  track_pixel_no?: string
  /** 追踪应用事件编号 */
  track_application_no?: string
  call_to_action?: string
  page_id?: string
  deep_link?: string
  use_letter_deep_link?: boolean
  origin_web_url?: string
  image_option?: string
  video_option?: string
  carousel_option?: string
  is_open_advanced?: boolean
  identity_type?: string
}

/** 发布账户数据（对应 MetaPublishAccountDataViewModel） */
export interface MetaPublishAccountData {
  accountId: number
  pageNo?: string
}

/** 受众数据（对应 MetaAudienceViewModel） */
export interface AudienceData {
  // ── 进阶赋能受众 ──
  audience_ids?: string[]
  excluded_audience_ids?: string[]
  is_advanced_audience?: boolean
  location_type?: string
  include_region_group?: string
  areas?: LocationAreas
  age_min_limit?: number
  is_audience_suggest?: boolean
  age_min?: number
  age_max?: number
  gender?: number
  languages?: string[]
  country_groups?: string[]
  excluded_country_groups?: string[]
  include_subdivision_position?: Record<string, any>[]
  exclude_subdivision_position?: Record<string, any>[]
  // ── 商品目录受众 ──
  product_audience_type?: string
  product_audience?: ProductAudience
  excluded_product_audience?: ExcludedProductAudience
  // ── 版位 ──
  is_manual_position?: boolean
  device_type?: string
  app_type?: string
  app_devices?: string[]
  app_excluded_devices?: string[]
  position_publisher?: {
    audience_network?: string[]
    facebook?: string[]
    instagram?: string[]
    messenger?: string[]
  }
  excluded_ad?: boolean
  app_min_system_ver?: string
  app_max_system_ver?: string
  open_wireless_carrier?: boolean
  facebook_inventory_plan?: string
  network_inventory_plan?: string
  content_exclusion_condition?: boolean
  // ── 兼容旧字段 ──
  genders?: string[]
  countries?: string[]
  placements?: string[]
}

// ══════════════════════════════════════════
//  平台特定发布数据
// ══════════════════════════════════════════

/** Meta 平台发布数据（对应 Swagger MetaPublishDataViewModel） */
export interface MetaPublishingData {
  campaign_data: CampaignData
  adset_data: AdSetData
  ad_data: AdData
  audience_data: AudienceData
  /** 发布账户数据 */
  account_data?: MetaPublishAccountData[]
}

export interface GooglePublishingData {
  campaign_data: CampaignData
  adset_data: AdSetData
  ad_data: AdData
  audience_data: AudienceData
}

export interface TikTokPublishingData {
  campaign_data: CampaignData
  adset_data: AdSetData
  ad_data: AdData
  budget_and_schedule_data: BudgetAndScheduleData
}

export interface SnapchatPublishingData {
  campaign_data: CampaignData
  adset_data: AdSetData
  ad_data: AdData
  audience_data: AudienceData
}

/** 预算和排期数据（TikTok 特有） */
export interface BudgetAndScheduleData {
  budget_type?: string
  budget?: number
  bid_type?: string
  bid_value?: number
  start_time?: string
  end_time?: string
  optimization_goal?: string
  vbo_window?: string
}

/** 发布信息 */
export interface PublishInfo {
  publisher: string
  publishTime: string
  publishType: string
  active: boolean
}

// ══════════════════════════════════════════
//  模板 DTO
// ══════════════════════════════════════════

/** 模板列表项 */
export interface TemplateDto {
  id: number
  templateId?: number
  templateName: string
  templateIcon?: string
  appName?: string
  appId?: string
  platform: PlatformType
  publishingAdType?: PublishingAdType
  deliveryMode?: string
  publishInfo?: PublishInfo
  publishStatus: TemplateStatus
  successCount: number
  plannedCount: number
  adAccountCount: number
  adAccountName?: string
  campaignCount: number
  campaignName?: string
  creator?: string
  departmentName?: string
  createdTime?: string
  updatedTime?: string
  /** 模板标签类型列表 */
  templateTagType?: string[]
}

/** 模板详情 */
export interface TemplateDetail extends TemplateDto {
  publishingType?: PublishingType
  /** 平台特定发布数据 — 只有当前平台字段有值 */
  metaPublishingData?: MetaPublishingData
  googlePublishingData?: GooglePublishingData
  tiktokPublishingData?: TikTokPublishingData
  snapchatPublishingData?: SnapchatPublishingData
}

/** 模板分页结果 */
export interface TemplatePagedResult {
  items: TemplateDto[]
  totalCount: number
}

/** 模板统计结果 */
export interface TemplateStatistics {
  commonTotal: number
  rarelyTotal: number
  removedTotal: number
}

/** AD 额度使用情况 */
export interface AdQuotaInfo {
  used: number
  total: number
  expandTotal: number
}

// ══════════════════════════════════════════
//  模板创建/编辑输入（对应 Swagger AdsPublishTemplateViewModel）
// ══════════════════════════════════════════

/** 模板保存输入（对应 Swagger AdsPublishTemplateViewModel） */
export interface AdsPublishTemplateViewModel {
  /** 版本号 */
  version?: number
  /** 模板 ID（编辑时必传） */
  templateId?: number
  /** 模板名称 */
  templateName: string
  /** 平台 */
  platform: PlatformType
  /** 广告类型 */
  publishingAdType?: PublishingAdType
  /** 应用 ID */
  applicationId?: number
  /** 像素 ID */
  pixelId?: number
  /** 商品目录 ID */
  productCatalogId?: number
  /** 资源内容（JSON 字符串） */
  resourceContent?: string
  /** 发布方式 */
  publishingType?: PublishingType
  /** 最大发布数量 */
  maxPublishCount?: number
  /** 是否平均分配 */
  publishAverage?: boolean
  /** Meta 平台发布数据 */
  metaPublishingData?: MetaPublishingData
}

/** @deprecated 保留兼容：旧的模板保存输入 */
export interface SaveTemplateInput {
  templateId?: number
  templateName: string
  description?: string
  platform: PlatformType
  publishingAdType?: PublishingAdType
  appId?: string
  pixelId?: string
  usePeople?: string
  metaPublishingData?: MetaPublishingData
  googlePublishingData?: GooglePublishingData
  tiktokPublishingData?: TikTokPublishingData
  snapchatPublishingData?: SnapchatPublishingData
}

/** 模板创建输入（简化版） */
export interface CreateTemplateInput {
  templateName: string
  platform: PlatformType
  appId?: string
  pixelId?: string
  deliveryMode?: string
  departmentId?: string
}

// ══════════════════════════════════════════
//  发布模板列表（对应 AdsPublishTemplateListDto）
// ══════════════════════════════════════════

/** 发布模板列表项 */
export interface AdsPublishTemplateItem {
  id: number
  version: string
  name: string
  platform: PlatformType
  publishingAdType: string
  resourceId: number
  publishAdCount: number
  lastPublishTime: string | null
  creationTime: string
  creatorId: number
  creatorName: string
}

/** 发布模板分页结果 */
export interface AdsPublishTemplatePagedResult {
  items: AdsPublishTemplateItem[]
  totalCount: number
}

// ══════════════════════════════════════════
//  辅助函数：SaveTemplateInput → AdsPublishTemplateViewModel
// ══════════════════════════════════════════

/** 将旧格式 SaveTemplateInput 转换为新的 AdsPublishTemplateViewModel */
export function toAdsPublishTemplateViewModel(
  input: SaveTemplateInput
): AdsPublishTemplateViewModel {
  return {
    version: undefined,
    templateId: input.templateId,
    templateName: input.templateName,
    platform: input.platform,
    publishingAdType: input.publishingAdType,
    applicationId: input.appId ? Number(input.appId) : undefined,
    pixelId: input.pixelId ? Number(input.pixelId) : undefined,
    productCatalogId: undefined,
    resourceContent: undefined,
    publishingType: undefined,
    maxPublishCount: undefined,
    publishAverage: undefined,
    metaPublishingData: input.platform === 2 ? input.metaPublishingData : undefined
  }
}

// ══════════════════════════════════════════
//  内部 snake_case → Swagger camelCase 转换
// ══════════════════════════════════════════

type AnyDict = Record<string, any>

/** 将前端内部 snake_case MetaPublishingData 转为后端 Swagger 期望的 camelCase */
export function toSwaggerMetaPublishData(data: MetaPublishingData): AnyDict {
  const result: AnyDict = {}

  // campaign_data
  if (data.campaign_data) {
    const c = data.campaign_data
    result.campaign_data = {
      nameRule: c.campaign_name_rule ?? '',
      adStructSplit: c.ad_struct_split ?? '',
      buyingType: c.buying_type ?? '',
      isOpenBudget: c.is_open_budget ?? false,
      budgetType: c.budget_type ?? '',
      budget: c.budget ?? 0,
      bidStrategy: c.bid_strategy ?? '',
      pacingType: c.pacing_type ?? '',
      openIOS14: c.open_ios14 ?? false,
      smartPromotionType: c.smart_promotion_type ?? 'GUIDED_CREATION',
      conversionType: c.conversion_type ?? 'NONE',
      isProductCatalog: c.is_product_catalog ?? false,
      isOpenSpecialAd: c.is_open_special_ad ?? false,
      specialAdCategories: c.special_ad_categories ?? [],
      countryCodes: (c.country_codes ?? []).map((code: string) => ({
        key: code,
        type: 'country',
        name: code,
        countryCode: code
      }))
    }
  }

  // adset_data — 广告系列开启预算时，广告组预算传 0
  if (data.adset_data) {
    const a = data.adset_data
    const campaignOpenBudget = data.campaign_data?.is_open_budget ?? false
    result.adset_data = {
      nameRule: a.adset_name_rule ?? '',
      isOpenBudget: a.is_open_budget ?? false,
      budgetType: campaignOpenBudget ? null : (a.budget_type ?? ''),
      budget: campaignOpenBudget ? 0 : (a.budget ?? 0),
      bidStrategy: campaignOpenBudget ? null : (a.bid_strategy ?? ''),
      bidAmount: a.bid_amount ?? 0,
      pacingType: a.pacing_type ?? '',
      isOpenSpendLimit: a.is_open_spend_limit ?? false,
      minSpendLimit: a.min_spend_limit ?? 0,
      spendLimit: a.spend_limit ?? 0,
      optimizationGoal: a.optimization_goal ?? '',
      appEventType: a.app_event_type ?? '',
      customEventType: a.custom_event_type ?? '',
      eventType: a.event_type ?? '',
      customEventId: a.custom_event_id ?? 0,
      billingEvent: a.billing_event ?? '',
      startTime: a.start_time ?? '',
      endTime: a.end_time ?? '',
      openTimeSchedule: a.open_time_schedule ?? false,
      timeScheduleZoneType: a.time_schedule_zone_type ?? '',
      timeSchedule: a.time_schedule ?? [],
      roasBid: a.roas_bid ?? 0,
      timeZoneType: a.time_zone_type ?? ''
    }
  }

  // ad_data
  if (data.ad_data) {
    const d = data.ad_data
    result.ad_data = {
      nameRule: d.ad_name_rule ?? '',
      callActionTypes:
        typeof d.call_action_types === 'string'
          ? d.call_action_types.split(',').filter(Boolean)
          : (d.call_action_types ?? []),
      pageNo: d.page_id ?? '',
      appDeepLink: d.app_deep_link ?? '',
      useLetterDeepLink: d.use_letter_deep_link ?? false,
      webUrl: d.web_url ?? '',
      displayUrl: d.display_url ?? '',
      trackPixelNo: d.track_pixel_no ?? '',
      trackPixelUrl: d.track_pixel_url ?? '',
      usePageIdentity: d.use_page_identity ?? false,
      isOpenAdvancedMaterial: d.is_open_advanced ?? false,
      multiAds: d.multi_ads ?? false,
      appProductPageId: d.app_product_page_id ?? '',
      openRemarketing: d.open_remarketing ?? false,
      originWebUrl: d.origin_web_url ?? ''
    }
  }

  // audience_data
  if (data.audience_data) {
    const u = data.audience_data
    const pp = u.position_publisher

    /** 有勾选子版位的平台名列表 → publisherPositions */
    const platformsWithPlacements: string[] = []
    if (pp?.facebook?.length) platformsWithPlacements.push('facebook')
    if (pp?.instagram?.length) platformsWithPlacements.push('instagram')
    if (pp?.audience_network?.length) platformsWithPlacements.push('audience_network')
    if (pp?.messenger?.length) platformsWithPlacements.push('messenger')

    result.audience_data = {
      isManualPosition: u.is_manual_position ?? false,
      publisherPositions: platformsWithPlacements,
      facebookPositions: pp?.facebook ?? [],
      instagramPositions: pp?.instagram ?? [],
      audienceNetworkPositions: pp?.audience_network ?? [],
      messengerPositions: pp?.messenger ?? [],
      gender: u.gender ?? 0,
      ageMax: u.age_max ?? 0,
      ageMin: u.age_min ?? 0,
      countryGroups: u.country_groups ?? [],
      excludedCountryGroups: u.excluded_country_groups ?? [],
      countries: [],
      languages: u.languages ?? [],
      locationType: u.location_type ?? '',
      isOpenTargetExtend: false,
      appType: u.app_type ?? '',
      appMinSystemVer: u.app_min_system_ver ?? '',
      appMaxSystemVer: u.app_max_system_ver ?? '',
      appDevices: u.app_devices ?? [],
      appExcludedDevices: u.app_excluded_devices ?? [],
      openWirelessCarrier: u.open_wireless_carrier ?? false,
      audiences: [],
      excludedAd: u.excluded_ad ?? false,
      facebookInventoryPlan: u.facebook_inventory_plan ?? 'NONE',
      networkInventoryPlan: u.network_inventory_plan ?? 'NONE',
      contentExclusionCondition: u.content_exclusion_condition ?? false,
      subdivisionPositionV1: u.include_subdivision_position ?? [],
      positionPublisher: u.position_publisher ?? {},
      areas: u.areas ?? { includes: [], excludes: [] },
      isAdvancedAudience: u.is_advanced_audience ?? false,
      excludeSubdivisionPosition: u.exclude_subdivision_position ?? [],
      isAudienceSuggest: u.is_audience_suggest ?? false,
      ageMinLimit: u.age_min_limit ?? 0
    }
  }

  // account_data
  result.account_data = data.account_data ?? []

  return result
}
