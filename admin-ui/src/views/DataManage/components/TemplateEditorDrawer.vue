<script setup lang="ts">
import { ref, reactive, computed, watch, nextTick } from 'vue'
import {
  ElDrawer,
  ElForm,
  ElFormItem,
  ElInput,
  ElSelect,
  ElOption,
  ElButton,
  ElInputNumber,
  ElSwitch,
  ElRadioGroup,
  ElRadioButton,
  ElMessage,
  ElTooltip
} from 'element-plus'
import { useI18n } from '@/hooks/web/useI18n'
import { useDictStore } from '@/store/modules/dict'
import { PLATFORM_OPTIONS } from '@/constants/platform'
import type { PlatformType } from '@/api/ads/channel/types'
import type {
  MetaPublishingData,
  AdsPublishTemplateViewModel,
  CampaignData,
  AdSetData,
  AdData,
  AudienceData,
  PublishingType,
  PublishingAdType
} from '@/api/ads/template/types'
import { toSwaggerMetaPublishData } from '@/api/ads/template/types'
import LocationPicker from './LocationPicker.vue'
import type { LocationValue } from './LocationPicker.vue'
import PlacementPicker from './PlacementPicker.vue'
import type { PlacementValue } from './PlacementPicker.vue'
import TagInput from './TagInput.vue'
import { getLanguageListApi } from '@/api/ads/audience'
import type { LanguageItem } from '@/api/ads/audience/types'
import { getPixelListApi } from '@/api/ads/pixel'
import type { PixelDto } from '@/api/ads/pixel/types'

const { t } = useI18n()
const dictStore = useDictStore()

// ══════════════════════════════════════════
//  Props & Emits
// ══════════════════════════════════════════

const props = defineProps<{
  visible: boolean
  templateId?: number
  initialPlatform?: PlatformType
}>()

const emits = defineEmits<{
  (e: 'update:visible', val: boolean): void
  (e: 'on-success'): void
  (e: 'on-add-success'): void
  (e: 'on-edit-success'): void
}>()

// ══════════════════════════════════════════
//  状态
// ══════════════════════════════════════════

const formRef = ref<InstanceType<typeof ElForm>>()
const loading = ref(false)
const saving = ref(false)
const isEdit = ref(false)
const stepActive = ref(0)

const stepList = [
  {
    title: t('templateEditor.stepBasic'),
    icon: '📋',
    desc: '设置模板名称、平台与广告类型'
  },
  {
    title: t('templateEditor.stepCampaign'),
    icon: '🎯',
    desc: '配置广告系列命名、预算与投放策略'
  },
  {
    title: t('templateEditor.stepAdSet'),
    icon: '👥',
    desc: '设置广告组预算、受众定位与版位'
  },
  {
    title: t('templateEditor.stepAd'),
    icon: '📢',
    desc: '配置广告创意、链接与追踪设定'
  }
]

const stepProgress = computed(() => Math.round(((stepActive.value + 1) / stepList.length) * 100))

// ── 语言列表（动态加载） ──────────────────
const languageList = ref<LanguageItem[]>([])
const languageLoading = ref(false)

async function loadLanguages() {
  if (languageList.value.length > 0) return // 已加载则跳过
  languageLoading.value = true
  try {
    const res = await getLanguageListApi({ platform: formData.platform })
    languageList.value = res?.data ?? []
  } catch (err) {
    console.error('加载语言列表失败', err)
  } finally {
    languageLoading.value = false
  }
}

// 进入广告组步骤时加载语言列表
watch(stepActive, (val) => {
  if (val === 2) loadLanguages()
})

// ── 像素列表（从 pixel API 动态加载） ──

const pixelList = ref<PixelDto[]>([])
const pixelLoading = ref(false)

/** 下拉框选项：仅来自 API */
const pixelOptions = computed(() =>
  pixelList.value.map((item) => ({
    label: `${item.pixelNo} - ${item.pixelName}`,
    value: item.id
  }))
)

async function loadPixels() {
  pixelLoading.value = true
  try {
    const res = await getPixelListApi({ page: 0, pageSize: 200 })
    pixelList.value = res?.data?.items ?? []
  } catch (err) {
    console.error('加载像素列表失败', err)
  } finally {
    pixelLoading.value = false
  }
}

/** 根据竞价策略返回对应金额标签 */
/** 实际生效的竞价策略：广告系列开启预算时取系列策略，否则取广告组策略 */
const effectiveBidStrategy = computed(() =>
  campaign.value.is_open_budget ? campaign.value.bid_strategy : adset.value.bid_strategy
)

/** 判断成效目标是否为 VALUE（转化价值） */
function isValueGoal(goal: string | undefined): boolean {
  return (goal ?? '').toUpperCase() === 'VALUE'
}

const bidAmountLabel = computed(() => {
  const isValue = isValueGoal(adset.value.optimization_goal)
  const strategy = (effectiveBidStrategy.value ?? '').toUpperCase()
  if (strategy === 'COST_CAP') {
    return isValue
      ? t('templateEditor.bidAmountValueCostCap')
      : t('templateEditor.bidAmountCostCap')
  }
  return isValue ? t('templateEditor.bidAmountValueBidCap') : t('templateEditor.bidAmountBidCap')
})

/** 是否需要显示出价金额：非 NONE / LOWEST_COST_WITHOUT_CAP 时显示 */
const showBidAmount = computed(() => {
  const s = (effectiveBidStrategy.value ?? '').toUpperCase()
  return s && s !== 'NONE' && s !== 'LOWEST_COST_WITHOUT_CAP'
})

// ── 表单数据 ──────────────────────────────

const formData = reactive<AdsPublishTemplateViewModel>({
  templateId: undefined,
  templateName: '',
  platform: 2,
  publishingAdType: 'PIXEL',
  applicationId: undefined,
  pixelId: undefined,
  productCatalogId: undefined,
  resourceContent: undefined,
  publishingType: 'NONE' as PublishingType,
  maxPublishCount: 0,
  publishAverage: false,
  metaPublishingData: createMetaPublishingData()
})

// 抽屉打开时加载像素列表（PIXEL 类型）
watch(
  () => props.visible,
  (val) => {
    if (val && formData.publishingAdType === 'PIXEL') loadPixels()
  },
  { immediate: true }
)

const metaData = computed(() => formData.metaPublishingData!)
const campaign = computed(() => metaData.value.campaign_data)
const adset = computed(() => metaData.value.adset_data)
const audience = computed(() => metaData.value.audience_data)
const ad = computed(() => metaData.value.ad_data)

/** 当前选中像素的 pixelNo（依赖 pixelList + formData，需在 formData 之后声明） */
const selectedPixelNo = computed(() => {
  if (!formData.pixelId) return ''
  const found = pixelList.value.find((p) => p.id === formData.pixelId)
  return found?.pixelNo ?? ''
})

/** 当基础信息选择像素变更时，自动同步跟踪像素 */
watch(selectedPixelNo, (pixelNo) => {
  if (pixelNo) {
    ad.value.track_pixel_no = pixelNo
  }
})

const campaignHasAdStructure = computed(() =>
  campaign.value.campaign_name_rule?.includes('#AD_STRUCTURE#')
)

// ── 其他 keep-alive 状态 ──────────────────

// LocationPicker v-model 双向绑定
const areasValue = computed<LocationValue>({
  get: () => ({
    includes: (audience.value.areas?.includes ?? []) as any[],
    excludes: (audience.value.areas?.excludes ?? []) as any[]
  }),
  set: (val: LocationValue) => {
    audience.value.areas = val as any
  }
})

// PlacementPicker 独立状态（平台勾选持久化）
const placementPlatformKeys = ref<string[]>(['facebook', 'instagram'])

// PlacementPicker v-model 双向绑定（selected 直接映射 position_publisher，无转换）
const placementValue = computed<PlacementValue>({
  get: () => ({
    selected: (audience.value?.position_publisher ?? {}) as Record<string, string[]>,
    isManual: audience.value?.is_manual_position ?? false,
    deviceType: audience.value?.device_type ?? 'ALL_DEVICE',
    platformKeys: placementPlatformKeys.value
  }),
  set: (val: PlacementValue) => {
    audience.value.is_manual_position = val.isManual
    audience.value.device_type = val.deviceType
    placementPlatformKeys.value = val.platformKeys
    audience.value.position_publisher = val.selected as any
  }
})

// ══════════════════════════════════════════
//  工厂函数（含完整默认值）
// ══════════════════════════════════════════

function createMetaPublishingData(): MetaPublishingData {
  return {
    campaign_data: createCampaignData(),
    adset_data: createAdSetData(),
    ad_data: createAdData(),
    audience_data: createAudienceData(),
    account_data: []
  }
}

function createCampaignData(): CampaignData {
  return {
    campaign_name_rule: '',
    ad_struct_split: '_',
    buying_type: 'AUCTION',
    is_open_special_ad: false,
    special_ad_categories: [],
    country_codes: [],
    open_ios14: false,
    budget_type: 'DAILY_BUDGET',
    budget: 100,
    is_open_budget: true,
    bid_strategy: 'LOWEST_COST_WITHOUT_CAP',
    pacing_type: 'STANDARD',
    smart_promotion_type: 'GUIDED_CREATION',
    conversion_type: 'WEBSITE',
    is_product_catalog: false
  }
}

