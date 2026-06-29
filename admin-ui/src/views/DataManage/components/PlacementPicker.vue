<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import {
  ElCheckbox,
  ElCheckboxGroup,
  ElRadioGroup,
  ElRadioButton,
  ElDivider,
  ElTag
} from 'element-plus'
import { useI18n } from '@/hooks/web/useI18n'
import { useDictStore } from '@/store/modules/dict'
import { getPositionTreesApi } from '@/api/ads/metaPublishing'
import type { PositionTreeCategory } from '@/api/ads/metaPublishing/types'

const { t } = useI18n()
const dictStore = useDictStore()

// ══════════════════════════════════════════
//  Types
// ══════════════════════════════════════════

export interface PlacementValue {
  /** 平台 → 版位值列表（tree API 原始 value，如 { facebook: ["feed","story"], instagram: ["stream"] }） */
  selected: Record<string, string[]>
  isManual: boolean
  deviceType: string
  platformKeys: string[]
}

interface PlacementItem {
  value: string
  name: string
  platform: string
  category: string
  disabled: boolean
}

// ══════════════════════════════════════════
//  Props / Emits
// ══════════════════════════════════════════

const props = defineProps<{
  modelValue: PlacementValue
}>()

const emits = defineEmits<{
  (e: 'update:modelValue', val: PlacementValue): void
}>()

// ══════════════════════════════════════════
//  四大平台 — 来自字典一级
// ══════════════════════════════════════════

const PLATFORM_KEYS = ['facebook', 'instagram', 'messenger', 'audience_network'] as const

/** 平台列表：从字典取一级节点（parentId 为 null/undefined/0），dictItemCode 转小写作为 key */
const platformList = computed(() => {
  const items = dictStore.getItems('MetaPublisherPositions')
  const isTopLevel = (i: any) => i.parentId === null || i.parentId === undefined || i.parentId === 0
  const topLevel = items.length > 0 ? items.filter(isTopLevel) : items
  if (topLevel.length === 0) {
    return PLATFORM_KEYS.map((k) => ({
      key: k,
      name: k.charAt(0).toUpperCase() + k.slice(1).replace('_', ' ')
    }))
  }
  return topLevel.map((i) => ({
    key: (i.dictItemCode ?? '').toLowerCase(),
    name: i.dictItemName ?? i.dictItemCode ?? ''
  }))
})

const platformKeys = ref<string[]>([...props.modelValue.platformKeys])

watch(
  () => props.modelValue.platformKeys,
  (val) => {
    const next = val ?? []
    if (next.length > 0 && next.join(',') !== platformKeys.value.join(',')) {
      platformKeys.value = next
    }
  }
)

// ══════════════════════════════════════════
//  API 版位树
// ══════════════════════════════════════════

const treeLoading = ref(false)
const treeCategories = ref<PositionTreeCategory[]>([])
const allPlacements = ref<PlacementItem[]>([])

/** 分类（左栏）列表 */
const categoryList = computed(() => {
  return treeCategories.value.map((c) => ({ key: c.name, name: c.name }))
})

/** 平台 → 可选版位值集合（排除 disabled） */
const platformValueMap = computed(() => {
  const m = new Map<string, Set<string>>()
  for (const p of allPlacements.value) {
    if (p.disabled) continue
    if (!m.has(p.platform)) m.set(p.platform, new Set())
    m.get(p.platform)!.add(p.value)
  }
  return m
})

async function loadPlacementTree() {
  treeLoading.value = true
  try {
    const res = await getPositionTreesApi()
    const cats = res?.data ?? []
    if (cats.length > 0) {
      treeCategories.value = cats
      const all: PlacementItem[] = []
      for (const cat of cats) {
        for (const child of cat.children ?? []) {
          if (child.parent_value != null) {
            all.push({
              value: child.value,
              name: child.name,
              platform: child.parent_value,
              category: cat.name,
              disabled: child.disabled
            })
          }
        }
      }
      allPlacements.value = all
    }
  } catch {
    console.error('加载版位树失败')
  } finally {
    treeLoading.value = false
  }
}

onMounted(loadPlacementTree)

