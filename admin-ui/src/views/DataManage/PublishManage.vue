<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ContentWrap } from '@/components/ContentWrap'
import {
  ElButton,
  ElMessage,
  ElSelect,
  ElOption,
  ElDialog,
  ElTable,
  ElTableColumn,
  ElPagination,
  ElInput,
  ElTooltip,
  ElTabs,
  ElTabPane,
  ElTag,
  ElCheckbox,
  ElMessageBox
} from 'element-plus'
import {
  duplicateCampaignInternal,
  duplicateAdSetInternal,
  getCampaignDuplicateLogListApi,
  getAdSetDuplicateLogListApi,
  getDuplicateDetailApi,
  batchDeleteCampaignsApi,
  batchDeleteAdSetsApi
} from '@/api/ads'
import type { BatchDeleteResultItem } from '@/api/ads'
import { getMediaAccountPageApi } from '@/api/ads/media'
import type { MediaAccountDto } from '@/api/ads/media/types'
import { getPageListApi } from '@/api/page'
import type { PageItem } from '@/api/page/types'
import { getCampaignListApi, getCampaignDetailApi } from '@/api/ads/campaign'
import { getSyncScheduleListApi, pushSyncScheduleApi } from '@/api/ads/syncSchedule'
import type { CampaignListItem, CampaignDetailResponse } from '@/api/ads/campaign/types'
import { getAdSetListApi } from '@/api/ads/adset'
import type { AdSetListItem } from '@/api/ads/adset/types'
import { getChannelListApi } from '@/api/ads/channel'
import type { AdsChannelDto } from '@/api/ads/channel/types'
import { getAccountListApi } from '@/api/ads/account'
import type { AdsAccountDto } from '@/api/ads/account/types'
import { PLATFORM_LABELS } from '@/constants/platform'
import { useI18n } from '@/hooks/web/useI18n'
// 模板管理
import { getTemplateDetailApi } from '@/api/ads/template'

const { t } = useI18n()
const route = useRoute()

// ══════════════════════════════════════════
//  模式判断：根据路由 name 区分广告系列/广告组
// ══════════════════════════════════════════

const isAdSetMode = computed(() => (route.path as string).includes('adset'))

// 复用项通用接口
interface TargetItem {
  targetNo: string
  targetName: string
}

// ══════════════════════════════════════════
//  表单字段
// ══════════════════════════════════════════

const authId = ref('')
const accountNo = ref('')
const targetNo = ref('')
const pageId = ref('')
const copyNumber = ref(1)
const loading = ref(false)

// ══════════════════════════════════════════
//  动态标签（根据模式切换）
// ══════════════════════════════════════════

const targetLabel = computed(() =>
  isAdSetMode.value ? t('publish.adSetNo') : t('publish.campaignNo')
)

const targetPlaceholder = computed(() => {
  if (!accountNo.value) return t('publish.selectAccountFirst')
  return isAdSetMode.value ? t('publish.adSetNoPlaceholder') : t('publish.campaignNoPlaceholder')
})

const targetRequired = computed(() =>
  isAdSetMode.value ? t('publish.adSetNoRequired') : t('publish.campaignNoRequired')
)

// ══════════════════════════════════════════
//  步骤指示器
// ══════════════════════════════════════════

const activeStep = computed(() => {
  if (!authId.value) return 0
  if (!copyNumber.value) return 1
  return 2
})

const stepItems = computed(() => [
  { label: '选择来源', active: activeStep.value >= 1, completed: activeStep.value > 1 },
  { label: '配置数量', active: activeStep.value >= 2, completed: activeStep.value > 2 },
  { label: '确认执行', active: false, completed: false }
])

// ══════════════════════════════════════════
//  摘要条依赖：按值查找显示名
// ══════════════════════════════════════════

const accountDisplayName = computed(() => {
  const item = accountOptions.value.find((i) => i.accountNo === accountNo.value)
  return item?.accountName ?? accountNo.value
})

const targetDisplayName = computed(() => {
  const item = targetOptions.value.find((i) => i.targetNo === targetNo.value)
  return item?.targetName ?? targetNo.value
})

const pageDisplayName = computed(() => {
  const item = pageOptions.value.find((i) => i.pageNo === pageId.value)
  return item?.pageName ?? pageId.value
})

// ══════════════════════════════════════════
//  快捷数量预设
// ══════════════════════════════════════════

const copyPresets = [1, 3, 5, 10, 250]

const isPresetActive = (n: number) => copyNumber.value === n

const setCopyNumber = (n: number) => {
  copyNumber.value = n
}

// ══════════════════════════════════════════
//  模板选择器（从模板管理页跳转过来）
// ══════════════════════════════════════════

const selectedTemplateId = ref<number | undefined>(undefined)
const selectedTemplateName = ref('')
const templateLoading = ref(false)

// 从 query params 加载模板
onMounted(async () => {
  const templateId = route.query.templateId
  if (templateId) {
    await loadTemplateForPublish(Number(templateId))
  }
})

async function loadTemplateForPublish(templateId: number) {
  templateLoading.value = true
  try {
    const res = await getTemplateDetailApi(templateId)
    const detail = res.data
    selectedTemplateId.value = templateId
    selectedTemplateName.value = detail.templateName
    // 如果模板有指定平台，可以预选对应授权通道
    // 目前仅记录模板选择状态，后续可扩展自动填充
  } catch (err) {
    ElMessage.error('加载模板失败')
    console.error(err)
  } finally {
    templateLoading.value = false
  }
}

function handleClearTemplate() {
  selectedTemplateId.value = undefined
  selectedTemplateName.value = ''
}

// ══════════════════════════════════════════
//  授权下拉
// ══════════════════════════════════════════

const authOptions = ref<AdsChannelDto[]>([])
const authLoading = ref(false)
const authLoaded = ref(false)

const loadAuthList = async () => {
  if (authLoaded.value) return
  authLoading.value = true
  try {
    const res = await getChannelListApi()
    authOptions.value = res.data?.items || []
    authLoaded.value = true
  } finally {
    authLoading.value = false
  }
}

// ══════════════════════════════════════════
//  广告账户下拉
// ══════════════════════════════════════════

const accountOptions = ref<AdsAccountDto[]>([])
const accountLoading = ref(false)

const loadAccountList = async (channelId: string) => {
  accountOptions.value = []
  accountLoading.value = true
  try {
    const res = await getAccountListApi({
      channelId,
      pageSize: 200,
      accountState: 1
    })
    accountOptions.value = res.data?.items || []
  } finally {
    accountLoading.value = false
  }
}

// 切换授权 → 清空下级并重新加载账户
watch(authId, (val) => {
  accountNo.value = ''
  targetNo.value = ''
  pageId.value = ''
  if (val) {
    loadAccountList(val)
  } else {
    accountOptions.value = []
  }
})