function createAdSetData(): AdSetData {
  return {
    adset_name_rule: '',
    is_open_spend_limit: false,
    min_spend_limit: undefined,
    spend_limit: undefined,
    budget_type: 'DAILY_BUDGET',
    budget: 50,
    optimization_goal: 'OFFSITE_CONVERSIONS',
    campaign_attribution: undefined,
    bid_strategy: 'LOWEST_COST_WITHOUT_CAP',
    pacing_type: 'STANDARD',
    event_type: 'STANDARD_EVENTS',
    app_event_type: undefined,
    custom_event_id: undefined,
    custom_event_type: undefined,
    bid_amount: 10,
    billing_event: 'IMPRESSIONS',
    attribution_spec: {
      attribution_type: 'DEFAULT',
      click_through: '7',
      engaged_video_view: undefined,
      view_through: '1'
    },
    start_time: '',
    end_time: '',
    time_zone_type: 'START_NOW',
    placements: ['AUTOMATIC_PLACEMENTS'],
    is_taiwan_finserv: false,
    is_unified_beneficiary: true,
    unified_beneficiary: undefined,
    unified_payor: undefined,
    beneficiary: '',
    payor: '',
    singapore_beneficiary: '',
    singapore_payor: '',
    taiwan_beneficiary: '',
    taiwan_payor: '',
    australia_beneficiary: '',
    australia_payor: ''
  }
}

function createAdData(): AdData {
  return {
    ad_name_rule: '',
    call_action_types: '',
    app_deep_link: '',
    app_product_page_id: '',
    web_url: '',
    display_url: '',
    use_page_identity: false,
    use_gateway_type: 'NO_USE',
    open_remarketing: false,
    multi_ads: false,
    track_pixel_url: '',
    track_pixel_no: undefined,
    track_application_no: undefined,
    call_to_action: 'SHOP_NOW',
    page_id: '',
    image_option: 'SINGLE_IMAGE',
    video_option: 'SINGLE_VIDEO',
    carousel_option: 'CAROUSEL',
    is_open_advanced: false
  }
}

function createAudienceData(): AudienceData {
  return {
    audience_ids: [],
    excluded_audience_ids: [],
    is_advanced_audience: true,
    location_type: 'RECENT_HOME',
    include_region_group: undefined,
    areas: { includes: [], excludes: [] },
    age_min_limit: undefined,
    is_audience_suggest: true,
    age_min: 18,
    age_max: 65,
    gender: 0,
    languages: [],
    include_subdivision_position: [],
    exclude_subdivision_position: [],
    product_audience_type: 'NO_INTERACTIVE',
    product_audience: undefined,
    excluded_product_audience: {
      excluded_product_audience_type: 'NO_EXCLUSION_CONDITION',
      day: 14,
      audiences: []
    },
    is_manual_position: false,
    device_type: 'ALL_DEVICE',
    app_type: 'ANDROID',
    app_devices: [],
    app_excluded_devices: [],
    position_publisher: { audience_network: [], facebook: [], instagram: [], messenger: [] },
    excluded_ad: false,
    app_min_system_ver: '',
    app_max_system_ver: '',
    open_wireless_carrier: false,
    facebook_inventory_plan: 'FACEBOOK_RELAXED',
    network_inventory_plan: 'AN_RELAXED',
    content_exclusion_condition: false
  }
}

// ══════════════════════════════════════════
//  下拉选项
// ══════════════════════════════════════════
//  Meta 字典 SortCode 常量
// ══════════════════════════════════════════

const META_BUDGET_TYPE = 'MetaBudgetType'
const META_BID_STRATEGY = 'MetaBidStrategy'
const META_OPTIMIZATION_GOAL = 'MetaOptimizationGoal'
const META_CONVERSION_EVENT = 'MetaPublishEventToActionEvent'
const META_BILLING_EVENT = 'MetaBillingEvent'
const META_PACING_TYPE = 'MetaPacingType'
const META_CALL_TO_TYPE = 'MetaCallToType'
const META_AD_NAME_RULE = 'AdNameRule'
const ADS_PUBLISHING_AD_TYPE = 'AdsPublishingAdType'

// ══════════════════════════════════════════
//  硬编码回退值（字典无数据时使用）
// ══════════════════════════════════════════

const FALLBACK_BUDGET_TYPES = [
  { label: '日预算', value: 'DAILY_BUDGET' },
  { label: '总预算', value: 'LIFETIME_BUDGET' }
]

const FALLBACK_BID_STRATEGIES = [
  { label: '最低费用 (Lowest Cost)', value: 'LOWEST_COST_WITHOUT_CAP' },
  { label: '费用上限 (Cost Cap)', value: 'COST_CAP' },
  { label: '竞价上限 (Bid Cap)', value: 'BID_CAP' },
  { label: '最低费用竞价上限 (Lowest Cost With Bid Cap)', value: 'LOWEST_COST_WITH_BID_CAP' }
]

const FALLBACK_PACING_TYPES = [
  { label: 'Standard', value: 'STANDARD' },
  { label: 'Accelerated', value: 'ACCELERATED' }
]

const FALLBACK_OPTIMIZATION_GOALS = [
  { label: '转化量 (Conversions)', value: 'OFFSITE_CONVERSIONS' },
  { label: '应用安装量 (App Installs)', value: 'APP_INSTALLS' },
  { label: '链接点击量 (Link Clicks)', value: 'LINK_CLICKS' },
  { label: '覆盖人数 (Reach)', value: 'REACH' },
  { label: '展示次数 (Impressions)', value: 'IMPRESSIONS' },
  { label: '价值 (Value)', value: 'VALUE' }
]

const FALLBACK_BILLING_EVENTS = [
  { label: '展示次数 (IMPRESSIONS)', value: 'IMPRESSIONS' },
  { label: '链接点击 (LINK_CLICKS)', value: 'LINK_CLICKS' }
]

const FALLBACK_APP_STANDARD_EVENTS = [
  { label: 'APP_INSTALL', value: 'APP_INSTALL' },
  { label: 'APP_LAUNCH', value: 'APP_LAUNCH' },
  { label: 'APP_PURCHASE', value: 'APP_PURCHASE' }
]

// APP 事件也来自 MetaPublishEventToActionEvent
const appStandardEventOptions = useDictOptions(META_CONVERSION_EVENT, FALLBACK_APP_STANDARD_EVENTS)
/** Pixel 标准转化事件选项（纯字典枚举，不做手写回退） */
const conversionEventOptions = computed(() => dictStore.getOptions(META_CONVERSION_EVENT))
const callToActionOptions = useDictOptions(META_CALL_TO_TYPE, [])

const FALLBACK_AD_NAME_RULES = [
  { label: '系列名+日期', value: '{campaign}_{date}' },
  { label: '系列名+广告组+广告', value: '{campaign}_{adset}_{ad}' },
  { label: '广告组+日期', value: '{adset}_{date}' },
  { label: '广告+日期', value: '{ad}_{date}' },
  { label: '国家+日期', value: '{country}_{date}' },
  { label: '系列名+成效目标+日期', value: '{campaign}_{objective}_{date}' },
  { label: '系列名+国家+日期', value: '{campaign}_{country}_{date}' },
  { label: '素材名称+日期', value: '{creative}_{date}' }
]
const adNameRuleOptions = useDictOptions(META_AD_NAME_RULE, FALLBACK_AD_NAME_RULES)

// ══════════════════════════════════════════
//  选项计算（字典优先，无数据回退硬编码）
// ══════════════════════════════════════════

function useDictOptions(sortCode: string, fallback: { label: string; value: string }[]) {
  return computed(() => {
    const opts = dictStore.getOptions(sortCode)
    return opts.length > 0 ? opts : fallback
  })
}

const platformOptions = PLATFORM_OPTIONS.filter((p) => p.value === 2)

const publishingAdTypeOptions = useDictOptions(ADS_PUBLISHING_AD_TYPE, [])

const budgetTypeOptions = useDictOptions(META_BUDGET_TYPE, FALLBACK_BUDGET_TYPES)
const bidStrategyOptions = computed(() =>
  useDictOptions(META_BID_STRATEGY, FALLBACK_BID_STRATEGIES).value.filter(
    (opt) => opt.value && opt.label !== '未设置'
  )
)
// ── 竞价策略 → 成效目标映射（Meta 官方值，大小写不敏感）
const BID_STRATEGY_VALUE_ONLY = new Set(['LOWEST_COST_WITH_BID_CAP', 'LOWEST_COST_WITH_MIN_ROAS'])
const BID_STRATEGY_NO_REACH_IMPRESSIONS = new Set(['COST_CAP'])

