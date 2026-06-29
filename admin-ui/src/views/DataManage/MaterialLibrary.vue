<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ContentWrap } from '@/components/ContentWrap'
import { Icon } from '@/components/Icon'
import { BaseButton } from '@/components/Button'
import {
  ElCheckbox,
  ElPagination,
  ElTag,
  ElEmpty,
  ElMessageBox,
  ElMessage,
  ElDialog,
  ElSkeleton,
  ElSkeletonItem
} from 'element-plus'
import { ElInput, ElSelect, ElOption } from 'element-plus'
import { useTable } from '@/hooks/web/useTable'
import { getMaterialListApi, deleteMaterialApi } from '@/api/ads/material'
import type { MaterialItem, MaterialListResponse } from '@/api/ads/material/types'
import MaterialUploadDrawer from './components/MaterialUploadDrawer.vue'

// ══════════════════════════════════════════
//  筛选条件
// ══════════════════════════════════════════

const filterText = ref('')
const filterType = ref('')

const typeOptions = [
  { label: '全部', value: '' },
  { label: '视频', value: 'VIDEO' },
  { label: '图片', value: 'IMAGE' }
]

// ══════════════════════════════════════════
//  useTable
// ══════════════════════════════════════════

const { tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const res = await getMaterialListApi({
      page: currentPage.value,
      pageSize: pageSize.value,
      filterText: filterText.value || null,
      materialType: (filterType.value || null) as any
    })

    const payload = res.data as any as MaterialListResponse
    const items: MaterialItem[] = payload.items || []
    return {
      list: items,
      total: payload.totalCount || 0
    }
  },
  fetchDelApi: async () => {
    return true
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

// ══════════════════════════════════════════
//  筛选操作
// ══════════════════════════════════════════

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  filterText.value = ''
  filterType.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  上传 Drawer
// ══════════════════════════════════════════

const uploadDrawerVisible = ref(false)

const openUploadDrawer = () => {
  uploadDrawerVisible.value = true
}

const handleUploadSuccess = () => {
  uploadDrawerVisible.value = false
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  上传预览
// ══════════════════════════════════════════

const uploadPreviewVisible = ref(false)
const uploadPreviewUrl = ref('')
const uploadPreviewTitle = ref('')
const uploadPreviewIsVideo = ref(false)

const handleUploadPreview = (file: any, url: string) => {
  uploadPreviewTitle.value = file.name || '预览'
  uploadPreviewUrl.value = url
  uploadPreviewIsVideo.value = file.raw?.type?.startsWith('video/') || false
  uploadPreviewVisible.value = true
}

// ══════════════════════════════════════════
//  卡片选择
// ══════════════════════════════════════════

const selectedMaterialIds = ref<Set<number>>(new Set())
const isAllSelected = computed(() => {
  const list = dataList.value as MaterialItem[]
  return list.length > 0 && list.every((item) => selectedMaterialIds.value.has(item.id))
})
const isIndeterminate = computed(() => {
  const list = dataList.value as MaterialItem[]
  const selected = list.filter((item) => selectedMaterialIds.value.has(item.id)).length
  return selected > 0 && selected < list.length
})

const toggleSelectAll = () => {
  const list = dataList.value as MaterialItem[]
  const next = new Set(selectedMaterialIds.value)
  if (isAllSelected.value) {
    list.forEach((item) => next.delete(item.id))
  } else {
    list.forEach((item) => next.add(item.id))
  }
  selectedMaterialIds.value = next
}

const toggleSelect = (id: number) => {
  const next = new Set(selectedMaterialIds.value)
  if (next.has(id)) {
    next.delete(id)
  } else {
    next.add(id)
  }
  selectedMaterialIds.value = next
}

const selectedCount = computed(() => selectedMaterialIds.value.size)

// ══════════════════════════════════════════
//  删除素材
// ══════════════════════════════════════════

const handleDeleteMaterial = async (item: MaterialItem) => {
  try {
    await ElMessageBox.confirm(`确定删除素材「${item.materialName}」吗？`, '删除确认', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
  } catch {
    return
  }
  try {
    await deleteMaterialApi(item.id)
    ElMessage.success('删除成功')
    getList()
  } catch (err: any) {
    ElMessage.error(err?.message || '删除失败')
  }
}

// ══════════════════════════════════════════
//  图片预览
// ══════════════════════════════════════════

const previewVisible = ref(false)
const previewUrl = ref('')

const handleCardClick = (item: MaterialItem) => {
  if (item.materialType === 'IMAGE') {
    previewUrl.value = item.materialUrl
    previewVisible.value = true
  }
}

// 获取缩略图URL
const getThumbUrl = (item: MaterialItem) => item.coverUrl || item.materialUrl

// ══════════════════════════════════════════
//  工具函数
// ══════════════════════════════════════════

const formatDuration = (seconds?: number): string => {
  if (!seconds) return ''
  const m = Math.floor(seconds / 60)
  const s = Math.floor(seconds % 60)
  return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
}

// ══════════════════════════════════════════
//  分页变化
// ══════════════════════════════════════════

const handleCurrentChange = (page: number) => {
  currentPage.value = page
  getList()
}

const handleSizeChange = (size: number) => {
  pageSize.value = size
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  错误占位图
// ══════════════════════════════════════════

const handleImageError = (e: Event) => {
  const img = e.target as HTMLImageElement
  img.src =
    'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTYwIiBoZWlnaHQ9IjE2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTYwIiBoZWlnaHQ9IjE2MCIgZmlsbD0iI2Y1ZjdmYSIvPjx0ZXh0IHg9IjgwIiB5PSI4MCIgZm9udC1zaXplPSIxMiIgZmlsbD0iIzkwOTM5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPua3oeW+lyBpbWFnZTwvdGV4dD48L3N2Zz4='
}

onMounted(() => {
  getList()
})
</script>

<template>
  <ContentWrap>
    <div class="material-library">
      <!-- ═══════════ 筛选 + 上传 ═══════════ -->
      <div class="ml-toolbar">
        <div class="ml-filter-bar">
          <ElInput
            v-model="filterText"
            placeholder="搜索素材名称"
            clearable
            class="ml-filter-input"
            @clear="handleSearch"
            @keyup.enter="handleSearch"
          />
          <ElSelect
            v-model="filterType"
            placeholder="类型"
            clearable
            class="ml-filter-select"
            @change="handleSearch"
            @clear="handleSearch"
          >
            <ElOption
              v-for="opt in typeOptions"
              :key="opt.value"
              :label="opt.label"
              :value="opt.value"
            />
          </ElSelect>
          <BaseButton type="primary" @click="handleSearch">查询</BaseButton>
          <BaseButton @click="handleReset">重置</BaseButton>
        </div>
        <BaseButton type="primary" @click="openUploadDrawer">
          <Icon icon="ep:upload" :size="14" class="mr-4px" />
          上传素材
        </BaseButton>
      </div>

      <!-- ═══════════ 素材网格 ═══════════ -->
      <div class="ml-grid-wrapper">
        <template v-if="loading">
          <div v-for="i in 8" :key="'sk-' + i" class="ml-card ml-card--skeleton">
            <ElSkeleton animated>
              <template #template>
                <ElSkeletonItem variant="image" class="ml-card__skeleton-img" />
                <div class="px-12px py-8px">
                  <ElSkeletonItem variant="text" style="width: 70%" />
                  <ElSkeletonItem variant="text" style="width: 40%" />
                </div>
              </template>
            </ElSkeleton>
          </div>
        </template>

        <template v-else-if="dataList.length === 0">
          <div class="ml-empty">
            <ElEmpty description="暂无素材数据" />
          </div>
        </template>

        <template v-else>
          <div class="ml-select-all-bar">
            <ElCheckbox
              :model-value="isAllSelected"
              :indeterminate="isIndeterminate"
              @change="toggleSelectAll"
            />
            <span v-if="selectedCount > 0" class="ml-selected-hint">
              已选 {{ selectedCount }} 个素材
            </span>
            <span v-else class="ml-selected-hint ml-selected-hint--dim"> 全选 </span>
          </div>

          <div
            v-for="item in dataList as MaterialItem[]"
            :key="item.id"
            class="ml-card"
            :class="{ 'ml-card--selected': selectedMaterialIds.has(item.id) }"
          >
            <div class="ml-card__check" @click.stop>
              <ElCheckbox
                :model-value="selectedMaterialIds.has(item.id)"
                @change="toggleSelect(item.id)"
              />
            </div>

            <div class="ml-card__thumb" @click="handleCardClick(item)">
              <img
                :src="getThumbUrl(item)"
                :alt="item.materialName"
                class="ml-card__img"
                loading="lazy"
                @error="handleImageError"
              />

              <div v-if="item.materialType === 'VIDEO'" class="ml-card__play-overlay">
                <div class="ml-card__play-icon">
                  <Icon icon="ep:video-play" :size="20" color="#fff" />
                </div>
                <span v-if="item.duration" class="ml-card__duration">
                  {{ formatDuration(item.duration) }}
                </span>
              </div>

              <div v-else class="ml-card__type-tag">
                <ElTag size="small" effect="dark" type="info">图片</ElTag>
              </div>
            </div>

            <div class="ml-card__info">
              <div class="ml-card__name" :title="item.materialName">{{ item.materialName }}</div>
              <button
                class="ml-card__delete-btn"
                title="删除"
                @click.stop="handleDeleteMaterial(item)"
              >
                <Icon icon="ep:delete" :size="14" />
              </button>
            </div>
          </div>
        </template>
      </div>

      <div v-if="total > 0" class="ml-pagination">
        <ElPagination
          :current-page="currentPage"
          :page-size="pageSize"
          :total="total"
          :page-sizes="[12, 24, 48, 96]"
          layout="total, sizes, prev, pager, next, jumper"
          background
          @current-change="handleCurrentChange"
          @size-change="handleSizeChange"
        />
      </div>
    </div>

    <!-- ═══════════ 图片预览弹窗 ═══════════ -->
    <ElDialog
      v-model="previewVisible"
      title="图片预览"
      width="80%"
      top="5vh"
      :destroy-on-close="true"
    >
      <div class="ml-preview">
        <img :src="previewUrl" class="ml-preview__img" />
      </div>
      <template #footer>
        <BaseButton @click="previewVisible = false">关闭</BaseButton>
      </template>
    </ElDialog>

    <!-- ═══════════ 上传预览弹窗 ═══════════ -->
    <ElDialog
      v-model="uploadPreviewVisible"
      :title="uploadPreviewTitle"
      width="80%"
      top="5vh"
      :destroy-on-close="true"
    >
      <div class="ml-preview">
        <img v-if="!uploadPreviewIsVideo" :src="uploadPreviewUrl" class="ml-preview__img" />
        <video v-else :src="uploadPreviewUrl" controls autoplay class="ml-preview__img"></video>
      </div>
      <template #footer>
        <BaseButton @click="uploadPreviewVisible = false">关闭</BaseButton>
      </template>
    </ElDialog>

    <!-- ═══════════ 上传素材 Drawer ═══════════ -->
    <MaterialUploadDrawer
      v-model:visible="uploadDrawerVisible"
      @success="handleUploadSuccess"
      @preview="handleUploadPreview"
    />
  </ContentWrap>
</template>

<style scoped>
.material-library {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.ml-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  flex-wrap: wrap;
}

.ml-filter-bar {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}

.ml-filter-input {
  width: 160px;
}

.ml-filter-select {
  width: 120px;
}

.ml-grid-wrapper {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 16px;
}

.ml-empty {
  grid-column: 1 / -1;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 300px;
}

.ml-select-all-bar {
  grid-column: 1 / -1;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 4px;
  font-size: 13px;
  color: var(--el-text-color-secondary);
}

.ml-selected-hint {
  font-weight: 500;
  color: var(--el-color-primary);
}

.ml-selected-hint--dim {
  font-weight: 400;
  color: var(--el-text-color-placeholder);
}

.ml-card {
  position: relative;
  overflow: hidden;
  background: var(--el-bg-color);
  border: 1px solid var(--el-border-color-light);
  border-radius: 10px;
  transition: all 0.2s ease;
}

.ml-card:hover {
  border-color: var(--el-color-primary-light-5);
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgb(0 0 0 / 6%);
}

.ml-card--selected {
  border-color: var(--el-color-primary);
  box-shadow: 0 0 0 2px var(--el-color-primary-light-7);
}

.ml-card--skeleton {
  padding: 0;
  border: 1px solid var(--el-border-color-lighter);
}

.ml-card__skeleton-img {
  width: 100%;
  height: 160px;
}

.ml-card__check {
  position: absolute;
  top: 8px;
  left: 8px;
  z-index: 2;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  background: rgb(255 255 255 / 85%);
  border-radius: 4px;
  backdrop-filter: blur(4px);
  opacity: 0;
  transition: opacity 0.15s ease;
}

.ml-card:hover .ml-card__check,
.ml-card--selected .ml-card__check {
  opacity: 1;
}

.ml-card__badge {
  position: absolute;
  top: 8px;
  right: 8px;
  z-index: 2;
}

.ml-card__thumb {
  position: relative;
  width: 100%;
  aspect-ratio: 16 / 9;
  overflow: hidden;
  cursor: pointer;
  background: var(--el-fill-color-lighter);
}

.ml-card__img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.ml-card__play-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: rgb(0 0 0 / 15%);
  transition: background 0.2s ease;
}

.ml-card:hover .ml-card__play-overlay {
  background: rgb(0 0 0 / 35%);
}

.ml-card__play-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  color: #fff;
  background: rgb(0 0 0 / 55%);
  border-radius: 50%;
  transition: transform 0.2s ease;
}

.ml-card:hover .ml-card__play-icon {
  transform: scale(1.1);
}

.ml-card__duration {
  position: absolute;
  right: 8px;
  bottom: 8px;
  padding: 1px 6px;
  font-family: 'JetBrains Mono', Consolas, monospace;
  font-size: 11px;
  color: #fff;
  background: rgb(0 0 0 / 65%);
  border-radius: 4px;
}

.ml-card__type-tag {
  position: absolute;
  bottom: 8px;
  left: 8px;
}

.ml-card__info {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 12px;
}

.ml-card__name {
  min-width: 0;
  overflow: hidden;
  font-size: 13px;
  font-weight: 500;
  color: var(--el-text-color-primary);
  text-overflow: ellipsis;
  white-space: nowrap;
  flex: 1;
}

.ml-card__delete-btn {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  padding: 0;
  color: var(--el-text-color-placeholder);
  cursor: pointer;
  background: transparent;
  border: none;
  border-radius: 6px;
  transition: all 0.15s ease;
}

.ml-card__delete-btn:hover {
  color: var(--el-color-danger);
  background: var(--el-color-danger-light-9);
}

.ml-pagination {
  display: flex;
  justify-content: flex-end;
  padding-top: 4px;
}

.ml-preview {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 200px;
}

.ml-preview__img {
  max-width: 100%;
  max-height: 70vh;
  object-fit: contain;
}
</style>