// ══════════════════════════════════════════
//  目标下拉（广告系列 / 广告组，级联账户）
// ══════════════════════════════════════════

const targetOptions = ref<TargetItem[]>([])
const targetLoading = ref(false)

const loadTargetList = async (accNo: string) => {
  targetOptions.value = []
  if (!accNo) return
  targetLoading.value = true
  try {
    if (isAdSetMode.value) {
      // 广告组模式
      const res = await getAdSetListApi({ accountNo: accNo })
      const items: AdSetListItem[] = (res as any)?.data?.items ?? (res as any)?.items ?? []
      targetOptions.value = items.map((i) => ({
        targetNo: i.adSetNo,
        targetName: i.adSetName
      }))
    } else {
      // 广告系列模式
      const res = await getCampaignListApi({ accountNo: accNo, page: 1, pageSize: 999 })
      const items: CampaignListItem[] = (res as any)?.data?.items ?? (res as any)?.items ?? []
      targetOptions.value = items.map((i) => ({
        targetNo: i.campaignNo,
        targetName: i.campaignName
      }))
    }
  } finally {
    targetLoading.value = false
  }
}

watch(accountNo, (val) => {
  targetNo.value = ''
  pageId.value = ''
  if (val) {
    loadTargetList(val)
  } else {
    targetOptions.value = []
  }
})

// ══════════════════════════════════════════
//  主页下拉
// ══════════════════════════════════════════

const pageOptions = ref<MediaAccountDto[]>([])
const pageLoading = ref(false)

const loadPageList = async () => {
  pageOptions.value = []
  pageLoading.value = true
  try {
    const res = await getMediaAccountPageApi({
      accountNo: accountNo.value || undefined
    })
    pageOptions.value = res.data?.items || []
  } finally {
    pageLoading.value = false
  }
}

// 选中授权后仅清空主页
watch(authId, (val) => {
  pageId.value = ''
  if (!val) {
    pageOptions.value = []
  }
})

// 切换账户时才加载主页列表（级联账户）
watch(accountNo, (val) => {
  pageId.value = ''
  if (val && authId.value) {
    loadPageList()
  } else {
    pageOptions.value = []
  }
})

// ══════════════════════════════════════════
//  提交
// ══════════════════════════════════════════

const handleDuplicate = async () => {
  if (!accountNo.value) {
    ElMessage.warning(t('publish.accountNoRequired'))
    return
  }
  if (!targetNo.value) {
    ElMessage.warning(targetRequired.value)
    return
  }
  if (!pageId.value) {
    ElMessage.warning(t('publish.pageIdRequired'))
    return
  }

  loading.value = true
  try {
    if (isAdSetMode.value) {
      await duplicateAdSetInternal({
        channelId: Number(authId.value),
        accountNo: accountNo.value,
        adSetNo: targetNo.value,
        pageNo: pageId.value,
        copyNumber: copyNumber.value
      })
    } else {
      await duplicateCampaignInternal({
        channelId: Number(authId.value),
        accountNo: accountNo.value,
        campaignNo: targetNo.value,
        pageNo: pageId.value,
        copyNumber: copyNumber.value
      })
    }
    ElMessage.success(t('publish.success'))
  } catch {
    ElMessage.error(t('publish.fail'))
  } finally {
    loading.value = false
  }
}

// ══════════════════════════════════════════
//  重置
// ══════════════════════════════════════════

const handleReset = () => {
  authId.value = ''
  accountNo.value = ''
  targetNo.value = ''
  pageId.value = ''
  copyNumber.value = 1
}

// ══════════════════════════════════════════
//  草稿存取（localStorage）
// ══════════════════════════════════════════

const DRAFT_KEY = computed(() =>
  isAdSetMode.value ? 'adset_publish_draft' : 'publish_manage_draft'
)

const handleSaveDraft = () => {
  const draft = {
    authId: authId.value,
    accountNo: accountNo.value,
    targetNo: targetNo.value,
    pageId: pageId.value,
    copyNumber: copyNumber.value
  }
  localStorage.setItem(DRAFT_KEY.value, JSON.stringify(draft))
  ElMessage.success('草稿已保存')
}

const restoreDraft = () => {
  try {
    const raw = localStorage.getItem(DRAFT_KEY.value)
    if (!raw) return
    const draft = JSON.parse(raw)
    authId.value = draft.authId || ''
    accountNo.value = draft.accountNo || ''
    targetNo.value = draft.targetNo || ''
    pageId.value = draft.pageId || ''
    copyNumber.value = draft.copyNumber || 1
  } catch {
    localStorage.removeItem(DRAFT_KEY.value)
  }
}

onMounted(() => {
  restoreDraft()
})

// ══════════════════════════════════════════
//  同步广告系列（仅广告系列模式）
// ══════════════════════════════════════════

const syncCampaignLoading = ref(false)

const handleSyncCampaign = async () => {
  if (!accountNo.value) {
    ElMessage.warning(t('publish.needSelectAccount'))
    return
  }
  syncCampaignLoading.value = true
  try {
    const res = await getSyncScheduleListApi({
      resourceId: accountNo.value,
      resourceType: 'AD_ACCOUNT',
      pageSize: 1000
    })
    const items = res?.data?.items || []

    if (items.length === 0) {
      ElMessage.warning(t('publish.noSyncSchedule'))
      return
    }

    const campaignItems = items.filter((i) => i.jobName === 'SyncAdCampaignJobArgs')

    if (campaignItems.length === 0) {
      ElMessage.warning(t('publish.noSyncSchedule'))
      return
    }

    let successCount = 0
    for (const item of campaignItems) {
      try {
        await pushSyncScheduleApi(item.id)
        successCount++
      } catch {
        // 单条失败继续
      }
    }

    ElMessage.success(
      `${t('publish.campaignSyncSubmitted')}（${successCount}/${campaignItems.length}）`
    )
  } catch (err: any) {
    ElMessage.error(t('publish.syncFailed') + (err?.message || ''))
  } finally {
    syncCampaignLoading.value = false
  }
}

// ══════════════════════════════════════════
//  广告系列详情（仅广告系列模式）
// ══════════════════════════════════════════

const detailLoading = ref(false)
const campaignDetail = ref<CampaignDetailResponse | null>(null)
const detailVisible = ref(false)

const handleFetchDetail = async () => {
  if (!targetNo.value) {
    ElMessage.warning(t('publish.campaignNoRequired'))
    return
  }
  detailLoading.value = true
  try {
    const res = await getCampaignDetailApi({
      accountNo: accountNo.value,
      campaignNo: targetNo.value
    })
    campaignDetail.value = res.data
    detailVisible.value = true
  } catch {
    ElMessage.error(t('publish.fetchDetailFailed'))
    campaignDetail.value = null
    detailVisible.value = false
  } finally {
    detailLoading.value = false
  }
}