// ── 成效目标 → 转化事件映射（Meta 官方规则）
//   VALUE             → 仅 PURCHASE（价值优化只支持购买事件）
//   OFFSITE_CONVERSIONS → 全部 Pixel 标准事件
//   APP_INSTALLS        → 仅应用安装事件
//   LINK_CLICKS/REACH/IMPRESSIONS → 无限制
const OPTIMIZATION_GOAL_EVENT_MAP: Record<string, string[]> = {
  VALUE: ['PURCHASE'],
  OFFSITE_CONVERSIONS: [],
  APP_INSTALLS: ['APP_INSTALL'],
  LINK_CLICKS: [],
  REACH: [],
  IMPRESSIONS: []
}
/** 广告组竞价策略：LOWEST_COST_WITH_BID_CAP 仅优化目标为 VALUE 时可选 */
const adsetBidStrategyOptions = computed(() =>
  bidStrategyOptions.value.filter(
    (opt) =>
      isValueGoal(adset.value.optimization_goal) ||
      !BID_STRATEGY_VALUE_ONLY.has(opt.value.toUpperCase())
  )
)
const pacingTypeOptions = useDictOptions(META_PACING_TYPE, FALLBACK_PACING_TYPES)
const optimizationGoalOptions = useDictOptions(META_OPTIMIZATION_GOAL, FALLBACK_OPTIMIZATION_GOALS)
/** 广告组成效目标：根据竞价策略过滤可选目标（Meta 官方规则） */
const adsetOptimizationGoalOptions = computed(() => {
  const allOpts = optimizationGoalOptions.value
  const strategy =
    (campaign.value.is_open_budget ? campaign.value.bid_strategy : adset.value.bid_strategy) ?? ''
  const s = strategy.toUpperCase()
  // LOWEST_COST_WITH_BID_CAP / LOWEST_COST_WITH_MIN_ROAS → 仅 VALUE
  if (BID_STRATEGY_VALUE_ONLY.has(s)) {
    return allOpts.filter((opt) => opt.value.toUpperCase() === 'VALUE')
  }
  // COST_CAP → 排除 REACH / IMPRESSIONS
  if (BID_STRATEGY_NO_REACH_IMPRESSIONS.has(s)) {
    return allOpts.filter((opt) => !['REACH', 'IMPRESSIONS'].includes(opt.value.toUpperCase()))
  }
  return allOpts
})
const billingEventOptions = useDictOptions(META_BILLING_EVENT, FALLBACK_BILLING_EVENTS)
/** 转化事件选项：根据成效目标过滤（Meta 官方规则） */
const adsetConversionEventOptions = computed(() => {
  const opts = conversionEventOptions.value
  const goal = (adset.value.optimization_goal ?? '').toUpperCase()
  const allowedEvents = OPTIMIZATION_GOAL_EVENT_MAP[goal]
  // 空数组或未命中 → 显示全部事件
  if (!allowedEvents || allowedEvents.length === 0) return opts
  return opts.filter((o) => allowedEvents.includes(o.value.toUpperCase()))
})

/** 成效目标变更 → 转化事件联动（取过滤后第一个枚举项） */
watch(
  () => adset.value.optimization_goal,
  () => {
    nextTick(() => {
      const available = adsetConversionEventOptions.value
      if (available.length === 0) return
      const current = (adset.value.custom_event_type ?? '').toUpperCase()
      const matched = available.some((o) => o.value.toUpperCase() === current)
      if (!matched) {
        adset.value.custom_event_type = available[0].value
      }
    })
  }
)

/** 特殊广告类别（字典优先） */
const specialAdCategoryOptions = useDictOptions('MetaSpecialAdStatementCategories', [
  { label: 'CREDIT', value: 'CREDIT' },
  { label: 'EMPLOYMENT', value: 'EMPLOYMENT' },
  { label: 'HOUSING', value: 'HOUSING' },
  { label: 'SOCIAL_ISSUES_ELECTIONS_POLITICS', value: 'SOCIAL_ISSUES_ELECTIONS_POLITICS' }
])
/** 销量网站广告（PIXEL）适用的行动号召 */
const pixelCallToActionOptions = computed(() => {
  const pixelCTAs = new Set([
    'SHOP_NOW',
    'LEARN_MORE',
    'SIGN_UP',
    'SUBSCRIBE',
    'DOWNLOAD',
    'CONTACT_US',
    'GET_QUOTE',
    'GET_OFFER',
    'BOOK_TRAVEL',
    'ORDER_NOW',
    'GET_DIRECTIONS',
    'CALL_NOW',
    'APPLY_NOW',
    'BUY_NOW',
    'DONATE_NOW',
    'LIKE_PAGE',
    'SEND_MESSAGE',
    'SEE_MENU',
    'GET_SHOWTIMES',
    'PURCHASE',
    'ADD_TO_CART',
    'NO_BUTTON'
  ])
  return callToActionOptions.value.filter((opt) => pixelCTAs.has(opt.value))
})

// ══════════════════════════════════════════
//  初始化
// ══════════════════════════════════════════

watch(
  () => props.visible,
  async (val) => {
    if (val) {
      stepActive.value = 0
      if (props.templateId) {
        await loadTemplate(props.templateId)
      } else {
        resetForm()
      }
      await nextTick()
      formRef.value?.clearValidate()
    }
  },
  { immediate: true }
)

/** 竞价策略变更 → 成效目标联动 */
watch(
  () => (campaign.value.is_open_budget ? campaign.value.bid_strategy : adset.value.bid_strategy),
  (_val) => {
    const goal = adset.value.optimization_goal ?? ''
    const available = adsetOptimizationGoalOptions.value
    if (!available.some((opt) => opt.value.toUpperCase() === goal.toUpperCase())) {
      adset.value.optimization_goal = available[0]?.value ?? 'OFFSITE_CONVERSIONS'
    }
  }
)
/** 优化目标变更时，检查竞价策略兼容性 */
watch(
  () => adset.value.optimization_goal,
  (val) => {
    const strategy =
      (campaign.value.is_open_budget
        ? campaign.value.bid_strategy
        : adset.value.bid_strategy
      )?.toUpperCase() ?? ''
    if (BID_STRATEGY_VALUE_ONLY.has(strategy) && !isValueGoal(val)) {
      adset.value.bid_strategy =
        adsetBidStrategyOptions.value[0]?.value ?? 'LOWEST_COST_WITHOUT_CAP'
    }
  }
)

// 年龄下限联动：选择下限后自动同步年龄范围最小值
watch(
  () => audience.value.age_min_limit,
  (val) => {
    if (val !== undefined && val !== null) {
      audience.value.age_min = val
    }
  }
)

// 细分定位 JSON 双向绑定
const subdivisionIncludeJson = computed({
  get: () => {
    const arr = audience.value.include_subdivision_position
    if (!arr || arr.length === 0) return ''
    return JSON.stringify(arr, null, 2)
  },
  set: (val: string) => {
    if (!val.trim()) {
      audience.value.include_subdivision_position = []
      return
    }
    try {
      audience.value.include_subdivision_position = JSON.parse(val)
    } catch {
      // 解析失败时不更新，保持旧值
    }
  }
})

async function loadTemplate(id: number) {
  loading.value = true
  try {
    const { getTemplateDetailApi } = await import('@/api/ads/template')
    const res = await getTemplateDetailApi(id)
    const detail = res.data
    isEdit.value = true
    formData.templateId = detail.templateId ?? detail.id
    formData.templateName = detail.templateName
    formData.platform = detail.platform
    formData.publishingAdType = (detail.publishingAdType as PublishingAdType) || 'PIXEL'
    formData.applicationId = detail.appId ? Number(detail.appId) : undefined
    formData.pixelId = undefined
    // 映射现有的发布类型
    formData.publishingType = (detail.publishingType as PublishingType) || 'NONE'
    if (detail.metaPublishingData) {
      formData.metaPublishingData = detail.metaPublishingData
      // 确保 account_data 初始化
      if (!formData.metaPublishingData.account_data) {
        formData.metaPublishingData.account_data = []
      }
    } else {
      formData.metaPublishingData = createMetaPublishingData()
    }
  } catch (err) {
    ElMessage.error('加载模板失败')
    console.error(err)
  } finally {
    loading.value = false
  }
}

function resetForm() {
  isEdit.value = false
  Object.assign(formData, {
    templateId: undefined,
    templateName: '',
    platform: props.initialPlatform ?? 2,
    publishingAdType: 'PIXEL',
    applicationId: undefined,
    pixelId: undefined,
    productCatalogId: undefined,
    resourceContent: undefined,
    publishingType: 'NONE' as PublishingType,
    maxPublishCount: 0,
    publishAverage: false
  })
  formData.metaPublishingData = createMetaPublishingData()
  syncDictDefaults()
}

