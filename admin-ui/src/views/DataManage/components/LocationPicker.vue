<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElInput, ElButton, ElCheckbox, ElTag, ElTabs, ElTabPane, ElDialog } from 'element-plus'
import { useI18n } from '@/hooks/web/useI18n'
import { getCountryListApi } from '@/api/ads/audience'
import type { CountryItem } from '@/api/ads/audience/types'
import type { PlatformType } from '@/api/ads/channel/types'

const { t } = useI18n()

// ══════════════════════════════════════════
//  Types
// ══════════════════════════════════════════

export interface RegionItem {
  key: string
  name: string
  type: 'country_group' | 'country' | 'region' | 'city'
  country_code?: string
  country_codes?: string[]
  region_id?: number
}

export interface LocationValue {
  includes: RegionItem[]
  excludes: RegionItem[]
}

// ══════════════════════════════════════════
//  Props / Emits
// ══════════════════════════════════════════

const props = defineProps<{
  modelValue: LocationValue
  /** 是否进阶赋能广告（隐藏排除功能） */
  isAdvancedAd?: boolean
  /** 平台类型，默认 META */
  platform?: PlatformType
}>()

const emits = defineEmits<{
  (e: 'update:modelValue', val: LocationValue): void
}>()

// ══════════════════════════════════════════
//  State
// ══════════════════════════════════════════

const state = reactive({
  areaList: [] as RegionItem[],
  searchVal: '',
  directionalList: [] as RegionItem[],
  excludeList: [] as RegionItem[],
  tabActive: 'includes' as 'includes' | 'excludes',
  loading: false,
  importVisible: false,
  selectedSearchVal: ''
})

const importText = ref('')
const importResults = ref<RegionItem[]>([])
const importLoading = ref(false)

// 全量国家列表（进阶赋能全选用）
const countryAll = ref<RegionItem[]>([])

// ══════════════════════════════════════════
//  Computed
// ══════════════════════════════════════════

const filteredAreas = computed(() => {
  if (!state.searchVal.trim()) return state.areaList
  const kw = state.searchVal.toLowerCase()
  return state.areaList.filter(
    (item) =>
      item.name.toLowerCase().includes(kw) ||
      item.key.toLowerCase().includes(kw) ||
      (item.country_code ?? '').toLowerCase().includes(kw)
  )
})

const filteredSelected = computed(() => {
  const list = state.tabActive === 'includes' ? state.directionalList : state.excludeList
  if (!state.selectedSearchVal.trim()) return list
  const kw = state.selectedSearchVal.toLowerCase()
  return list.filter((item) => item.name.toLowerCase().includes(kw))
})

const directionalCount = computed(() => state.directionalList.length)
const excludeCount = computed(() => state.excludeList.length)

/** 按钮状态 Map */
const current = computed(() => {
  const map = new Map<
    string,
    { isDirectional: boolean; directionalTip: string; isExclude: boolean; excludeTip: string }
  >()
  for (const item of state.areaList) {
    const inD = state.directionalList.some((d) => isSameOrContained(d, item))
    const inE = state.excludeList.some((d) => isSameOrContained(d, item))
    const dTip = inD ? '已在定向列表中' : ''
    const eTip = inE ? '已在排除列表中' : ''
    map.set(item.key, {
      isDirectional: inD,
      directionalTip: dTip,
      isExclude: inE || item.key === 'worldwide',
      excludeTip: item.key === 'worldwide' ? '全球不可排除' : eTip
    })
  }
  return map
})

const isAll = computed({
  get: () =>
    countryAll.value.length > 0 &&
    directionalCount.value + excludeCount.value >= countryAll.value.length,
  set: (v: boolean) => {
    if (v) {
      state.directionalList = [...countryAll.value]
      state.excludeList = []
    } else {
      state.directionalList = []
      state.excludeList = []
    }
    emitModelInput()
  }
})

const indeterminate = computed(
  () => !isAll.value && (directionalCount.value > 0 || excludeCount.value > 0)
)

// ══════════════════════════════════════════
//  包含关系判断
// ══════════════════════════════════════════

function isSameOrContained(parent: RegionItem, child: RegionItem): boolean {
  if (parent.key === child.key) return true
  if (parent.type === 'country_group' && parent.country_codes?.includes(child.country_code ?? ''))
    return true
  if (parent.type === 'country' && parent.country_code === child.country_code) return true
  if (parent.type === 'city' && child.type === 'region') return parent.region_id === child.region_id
  return false
}