/** 树加载完成后，如果已勾选的平台没有版位数据，补齐 */
watch(allPlacements, (all) => {
  if (all.length === 0) return
  const selected = { ...props.modelValue.selected }
  let changed = false
  for (const pk of platformKeys.value) {
    const existing = new Set(selected[pk] ?? [])
    for (const v of platformValueMap.value.get(pk) ?? []) {
      if (!existing.has(v)) {
        if (!selected[pk]) selected[pk] = []
        selected[pk]!.push(v)
        changed = true
      }
    }
  }
  if (changed) {
    emitUpdate({ ...props.modelValue, selected })
  }
})

// ══════════════════════════════════════════
//  选中状态
// ══════════════════════════════════════════

const isManual = ref(props.modelValue.isManual ?? false)
const deviceType = ref(props.modelValue.deviceType ?? 'ALL_DEVICE')
const activeCategory = ref('')

// 左侧分类选中第一个
watch(categoryList, (list) => {
  if (list.length > 0 && !list.find((c) => c.key === activeCategory.value)) {
    activeCategory.value = list[0].key
  }
})

const placementMode = ref<'advanced' | 'manual'>(props.modelValue.isManual ? 'manual' : 'advanced')

function onPlacementModeChange(val: string) {
  const manual = val === 'manual'
  isManual.value = manual
  if (manual) {
    // 切手动 → 全选所有平台 + 所有版位
    const allPlatforms = collectAllAvailablePlatforms()
    selectedPlatformKeys.value = allPlatforms
    selectAllPlacements(allPlatforms)
  }
}

/** 收集 API 中出现的所有平台 */
function collectAllAvailablePlatforms(): string[] {
  const seen = new Set<string>()
  for (const p of allPlacements.value) {
    seen.add(p.platform)
  }
  return [...seen]
}

/** 全选所有平台下的版位 */
function selectAllPlacements(plats: string[]) {
  const selected: Record<string, string[]> = {}
  for (const pk of plats) {
    const vals = platformValueMap.value.get(pk)
    if (vals && vals.size > 0) {
      selected[pk] = [...vals]
    }
  }
  emitUpdate({ selected, isManual: true, deviceType: deviceType.value, platformKeys: plats })
}

// ══════════════════════════════════════════
//  版位大类 CheckboxGroup v-model
// ══════════════════════════════════════════

const selectedPlatformKeys = ref<string[]>([...platformKeys.value])

watch(platformKeys, (val) => {
  if (val.join(',') !== selectedPlatformKeys.value.join(',')) {
    selectedPlatformKeys.value = val
  }
})

watch(selectedPlatformKeys, (cur, prev) => {
  const prevArr = prev ?? []
  const added = cur.filter((k) => !prevArr.includes(k))
  const removed = prevArr.filter((k) => !cur.includes(k))
  const selected = { ...props.modelValue.selected }
  let changed = false
  for (const pk of added) {
    const vals = platformValueMap.value.get(pk)
    if (vals && vals.size > 0) {
      selected[pk] = [...vals]
      changed = true
    }
  }
  for (const pk of removed) {
    if (selected[pk]) {
      delete selected[pk]
      changed = true
    }
  }
  if (changed) {
    emitUpdate({
      selected,
      isManual: isManual.value,
      deviceType: deviceType.value,
      platformKeys: [...cur]
    })
  }
})

// ══════════════════════════════════════════
//  左栏 / 右栏
// ══════════════════════════════════════════

const subPlacements = computed(() => {
  return allPlacements.value.filter((p) => p.category === activeCategory.value)
})

function selectCategory(key: string) {
  activeCategory.value = key
}

function isPlacementSelected(p: PlacementItem): boolean {
  return (props.modelValue.selected[p.platform] ?? []).includes(p.value)
}

function placementChecked(p: PlacementItem): boolean {
  const vals = props.modelValue.selected[p.platform] ?? []
  return vals.includes(p.value)
}

