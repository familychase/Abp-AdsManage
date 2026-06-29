<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { Table } from '@/components/Table'
import { ref, unref, computed, watch } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { getCampaignRptListApi } from '@/api/assets/campaignRptManage'
import type { CampaignRptItem } from '@/api/assets/campaignRptManage/types'
import { campaignBatchDeleteApi } from '@/api/ads/campaign'
import type { CampaignBatchDeleteItem } from '@/api/ads/campaign/types'
import { Icon } from '@/components/Icon'
import { useClipboard } from '@/hooks/web/useClipboard'
import {
  ElInput,
  ElMessage,
  ElMessageBox,
  ElDialog,
  ElTable,
  ElTableColumn,
  ElTag
} from 'element-plus'
import { BaseButton } from '@/components/Button'
import { useI18n } from '@/hooks/web/useI18n'

const { t } = useI18n()

const { copy } = useClipboard()

const handleCopy = (val: string) => {
  copy(val)
  ElMessage.success(t('campaignRpt.copySuccess'))
}

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { pageSize, currentPage } = tableState
    const res = await getCampaignRptListApi({
      page: unref(currentPage),
      pageSize: unref(pageSize),
      accountNo: unref(accountNoFilter) || undefined,
      campaignNo: unref(campaignNoFilter) || undefined,
      campaignName: unref(campaignNameFilter) || undefined
    })
    return {
      list: res.data.items || [],
      total: res.data.totalCount || 0
    }
  }
})

const { total, loading, dataList, pageSize, currentPage } = tableState
const { getList, getElTableExpose } = tableMethods

const accountNoFilter = ref('')
const campaignNoFilter = ref('')
const campaignNameFilter = ref('')

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  accountNoFilter.value = ''
  campaignNoFilter.value = ''
  campaignNameFilter.value = ''
  currentPage.value = 1
  getList()
}

const formatPrice = (val: number) => {
  if (val == null || isNaN(val)) return '-'
  return `$${val.toFixed(2)}`
}

const formatNum = (val: number) => {
  if (val == null || isNaN(val)) return '-'
  return val.toLocaleString()
}