// ══════════════════════════════════════════
//  定向/排除操作
// ══════════════════════════════════════════

function handleDirectionalClick(item: RegionItem) {
  state.tabActive = 'includes'
  if (item.key === 'worldwide') {
    state.directionalList = [item]
    state.excludeList = []
  } else {
    removeExcludeByItem(item)
    state.directionalList = state.directionalList.filter((d) => d.key !== 'worldwide')
    filterSubAreas(state.directionalList, item)
    if (!state.directionalList.some((d) => d.key === item.key)) {
      state.directionalList.push(item)
    }
  }
  emitModelInput()
}

function handleExcludeClick(item: RegionItem) {
  state.tabActive = 'excludes'
  state.directionalList = state.directionalList.filter((d) => d.key !== item.key)
  filterSubAreas(state.excludeList, item)
  if (!state.excludeList.some((d) => d.key === item.key)) {
    state.excludeList.push(item)
  }
  emitModelInput()
}

function removeDirectional(key: string) {
  state.directionalList = state.directionalList.filter((i) => i.key !== key)
  emitModelInput()
}

function removeExclude(key: string) {
  state.excludeList = state.excludeList.filter((i) => i.key !== key)
  emitModelInput()
}

function removeExcludeByItem(item: RegionItem) {
  state.excludeList = state.excludeList.filter((i) => i.key !== item.key)
}

function filterSubAreas(list: RegionItem[], area: RegionItem) {
  const idx = list.findIndex((i) => isSameOrContained(area, i) && i.key !== area.key)
  if (idx >= 0) list.splice(idx, 1)
}

function handleRemoveAll() {
  state.directionalList = []
  state.excludeList = []
  emitModelInput()
}

function emitModelInput() {
  emits('update:modelValue', {
    includes: [...state.directionalList],
    excludes: [...state.excludeList]
  })
}

// ══════════════════════════════════════════
//  数据加载
// ══════════════════════════════════════════

/** API 返回的 CountryItem → 组件内部 RegionItem */
function toRegionItem(item: CountryItem): RegionItem {
  return {
    key: item.key,
    name: item.name,
    type: item.type,
    country_code: item.countryCode,
    country_codes: item.countryCodes,
    region_id: item.regionId
  }
}

async function loadAreas() {
  state.loading = true
  try {
    const res = await getCountryListApi({
      platform: props.platform ?? 2
    })
    const list = (res?.data ?? []).map(toRegionItem)
    state.areaList = list
    countryAll.value = list
  } catch (err) {
    console.error('加载地区列表失败', err)
  } finally {
    state.loading = false
  }
}

// ══════════════════════════════════════════
//  批量导入
// ══════════════════════════════════════════

async function handleImportText() {
  if (!importText.value.trim()) return
  importLoading.value = true
  try {
    const codes = importText.value
      .split(/[,;\s\n]+/)
      .map((s) => s.trim().toUpperCase())
      .filter(Boolean)
    const found = state.areaList.filter(
      (a) => codes.includes(a.key) || codes.includes(a.country_code ?? '')
    )
    importResults.value = found
  } finally {
    importLoading.value = false
  }
}

function confirmImport() {
  for (const item of importResults.value) {
    if (!state.directionalList.some((d) => d.key === item.key)) {
      state.directionalList.push(item)
    }
  }
  emitModelInput()
  state.importVisible = false
  importText.value = ''
  importResults.value = []
}

// ══════════════════════════════════════════
//  Init
// ══════════════════════════════════════════

watch(
  () => [props.modelValue.includes, props.modelValue.excludes],
  ([inc, exc]) => {
    state.directionalList = inc ?? []
    state.excludeList = exc ?? []
  },
  { immediate: true, deep: true }
)

async function init() {
  if (state.areaList.length === 0) await loadAreas()
}

defineExpose({ init, countryAll, loadAreas })

// auto-init
loadAreas()

// ══════════════════════════════════════════
//  暴露给 template 的级别标签映射
// ══════════════════════════════════════════

const levelLabels: Record<string, string> = {
  country_group: '大洲',
  country: '国家/地区',
  region: '区域',
  city: '城市'
}
</script>