function togglePlacement(p: PlacementItem) {
  const selected = { ...props.modelValue.selected }
  if (!selected[p.platform]) selected[p.platform] = []
  const arr = selected[p.platform]!
  const idx = arr.indexOf(p.value)
  if (idx >= 0) {
    arr.splice(idx, 1)
  } else {
    arr.push(p.value)
  }
  if (arr.length === 0) delete selected[p.platform]
  emitUpdate({
    selected,
    isManual: isManual.value,
    deviceType: deviceType.value,
    platformKeys: [...selectedPlatformKeys.value]
  })
}

function toggleCategoryAll(catKey: string, checked: boolean) {
  const children = allPlacements.value.filter((p) => p.category === catKey && !p.disabled)
  const { indeterminate } = categoryCheckedState(catKey)
  const shouldCheck = indeterminate ? true : checked
  const selected = { ...props.modelValue.selected }
  for (const child of children) {
    if (!selected[child.platform]) selected[child.platform] = []
    const arr = selected[child.platform]!
    if (shouldCheck) {
      if (!arr.includes(child.value)) arr.push(child.value)
    } else {
      selected[child.platform] = arr.filter((v) => v !== child.value)
      if (selected[child.platform]!.length === 0) delete selected[child.platform]
    }
  }
  emitUpdate({
    selected,
    isManual: isManual.value,
    deviceType: deviceType.value,
    platformKeys: [...selectedPlatformKeys.value]
  })
}

/** 指定分类的勾选状态（只统计非 disabled 项） */
function categoryCheckedState(catKey: string): { checked: boolean; indeterminate: boolean } {
  const children = allPlacements.value.filter((p) => p.category === catKey && !p.disabled)
  if (children.length === 0) return { checked: false, indeterminate: false }
  const checkedCount = children.filter((c) => isPlacementSelected(c)).length
  return {
    checked: checkedCount === children.length,
    indeterminate: checkedCount > 0 && checkedCount < children.length
  }
}

const selectedCount = computed(() => {
  let count = 0
  for (const arr of Object.values(props.modelValue.selected)) {
    count += (arr as string[]).length
  }
  return count
})

// ══════════════════════════════════════════
//  提交
// ══════════════════════════════════════════

function emitUpdate(val: PlacementValue) {
  emits('update:modelValue', val)
}

// isManual/deviceType 外部同步
watch(
  () => props.modelValue.isManual,
  (val) => {
    isManual.value = val ?? false
    placementMode.value = val ? 'manual' : 'advanced'
  }
)
watch(
  () => props.modelValue.deviceType,
  (val) => {
    deviceType.value = val ?? 'ALL_DEVICE'
  }
)
watch(
  () => props.modelValue.platformKeys,
  (val) => {
    const next = val ?? []
    if (next.length > 0 && next.join(',') !== selectedPlatformKeys.value.join(',')) {
      selectedPlatformKeys.value = next
    }
  }
)
</script>