// ══════════════════════════════════════════
//  个号主页列表弹窗
// ══════════════════════════════════════════

const pageDialogVisible = ref(false)
const pageTableLoading = ref(false)
const pageTableData = ref<PageItem[]>([])
const pageTableTotal = ref(0)
const pageTablePage = ref(1)
const pageTablePageSize = ref(20)
const pageSearchText = ref('')

const handleViewPages = async () => {
  pageDialogVisible.value = true
  pageTablePage.value = 1
  await fetchPageList()
}

const fetchPageList = async () => {
  pageTableLoading.value = true
  try {
    const res = await getPageListApi({
      page: pageTablePage.value,
      pageSize: pageTablePageSize.value,
      filter: pageSearchText.value || undefined,
      channelId: authId.value ? Number(authId.value) : undefined
    })
    pageTableData.value = (res.data as any)?.items ?? []
    pageTableTotal.value = (res.data as any)?.totalCount ?? 0
  } finally {
    pageTableLoading.value = false
  }
}

const handlePageSearch = () => {
  pageTablePage.value = 1
  fetchPageList()
}

const handlePageSizeChange = (size: number) => {
  pageTablePageSize.value = size
  pageTablePage.value = 1
  fetchPageList()
}

const handlePageCurrentChange = (page: number) => {
  pageTablePage.value = page
  fetchPageList()
}

// ══════════════════════════════════════════
//  顶部 Tab：复制发布 / 发布记录
// ══════════════════════════════════════════

const activeTab = ref<'duplicate' | 'log'>('duplicate')

const handleTabChange = (tab: string | number) => {
  if (tab === 'log') {
    resetLogFilters()
    fetchLogList()
  }
}

watch(
  () => route.path,
  () => {
    // 模式切换（广告系列↔广告组）时重置日志并刷新
    if (activeTab.value === 'log') {
      resetLogFilters()
      fetchLogList()
    }
  }
)

// ══════════════════════════════════════════
//  发布记录 Tab：日志列表
// ══════════════════════════════════════════

interface DuplicateLogItem {
  id: number | string
  logId?: number
  duplicateSource?: string
  state?: string
  adObjectNo?: string
  accountNo?: string
  duplicateAccountNo?: string
  copyNumber?: number
  createTime?: string
  creationTime?: string
  finishTime?: string
  endTime?: string
  scheduleTime?: string
  campaignNo?: string
  adSetNo?: string
  pageNo?: string
  channelId?: number
  channelName?: string
  duplicateContent?: string
  errorMessage?: string
  message?: string
  remark?: string
}

const logLoading = ref(false)
const logList = ref<DuplicateLogItem[]>([])
const logTotal = ref(0)
const logPage = ref(1)
const logPageSize = ref(20)
const logFilterAdObjectNo = ref('')
const logFilterAccountNo = ref('')
const logFilterState = ref<string>('')

const stateOptions = [
  { label: '待处理', value: 'PENDING' },
  { label: '处理中', value: 'IN_PROGRESS' },
  { label: '成功', value: 'SUCCESS' },
  { label: '失败', value: 'FAILED' }
]

const stateTypeMap: Record<string, 'success' | 'warning' | 'info' | 'danger' | 'primary'> = {
  PENDING: 'info',
  IN_PROGRESS: 'warning',
  SUCCESS: 'success',
  FAILED: 'danger'
}

const stateLabelMap: Record<string, string> = {
  PENDING: '待处理',
  IN_PROGRESS: '处理中',
  SUCCESS: '成功',
  FAILED: '失败'
}

const resetLogFilters = () => {
  logFilterAdObjectNo.value = ''
  logFilterAccountNo.value = ''
  logFilterState.value = ''
  logPage.value = 1
}

const fetchLogList = async () => {
  logLoading.value = true
  try {
    const params = {
      Page: logPage.value,
      PageSize: logPageSize.value,
      AdObjectNo: logFilterAdObjectNo.value || undefined,
      AccountNo: logFilterAccountNo.value || undefined,
      State: (logFilterState.value || undefined) as
        | 'PENDING'
        | 'IN_PROGRESS'
        | 'SUCCESS'
        | 'FAILED'
        | undefined
    } as any
    const apiCall = isAdSetMode.value ? getAdSetDuplicateLogListApi : getCampaignDuplicateLogListApi
    const res: any = await apiCall(params)
    const payload = res?.data ?? res
    logList.value = payload?.items ?? payload?.Items ?? []
    logTotal.value = payload?.totalCount ?? payload?.TotalCount ?? 0
  } catch {
    logList.value = []
    logTotal.value = 0
  } finally {
    logLoading.value = false
  }
}

// 详情弹窗
const detailDialogVisible = ref(false)
const logDetailLoading = ref(false)
const detailList = ref<DuplicateLogItem[]>([])
const detailAccountNo = ref('')
const detailPage = ref(1)
const detailPageSize = ref(20)

// 删除状态：Map<id, { status, errorMessage? }>
const deleteStatusMap = ref<Map<number, { status: 'success' | 'error'; errorMessage?: string }>>(
  new Map()
)

const applyDeleteResults = (results: any[]) => {
  const next = new Map(deleteStatusMap.value)
  const itemMap = new Map<string, DuplicateLogItem>()
  for (const d of detailList.value) {
    const key = String(d.campaignNo || d.adObjectNo || '')
    if (key) itemMap.set(key, d)
  }
  for (const r of results) {
    const no = String(r.campaignNo ?? r.adSetNo ?? r.adObjectNo ?? '')
    const item = itemMap.get(no)
    if (item) {
      next.set(Number(item.id), {
        status: r.success ? 'success' : 'error',
        errorMessage: r.errorMessage ?? undefined
      })
    }
  }
  deleteStatusMap.value = next
}

const detailTotal = computed(() => detailList.value.length)

const detailPaginatedList = computed(() => {
  const start = (detailPage.value - 1) * detailPageSize.value
  return detailList.value.slice(start, start + detailPageSize.value)
})

// 批量选择
const selectedDetailIds = ref<Set<number>>(new Set())

const isAllSelected = computed(() => {
  const pageList = detailPaginatedList.value
  return pageList.length > 0 && pageList.every((d) => selectedDetailIds.value.has(Number(d.id)))
})

const isIndeterminate = computed(() => {
  const pageList = detailPaginatedList.value
  const selected = pageList.filter((d) => selectedDetailIds.value.has(Number(d.id))).length
  return selected > 0 && selected < pageList.length
})

const selectedCount = computed(() => selectedDetailIds.value.size)

const toggleSelectDetail = (id: number) => {
  const set = selectedDetailIds.value
  const next = new Set(set)
  if (next.has(id)) next.delete(id)
  else next.add(id)
  selectedDetailIds.value = next
}