<template>
  <div class="location-picker">
    <!-- ═══════════ 主区域 ═══════════ -->
    <div class="lp-body">
      <!-- ═══ 左侧：国家列表 ═══ -->
      <div class="lp-left">
        <div class="lp-panel-header">
          <span class="lp-panel-title">{{ t('locationPicker.countries') }}</span>
          <ElCheckbox
            v-if="props.isAdvancedAd"
            v-model="isAll"
            :indeterminate="indeterminate"
            :disabled="state.loading"
          >
            {{ t('locationPicker.selectAll') }}
          </ElCheckbox>
        </div>

        <div class="lp-search-box">
          <ElInput
            v-model="state.searchVal"
            :placeholder="t('locationPicker.searchPlaceholder')"
            clearable
            prefix-icon="Search"
            size="small"
          />
        </div>

        <div class="lp-list" v-loading="state.loading">
          <div v-for="item in filteredAreas" :key="item.key" class="lp-list-item">
            <div class="lp-list-item-info">
              <span class="lp-list-item-name">{{ item.name }}</span>
              <ElTag size="small" type="info" class="lp-list-item-type">
                {{ levelLabels[item.type] || item.type }}
              </ElTag>
            </div>
            <div class="lp-list-item-actions">
              <ElButton
                size="small"
                type="primary"
                link
                :disabled="current.get(item.key)?.isDirectional"
                :title="current.get(item.key)?.directionalTip"
                @click="handleDirectionalClick(item)"
              >
                {{ t('locationPicker.target') }}
              </ElButton>
              <ElButton
                v-if="!props.isAdvancedAd"
                size="small"
                type="danger"
                link
                :disabled="current.get(item.key)?.isExclude"
                :title="current.get(item.key)?.excludeTip"
                @click="handleExcludeClick(item)"
              >
                {{ t('locationPicker.exclude') }}
              </ElButton>
            </div>
          </div>
          <div v-if="!state.loading && filteredAreas.length === 0" class="lp-empty">
            {{ t('locationPicker.noResults') }}
          </div>
        </div>
      </div>

      <!-- ═══ 右侧：已选择 ═══ -->
      <div class="lp-right">
        <div class="lp-panel-header">
          <span class="lp-panel-title">{{ t('locationPicker.selected') }}</span>
          <div class="lp-panel-actions">
            <ElButton size="small" @click="state.importVisible = true">
              {{ t('locationPicker.batchImport') }}
            </ElButton>
            <ElButton
              size="small"
              @click="handleRemoveAll"
              :disabled="directionalCount + excludeCount === 0"
            >
              {{ t('locationPicker.clear') }}
            </ElButton>
          </div>
        </div>

        <div class="lp-search-box">
          <ElInput
            v-model="state.selectedSearchVal"
            :placeholder="t('locationPicker.searchSelected')"
            clearable
            size="small"
          />
        </div>

        <ElTabs v-model="state.tabActive" class="lp-tabs" stretch>
          <ElTabPane
            :label="`${t('locationPicker.target')} (${directionalCount})`"
            name="includes"
          />
          <ElTabPane
            v-if="!props.isAdvancedAd"
            :label="`${t('locationPicker.exclude')} (${excludeCount})`"
            name="excludes"
          />
        </ElTabs>

        <div v-if="state.tabActive === 'includes'" class="lp-selected-list">
          <template v-if="filteredSelected.length">
            <div v-for="item in filteredSelected" :key="item.key" class="lp-selected-item">
              <span class="lp-selected-name">{{ item.name }}</span>
              <span class="lp-selected-delete" @click="removeDirectional(item.key)">✕</span>
            </div>
          </template>
          <div v-else class="lp-empty">{{ t('locationPicker.noItems') }}</div>
        </div>

        <div v-if="!props.isAdvancedAd && state.tabActive === 'excludes'" class="lp-selected-list">
          <template v-if="filteredSelected.length">
            <div
              v-for="item in filteredSelected"
              :key="item.key"
              class="lp-selected-item is-exclude"
            >
              <span class="lp-selected-name">{{ item.name }}</span>
              <span class="lp-selected-delete" @click="removeExclude(item.key)">✕</span>
            </div>
          </template>
          <div v-else class="lp-empty">{{ t('locationPicker.noItems') }}</div>
        </div>
      </div>
    </div>

    <!-- ═══════════ 批量导入对话框 ═══════════ -->
    <ElDialog
      v-model="state.importVisible"
      :title="t('locationPicker.batchImportTitle')"
      width="480px"
    >
      <div class="import-area">
        <ElInput
          v-model="importText"
          type="textarea"
          :rows="5"
          :placeholder="t('locationPicker.importPlaceholder')"
        />
        <ElButton
          type="primary"
          size="small"
          :loading="importLoading"
          style="margin-top: 8px"
          @click="handleImportText"
        >
          {{ t('locationPicker.match') }}
        </ElButton>
      </div>
      <div v-if="importResults.length" class="import-results">
        <p
          >{{ t('locationPicker.matchResult') }}：{{ importResults.length }}
          {{ t('locationPicker.matchItems') }}</p
        >
        <div class="import-result-tags">
          <ElTag
            v-for="item in importResults"
            :key="item.key"
            closable
            @close="importResults = importResults.filter((i) => i.key !== item.key)"
          >
            {{ item.name }}
          </ElTag>
        </div>
      </div>
      <template #footer>
        <ElButton @click="state.importVisible = false">{{ t('common.cancelText') }}</ElButton>
        <ElButton type="primary" @click="confirmImport" :disabled="!importResults.length">
          {{ t('common.confirm') }}
        </ElButton>
      </template>
    </ElDialog>
  </div>