/** 将依赖字典的字段默认值同步为字典第一个选项 */
function syncDictDefaults() {
  const meta = formData.metaPublishingData
  if (!meta) return
  const { campaign_data: c, adset_data: a, ad_data: d } = meta
  if (budgetTypeOptions.value.length > 0) {
    c.budget_type = budgetTypeOptions.value[0].value
    a.budget_type = budgetTypeOptions.value[0].value
  }
  if (bidStrategyOptions.value.length > 0) {
    c.bid_strategy = bidStrategyOptions.value[0].value
    a.bid_strategy = bidStrategyOptions.value[0].value
  }
  if (pacingTypeOptions.value.length > 0) {
    c.pacing_type = pacingTypeOptions.value[0].value
    a.pacing_type = pacingTypeOptions.value[0].value
  }
  if (optimizationGoalOptions.value.length > 0) {
    a.optimization_goal = optimizationGoalOptions.value[0].value
  }
  // 转化事件默认选中过滤后的第一个枚举项
  const events = adsetConversionEventOptions.value
  if (events.length > 0) {
    a.custom_event_type = events[0].value
  }
  if (billingEventOptions.value.length > 0) {
    a.billing_event = billingEventOptions.value[0].value
  }
  // 命名规则默认"素材名称"标签（新增模板时）
  if (adNameRuleOptions.value.length > 0) {
    const creativeOpt = adNameRuleOptions.value.find((o) => o.label === '素材名称')
    if (creativeOpt) {
      c.campaign_name_rule = creativeOpt.value
      a.adset_name_rule = creativeOpt.value
      d.ad_name_rule = creativeOpt.value
    }
  }
}

/** dict AdNameRule 加载后，新增模板时补设命名规则默认"素材名称"（dict 异步加载场景兜底） */
watch(adNameRuleOptions, (opts) => {
  if (opts.length === 0 || isEdit.value) return
  const creativeOpt = opts.find((o) => o.label === '素材名称')
  if (!creativeOpt) return

  const meta = formData.metaPublishingData
  if (!meta) return
  const { campaign_data: c, adset_data: a, ad_data: d } = meta
  if (!c.campaign_name_rule) c.campaign_name_rule = creativeOpt.value
  if (!a.adset_name_rule) a.adset_name_rule = creativeOpt.value
  if (!d.ad_name_rule) d.ad_name_rule = creativeOpt.value
})

// ══════════════════════════════════════════
//  步骤导航
// ══════════════════════════════════════════

const isFirstStep = computed(() => stepActive.value === 0)
const isLastStep = computed(() => stepActive.value === stepList.length - 1)

async function goNext() {
  try {
    await formRef.value?.validate()
    // Step 2 (受众) — 国家/地区必选
    if (stepActive.value === 2) {
      const includes = audience.value.areas?.includes ?? []
      if (includes.length === 0) {
        ElMessage.warning('请至少选择一个国家/地区')
        return
      }
    }
    if (!isLastStep.value) {
      stepActive.value++
      scrollFormTop()
    }
  } catch {
    /* validation failed */
  }
}

function goPrevious() {
  if (!isFirstStep.value) {
    stepActive.value--
    scrollFormTop()
  }
}

function goToStep(idx: number) {
  if (idx < stepActive.value) {
    stepActive.value = idx
    scrollFormTop()
  }
}

/** 滚动表单到顶部 */
function scrollFormTop() {
  nextTick(() => {
    const el = document.querySelector('.template-editor .editor-form')
    el?.scrollTo({ top: 0, behavior: 'instant' as ScrollBehavior })
  })
}

// ══════════════════════════════════════════
//  提交
// ══════════════════════════════════════════

async function handleSave() {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }
  // 国家/地区必选
  if ((audience.value.areas?.includes ?? []).length === 0) {
    ElMessage.warning('请至少选择一个国家/地区')
    return
  }
  saving.value = true
  try {
    const { addTemplateApi, editTemplateApi } = await import('@/api/ads/template')
    const params = buildSaveParams()
    if (isEdit.value && formData.templateId && formData.templateId > 0) {
      await editTemplateApi(params)
      emits('on-edit-success')
    } else {
      params.templateId = 0
      await addTemplateApi(params)
      emits('on-add-success')
    }
    ElMessage.success(t('common.successfulOperation'))
    emits('on-success')
    handleClose()
  } catch (err: any) {
    ElMessage.error(err?.message || t('common.operationFailed'))
  } finally {
    saving.value = false
  }
}

async function handleSaveAsNew() {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }
  saving.value = true
  try {
    const { addTemplateApi } = await import('@/api/ads/template')
    const params = buildSaveParams()
    params.templateId = 0
    await addTemplateApi(params)
    ElMessage.success(t('common.successfulOperation'))
    emits('on-add-success')
    emits('on-success')
    handleClose()
  } catch (err: any) {
    ElMessage.error(err?.message || t('common.operationFailed'))
  } finally {
    saving.value = false
  }
}

function buildSaveParams(): AdsPublishTemplateViewModel {
  const rawMeta =
    formData.platform === 2 && formData.metaPublishingData
      ? toSwaggerMetaPublishData(formData.metaPublishingData)
      : undefined
  return {
    version: undefined,
    templateId: formData.templateId,
    templateName: formData.templateName,
    platform: formData.platform,
    publishingAdType: formData.publishingAdType,
    applicationId: formData.applicationId,
    pixelId: formData.pixelId,
    productCatalogId: formData.productCatalogId,
    resourceContent: formData.resourceContent,
    metaPublishingData: rawMeta as any
  }
}

function handleClose() {
  emits('update:visible', false)
}

// ══════════════════════════════════════════
//  校验
// ══════════════════════════════════════════

/** 表单校验规则 — 按当前步骤动态切换 */
const activeRules = computed(() => {
  switch (stepActive.value) {
    case 0:
      return {
        templateName: [
          { required: true, message: t('templateEditor.nameRequired'), trigger: 'blur' }
        ],
        platform: [
          { required: true, message: t('templateEditor.platformRequired'), trigger: 'change' }
        ],
        publishingAdType: [
          { required: true, message: t('templateEditor.adTypeRequired'), trigger: 'change' }
        ],
        pixelId: [{ required: true, message: t('templateEditor.pixelRequired'), trigger: 'change' }]
      }
    case 3:
      return {
        'metaPublishingData.ad_data.web_url': [
          { required: true, message: '请输入落地页 URL', trigger: 'blur' }
        ]
      }
    default:
      return {}
  }
})
</script>