const crudSchemas = computed<CrudSchema[]>(() => [
  {
    field: 'index',
    label: t('campaignRpt.index'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'index', width: 60 }
  },
  {
    field: 'selection',
    label: '',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection', width: 50 }
  },
  {
    field: 'accountName',
    label: t('campaignRpt.accountName'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      minWidth: 100,
      slots: {
        default: (data: any) => {
          const item = data.row as CampaignRptItem
          return (
            <div>
              <div style="font-weight:500">{item.accountName}</div>
              <div style="color:#999;font-size:12px;display:inline-flex;align-items:center;gap:4px">
                <span>编号: {item.accountNo}</span>
                <span onClick={() => handleCopy(item.accountNo)}>
                  <Icon icon="ep:copy-document" size={12} style="cursor:pointer;color:#999" />
                </span>
              </div>
            </div>
          )
        }
      }
    }
  },
  {
    field: 'campaignName',
    label: t('campaignRpt.campaignName'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      minWidth: 200,
      slots: {
        default: (data: any) => {
          const item = data.row as CampaignRptItem
          return (
            <div>
              <div style="font-weight:500">{item.campaignName}</div>
              <div style="color:#999;font-size:12px;display:inline-flex;align-items:center;gap:4px">
                <span style="display:inline-flex;align-items:center;gap:6px">
                  编号：{item.campaignNo}
                  <span onClick={() => handleCopy(item.campaignNo)}>
                    <Icon
                      icon="ep:copy-document"
                      size={14}
                      style="cursor:pointer;color:#999;flex-shrink:0"
                    />
                  </span>
                </span>
              </div>
            </div>
          )
        }
      }
    }
  },
  {
    field: 'platform',
    label: t('campaignRpt.platform'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { width: 90 }
  },
  {
    field: 'reportDate',
    label: t('campaignRpt.reportDate'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 110,
      slots: {
        default: (data: any) => {
          const item = data.row as CampaignRptItem
          const d = item.reportDate ? item.reportDate.split('T')[0] : '-'
          return <span>{d}</span>
        }
      }
    }
  },
  {
    field: 'spend',
    label: t('campaignRpt.spend'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 100,
      align: 'right',
      slots: {
        default: (data: any) => {
          return <span>{formatPrice((data.row as CampaignRptItem).spend)}</span>
        }
      }
    }
  },
  {
    field: 'impressions',
    label: t('campaignRpt.impressions'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 110,
      align: 'right',
      slots: {
        default: (data: any) => {
          return <span>{formatNum((data.row as CampaignRptItem).impressions)}</span>
        }
      }
    }
  },
  {
    field: 'clicks',
    label: t('campaignRpt.clicks'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 100,
      align: 'right',
      slots: {
        default: (data: any) => {
          return <span>{formatNum((data.row as CampaignRptItem).clicks)}</span>
        }
      }
    }
  },
  {
    field: 'converts',
    label: t('campaignRpt.converts'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 100,
      align: 'right',
      slots: {
        default: (data: any) => {
          return <span>{formatNum((data.row as CampaignRptItem).converts)}</span>
        }
      }
    }
  },
  {
    field: 'cpc',
    label: t('campaignRpt.cpc'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 100,
      align: 'right',
      slots: {
        default: (data: any) => {
          return <span>{formatPrice((data.row as CampaignRptItem).cpc)}</span>
        }
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
//  批量删除
// ══════════════════════════════════════════

const batchDeleteLoading = ref(false)
const batchResultDialogVisible = ref(false)
const batchResultItems = ref<CampaignBatchDeleteItem[]>([])

const handleBatchDelete = async () => {
  const elTableRef = await getElTableExpose()
  const selected = elTableRef?.getSelectionRows() || []
  if (selected.length === 0) {
    ElMessage.warning(t('campaignRpt.selectDelWarning'))
    return
  }
  try {
    await ElMessageBox.confirm(
      t('campaignRpt.batchDelConfirm', { count: selected.length }),
      t('common.delWarning'),
      { confirmButtonText: t('common.ok'), cancelButtonText: t('common.cancel'), type: 'warning' }
    )
  } catch {
    return
  }

  batchDeleteLoading.value = true
  try {
    const campaignNos = selected.map((r: any) => r.campaignNo)
    const res = await campaignBatchDeleteApi({ campaignNos })
    batchResultItems.value = (res.data as any)?.items ?? (res.data as any) ?? []
    batchResultDialogVisible.value = true
  } catch (err: any) {
    ElMessage.error(t('campaignRpt.batchDelFailed') + (err?.message || ''))
  } finally {
    batchDeleteLoading.value = false
  }
}
</script>

<template>
  <ContentWrap>
    <div class="mb-10px flex gap-10px">
      <ElInput
        v-model="accountNoFilter"
        :placeholder="t('campaignRpt.accountNoPlaceholder')"
        clearable
        style="width: 200px"
        @keyup.enter="handleSearch"
        @clear="handleSearch"
      />
      <ElInput
        v-model="campaignNoFilter"
        :placeholder="t('campaignRpt.campaignNoPlaceholder')"
        clearable
        style="width: 200px"
        @keyup.enter="handleSearch"
        @clear="handleSearch"
      />
      <ElInput
        v-model="campaignNameFilter"
        :placeholder="t('campaignRpt.campaignNamePlaceholder')"
        clearable
        style="width: 200px"
        @keyup.enter="handleSearch"
        @clear="handleSearch"
      />
      <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
      <BaseButton @click="handleReset">{{ t('common.reset') }}</BaseButton>
      <BaseButton type="danger" :loading="batchDeleteLoading" @click="handleBatchDelete">
        {{ t('campaignRpt.batchDelete') }}
      </BaseButton>
    </div>
    <Table
      v-model:current-page="currentPage"
      v-model:page-size="pageSize"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      @register="tableRegister"
      :border="true"
      stripe
    />

    <!-- 批量删除结果弹窗 -->
    <ElDialog
      v-model="batchResultDialogVisible"
      :title="t('campaignRpt.batchDelResult')"
      width="700px"
      top="10vh"
    >
      <ElTable :data="batchResultItems" border stripe max-height="55vh">
        <ElTableColumn prop="campaignNo" :label="t('campaignRpt.campaignNo')" min-width="180" />
        <ElTableColumn :label="t('campaignRpt.status')" width="100">
          <template #default="{ row }">
            <ElTag :type="row.success ? 'success' : 'danger'" size="small">
              {{ row.success ? t('campaignRpt.success') : t('campaignRpt.failed') }}
            </ElTag>
          </template>
        </ElTableColumn>
        <ElTableColumn prop="errorMessage" :label="t('campaignRpt.errorLog')" min-width="300">
          <template #default="{ row }">
            <span v-if="row.errorMessage" style="color: #f56c6c">{{ row.errorMessage }}</span>
            <span v-else>-</span>
          </template>
        </ElTableColumn>
      </ElTable>
      <template #footer>
        <BaseButton @click="batchResultDialogVisible = false">{{ t('common.close') }}</BaseButton>
      </template>
    </ElDialog>
  </ContentWrap>
</template>