</template>

<style scoped>
.location-picker {
  width: 100%;
  overflow: hidden;
  border: 1px solid var(--el-border-color-light);
  border-radius: 6px;
}

.lp-body {
  display: flex;
  height: 400px;
}

/* ── 左/右面板通用 ── */
.lp-left,
.lp-right {
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.lp-left {
  flex: 7;
  border-right: 1px solid var(--el-border-color-lighter);
}

.lp-right {
  flex: 3;
}

.lp-panel-header {
  display: flex;
  padding: 10px 12px;
  background: var(--el-fill-color-light);
  border-bottom: 1px solid var(--el-border-color-lighter);
  align-items: center;
  justify-content: space-between;
}

.lp-panel-title {
  font-size: 13px;
  font-weight: 600;
}

.lp-panel-actions {
  display: flex;
  gap: 4px;
}

.lp-search-box {
  padding: 8px 12px;
}

/* ── 左侧列表 ── */
.lp-list {
  padding: 0 4px;
  overflow-y: auto;
  flex: 1;
}

.lp-list-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 6px 8px;
  border-radius: 4px;
  transition: background 0.15s;
}

.lp-list-item:hover {
  background: var(--el-fill-color-light);
}

.lp-list-item-info {
  display: flex;
  align-items: center;
  gap: 6px;
  min-width: 0;
}

.lp-list-item-name {
  overflow: hidden;
  font-size: 13px;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.lp-list-item-type {
  flex-shrink: 0;
}

.lp-list-item-actions {
  display: flex;
  gap: 4px;
  flex-shrink: 0;
}

/* ── 右侧 Tabs ── */
.lp-tabs {
  margin: 0 8px;
}

.lp-tabs :deep(.el-tabs__header) {
  margin-bottom: 0;
}

/* ── 右侧已选列表 ── */
.lp-selected-list {
  padding: 8px 12px;
  overflow-y: auto;
  flex: 1;
}

.lp-selected-item {
  display: flex;
  padding: 6px 8px;
  margin-bottom: 2px;
  background: var(--el-color-primary-light-9);
  border-radius: 4px;
  align-items: center;
  justify-content: space-between;
}

.lp-selected-item.is-exclude {
  background: var(--el-color-danger-light-9);
}

.lp-selected-name {
  overflow: hidden;
  font-size: 13px;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.lp-selected-delete {
  padding: 0 4px;
  font-size: 12px;
  color: var(--el-text-color-secondary);
  cursor: pointer;
  flex-shrink: 0;
}

.lp-selected-delete:hover {
  color: var(--el-color-danger);
}

/* ── 空状态 ── */
.lp-empty {
  padding: 24px 0;
  font-size: 13px;
  color: var(--el-text-color-placeholder);
  text-align: center;
}

/* ── 批量导入 ── */
.import-area {
  margin-bottom: 12px;
}

.import-result-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
  margin-top: 8px;
}

.import-results p {
  margin: 0 0 4px;
  font-size: 13px;
  color: var(--el-text-color-secondary);
}
</style>