const toggleSelectAll = () => {
  const pageList = detailPaginatedList.value
  const set = selectedDetailIds.value
  const next = new Set(set)
  if (isAllSelected.value) {
    pageList.forEach((d) => next.delete(Number(d.id)))
  } else {
    pageList.forEach((d) => next.add(Number(d.id)))
  }
  selectedDetailIds.value = next
}

const handleBatchDelete = async () => {
  const nos = detailList.value
    .filter((d) => selectedDetailIds.value.has(Number(d.id)))
    .map((d) => String(d.campaignNo || d.adObjectNo || ''))
    .filter(Boolean)

  if (nos.length === 0) {
    ElMessage.warning(
      isAdSetMode.value ? '所选明细缺少广告组编号，无法删除' : '所选明细缺少广告系列编号，无法删除'
    )
    return
  }

  try {
    await ElMessageBox.confirm(
      `确定删除选中的 ${nos.length} 条复制记录吗？此操作不可撤销。`,
      t('common.delWarning'),
      {
        confirmButtonText: t('common.delOk'),
        cancelButtonText: t('common.delCancel'),
        type: 'warning'
      }
    )
  } catch {
    return
  }

  try {
    const res: any = isAdSetMode.value
      ? await batchDeleteAdSetsApi({ accountNo: detailAccountNo.value, adSetNos: nos })
      : await batchDeleteCampaignsApi({ accountNo: detailAccountNo.value, campaignNos: nos })
    const results: BatchDeleteResultItem[] = (res.data as any)?.data ?? res.data ?? []
    applyDeleteResults(results)
    selectedDetailIds.value = new Set()
  } catch (err: any) {
    ElMessage.error(err?.message || '删除失败')
  }
}

const openDetailDialog = async (row: DuplicateLogItem) => {
  detailDialogVisible.value = true
  detailPage.value = 1
  selectedDetailIds.value = new Set()
  deleteStatusMap.value = new Map()
  detailAccountNo.value = row.accountNo || ''
  logDetailLoading.value = true
  try {
    const res: any = await getDuplicateDetailApi(Number(row.id))
    const data = res?.data
    // 兼容单条或数组返回
    detailList.value = Array.isArray(data) ? data : [data]
  } catch (err: any) {
    ElMessage.error(t('publish.fetchDuplicateDetailFailed') + (err?.message || ''))
    detailList.value = [row]
  } finally {
    logDetailLoading.value = false
  }
}

const handleDeleteDetail = async (item: DuplicateLogItem) => {
  const no = String(item.campaignNo || item.adObjectNo || '')
  if (!no) {
    ElMessage.warning(
      isAdSetMode.value ? '该明细缺少广告组编号，无法删除' : '该明细缺少广告系列编号，无法删除'
    )
    return
  }
  try {
    await ElMessageBox.confirm(`确定删除该条复制记录吗？此操作不可撤销。`, t('common.delWarning'), {
      confirmButtonText: t('common.delOk'),
      cancelButtonText: t('common.delCancel'),
      type: 'warning'
    })
  } catch {
    return
  }

  try {
    const res: any = isAdSetMode.value
      ? await batchDeleteAdSetsApi({ accountNo: detailAccountNo.value, adSetNos: [no] })
      : await batchDeleteCampaignsApi({ accountNo: detailAccountNo.value, campaignNos: [no] })
    const results: BatchDeleteResultItem[] = (res.data as any)?.data ?? res.data ?? []
    applyDeleteResults(results)
  } catch (err: any) {
    const next = new Map(deleteStatusMap.value)
    next.set(Number(item.id), { status: 'error', errorMessage: err?.message || '请求失败' })
    deleteStatusMap.value = next
    ElMessage.error(err?.message || '删除失败')
  }
}

const closeDetailDialog = () => {
  detailDialogVisible.value = false
  detailList.value = []
  selectedDetailIds.value = new Set()
  deleteStatusMap.value = new Map()
}

const handleLogSearch = () => {
  logPage.value = 1
  fetchLogList()
}

const handleLogReset = () => {
  resetLogFilters()
  fetchLogList()
}

const handleLogSizeChange = (size: number) => {
  logPageSize.value = size
  logPage.value = 1
  fetchLogList()
}

const handleLogCurrentChange = (page: number) => {
  logPage.value = page
  fetchLogList()
}
</script>

