<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import {
  ElTag,
  ElInput,
  ElSelect,
  ElOption,
  ElMessage,
  ElMessageBox,
  ElDialog,
  ElPagination,
  ElCheckbox
} from 'element-plus'
import { ref, unref, computed, watch } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'

import { getDuplicateLogListApi, getDuplicateDetailApi, batchDeleteCampaignsApi } from '@/api/ads'
import { BatchDeleteResultItem } from '@/api/ads/index'
import type { DuplicateLogDto, DuplicateState } from '@/api/ads/types'

const { t } = useI18n()

// ══════════════════════════════════════════
//  状态 映射
// ══════════════════════════════════════════

const STATE_LABELS: Record<DuplicateState, string> = {
  PENDING: '待处理',
  SUCCESS: '成功',
  FAILED: '失败',
  IN_PROGRESS: '处理中'
}

const STATE_TAG_TYPE: Record<
  DuplicateState,
  'primary' | 'success' | 'warning' | 'info' | 'danger'
> = {
  PENDING: 'info',
  SUCCESS: 'success',
  FAILED: 'danger',
  IN_PROGRESS: 'warning'
}

// ══════════════════════════════════════════
//  查询条件
// ══════════════════════════════════════════

const searchAdObjectNo = ref('')
const searchAccountNo = ref('')
const searchState = ref<DuplicateState | ''>('')

// ══════════════════════════════════════════
//  表格 & 数据
// ══════════════════════════════════════════

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const res = await getDuplicateLogListApi({
      Page: unref(currentPage),
      PageSize: unref(pageSize),
      AdObjectNo: searchAdObjectNo.value || undefined,
      AccountNo: searchAccountNo.value || undefined,
      State: searchState.value || undefined
    })
    const data = res.data
    return {
      list: (data as any)?.items ?? [],
      total: (data as any)?.totalCount ?? 0
    }
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  searchAdObjectNo.value = ''
  searchAccountNo.value = ''
  searchState.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  列定义
// ══════════════════════════════════════════