<template>
  <div class="placement-picker">
    <!-- ═══════════ 模式 & 设备 ═══════════ -->
    <div class="pp-top-bar">
      <div class="pp-top-bar-left">
        <ElRadioGroup v-model="placementMode" size="small" @change="onPlacementModeChange">
          <ElRadioButton :value="'advanced'">{{
            dictStore.getLabel('MetaPositionType', 'POSITION_AUTO')
          }}</ElRadioButton>
          <ElRadioButton :value="'manual'">{{
            dictStore.getLabel('MetaPositionType', 'POSITION_MANUAL')
          }}</ElRadioButton>
        </ElRadioGroup>
      </div>
    </div>

    <!-- ═══════════ 四大平台（字典一级） ═══════════ -->
    <div v-if="isManual" class="pp-platform-bar">
      <ElCheckboxGroup v-model="selectedPlatformKeys" size="small">
        <ElCheckbox v-for="p in platformList" :key="p.key" :label="p.key">{{ p.name }}</ElCheckbox>
      </ElCheckboxGroup>
    </div>

    <!-- ═══════════ 加载中 ═══════════ -->
    <div v-if="treeLoading" class="pp-loading">
      <span class="pp-loading__text">正在加载版位数据...</span>
    </div>

    <!-- ═══════════ 手动：双栏级联（API 版位树） ═══════════ -->
    <template v-if="isManual && !treeLoading">
      <ElDivider style="margin: 8px 0" />
      <div class="pp-cascader-body">
        <!-- 左栏：版位分类（API 一级） -->
        <div class="pp-cascader-left">
          <div class="pp-cascader-title">{{ t('placementPicker.categories') }}</div>
          <div
            v-for="cat in categoryList"
            :key="cat.key"
            class="pp-cat-item"
            :class="{ 'is-active': activeCategory === cat.key }"
            @click="selectCategory(cat.key)"
          >
            <ElCheckbox
              :model-value="categoryCheckedState(cat.key).checked"
              :indeterminate="categoryCheckedState(cat.key).indeterminate"
              size="small"
              @change="toggleCategoryAll(cat.key, $event as boolean)"
              @click.stop
            />
            <span class="pp-cat-name">{{ cat.name }}</span>
          </div>
        </div>

        <!-- 右栏：子版位（API tree children） -->
        <div class="pp-cascader-right">
          <div class="pp-cascader-title">
            {{ t('placementPicker.subPlacements') }}
            <ElTag v-if="selectedCount" size="small" type="primary" effect="dark" round>
              {{ selectedCount }}
            </ElTag>
          </div>
          <div class="pp-cascader-sub-list">
            <div
              v-for="p in subPlacements"
              :key="`${p.platform}:${p.value}`"
              class="pp-sub-item"
              :class="{
                'is-selected': placementChecked(p),
                'is-disabled': p.disabled
              }"
              @click="!p.disabled && togglePlacement(p)"
            >
              <ElCheckbox
                :model-value="placementChecked(p)"
                :disabled="p.disabled"
                size="small"
                @click.stop
                @change="togglePlacement(p)"
              />
              <span class="pp-sub-name">{{ p.name }}</span>
              <span class="pp-sub-platform">{{ p.platform }}</span>
            </div>
            <div v-if="subPlacements.length === 0" class="pp-empty">
              {{ t('placementPicker.noSubPlacements') }}
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.placement-picker {
  width: 100%;
  overflow: hidden;
  border: 1px solid var(--el-border-color-light);
  border-radius: 6px;
}

.pp-top-bar {
  display: flex;
  padding: 12px 16px;
  background: var(--el-fill-color-light);
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.pp-platform-bar {
  padding: 10px 16px;
  border-bottom: 1px solid var(--el-border-color-lighter);
}

.pp-cascader-body {
  display: flex;
  height: 320px;
}

.pp-cascader-left,
.pp-cascader-right {
  overflow-y: auto;
}

.pp-cascader-left {
  width: 200px;
  flex-shrink: 0;
  background: var(--el-fill-color-lighter);
  border-right: 1px solid var(--el-border-color-lighter);
}

.pp-cascader-right {
  flex: 1;
}

.pp-cascader-title {
  position: sticky;
  top: 0;
  z-index: 1;
  display: flex;
  padding: 10px 14px;
  font-size: 13px;
  font-weight: 600;
  color: var(--el-text-color-primary);
  background: var(--el-bg-color);
  border-bottom: 1px solid var(--el-border-color-lighter);
  align-items: center;
  gap: 8px;
}

.pp-cat-item,
.pp-sub-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 14px;
  cursor: pointer;
  transition: background 0.15s;
}

.pp-cat-item:hover,
.pp-sub-item:hover {
  background: var(--el-fill-color-light);
}

.pp-cat-item.is-active {
  background: var(--el-color-primary-light-9);
}

.pp-sub-item.is-selected {
  background: var(--el-color-primary-light-9);
}

.pp-sub-item.is-disabled {
  cursor: not-allowed;
  opacity: 0.4;
}

.pp-cat-name,
.pp-sub-name {
  overflow: hidden;
  font-size: 13px;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.pp-sub-platform {
  margin-left: auto;
  font-size: 11px;
  color: var(--el-text-color-placeholder);
  flex-shrink: 0;
}

.pp-empty {
  padding: 20px 0;
  font-size: 13px;
  color: var(--el-text-color-placeholder);
  text-align: center;
}

.pp-loading {
  display: flex;
  padding: 40px 0;
  align-items: center;
  justify-content: center;
}

.pp-loading__text {
  font-size: 13px;
  color: var(--el-text-color-secondary);
}
</style>