<template>
  <ContentWrap>
    <ElTabs v-model="activeTab" class="publish-manage-tabs" @tab-change="handleTabChange">
      <ElTabPane :label="t('publish.tabDuplicate')" name="duplicate">
        <div class="publish-manage">
          <!-- ═══════════ 步骤指示器 ═══════════ -->
          <div class="steps-bar">
            <div
              v-for="(step, idx) in stepItems"
              :key="idx"
              class="step-item"
              :class="{ 'is-active': step.active, 'is-completed': step.completed }"
            >
              <div class="step-dot">
                <span v-if="step.completed" class="step-check">✓</span>
                <span v-else class="step-num">{{ idx + 1 }}</span>
              </div>
              <span class="step-label">{{ step.label }}</span>
              <span v-if="idx < stepItems.length - 1" class="step-line"></span>
            </div>
          </div>

          <!-- ═══════════ 表单卡片 ═══════════ -->
          <div class="form-card">
            <!-- 模板选择区（可选） -->
            <div v-if="selectedTemplateId" class="template-bar">
              <div class="template-bar-left">
                <span class="template-bar-label">当前模板：</span>
                <ElTag type="success" size="small" effect="dark">
                  {{ selectedTemplateName }}
                </ElTag>
                <span class="template-bar-id">ID: {{ selectedTemplateId }}</span>
              </div>
              <ElButton type="danger" size="small" link @click="handleClearTemplate">
                清除模板
              </ElButton>
            </div>

            <!-- 来源配置区 -->
            <div class="form-section">
              <h3 class="section-title">来源配置</h3>

              <div class="source-grid">
                <!-- 授权用户 -->
                <div class="field-group">
                  <label class="field-label">
                    {{ t('publish.authUser') }}
                    <span class="required-mark">*</span>
                  </label>
                  <ElSelect
                    v-model="authId"
                    :loading="authLoading"
                    filterable
                    clearable
                    :placeholder="t('publish.authPlaceholder')"
                    @visible-change="(v: boolean) => v && loadAuthList()"
                  >
                    <ElOption
                      v-for="item in authOptions"
                      :key="item.id"
                      :label="`${item.channelName}【${PLATFORM_LABELS[item.platform] || item.platform}】`"
                      :value="String(item.id)"
                    />
                  </ElSelect>
                </div>

                <!-- 广告账户 -->
                <div class="field-group">
                  <label class="field-label">
                    {{ t('publish.accountNo') }}
                    <span class="required-mark">*</span>
                  </label>
                  <ElSelect
                    v-model="accountNo"
                    :loading="accountLoading"
                    :disabled="!authId"
                    filterable
                    allow-create
                    clearable
                    :placeholder="
                      authId ? t('publish.accountNoPlaceholder') : t('publish.selectAuthFirst')
                    "
                  >
                    <ElOption
                      v-for="item in accountOptions"
                      :key="item.accountNo ?? ''"
                      :label="`${item.accountName}【${item.accountNo}】`"
                      :value="item.accountNo ?? ''"
                    />
                  </ElSelect>
                </div>

                <!-- 目标（广告系列 / 广告组） -->
                <div class="field-group">
                  <label class="field-label">
                    {{ targetLabel }}
                    <span class="required-mark">*</span>
                  </label>
                  <ElSelect
                    v-model="targetNo"
                    :loading="targetLoading"
                    :disabled="!accountNo"
                    filterable
                    allow-create
                    clearable
                    :placeholder="targetPlaceholder"
                  >
                    <ElOption
                      v-for="item in targetOptions"
                      :key="item.targetNo"
                      :label="`${item.targetName}【${item.targetNo}】`"
                      :value="item.targetNo"
                    />
                  </ElSelect>
                </div>

                <!-- 主页 -->
                <div class="field-group">
                  <label class="field-label">
                    {{ t('publish.pageId') }}
                    <span class="required-mark">*</span>
                  </label>
                  <ElSelect
                    v-model="pageId"
                    :loading="pageLoading"
                    :disabled="!accountNo"
                    filterable
                    clearable
                    :placeholder="
                      accountNo ? t('publish.pageIdPlaceholder') : t('publish.selectAccountFirst')
                    "
                  >
                    <ElOption
                      v-for="item in pageOptions"
                      :key="item.pageNo"
                      :label="`${item.pageName}【${item.pageNo}】`"
                      :value="item.pageNo"
                    />
                  </ElSelect>
                </div>
              </div>
            </div>

            <!-- 分割线 -->
            <div class="section-divider"></div>

            <!-- 复制设置区 -->
            <div class="form-section">
              <h3 class="section-title">复制设置</h3>

              <div class="copy-row">
                <div class="field-group copy-number-group">
                  <label class="field-label">
                    {{ t('publish.copyNumber') }}
                    <span class="required-mark">*</span>
                  </label>
                  <div class="counter-wrap">
                    <button
                      class="counter-btn counter-btn--minus"
                      :disabled="copyNumber <= 1"
                      @click="copyNumber > 1 && copyNumber--"
                    >
                      −
                    </button>
                    <input
                      v-model.number="copyNumber"
                      type="number"
                      class="counter-input"
                      :min="1"
                      :max="250"
                    />
                    <button
                      class="counter-btn counter-btn--plus"
                      :disabled="copyNumber >= 250"
                      @click="copyNumber < 250 && copyNumber++"
                    >
                      +
                    </button>
                  </div>
                </div>

                <div class="field-group preset-group">
                  <label class="field-label">快捷选择</label>
                  <div class="preset-pills">
                    <button
                      v-for="n in copyPresets"
                      :key="n"
                      class="preset-pill"
                      :class="{ 'is-active': isPresetActive(n) }"
                      @click="setCopyNumber(n)"
                    >
                      {{ n }} 次
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- 底部操作栏 -->
            <div class="form-footer">
              <div class="footer-left">
                <ElButton @click="handleReset">{{ t('common.reset') }}</ElButton>
                <ElTooltip content="保存当前配置以便下次继续" placement="top">
                  <ElButton plain @click="handleSaveDraft">保存草稿</ElButton>
                </ElTooltip>
                <!-- 广告系列专有按钮：获取详情 + 同步 -->
                <template v-if="!isAdSetMode">
                  <ElButton
                    :loading="detailLoading"
                    :disabled="!targetNo"
                    @click="handleFetchDetail"
                  >
                    {{ t('publish.fetchDetail') }}
                  </ElButton>
                  <ElButton
                    :loading="syncCampaignLoading"
                    :disabled="!accountNo"
                    @click="handleSyncCampaign"
                  >
                    {{ t('publish.syncCampaign') }}
                  </ElButton>
                </template>
                <ElButton type="info" plain :disabled="!authId" @click="handleViewPages">
                  {{ t('publish.viewPages') }}
                </ElButton>
              </div>
              <div class="footer-right">
                <ElButton type="primary" :loading="loading" size="large" @click="handleDuplicate">
                  {{ t('publish.submit') }}
                </ElButton>
              </div>
            </div>
          </div>

          <!-- ═══════════ 摘要提示条 ═══════════ -->
          <div v-if="accountNo || targetNo" class="summary-bar">
            <span class="summary-icon">ℹ</span>
            <span class="summary-text">
              已选择
              <strong>{{
                authOptions.find((i) => String(i.id) === authId)?.channelName || '-'
              }}</strong>
              &gt;
              <strong>{{ accountDisplayName || '-' }}</strong>
              &gt;
              <strong>{{ targetDisplayName || '-' }}</strong>
              &gt;
              <strong>{{ pageDisplayName || '-' }}</strong>
              &nbsp;&nbsp;|&nbsp;&nbsp; 将复制 <strong>{{ copyNumber }}</strong> 次
            </span>
          </div>

          <!-- ═══════════ 广告系列详情（仅广告系列模式） ═══════════ -->
          <div v-if="!isAdSetMode && detailVisible" class="detail-section">
            <template v-if="campaignDetail">
              <div class="detail-card">
                <div class="detail-header">
                  <span class="detail-title">{{ t('publish.campaignDetailTitle') }}</span>
                  <ElButton text size="small" @click="detailVisible = false">{{
                    t('publish.collapse')
                  }}</ElButton>
                </div>
                <div v-if="campaignDetail.message" class="detail-alert">
                  {{ campaignDetail.message }}
                </div>
                <div class="detail-grid">
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.campaignNoLabel') }}</span>
                    <span class="detail-val">{{ campaignDetail.campaignNo }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.campaignNameLabel') }}</span>
                    <span class="detail-val">{{ campaignDetail.campaignName }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.status') }}</span>
                    <span class="detail-val">{{ campaignDetail.status }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.objective') }}</span>
                    <span class="detail-val">{{ campaignDetail.objective }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.dailyBudget') }}</span>
                    <span class="detail-val">{{ campaignDetail.dailyBudget }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-key">{{ t('publish.lifetimeBudget') }}</span>
                    <span class="detail-val">{{ campaignDetail.lifetimeBudget }}</span>
                  </div>
                </div>
              </div>
            </template>
          </div>
        </div>

        <!-- 个号主页列表弹窗 -->
        <ElDialog
          v-model="pageDialogVisible"
          :title="t('publish.viewPages')"
          width="900px"
          top="8vh"
        >
          <div style="display: flex; gap: 12px; margin-bottom: 16px">
            <ElInput
              v-model="pageSearchText"
              :placeholder="t('pageManage.searchPlaceholder')"
              clearable
              style="width: 240px"
              @keyup.enter="handlePageSearch"
              @clear="handlePageSearch"
            />
            <ElButton type="primary" @click="handlePageSearch">{{ t('common.query') }}</ElButton>
          </div>
          <ElTable
            v-loading="pageTableLoading"
            :data="pageTableData"
            border
            stripe
            max-height="55vh"
          >
            <ElTableColumn prop="pageNo" :label="t('pageManage.pageNo')" min-width="160" />
            <ElTableColumn prop="pageName" :label="t('pageManage.pageName')" min-width="180" />
            <ElTableColumn prop="accountNo" :label="t('pageManage.accountNo')" min-width="140" />
            <ElTableColumn prop="category" :label="t('pageManage.category')" width="120" />
            <ElTableColumn prop="lastSyncTime" :label="t('pageManage.lastSyncTime')" width="180" />
            <ElTableColumn prop="creationTime" :label="t('pageManage.creationTime')" width="180" />
          </ElTable>
          <div style="display: flex; justify-content: flex-end; margin-top: 16px">
            <ElPagination
              v-model:current-page="pageTablePage"
              v-model:page-size="pageTablePageSize"
              :total="pageTableTotal"
              :page-sizes="[10, 20, 50, 100]"
              layout="total, sizes, prev, pager, next"
              @size-change="handlePageSizeChange"
              @current-change="handlePageCurrentChange"
            />
          </div>
          <template #footer>
            <ElButton @click="pageDialogVisible = false">{{ t('common.close') }}</ElButton>
          </template>
        </ElDialog>
      </ElTabPane>
      <ElTabPane :label="t('publish.tabLog')" name="log">
        <div class="publish-log">
          <!-- 搜索栏 -->
          <div class="log-search-bar">
            <ElInput
              v-model="logFilterAdObjectNo"
              :placeholder="t('publish.logFilterAdObjectNo')"
              clearable
              style="width: 200px"
              @keyup.enter="handleLogSearch"
              @clear="handleLogSearch"
            />
            <ElInput
              v-model="logFilterAccountNo"
              :placeholder="t('publish.logFilterAccountNo')"
              clearable
              style="width: 200px"
              @keyup.enter="handleLogSearch"
              @clear="handleLogSearch"
            />
            <ElSelect
              v-model="logFilterState"
              :placeholder="t('publish.logFilterState')"
              clearable
              style="width: 140px"
              @change="handleLogSearch"
            >
              <ElOption
                v-for="opt in stateOptions"
                :key="opt.value"
                :label="opt.label"
                :value="opt.value"
              />
            </ElSelect>
            <ElButton type="primary" @click="handleLogSearch">
              {{ t('common.query') }}
            </ElButton>
            <ElButton @click="handleLogReset">{{ t('common.reset') }}</ElButton>
          </div>

          <!-- 表格 -->
          <ElTable
            v-loading="logLoading"
            :data="logList"
            border
            stripe
            :row-key="(row: DuplicateLogItem) => String(row.id)"
            style="width: 100%"
          >
            <ElTableColumn
              prop="adObjectNo"
              :label="t('publish.logAdObjectNo')"
              min-width="160"
              show-overflow-tooltip
            />
            <ElTableColumn
              prop="accountNo"
              :label="t('publish.logAccountNo')"
              min-width="140"
              show-overflow-tooltip
            />
            <ElTableColumn prop="state" :label="t('publish.logState')" width="100">
              <template #default="{ row }">
                <ElTag :type="stateTypeMap[row.state] || 'info'" size="small">
                  {{ stateLabelMap[row.state] || row.state || '-' }}
                </ElTag>
              </template>
            </ElTableColumn>
            <ElTableColumn
              prop="scheduleTime"
              :label="t('publish.logScheduleTime')"
              min-width="180"
            >
              <template #default="{ row }">
                {{ row.scheduleTime ?? row.creationTime ?? '-' }}
              </template>
            </ElTableColumn>
            <ElTableColumn prop="endTime" :label="t('publish.logEndTime')" min-width="180">
              <template #default="{ row }">
                {{ row.endTime ?? row.finishTime ?? '-' }}
              </template>
            </ElTableColumn>
            <ElTableColumn prop="copyNumber" :label="t('publish.logCopyNumber')" width="100">
              <template #default="{ row }">
                {{ row.copyNumber ?? '-' }}
              </template>
            </ElTableColumn>
            <ElTableColumn
              prop="pageNo"
              :label="t('publish.logPageNo')"
              min-width="160"
              show-overflow-tooltip
            />
            <ElTableColumn :label="t('tableDemo.action')" fixed="right" width="120">
              <template #default="{ row }">
                <BaseButton type="primary" size="small" @click="openDetailDialog(row)">
                  {{ t('publish.viewDetail') }}
                </BaseButton>
              </template>
            </ElTableColumn>
            <template #empty>
              <ElEmpty :description="t('publish.logEmpty')" />
            </template>
          </ElTable>

          <div class="log-pagination">
            <ElPagination
              v-model:current-page="logPage"
              v-model:page-size="logPageSize"
              :page-sizes="[10, 20, 50, 100]"
              :total="logTotal"
              layout="total, sizes, prev, pager, next, jumper"
              background
              @size-change="handleLogSizeChange"
              @current-change="handleLogCurrentChange"
            />
          </div>
        </div>
      </ElTabPane>
    </ElTabs>

    <!-- 复制发布详情弹窗 -->
    <ElDialog
      v-model="detailDialogVisible"
      :title="t('publish.detailTitle')"
      width="880px"
      top="10vh"
      @close="closeDetailDialog"
    >
      <div v-loading="logDetailLoading" class="detail-container">
        <!-- 汇总条 -->
        <div v-if="detailTotal > 0" class="detail-summary">
          <div class="detail-summary__left">
            <ElCheckbox
              v-model="isAllSelected"
              :indeterminate="isIndeterminate"
              size="small"
              @change="toggleSelectAll"
            />
            <span>
              共 <strong>{{ detailTotal }}</strong> 条明细
              <template v-if="selectedCount > 0">
                &nbsp;已选 <strong>{{ selectedCount }}</strong> 条
              </template>
            </span>
          </div>
          <ElButton
            v-if="selectedCount > 0"
            type="danger"
            size="small"
            plain
            @click="handleBatchDelete"
          >
            批量删除（{{ selectedCount }}）
          </ElButton>
        </div>

        <div v-for="row in detailPaginatedList" :key="row.id" class="detail-card">
          <!-- 头部：系列编号 + 状态 + 时间 -->
          <div class="detail-card__header">
            <div class="detail-card__left">
              <ElCheckbox
                :model-value="selectedDetailIds.has(Number(row.id))"
                size="small"
                @change="toggleSelectDetail(Number(row.id))"
              />
              <span class="detail-card__campaign">{{
                row.campaignNo || row.adObjectNo || '-'
              }}</span>
              <ElTag v-if="row.state" :type="stateTypeMap[row.state] || 'info'" size="small">
                {{ stateLabelMap[row.state] || row.state }}
              </ElTag>
            </div>
            <div class="detail-card__right">
              <span v-if="row.creationTime" class="detail-card__time">{{ row.creationTime }}</span>
              <!-- 删除状态 -->
              <ElTag
                v-if="deleteStatusMap.has(Number(row.id))"
                :type="
                  deleteStatusMap.get(Number(row.id))!.status === 'success' ? 'success' : 'danger'
                "
                size="small"
              >
                {{
                  deleteStatusMap.get(Number(row.id))!.status === 'success'
                    ? '已删除'
                    : deleteStatusMap.get(Number(row.id))!.errorMessage || '删除失败'
                }}
              </ElTag>
              <ElButton v-else type="danger" size="small" text @click="handleDeleteDetail(row)">
                删除
              </ElButton>
            </div>
          </div>

          <!-- 错误信息（如果有） -->
          <div v-if="row.errorMessage" class="detail-card__error">
            <span class="detail-card__error-label">错误信息</span>
            <span class="detail-card__error-text">{{ row.errorMessage }}</span>
          </div>

          <!-- 复制内容 -->
          <div class="detail-card__content">
            <span class="detail-card__label">复制内容</span>
            <pre class="detail-card__body">{{ row.duplicateContent || '-' }}</pre>
          </div>
        </div>

        <!-- 分页器 -->
        <div v-if="detailTotal > detailPageSize" class="detail-pagination">
          <ElPagination
            v-model:current-page="detailPage"
            v-model:page-size="detailPageSize"
            :total="detailTotal"
            :page-sizes="[20, 50, 100]"
            layout="total, sizes, prev, pager, next"
            small
            background
          />
        </div>

        <div v-if="!logDetailLoading && detailList.length === 0" class="detail-empty">
          {{ t('common.selectText') }}...
        </div>
      </div>
      <template #footer>
        <BaseButton @click="closeDetailDialog">{{ t('common.close') }}</BaseButton>
      </template>
    </ElDialog>
  </ContentWrap>
</template>

<style scoped>
@media (width <= 768px) {
  .source-grid {
    grid-template-columns: 1fr;
  }
}

@media (width <= 640px) {
  .copy-row {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}

.publish-manage {
  font-size: 14px;
  color: var(--el-text-color-primary);
}

.publish-manage-tabs {
  :deep(.el-tabs__nav-wrap)::after {
    background-color: transparent;
  }
}

.publish-log {
  padding-top: 4px;
}

.log-search-bar {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  align-items: center;
  margin-bottom: 16px;
}

.log-pagination {
  display: flex;
  justify-content: flex-end;
  margin-top: 16px;
}

.publish-log-detail {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.publish-log-detail .detail-row {
  display: flex;
  gap: 12px;
  font-size: 14px;
}

.publish-log-detail .detail-row-block {
  flex-direction: column;
}

.publish-log-detail .detail-key {
  min-width: 96px;
  color: var(--el-text-color-secondary);
}

.publish-log-detail .detail-val {
  color: var(--el-text-color-primary);
  word-break: break-all;
}

.publish-log-detail .detail-pre {
  max-height: 240px;
  padding: 10px;
  margin: 0;
  overflow: auto;
  font-size: 12px;
  line-height: 1.6;
  white-space: pre-wrap;
  background: var(--el-fill-color-light);
  border-radius: 4px;
}

.publish-log-detail .detail-pre-error {
  color: var(--el-color-danger);
}

.detail-container {
  min-height: 120px;
}

.detail-summary {
  display: flex;
  padding: 8px 14px;
  margin-bottom: 14px;
  font-size: 13px;
  color: var(--el-text-color-secondary);
  background: var(--el-fill-color-lighter);
  border-radius: 8px;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.detail-summary__left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.detail-summary strong {
  font-weight: 600;
  color: var(--el-color-primary);
}

.detail-pagination {
  display: flex;
  padding-top: 12px;
  margin-top: 16px;
  border-top: 1px solid var(--el-border-color-lighter);
  justify-content: center;
}

.detail-card {
  padding: 16px 20px;
  background: var(--el-fill-color-blank);
  border: 1px solid var(--el-border-color-light);
  border-radius: 10px;
  transition: border-color 0.2s ease;
}

.detail-card:hover {
  border-color: var(--el-color-primary-light-5);
}

.detail-card + .detail-card {
  margin-top: 12px;
}

.detail-card__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  flex-wrap: wrap;
}

.detail-card__left {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}

.detail-card__right {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.detail-card__campaign {
  font-family: 'JetBrains Mono', Consolas, monospace;
  font-size: 14px;
  font-weight: 600;
  letter-spacing: -0.01em;
  color: var(--el-text-color-primary);
}

.detail-card__time {
  font-size: 12px;
  color: var(--el-text-color-placeholder);
  white-space: nowrap;
}

.detail-card__error {
  display: flex;
  padding: 10px 14px;
  margin-top: 12px;
  background: #fef0f0;
  border: 1px solid #fde2e2;
  border-radius: 8px;
  gap: 10px;
  align-items: flex-start;
}

.detail-card__error-label {
  padding: 1px 6px;
  font-size: 12px;
  font-weight: 600;
  color: #f56c6c;
  background: #fff;
  border: 1px solid #fde2e2;
  border-radius: 4px;
  flex-shrink: 0;
}

.detail-card__error-text {
  font-size: 13px;
  line-height: 1.6;
  color: #e05050;
  word-break: break-all;
}

.detail-card__content {
  margin-top: 14px;
}

.detail-card__label {
  display: inline-block;
  margin-bottom: 8px;
  font-size: 11px;
  font-weight: 600;
  letter-spacing: 0.05em;
  color: var(--el-text-color-placeholder);
  text-transform: uppercase;
}

.detail-card__body {
  max-height: 300px;
  padding: 12px 16px;
  margin: 0;
  overflow: auto;
  font-family: 'JetBrains Mono', Consolas, monospace;
  font-size: 13px;
  line-height: 1.7;
  color: var(--el-text-color-regular);
  word-break: break-all;
  white-space: pre-wrap;
  background: var(--el-fill-color-lighter);
  border-radius: 8px;
}

.detail-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
  font-size: 14px;
  color: var(--el-text-color-placeholder);
}

/* ═══════════ 步骤指示器 ═══════════ */
.steps-bar {
  display: flex;
  align-items: center;
  gap: 0;
  padding: 16px 24px;
  margin-bottom: 20px;
  background: var(--el-fill-color-lighter);
  border-radius: 10px;
}

.step-item {
  position: relative;
  display: flex;
  min-width: 0;
  align-items: center;
  gap: 8px;
  flex: 1;
}

.step-dot {
  display: flex;
  width: 28px;
  height: 28px;
  background: var(--el-border-color);
  border: 2px solid var(--el-border-color);
  border-radius: 50%;
  transition: all 0.3s ease;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.step-item.is-active .step-dot {
  background: var(--el-color-primary);
  border-color: var(--el-color-primary);
}

.step-item.is-completed .step-dot {
  background: var(--el-color-primary);
  border-color: var(--el-color-primary);
}

.step-num {
  font-size: 12px;
  font-weight: 600;
  line-height: 1;
  color: var(--el-text-color-secondary);
}

.step-item.is-active .step-num,
.step-item.is-completed .step-num {
  color: #fff;
}

.step-check {
  font-size: 13px;
  font-weight: 700;
  line-height: 1;
  color: #fff;
}

.step-label {
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-secondary);
  white-space: nowrap;
  transition: color 0.3s ease;
}

.step-item.is-active .step-label {
  color: var(--el-text-color-primary);
}

.step-item.is-completed .step-label {
  color: var(--el-text-color-primary);
}

.step-line {
  flex: 1;
  height: 2px;
  margin: 0 12px;
  background: var(--el-border-color);
  border-radius: 1px;
  transition: background 0.3s ease;
}

.step-item.is-completed .step-line {
  background: var(--el-color-primary);
}

/* ═══════════ 模板选择条 ═══════════ */
.template-bar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 28px;
  background: var(--el-color-success-light-9);
  border-bottom: 1px solid var(--el-color-success-light-5);
}

.template-bar-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.template-bar-label {
  font-size: 13px;
  color: var(--el-text-color-secondary);
}

.template-bar-id {
  font-size: 12px;
  color: var(--el-text-color-placeholder);
}

/* ═══════════ 表单卡片 ═══════════ */
.form-card {
  overflow: hidden;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color-light);
  border-radius: 12px;
}

/* ═══════════ 分区 ═══════════ */
.form-section {
  padding: 24px 28px 20px;
}

.section-title {
  margin: 0 0 18px;
  font-size: 15px;
  font-weight: 600;
  letter-spacing: -0.01em;
  color: var(--el-text-color-primary);
}

.section-divider {
  height: 1px;
  margin: 0 28px;
  background: var(--el-border-color-lighter);
}

/* ═══════════ 来源 2×2 网格 ═══════════ */
.source-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.field-group {
  display: flex;
  flex-direction: column;
}

.field-group :deep(.el-select) {
  width: 100%;
}

.field-label {
  display: block;
  margin-bottom: 6px;
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-regular);
}

.required-mark {
  margin-left: 1px;
  color: var(--el-color-danger);
}

/* ═══════════ 复制数量 ═══════════ */
.copy-row {
  display: grid;
  grid-template-columns: 260px 1fr;
  gap: 32px;
  align-items: start;
}

/* 步进器 */
.counter-wrap {
  display: flex;
  align-items: center;
}

.counter-btn {
  display: flex;
  width: 36px;
  height: 36px;
  font-family: inherit;
  font-size: 16px;
  line-height: 1;
  color: var(--el-text-color-secondary);
  cursor: pointer;
  background: var(--el-fill-color-lighter);
  border: 1px solid var(--el-border-color);
  transition: all 0.15s ease;
  align-items: center;
  justify-content: center;
}

.counter-btn--minus {
  border-right: none;
  border-radius: 6px 0 0 6px;
}

.counter-btn--plus {
  border-left: none;
  border-radius: 0 6px 6px 0;
}

.counter-btn:hover:not(:disabled) {
  color: var(--el-text-color-primary);
  background: var(--el-fill-color);
}

.counter-btn:disabled {
  cursor: not-allowed;
  opacity: 0.4;
}

.counter-input {
  width: 64px;
  height: 36px;
  font-family: inherit;
  font-size: 15px;
  font-weight: 600;
  color: var(--el-text-color-primary);
  text-align: center;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color);
  border-right: none;
  border-left: none;
  outline: none;
}

.counter-input:focus {
  border-color: var(--el-color-primary);
  box-shadow: 0 0 0 2px rgba(var(--el-color-primary-rgb, 64, 158, 255), 0.12);
}

.counter-input::-webkit-outer-spin-button,
.counter-input::-webkit-inner-spin-button {
  margin: 0;
  appearance: none;
}

.counter-input[type='number'] {
  appearance: textfield;
}

/* 快捷药丸 */
.preset-group {
  min-width: 0;
}

.preset-pills {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.preset-pill {
  padding: 5px 16px;
  font-family: inherit;
  font-size: 13px;
  color: var(--el-text-color-secondary);
  white-space: nowrap;
  cursor: pointer;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color);
  border-radius: 20px;
  transition: all 0.15s ease;
}

.preset-pill:hover {
  color: var(--el-color-primary);
  border-color: var(--el-color-primary);
}

.preset-pill.is-active {
  font-weight: 600;
  color: var(--el-color-primary);
  background: var(--el-color-primary-light-9);
  border-color: var(--el-color-primary);
}

/* ═══════════ 底部操作栏 ═══════════ */
.form-footer {
  display: flex;
  padding: 18px 28px;
  background: var(--el-fill-color-lighter);
  border-top: 1px solid var(--el-border-color-light);
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
}

.footer-left {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.footer-right {
  flex-shrink: 0;
}

/* ═══════════ 摘要提示条 ═══════════ */
.summary-bar {
  display: flex;
  padding: 12px 16px;
  margin-top: 16px;
  font-size: 13px;
  color: var(--el-text-color-regular);
  background: var(--el-color-info-light-9);
  border: 1px solid var(--el-color-info-light-7);
  border-radius: 8px;
  align-items: center;
  gap: 8px;
}

.summary-icon {
  flex-shrink: 0;
  font-size: 16px;
  color: var(--el-color-info);
}

.summary-text strong {
  font-weight: 600;
  color: var(--el-text-color-primary);
}

/* ═══════════ 详情卡片 ═══════════ */
.detail-section {
  margin-top: 20px;
}

.detail-card {
  padding: 16px;
  background: var(--el-fill-color-blank);
  border: 1px solid var(--el-border-color);
  border-radius: 8px;
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.detail-title {
  font-size: 15px;
  font-weight: 600;
}

.detail-alert {
  padding: 10px 14px;
  margin-bottom: 12px;
  font-size: 13px;
  color: #f56c6c;
  background: #fef0f0;
  border-radius: 6px;
}

.detail-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px 24px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  padding: 6px 0;
  border-bottom: 1px dashed var(--el-border-color-lighter);
}

.detail-key {
  font-size: 13px;
  color: #909399;
}

.detail-val {
  font-size: 13px;
  font-weight: 500;
  text-align: right;
}
</style>