const crudSchemas = computed<CrudSchema[]>(() => [
  {
    field: 'adObjectNo',
    label: t('duplicate.adObjectNo'),
    search: { hidden: true },
    table: { minWidth: '160px' }
  },
  {
    field: 'accountNo',
    label: t('duplicate.accountNo'),
    search: { hidden: true },
    table: { minWidth: '140px' }
  },
  {
    field: 'state',
    label: t('duplicate.state'),
    search: { hidden: true },
    table: {
      width: '100px',
      slots: {
        default: (data: any) => {
          const row: DuplicateLogDto = data.row
          const st = (row.state || 'PENDING') as DuplicateState
          return (
            <ElTag type={STATE_TAG_TYPE[st] || 'info'} size="small">
              {STATE_LABELS[st] || st}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'scheduleTime',
    label: t('duplicate.scheduleTime'),
    search: { hidden: true },
    table: {
      width: '180px',
      slots: {
        default: (data: any) => {
          const val = data.row.scheduleTime ?? data.row.ScheduleTime ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'endTime',
    label: t('duplicate.endTime'),
    search: { hidden: true },
    table: {
      width: '180px',
      slots: {
        default: (data: any) => {
          const val = data.row.endTime ?? data.row.EndTime ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'copyNumber',
    label: t('duplicate.copyNumberInfo'),
    search: { hidden: true },
    table: {
      width: '100px',
      slots: {
        default: (data: any) => {
          const val = data.row.copyNumber ?? data.row.CopyNumber
          return <span>{val ?? '-'}</span>
        }
      }
    }
  },
  {
    field: 'pageNo',
    label: t('duplicate.pageNo'),
    search: { hidden: true },
    table: {
      width: '160px',
      slots: {
        default: (data: any) => {
          const val = data.row.pageNo ?? data.row.PageNo ?? '-'
          return <span>{val}</span>
        }
      }
    }
  },
  {
    field: 'action',
    label: t('tableDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      fixed: 'right',
      width: '120px',
      slots: {
        default: (data: any) => (
          <BaseButton type="primary" size="small" onClick={() => openDetailDialog(data.row)}>
            {t('duplicate.viewDetail')}
          </BaseButton>
        )
      }
    }
  }
])

// 初始化和监听 locale 变化时重新生成 allSchemas
const { allSchemas } = useCrudSchemas(crudSchemas.value)
watch(crudSchemas, (val) => {
  const { allSchemas: fresh } = useCrudSchemas(val)
  Object.assign(allSchemas, fresh)
})

// ══════════════════════════════════════════
//  详情弹窗
// ══════════════════════════════════════════

const detailDialogVisible = ref(false)
const detailLoading = ref(false)
const detailList = ref<DuplicateLogDto[]>([])
const detailAccountNo = ref('')
const detailPage = ref(1)
const detailPageSize = ref(20)

// 删除状态：Map<id, { status, errorMessage? }>
const deleteStatusMap = ref<Map<number, { status: 'success' | 'error'; errorMessage?: string }>>(
  new Map()
)

/**
 * 根据 batch_delete 返回的 per-item 结果更新状态 Map
 */
const applyDeleteResults = (results: BatchDeleteResultItem[]) => {
  const next = new Map(deleteStatusMap.value)
  // 建立 campaignNo → DuplicateLogDto 映射
  const campaignMap = new Map<string, DuplicateLogDto>()
  for (const d of detailList.value) {
    if (d.campaignNo) campaignMap.set(d.campaignNo, d)
  }
  for (const r of results) {
    const item = campaignMap.get(r.campaignNo)
    if (item) {
      next.set(item.id, {
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

// ══════════════════════════════════════════
//  批量选择
// ══════════════════════════════════════════

const selectedDetailIds = ref<Set<number>>(new Set())

const isAllSelected = computed(() => {
  const pageList = detailPaginatedList.value
  return pageList.length > 0 && pageList.every((d) => selectedDetailIds.value.has(d.id))
})

const isIndeterminate = computed(() => {
  const pageList = detailPaginatedList.value
  const selected = pageList.filter((d) => selectedDetailIds.value.has(d.id)).length
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
    // 取消选中当前页
    pageList.forEach((d) => next.delete(d.id))
  } else {
    // 选中当前页
    pageList.forEach((d) => next.add(d.id))
  }
  selectedDetailIds.value = next
}

const handleBatchDelete = async () => {
  const selectedIds = [...selectedDetailIds.value]
  if (selectedIds.length === 0) return

  // 从 detailList 中找出选中的 campaignNo
  const campaignNos = detailList.value
    .filter((d) => selectedDetailIds.value.has(d.id))
    .map((d) => d.campaignNo || '')
    .filter(Boolean)

  if (campaignNos.length === 0) return

  try {
    await ElMessageBox.confirm(
      `确定删除选中的 ${campaignNos.length} 条复制记录吗？此操作不可撤销。`,
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
    const res = await batchDeleteCampaignsApi({
      accountNo: detailAccountNo.value,
      campaignNos
    })
    const results: BatchDeleteResultItem[] = (res.data as any)?.data ?? res.data ?? []
    applyDeleteResults(results)
    selectedDetailIds.value = new Set()
  } catch (err: any) {
    ElMessage.error(err?.message || '删除失败')
  }
}

const openDetailDialog = async (row: DuplicateLogDto) => {
  detailDialogVisible.value = true
  detailPage.value = 1
  selectedDetailIds.value = new Set()
  deleteStatusMap.value = new Map()
  detailAccountNo.value = row.accountNo || ''
  detailLoading.value = true
  try {
    const res = await getDuplicateDetailApi(row.id)
    const data = res.data
    // 兼容单条或数组返回
    detailList.value = Array.isArray(data) ? data : [data]
  } catch (err: any) {
    ElMessage.error(t('duplicate.fetchDetailFailed') + (err?.message || ''))
    detailList.value = [row]
  } finally {
    detailLoading.value = false
  }
}

// ══════════════════════════════════════════
//  删除单条明细
// ══════════════════════════════════════════

const handleDeleteDetail = async (item: DuplicateLogDto) => {
  try {
    await ElMessageBox.confirm(`确定删除该条复制记录吗？此操作不可撤销。`, t('common.delWarning'), {
      confirmButtonText: t('common.delOk'),
      cancelButtonText: t('common.delCancel'),
      type: 'warning'
    })
  } catch {
    return // 取消
  }

  try {
    const res = await batchDeleteCampaignsApi({
      accountNo: detailAccountNo.value,
      campaignNos: [item.campaignNo || '']
    })
    const results: BatchDeleteResultItem[] = (res.data as any)?.data ?? res.data ?? []
    applyDeleteResults(results)
  } catch (err: any) {
    const next = new Map(deleteStatusMap.value)
    next.set(item.id, { status: 'error', errorMessage: err?.message || '请求失败' })
    deleteStatusMap.value = next
    ElMessage.error(err?.message || '删除失败')
  }
}
</script>

<template>
  <ContentWrap>
    <!-- 搜索栏 -->
    <div
      style="
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        flex-wrap: wrap;
        gap: 12px;
        margin-bottom: 20px;
      "
    >
      <div style="display: flex; align-items: center; flex-wrap: wrap; gap: 12px">
        <ElInput
          v-model="searchAdObjectNo"
          :placeholder="t('duplicate.adObjectNoPlaceholder')"
          clearable
          style="width: 200px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElInput
          v-model="searchAccountNo"
          :placeholder="t('duplicate.accountNoPlaceholder')"
          clearable
          style="width: 200px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElSelect
          v-model="searchState"
          :placeholder="t('duplicate.state')"
          clearable
          style="width: 140px"
          @change="handleSearch"
        >
          <ElOption v-for="(label, val) in STATE_LABELS" :key="val" :label="label" :value="val" />
        </ElSelect>
        <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
        <BaseButton @click="handleReset">{{ t('common.reset') }}</BaseButton>
      </div>
    </div>

    <!-- 表格 -->
    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      @register="tableRegister"
    />

    <!-- 详情弹窗 -->
    <ElDialog
      v-model="detailDialogVisible"
      :title="t('duplicate.detailTitle')"
      width="880px"
      top="10vh"
    >
      <div v-loading="detailLoading" class="detail-container">
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
                :model-value="selectedDetailIds.has(row.id)"
                size="small"
                @change="toggleSelectDetail(row.id)"
              />
              <span class="detail-card__campaign">{{ row.campaignNo || '-' }}</span>
              <ElTag
                v-if="row.state"
                :type="STATE_TAG_TYPE[row.state as DuplicateState] || 'info'"
                size="small"
              >
                {{ STATE_LABELS[row.state as DuplicateState] || row.state }}
              </ElTag>
            </div>
            <div class="detail-card__right">
              <span v-if="row.creationTime" class="detail-card__time">{{ row.creationTime }}</span>
              <!-- 删除状态 -->
              <ElTag
                v-if="deleteStatusMap.has(row.id)"
                :type="deleteStatusMap.get(row.id)!.status === 'success' ? 'success' : 'danger'"
                size="small"
              >
                {{ deleteStatusMap.get(row.id)!.status === 'success' ? '已删除' : deleteStatusMap.get(row.id)!.errorMessage || '删除失败' }}
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

        <div v-if="!detailLoading && detailList.length === 0" class="detail-empty">
          {{ t('common.selectText') }}...
        </div>
      </div>
      <template #footer>
        <BaseButton @click="detailDialogVisible = false">{{ t('common.close') }}</BaseButton>
      </template>
    </ElDialog>
  </ContentWrap>
</template>

<style scoped>
/* ═══════════ 详情弹窗 ─ 卡片列表 ═══════════ */
.detail-container {
  min-height: 120px;
}

/* 汇总条 */
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

/* 分页器 */
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

/* 头部行 */
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

/* 错误块 */
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

/* 复制内容区 */
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

/* 空状态 */
.detail-empty {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
  font-size: 14px;
  color: var(--el-text-color-placeholder);
}
</style>