<template>
  <ElDrawer
    :model-value="visible"
    size="70%"
    direction="rtl"
    :close-on-click-modal="false"
    destroy-on-close
    @close="handleClose"
  >
    <!-- ═══════════ 自定义头部 ═══════════ -->
    <template #header>
      <div class="drawer-header">
        <div class="drawer-header__icon">
          <span class="drawer-header__emoji">{{ isEdit ? '✏️' : '✨' }}</span>
        </div>
        <div class="drawer-header__info">
          <h3 class="drawer-header__title">
            {{ isEdit ? t('templateEditor.editTitle') : t('templateEditor.addTitle') }}
          </h3>
          <p class="drawer-header__sub">
            {{ isEdit ? '修改模板配置并保存' : '逐步填写模板信息，完成后保存即可' }}
          </p>
        </div>
      </div>
    </template>

    <!-- ═══════════ 主体内容 ═══════════ -->
    <div v-loading="loading" class="template-editor">
      <!-- ──── 步骤导航 ──── -->
      <div class="step-nav">
        <!-- 进度条背景 -->
        <div class="step-nav__progress">
          <div class="step-nav__bar" :style="{ width: stepProgress + '%' }"></div>
        </div>
        <!-- 步骤点 -->
        <div class="step-nav__dots">
          <div
            v-for="(item, idx) in stepList"
            :key="idx"
            class="step-dot"
            :class="{
              'is-active': stepActive === idx,
              'is-done': stepActive > idx,
              'is-clickable': stepActive > idx
            }"
            @click="goToStep(idx)"
          >
            <div class="step-dot__circle">
              <span v-if="stepActive > idx" class="step-dot__check">✓</span>
              <span v-else class="step-dot__icon">{{ item.icon }}</span>
            </div>
            <div class="step-dot__label">
              <span class="step-dot__title">{{ item.title }}</span>
              <span class="step-dot__desc">{{ item.desc }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- ──── 表单区域 ──── -->
      <ElForm
        ref="formRef"
        :model="formData"
        :rules="activeRules"
        label-position="top"
        class="editor-form"
      >
        <!-- ════ 步骤 0: 基础信息 ════ -->
        <div v-show="stepActive === 0" class="step-content">
          <div class="form-card">
            <div class="form-card__section">
              <div class="section-label">
                <span class="section-label__dot"></span>
                基本信息
              </div>
              <ElFormItem :label="t('templateEditor.templateName')" prop="templateName">
                <ElInput
                  v-model="formData.templateName"
                  :placeholder="t('templateEditor.namePlaceholder')"
                  maxlength="60"
                  show-word-limit
                  size="large"
                />
              </ElFormItem>
            </div>

            <div class="form-card__section">
              <div class="section-label">
                <span class="section-label__dot"></span>
                平台与类型
              </div>
              <ElFormItem :label="t('templateEditor.platform')" prop="platform">
                <ElRadioGroup v-model="formData.platform" :disabled="isEdit">
                  <ElRadioButton
                    v-for="opt in platformOptions"
                    :key="opt.value"
                    :value="Number(opt.value)"
                  >
                    {{ opt.label }}
                  </ElRadioButton>
                </ElRadioGroup>
              </ElFormItem>

              <ElFormItem :label="t('templateEditor.adType')" prop="publishingAdType">
                <ElRadioGroup v-model="formData.publishingAdType">
                  <ElRadioButton
                    v-for="opt in publishingAdTypeOptions"
                    :key="opt.value"
                    :value="opt.value"
                  >
                    {{ opt.label }}
                  </ElRadioButton>
                </ElRadioGroup>
              </ElFormItem>

              <ElFormItem
                v-if="formData.publishingAdType === 'PIXEL'"
                :label="t('templateEditor.pixel')"
                prop="pixelId"
              >
                <ElSelect
                  v-model="formData.pixelId"
                  filterable
                  clearable
                  :loading="pixelLoading"
                  :placeholder="t('templateEditor.pixelPlaceholder')"
                >
                  <ElOption
                    v-for="opt in pixelOptions"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </ElSelect>
              </ElFormItem>

              <ElFormItem v-if="formData.publishingAdType === 'PRODUCT_CATALOG'" label="商品目录">
                <ElInputNumber
                  v-model="formData.productCatalogId"
                  :min="0"
                  controls-position="right"
                  placeholder="请输入商品目录 ID"
                  style="width: 100%"
                />
              </ElFormItem>
            </div>
          </div>
        </div>

        <!-- ════ 步骤 1: 广告系列 ════ -->

        <div v-show="stepActive === 1" class="step-content">
          <!-- 上一步摘要 -->
          <div class="step-content__summary">
            <span class="step-summary__label">模板：{{ formData.templateName || '未命名' }}</span>
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label">平台：{{ platformOptions[0]?.label || 'Meta' }}</span>
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label">类型：{{ formData.publishingAdType }}</span>
          </div>
          <!-- 命名规则卡片 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--accent"></span>
              命名规则
            </div>
            <ElFormItem :label="t('templateEditor.campaignName')">
              <TagInput
                v-model="campaign.campaign_name_rule"
                :options="adNameRuleOptions"
                :placeholder="t('templateEditor.adNameRulePlaceholder')"
              />
            </ElFormItem>

            <ElFormItem v-if="campaignHasAdStructure" :label="t('templateEditor.adStructSplit')">
              <div class="field-row">
                <ElInput v-model="campaign.ad_struct_split" maxlength="10" class="input-xs" />
                <ElTooltip :content="t('templateEditor.adStructSplitTip')" placement="top">
                  <span class="tip-icon">?</span>
                </ElTooltip>
              </div>
            </ElFormItem>
          </div>

          <!-- 特殊广告类别卡片 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--warning"></span>
              特殊广告类别
            </div>
            <ElFormItem :label="t('templateEditor.specialAdCategory')">
              <ElSwitch v-model="campaign.is_open_special_ad" />
            </ElFormItem>

            <template v-if="campaign.is_open_special_ad">
              <ElFormItem :label="t('templateEditor.specialAdCategories')">
                <ElSelect
                  v-model="campaign.special_ad_categories"
                  multiple
                  :placeholder="t('templateEditor.specialAdCategoriesPlaceholder')"
                >
                  <ElOption
                    v-for="opt in specialAdCategoryOptions"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </ElSelect>
              </ElFormItem>
              <ElFormItem :label="t('templateEditor.countryCodes')">
                <ElSelect
                  v-model="campaign.country_codes"
                  multiple
                  filterable
                  :placeholder="t('templateEditor.countryCodesPlaceholder')"
                >
                  <ElOption label="US" value="US" />
                  <ElOption label="GB" value="GB" />
                  <ElOption label="CA" value="CA" />
                  <ElOption label="AU" value="AU" />
                </ElSelect>
              </ElFormItem>
            </template>

            <ElFormItem
              v-if="formData.publishingAdType === 'APP'"
              :label="t('templateEditor.ios14')"
            >
              <ElSwitch v-model="campaign.open_ios14" />
              <ElTooltip :content="t('templateEditor.ios14Tip')" placement="top">
                <span class="tip-icon">?</span>
              </ElTooltip>
            </ElFormItem>
          </div>

          <!-- 预算与出价卡片 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--success"></span>
              预算与出价
            </div>
            <ElFormItem :label="t('templateEditor.campaignBudgetSwitch')">
              <ElSwitch v-model="campaign.is_open_budget" />
              <ElTooltip :content="t('templateEditor.campaignBudgetSwitchTip')" placement="top">
                <span class="tip-icon">?</span>
              </ElTooltip>
            </ElFormItem>

            <template v-if="campaign.is_open_budget">
              <ElFormItem :label="t('templateEditor.budgetType')">
                <div class="field-row">
                  <ElSelect v-model="campaign.budget_type" class="select-md">
                    <ElOption
                      v-for="opt in budgetTypeOptions"
                      :key="opt.value"
                      :label="opt.label"
                      :value="opt.value"
                    />
                  </ElSelect>
                  <ElInputNumber
                    v-model="campaign.budget"
                    :min="1"
                    :max="999999"
                    :precision="0"
                    controls-position="right"
                    class="input-number-md"
                  />
                  <span class="field-suffix">USD</span>
                </div>
              </ElFormItem>
              <ElFormItem :label="t('templateEditor.bidStrategy')">
                <ElSelect v-model="campaign.bid_strategy">
                  <ElOption
                    v-for="opt in bidStrategyOptions"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </ElSelect>
              </ElFormItem>
              <ElFormItem :label="t('templateEditor.pacingType')">
                <ElRadioGroup v-model="campaign.pacing_type">
                  <ElRadioButton
                    v-for="opt in pacingTypeOptions"
                    :key="opt.value"
                    :value="opt.value"
                  >
                    {{ opt.label }}
                  </ElRadioButton>
                </ElRadioGroup>
              </ElFormItem>
            </template>
          </div>
        </div>

        <!-- ════ 步骤 2: 广告组 ════ -->

        <div v-show="stepActive === 2" class="step-content">
          <!-- 上一步摘要 -->
          <div class="step-content__summary">
            <span class="step-summary__label"
              >系列名称：{{ campaign.campaign_name_rule || '未设置' }}</span
            >
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label">
              预算：{{
                campaign.is_open_budget
                  ? (campaign.budget_type === 'DAILY_BUDGET' ? '日预算' : '总预算') +
                    ' ' +
                    campaign.budget +
                    ' USD'
                  : '未开启'
              }}
            </span>
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label">竞价：{{ campaign.bid_strategy }}</span>
          </div>
          <!-- 命名规则 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--accent"></span>
              命名规则
            </div>
            <ElFormItem :label="t('templateEditor.adsetName')">
              <TagInput
                v-model="adset.adset_name_rule"
                :options="adNameRuleOptions"
                :placeholder="t('templateEditor.adNameRulePlaceholder')"
              />
            </ElFormItem>
          </div>

          <!-- 预算与出价 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--success"></span>
              预算与出价
            </div>
            <ElFormItem
              v-if="campaign.is_open_budget"
              :label="t('templateEditor.spendLimitSwitch')"
            >
              <ElSwitch v-model="adset.is_open_spend_limit" />
            </ElFormItem>
            <template v-if="campaign.is_open_budget && adset.is_open_spend_limit">
              <div class="field-row">
                <ElFormItem :label="t('templateEditor.minSpendLimit')" class="field-row__item">
                  <ElInputNumber
                    v-model="adset.min_spend_limit"
                    :min="0"
                    :precision="2"
                    controls-position="right"
                    class="input-number-sm"
                  />
                  <span class="field-suffix">USD</span>
                </ElFormItem>
                <ElFormItem :label="t('templateEditor.spendLimit')" class="field-row__item">
                  <ElInputNumber
                    v-model="adset.spend_limit"
                    :min="0"
                    :precision="2"
                    controls-position="right"
                    class="input-number-sm"
                  />
                  <span class="field-suffix">USD</span>
                </ElFormItem>
              </div>
            </template>

            <ElFormItem
              v-if="!campaign.is_open_budget"
              :label="t('templateEditor.adsetBudgetType')"
            >
              <div class="field-row">
                <ElSelect v-model="adset.budget_type" class="select-sm">
                  <ElOption
                    v-for="opt in budgetTypeOptions"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </ElSelect>
                <ElInputNumber
                  v-model="adset.budget"
                  :min="1"
                  :max="999999"
                  :precision="0"
                  controls-position="right"
                  class="input-number-sm"
                />
                <span class="field-suffix">USD</span>
              </div>
            </ElFormItem>
          </div>

          <!-- 优化与转化 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--info"></span>
              优化与转化
            </div>
            <ElFormItem
              :label="t('templateEditor.optimizationGoal')"
              prop="adset.optimization_goal"
            >
              <ElSelect v-model="adset.optimization_goal">
                <ElOption
                  v-for="opt in adsetOptimizationGoalOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
            </ElFormItem>

            <ElFormItem
              v-if="formData.publishingAdType === 'APP' && campaign.open_ios14"
              :label="t('templateEditor.campaignAttribution')"
            >
              <ElSelect
                v-model="adset.campaign_attribution"
                :placeholder="t('templateEditor.campaignAttributionPlaceholder')"
              >
                <ElOption label="SKAN" value="SKAN" />
                <ElOption label="AAP" value="AAP" />
              </ElSelect>
            </ElFormItem>

            <ElFormItem v-if="!campaign.is_open_budget" :label="t('templateEditor.bidStrategy')">
              <ElSelect v-model="adset.bid_strategy">
                <ElOption
                  v-for="opt in adsetBidStrategyOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
            </ElFormItem>

            <ElFormItem v-if="!campaign.is_open_budget" :label="t('templateEditor.pacingType')">
              <ElRadioGroup v-model="adset.pacing_type">
                <ElRadioButton v-for="opt in pacingTypeOptions" :key="opt.value" :value="opt.value">
                  {{ opt.label }}
                </ElRadioButton>
              </ElRadioGroup>
            </ElFormItem>

            <ElFormItem
              :label="
                formData.publishingAdType === 'APP'
                  ? t('templateEditor.appEventType')
                  : t('templateEditor.customEventType')
              "
            >
              <ElSelect
                v-if="formData.publishingAdType === 'APP'"
                v-model="adset.app_event_type"
                class="select-flex"
                :placeholder="t('templateEditor.appEventTypePlaceholder')"
              >
                <ElOption
                  v-for="opt in appStandardEventOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
              <ElSelect
                v-else
                v-model="adset.custom_event_type"
                class="select-flex"
                :placeholder="t('templateEditor.customEventTypePlaceholder')"
                :key="adset.optimization_goal"
              >
                <ElOption
                  v-for="opt in adsetConversionEventOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
            </ElFormItem>

            <ElFormItem v-if="showBidAmount" :label="bidAmountLabel">
              <ElInputNumber
                v-model="adset.bid_amount"
                :min="0.01"
                :max="1000"
                :precision="2"
                controls-position="right"
                class="input-number-sm"
              />
              <span class="field-suffix">USD</span>
            </ElFormItem>

            <ElFormItem v-if="!campaign.is_open_budget" :label="t('templateEditor.billingEvent')">
              <ElSelect v-model="adset.billing_event">
                <ElOption
                  v-for="opt in billingEventOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
            </ElFormItem>
          </div>

          <!-- 地区设置 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--purple"></span>
              {{ t('templateEditor.areaSettings') }}
            </div>
            <ElFormItem :label="t('templateEditor.areas')" class="full-width-item">
              <LocationPicker v-model="areasValue" :platform="formData.platform" />
            </ElFormItem>
          </div>

          <!-- 受众设置 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--warning"></span>
              受众设置
            </div>
            <ElFormItem :label="t('templateEditor.advancedAudience')">
              <ElRadioGroup v-model="audience.is_advanced_audience">
                <ElRadioButton :value="true">
                  {{ t('templateEditor.advancedAudienceOn') }}
                </ElRadioButton>
                <ElRadioButton :value="false">
                  {{ t('templateEditor.advancedAudienceOff') }}
                </ElRadioButton>
              </ElRadioGroup>
            </ElFormItem>

            <!-- 原始受众 -->
            <template v-if="!audience.is_advanced_audience">
              <ElFormItem :label="t('templateEditor.ageRange')">
                <div class="field-row">
                  <ElSelect v-model="audience.age_min" class="select-xs" placeholder="最小">
                    <ElOption
                      v-for="a in 52"
                      :key="a + 13"
                      :label="String(a + 13)"
                      :value="a + 13"
                    />
                  </ElSelect>
                  <span class="field-separator">—</span>
                  <ElSelect v-model="audience.age_max" class="select-xs" placeholder="最大">
                    <ElOption
                      v-for="a in 52"
                      :key="a + 13"
                      :label="String(a + 13)"
                      :value="a + 13"
                    />
                  </ElSelect>
                </div>
              </ElFormItem>

              <ElFormItem :label="t('templateEditor.gender')">
                <ElRadioGroup v-model="audience.gender">
                  <ElRadioButton :value="0">{{ t('templateEditor.genderAll') }}</ElRadioButton>
                  <ElRadioButton :value="1">{{ t('templateEditor.genderMale') }}</ElRadioButton>
                  <ElRadioButton :value="2">{{ t('templateEditor.genderFemale') }}</ElRadioButton>
                </ElRadioGroup>
              </ElFormItem>

              <ElFormItem :label="t('templateEditor.languages')">
                <ElSelect
                  v-model="audience.languages"
                  multiple
                  filterable
                  :placeholder="t('templateEditor.languagesPlaceholder')"
                >
                  <ElOption
                    v-for="item in languageList"
                    :key="item.id"
                    :label="item.name"
                    :value="item.code"
                  />
                </ElSelect>
              </ElFormItem>
            </template>

            <!-- 进阶赋能受众 -->
            <template v-else>
              <ElFormItem>
                <template #label>
                  {{ t('templateEditor.ageMinLimit') }}
                  <ElTooltip :content="t('templateEditor.ageMinLimitTip')" placement="top">
                    <span class="tip-icon">?</span>
                  </ElTooltip>
                </template>
                <ElSelect v-model="audience.age_min_limit" class="select-xs" placeholder="下限">
                  <ElOption v-for="a in 8" :key="a + 17" :label="String(a + 17)" :value="a + 17" />
                </ElSelect>
              </ElFormItem>

              <ElFormItem :label="t('templateEditor.audienceSuggest')">
                <ElSwitch v-model="audience.is_audience_suggest" />
              </ElFormItem>

              <template v-if="audience.is_audience_suggest">
                <ElFormItem :label="t('templateEditor.ageRange')">
                  <div class="field-row">
                    <ElSelect
                      v-model="audience.age_min"
                      class="select-xs"
                      placeholder="最小"
                      :disabled="
                        audience.age_min_limit !== undefined &&
                        (audience.age_min ?? 0) < (audience.age_min_limit ?? 0)
                      "
                    >
                      <ElOption
                        v-for="a in 52"
                        :key="a + 13"
                        :label="String(a + 13)"
                        :value="a + 13"
                      />
                    </ElSelect>
                    <span class="field-separator">—</span>
                    <ElSelect v-model="audience.age_max" class="select-xs" placeholder="最大">
                      <ElOption
                        v-for="a in 52"
                        :key="a + 13"
                        :label="String(a + 13)"
                        :value="a + 13"
                      />
                    </ElSelect>
                  </div>
                </ElFormItem>

                <ElFormItem :label="t('templateEditor.gender')">
                  <ElRadioGroup v-model="audience.gender">
                    <ElRadioButton :value="0">{{ t('templateEditor.genderAll') }}</ElRadioButton>
                    <ElRadioButton :value="1">{{ t('templateEditor.genderMale') }}</ElRadioButton>
                    <ElRadioButton :value="2">{{ t('templateEditor.genderFemale') }}</ElRadioButton>
                  </ElRadioGroup>
                </ElFormItem>

                <ElFormItem :label="t('templateEditor.languages')">
                  <ElSelect v-model="audience.languages" multiple filterable>
                    <ElOption
                      v-for="item in languageList"
                      :key="item.id"
                      :label="item.name"
                      :value="item.code"
                    />
                  </ElSelect>
                </ElFormItem>
              </template>

              <ElFormItem :label="t('templateEditor.subdivisionInclude')">
                <ElInput
                  v-model="subdivisionIncludeJson"
                  :placeholder="t('templateEditor.subdivisionPositionPlaceholder')"
                />
              </ElFormItem>
            </template>
          </div>

          <!-- 版位设置 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--accent"></span>
              版位设置
            </div>
            <ElFormItem :label="t('templateEditor.placement')" class="full-width-item">
              <PlacementPicker v-model="placementValue" />
            </ElFormItem>

            <template v-if="audience.is_manual_position">
              <ElFormItem :label="t('templateEditor.excludedAd')">
                <ElSwitch v-model="audience.excluded_ad" />
              </ElFormItem>

              <!-- 最低/最高版本: 已隐藏 -->
              <!-- <div class="field-row">
                <ElFormItem :label="t('templateEditor.minSystemVer')" class="field-row__item">
                  <ElSelect v-model="audience.app_min_system_ver" class="select-sm" placeholder="最低版本">
                    <ElOption label="不限" value="" />
                    <ElOption
                      v-for="v in ['4.0','4.1','4.2','4.3','4.4','5.0','5.1','6.0','7.0','8.0','9.0','10.0','11.0','12.0','13.0','14.0']"
                      :key="v"
                      :label="v"
                      :value="v"
                    />
                  </ElSelect>
                </ElFormItem>
                <ElFormItem :label="t('templateEditor.maxSystemVer')" class="field-row__item">
                  <ElSelect v-model="audience.app_max_system_ver" class="select-sm" placeholder="最高版本">
                    <ElOption label="不限" value="" />
                    <ElOption
                      v-for="v in ['4.0','4.1','4.2','4.3','4.4','5.0','5.1','6.0','7.0','8.0','9.0','10.0','11.0','12.0','13.0','14.0']"
                      :key="v"
                      :label="v"
                      :value="v"
                    />
                  </ElSelect>
                </ElFormItem>
              </div> -->

              <ElFormItem :label="t('templateEditor.wirelessCarrier')">
                <ElSwitch v-model="audience.open_wireless_carrier" />
              </ElFormItem>

              <!-- 库存筛选方案: 已隐藏 -->
              <!-- <ElFormItem :label="t('templateEditor.fbInventoryPlan')">
                <ElSelect v-model="audience.facebook_inventory_plan">
                  <ElOption label="放宽 (Relaxed)" value="FACEBOOK_RELAXED" />
                  <ElOption label="标准 (Standard)" value="FACEBOOK_STANDARD" />
                  <ElOption label="严格 (Limited)" value="FACEBOOK_LIMITED" />
                </ElSelect>
              </ElFormItem>

              <ElFormItem :label="t('templateEditor.anInventoryPlan')">
                <ElSelect v-model="audience.network_inventory_plan">
                  <ElOption label="放宽 (Relaxed)" value="AN_RELAXED" />
                  <ElOption label="标准 (Standard)" value="AN_STANDARD" />
                  <ElOption label="严格 (Limited)" value="AN_LIMITED" />
                </ElSelect>
              </ElFormItem> -->

              <ElFormItem :label="t('templateEditor.contentExclusion')">
                <ElSwitch v-model="audience.content_exclusion_condition" />
              </ElFormItem>
            </template>
          </div>
        </div>

        <!-- ════ 步骤 3: 广告 ════ -->

        <div v-show="stepActive === 3" class="step-content">
          <!-- 上一步摘要 -->
          <div class="step-content__summary">
            <span class="step-summary__label">组名称：{{ adset.adset_name_rule || '未设置' }}</span>
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label">优化目标：{{ adset.optimization_goal }}</span>
            <span class="step-summary__sep">|</span>
            <span class="step-summary__label"
              >受众：{{ audience.is_advanced_audience ? '进阶受众' : '基础受众' }}</span
            >
          </div>
          <!-- 广告创意 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--accent"></span>
              广告创意
            </div>
            <ElFormItem :label="t('templateEditor.adName')">
              <TagInput
                v-model="ad.ad_name_rule"
                :options="adNameRuleOptions"
                :placeholder="t('templateEditor.adNameRulePlaceholder')"
              />
            </ElFormItem>

            <ElFormItem :label="t('templateEditor.callActionTypes')">
              <ElSelect
                v-model="ad.call_action_types"
                :placeholder="t('templateEditor.callActionTypesPlaceholder')"
              >
                <ElOption
                  v-for="opt in pixelCallToActionOptions"
                  :key="opt.value"
                  :label="opt.label"
                  :value="opt.value"
                />
              </ElSelect>
            </ElFormItem>
          </div>

          <!-- 链接与跳转 -->
          <div class="form-card">
            <div class="section-label">
              <span class="section-label__dot section-label__dot--info"></span>
              链接与跳转
            </div>
            <ElFormItem
              :label="t('templateEditor.webUrl')"
              prop="metaPublishingData.ad_data.web_url"
            >
              <ElInput v-model="ad.web_url" :placeholder="t('templateEditor.webUrlPlaceholder')" />
            </ElFormItem>

            <ElFormItem :label="t('templateEditor.displayUrl')">
              <ElInput
                v-model="ad.display_url"
                :placeholder="t('templateEditor.displayUrlPlaceholder')"
              />
            </ElFormItem>

            <ElFormItem :label="t('templateEditor.usePageIdentity')">
              <ElSwitch v-model="ad.use_page_identity" />
              <ElTooltip :content="t('templateEditor.usePageIdentityTip')" placement="top">
                <span class="tip-icon">?</span>
              </ElTooltip>
            </ElFormItem>

            <ElFormItem :label="t('templateEditor.multiAds')">
              <ElSwitch v-model="ad.multi_ads" />
              <ElTooltip :content="t('templateEditor.multiAdsTip')" placement="top">
                <span class="tip-icon">?</span>
              </ElTooltip>
            </ElFormItem>
          </div>

          <!-- 追踪设定 -->
          <div class="form-card form-card--collapsed">
            <details class="form-details">
              <summary class="form-details__summary">
                <span class="section-label section-label--inline">
                  <span class="section-label__dot section-label__dot--purple"></span>
                  {{ t('templateEditor.trackSettings') }}
                </span>
                <svg
                  class="form-details__arrow"
                  width="14"
                  height="14"
                  viewBox="0 0 24 24"
                  fill="none"
                >
                  <path
                    d="M6 9l6 6 6-6"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                  />
                </svg>
              </summary>
              <div class="form-details__body">
                <ElFormItem :label="t('templateEditor.trackPixelUrl')">
                  <ElInput
                    v-model="ad.track_pixel_url"
                    :placeholder="t('templateEditor.trackPixelUrlPlaceholder')"
                  />
                </ElFormItem>
                <ElFormItem :label="t('templateEditor.trackPixelNo')">
                  <ElInput
                    :model-value="ad.track_pixel_no || '请先在基础信息中选择像素'"
                    disabled
                  />
                </ElFormItem>
              </div>
            </details>
          </div>
        </div>
      </ElForm>
    </div>

    <!-- ═══════════ 底部操作栏 ═══════════ -->
    <template #footer>
      <div class="drawer-footer">
        <div class="drawer-footer__left">
          <ElButton v-if="isEdit" type="warning" plain :loading="saving" @click="handleSaveAsNew">
            <span class="btn-icon">📋</span>
            {{ t('templateEditor.saveAsNew') }}
          </ElButton>
        </div>
        <div class="drawer-footer__right">
          <ElButton @click="handleClose" class="btn-cancel">
            {{ t('common.cancel') }}
          </ElButton>
          <ElButton v-if="!isFirstStep" @click="goPrevious">
            {{ t('templateEditor.previous') }}
          </ElButton>
          <ElButton v-if="!isLastStep" type="primary" @click="goNext">
            {{ t('templateEditor.next') }}
          </ElButton>
          <ElButton
            v-if="isLastStep"
            type="primary"
            :loading="saving"
            @click="handleSave"
            class="btn-save"
          >
            <span v-if="!saving" class="btn-icon">💾</span>
            {{ t('templateEditor.save') }}
          </ElButton>
        </div>
      </div>
    </template>
  </ElDrawer>
</template>

<style scoped>
@keyframes stepEnter {
  from {
    opacity: 0;
    transform: translateY(12px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.template-editor {
  display: flex;
  height: 100%;
  flex-direction: column;
}

/* ══════════════════════════════════════════
   自定义头部
   ══════════════════════════════════════════ */

.drawer-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 20px;
}

.drawer-header__icon {
  display: flex;
  width: 36px;
  height: 36px;
  background: linear-gradient(
    135deg,
    var(--el-color-primary-light-8),
    var(--el-color-primary-light-6)
  );
  border-radius: 10px;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.drawer-header__emoji {
  font-size: 18px;
}

.drawer-header__title {
  margin: 0;
  font-size: 16px;
  font-weight: 700;
  color: var(--el-text-color-primary);
}

.drawer-header__sub {
  display: none;
}

/* ══════════════════════════════════════════
   步骤导航
   ══════════════════════════════════════════ */

.step-nav {
  padding: 8px 24px 0;
  flex-shrink: 0;
}

/* ── 进度条 ── */
.step-nav__progress {
  height: 2px;
  margin-bottom: 10px;
  overflow: hidden;
  background: var(--el-fill-color-light);
  border-radius: 99px;
}

.step-nav__bar {
  height: 100%;
  background: linear-gradient(90deg, var(--el-color-primary), var(--el-color-primary-light-3));
  border-radius: 99px;
  transition: width 0.5s cubic-bezier(0.4, 0, 0.2, 1);
}

/* ── 步骤点 ── */
.step-nav__dots {
  display: flex;
  justify-content: space-between;
}

.step-dot {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  flex: 1;
  min-width: 0;
  cursor: default;
  transition: opacity 0.3s;
}

.step-dot.is-clickable {
  cursor: pointer;
}

.step-dot.is-clickable:hover .step-dot__circle {
  transform: scale(1.08);
}

.step-dot:not(.is-active, .is-done) {
  opacity: 0.45;
}

.step-dot__circle {
  position: relative;
  z-index: 2;
  display: flex;
  width: 26px;
  height: 26px;
  background: var(--el-fill-color-light);
  border: 2px solid var(--el-border-color);
  border-radius: 50%;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.step-dot.is-active .step-dot__circle {
  background: var(--el-color-primary-light-9);
  border-color: var(--el-color-primary);
  transform: scale(1.1);
  box-shadow: 0 0 0 3px rgb(59 130 246 / 8%);
}

.step-dot.is-done .step-dot__circle {
  background: var(--el-color-success-light-8);
  border-color: var(--el-color-success);
}

.step-dot__icon {
  font-size: 12px;
  line-height: 1;
}

.step-dot__check {
  font-size: 11px;
  font-weight: 700;
  color: var(--el-color-success);
}

.step-dot__label {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1px;
  text-align: center;
}

.step-dot__title {
  font-size: 11px;
  font-weight: 600;
  color: var(--el-text-color-primary);
  white-space: nowrap;
}

.step-dot__desc {
  display: none;
}

/* ── 步骤流转指示条 ── */
.step-flow {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  padding: 6px 24px 0;
  font-size: 12px;
  color: var(--el-text-color-placeholder);
  flex-shrink: 0;
}

.step-flow__item {
  padding: 2px 8px;
  background: var(--el-fill-color-light);
  border-radius: 10px;
  transition: all 0.25s;
}

.step-flow__item.is-active {
  font-weight: 600;
  color: var(--el-color-primary);
  background: var(--el-color-primary-light-9);
}

.step-flow__arrow {
  color: var(--el-border-color);
}

/* ── 步骤摘要卡片 ── */
.step-content__summary {
  display: flex;
  padding: 6px 12px;
  font-size: 12px;
  background: var(--el-color-primary-light-9);
  border: 1px solid var(--el-color-primary-light-7);
  border-radius: 6px;
  align-items: center;
  gap: 6px;
  flex-wrap: wrap;
}

.step-summary__label {
  color: var(--el-text-color-regular);
}

.step-summary__sep {
  color: var(--el-border-color-darker);
}

/* ── 步骤过渡动画 ── */
.step-fade-enter-active,
.step-fade-leave-active {
  transition:
    opacity 0.25s ease,
    transform 0.25s ease;
}

.step-fade-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.step-fade-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}

/* ══════════════════════════════════════════
   表单区域
   ══════════════════════════════════════════ */

.editor-form {
  padding: 16px 28px 32px;
  overflow-y: auto;
  flex: 1;
}

.step-content {
  display: flex;
  width: 100%;
  animation: stepEnter 0.35s cubic-bezier(0.4, 0, 0.2, 1);
  flex-direction: column;
  gap: 16px;
}

/* ══════════════════════════════════════════
   表单卡片
   ══════════════════════════════════════════ */

.form-card {
  padding: 20px 24px;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 12px;
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

.form-card:hover {
  border-color: var(--el-border-color-light);
  box-shadow: 0 2px 8px rgb(0 0 0 / 4%);
}

.form-card__section {
  padding-bottom: 16px;
}

.form-card__section + .form-card__section {
  padding-top: 16px;
  border-top: 1px solid var(--el-border-color-extra-light);
}

/* ── 折叠式卡片 ── */
.form-card--collapsed {
  padding: 0;
}

.form-details__summary {
  display: flex;
  padding: 14px 24px;
  cursor: pointer;
  transition: background 0.2s;
  user-select: none;
  align-items: center;
  justify-content: space-between;
}

.form-details__summary:hover {
  background: var(--el-fill-color-lighter);
}

.form-details__arrow {
  color: var(--el-text-color-secondary);
  transition: transform 0.3s;
}

.form-details[open] .form-details__arrow {
  transform: rotate(180deg);
}

.form-details__body {
  padding: 0 24px 20px;
}

/* ══════════════════════════════════════════
   区块标签
   ══════════════════════════════════════════ */

.section-label {
  display: flex;
  margin-bottom: 16px;
  font-size: 14px;
  font-weight: 600;
  color: var(--el-text-color-primary);
  align-items: center;
  gap: 8px;
}

.section-label--inline {
  margin-bottom: 0;
}

.section-label__dot {
  display: inline-block;
  width: 8px;
  height: 8px;
  background: var(--el-color-primary);
  border-radius: 50%;
  flex-shrink: 0;
}

.section-label__dot--accent {
  background: var(--el-color-warning);
}

.section-label__dot--success {
  background: var(--el-color-success);
}

.section-label__dot--info {
  background: var(--el-color-info);
}

.section-label__dot--warning {
  background: var(--el-color-danger);
}

.section-label__dot--purple {
  background: #a855f7;
}

/* ══════════════════════════════════════════
   表单行 & 控件
   ══════════════════════════════════════════ */

.field-row {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.field-row__item {
  flex: 1;
  min-width: 0;
}

.field-separator {
  font-weight: 500;
  color: var(--el-text-color-placeholder);
}

.tip-icon {
  display: inline-flex;
  width: 16px;
  height: 16px;
  font-size: 11px;
  font-weight: 700;
  color: var(--el-text-color-placeholder);
  cursor: help;
  background: var(--el-fill-color);
  border: 1px solid var(--el-border-color);
  border-radius: 50%;
  transition: all 0.2s;
  user-select: none;
  align-items: center;
  justify-content: center;
}

.tip-icon:hover {
  color: var(--el-color-white);
  background: var(--el-color-primary);
  border-color: var(--el-color-primary);
}

.field-suffix {
  margin-left: 8px;
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-secondary);
}

.select-xs {
  width: 120px;
}

.select-sm {
  width: 180px;
}

.select-md {
  width: 220px;
}

.select-flex {
  flex: 1;
  min-width: 0;
}

.input-xs {
  width: 120px;
}

.input-number-sm {
  width: 180px;
}

.input-number-md {
  width: 220px;
}

.full-width-item {
  width: 100% !important;
}

.full-width-item :deep(.el-form-item__content) {
  width: 100%;
}

.btn-icon {
  font-size: 14px;
  line-height: 1;
}

/* ══════════════════════════════════════════
   底部栏
   ══════════════════════════════════════════ */

.drawer-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.drawer-footer__right {
  display: flex;
  gap: 10px;
}

.btn-cancel {
  color: var(--el-text-color-regular);
  background: var(--el-fill-color-light);
  border-color: transparent;
}

.btn-cancel:hover {
  color: var(--el-text-color-primary);
  background: var(--el-fill-color);
}

.btn-save {
  font-weight: 600;
}

/* ══════════════════════════════════════════
   Element Plus 深度覆写
   ══════════════════════════════════════════ */

/* ── Drawer 容器 ── */
:deep(.el-drawer__header) {
  padding: 0;
  margin-bottom: 0;
}

:deep(.el-drawer__body) {
  padding: 0;
}

:deep(.el-drawer__footer) {
  padding: 14px 24px;
  background: var(--el-bg-color);
  border-top: 1px solid var(--el-border-color-extra-light);
}

/* ── 表单 ── */
:deep(.el-form-item) {
  margin-bottom: 18px;
}

/* Switch 类表单项：标签与控件同行 */
:deep(.el-form-item:has(.el-switch)) {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0;
}

:deep(.el-form-item:has(.el-switch) .el-form-item__label) {
  width: auto;
  margin-right: 12px;
  margin-bottom: 0;
  line-height: 32px;
  flex-shrink: 0;
}

:deep(.el-form-item:has(.el-switch) .el-form-item__content) {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  margin-left: 0 !important;
}

:deep(.el-form-item__label) {
  margin-bottom: 6px;
  font-size: 13px;
  font-weight: 600;
  color: var(--el-text-color-regular);
}

/* ── Switch 美化 ── */
:deep(.el-switch) {
  --el-switch-on-color: var(--el-color-primary);
  --el-switch-off-color: var(--el-border-color);
}

/* ── RadioButton 美化 ── */
:deep(.el-radio-group) {
  --el-radio-button-size: 32px;
}

:deep(.el-radio-button__inner) {
  min-width: 64px;
  padding: 6px 16px;
  font-size: 13px;
  font-weight: 500;
  line-height: 1;
}

:deep(.el-radio-button:first-child .el-radio-button__inner) {
  border-radius: 8px 0 0 8px;
}

:deep(.el-radio-button:last-child .el-radio-button__inner) {
  border-radius: 0 8px 8px 0;
}

:deep(.el-radio-button:first-child:last-child .el-radio-button__inner) {
  border-radius: 8px;
}

/* ── Input / Select 美化 ── */
:deep(.el-input__wrapper) {
  border-radius: 8px;
  box-shadow: 0 0 0 1px var(--el-border-color) inset;
  transition:
    box-shadow 0.2s,
    border-color 0.2s;
}

:deep(.el-input__wrapper:hover) {
  box-shadow: 0 0 0 1px var(--el-border-color-darker) inset;
}

:deep(.el-input.is-focus .el-input__wrapper),
:deep(.el-select .el-input.is-focus .el-input__wrapper) {
  box-shadow:
    0 0 0 1px var(--el-color-primary) inset,
    0 0 0 3px rgb(59 130 246 / 8%);
}

/* ── InputNumber ── */
:deep(.el-input-number .el-input__wrapper) {
  padding-right: 4px;
  border-radius: 8px;
}

/* ── 滚动条 ── */
.editor-form::-webkit-scrollbar {
  width: 6px;
}

.editor-form::-webkit-scrollbar-track {
  background: transparent;
}

.editor-form::-webkit-scrollbar-thumb {
  background: var(--el-border-color-darker);
  border-radius: 99px;
}

.editor-form::-webkit-scrollbar-thumb:hover {
  background: var(--el-text-color-placeholder);
}

/* ══════════════════════════════════════════
   CSS 变量 & 全局
   ══════════════════════════════════════════ */
</style>
