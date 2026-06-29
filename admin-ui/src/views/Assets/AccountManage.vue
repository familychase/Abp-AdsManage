<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import {
  ElTag,
  ElSelect,
  ElOption,
  ElInput,
  ElMessage,
  ElDialog,
  ElTable,
  ElTableColumn
} from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'
import { PLATFORM_LABELS, PLATFORM_OPTIONS } from '@/constants/platform'
import { getAccountListApi } from '@/api/ads/account'
import type { AdsAccountDto } from '@/api/ads/account/types'
import type { PlatformType } from '@/api/ads/channel/types'
import {
  createSyncScheduleApi,
  getSyncScheduleListApi,
  pushSyncScheduleApi
} from '@/api/ads/syncSchedule'

const { t } = useI18n()

// ══════════════════════════════════════════
//  搜索条件
// ══════════════════════════════════════════

const searchFilterText = ref('')
const searchPlatform = ref<PlatformType | ''>('')
const searchAccountState = ref<number | ''>('')
const searchAccountNo = ref('')
const searchAccountName = ref('')

const getSearchParams = () => ({
  page: unref(currentPage),
  pageSize: unref(pageSize),
  filterText: searchFilterText.value || undefined,
  platform: searchPlatform.value !== '' ? (searchPlatform.value as PlatformType) : undefined,
  accountState: searchAccountState.value !== '' ? (searchAccountState.value as number) : undefined,
  accountNo: searchAccountNo.value || undefined,
  accountName: searchAccountName.value || undefined
})

// ══════════════════════════════════════════
//  表格数据
// ══════════════════════════════════════════

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const res = await getAccountListApi(getSearchParams())
    const items = res.data.items || []
    return {
      list: items,
      total: res.data.totalCount || 0
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
  searchFilterText.value = ''
  searchPlatform.value = ''
  searchAccountState.value = ''
  searchAccountNo.value = ''
  searchAccountName.value = ''
  currentPage.value = 1
  getList()
}

// ══════════════════════════════════════════
//  平台颜色映射
// ══════════════════════════════════════════

const platformColorMap: Record<PlatformType, string> = {
  0: '#909399',
  1: '#4285f4',
  2: '#1877f2',
  3: '#000'
}

// ══════════════════════════════════════════
//  账户状态映射
// ══════════════════════════════════════════

// ══════════════════════════════════════════
//  表格列定义
// ══════════════════════════════════════════

const crudSchemas = reactive<CrudSchema[]>([
  {
    field: 'selection',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: { type: 'selection' }
  },
  {
    field: 'index',
    label: t('tableDemo.index'),
    form: { hidden: true },
    search: { hidden: true },
    detail: { hidden: true },
    table: { type: 'index' }
  },
  {
    field: 'accountNo',
    label: t('accountManage.accountNo'),
    search: { hidden: true },
    table: {
      minWidth: '200px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          return <span style="font-family: monospace; font-size: 13px">{row.accountNo || '-'}</span>
        }
      }
    }
  },
  {
    field: 'accountName',
    label: t('accountManage.accountName'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          return <div style="font-weight: 500; color: #409eff">{row.accountName || '-'}</div>
        }
      }
    }
  },
  {
    field: 'platform',
    label: t('accountManage.platform'),
    search: { hidden: true },
    table: {
      minWidth: '120px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          const plt = Number(row.platform)
          return (
            <ElTag
              style={{
                border: 'none',
                color: '#fff',
                backgroundColor: platformColorMap[plt] || '#909399'
              }}
              size="small"
            >
              {PLATFORM_LABELS[plt] || row.platform}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'accountState',
    label: t('accountManage.accountState'),
    search: { hidden: true },
    table: {
      minWidth: '100px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          const isNormal = row.accountState === 1 || String(row.accountState) === 'NORMAL'
          const cfg = isNormal
            ? { type: 'success' as const, label: t('common.normal') }
            : { type: 'danger' as const, label: t('common.abnormal') }
          return <ElTag type={cfg.type}>{cfg.label}</ElTag>
        }
      }
    }
  },
  {
    field: 'balance',
    label: t('accountManage.balance'),
    search: { hidden: true },
    table: {
      minWidth: '130px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          const val = row.balance ?? 0
          const isLow = val < 10000
          return (
            <span style={{ color: isLow ? '#f56c6c' : '#67c23a', fontWeight: 500 }}>
              $ {val.toLocaleString('en-US', { minimumFractionDigits: 2 })}
            </span>
          )
        }
      }
    }
  },
  {
    field: 'timezone',
    label: t('accountManage.timezone'),
    search: { hidden: true },
    table: {
      minWidth: '160px',
      slots: {
        default: (data: any) => {
          const row: AdsAccountDto = data.row
          const tz = row.timezone
          const offset = row.utcTimezoneOffset
          return (
            <span style="font-size: 12px">
              {tz || '-'}
              {tz && offset && <span style="color: #409eff">({offset})</span>}
            </span>
          )
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('accountManage.creationTime'),
    search: { hidden: true },
    table: { minWidth: '160px' }
  },
  {
    field: 'action',
    width: '100px',
    label: t('tableDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      fixed: 'right',
      width: '100px',
      slots: {
        default: (data: any) => (
          <BaseButton type="primary" size="small" onClick={() => openSyncDialog(data.row)}>
            {t('accountManage.syncData')}
          </BaseButton>
        )
      }
    }
  }
])

// ══════════════════════════════════════════
//  同步弹窗
// ══════════════════════════════════════════

const syncDialogVisible = ref(false)
const syncAccountRef = ref<AdsAccountDto | null>(null)
const syncDataList = ref<
  Array<{ id?: number; jobName?: string; scheduleKey?: string; name?: string }>
>([])
const syncLoading = ref(false)

const openSyncDialog = async (row: AdsAccountDto) => {
  syncAccountRef.value = row
  syncDialogVisible.value = true
  syncLoading.value = true
  try {
    const res = await getSyncScheduleListApi({
      resourceType: 'AD_ACCOUNT',
      resourceId: row.accountNo || ''
    })
    syncDataList.value = ((res.data as any)?.items || res.data || []) as any[]
  } catch {
    syncDataList.value = []
    ElMessage.warning(t('accountManage.syncLoadFailed'))
  } finally {
    syncLoading.value = false
  }
}

const handleSyncItem = async (item: { id?: number; name?: string; jobName?: string }) => {
  const row = syncAccountRef.value
  if (!row) return

  try {
    if (item.id) {
      await pushSyncScheduleApi(item.id)
    } else {
      await createSyncScheduleApi({
        actionType: 0,
        jobName: item.jobName || item.name || `${row.accountName || ''} - Sync`,
        platform: row.platform,
        resourceId: row.accountNo || '',
        resourceType: 'AD_ACCOUNT',
        nextPublishTime: new Date().toISOString()
      })
    }
    ElMessage.success(t('accountManage.syncSubmitSuccess'))
  } catch (err: any) {
    ElMessage.error(t('accountManage.syncFailed') + (err?.message || ''))
  }
}

const { allSchemas } = useCrudSchemas(crudSchemas)
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
          v-model="searchFilterText"
          :placeholder="t('accountManage.searchPlaceholder')"
          clearable
          style="width: 200px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElSelect
          v-model="searchPlatform"
          :placeholder="t('accountManage.platform')"
          clearable
          style="width: 140px"
          @change="handleSearch"
        >
          <ElOption
            v-for="opt in PLATFORM_OPTIONS.filter((o) => o.value !== 0)"
            :key="opt.value"
            :label="opt.label"
            :value="opt.value"
          />
        </ElSelect>
        <ElSelect
          v-model="searchAccountState"
          :placeholder="t('accountManage.accountState')"
          clearable
          style="width: 140px"
          @change="handleSearch"
        >
          <ElOption :label="t('common.normal')" :value="1" />
          <ElOption :label="t('common.abnormal')" :value="2" />
        </ElSelect>
        <ElInput
          v-model="searchAccountNo"
          :placeholder="t('accountManage.accountNo')"
          clearable
          style="width: 160px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <ElInput
          v-model="searchAccountName"
          :placeholder="t('accountManage.accountName')"
          clearable
          style="width: 160px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
        <BaseButton @click="handleReset">{{ t('common.reset') }}</BaseButton>
      </div>
      <div style="display: flex; gap: 10px"></div>
    </div>

    <!-- 表格 -->
    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      table-layout="auto"
      @register="tableRegister"
    />

    <!-- 同步数据弹窗 -->
    <ElDialog
      v-model="syncDialogVisible"
      :title="t('accountManage.syncTitle')"
      width="540px"
      top="15vh"
    >
      <ElTable v-loading="syncLoading" :data="syncDataList" border stripe>
        <ElTableColumn type="index" label="#" width="60" />
        <ElTableColumn prop="jobNameDisplay" :label="t('common.name')" />
        <ElTableColumn :label="t('tableDemo.action')" width="120">
          <template #default="{ row }">
            <BaseButton type="primary" size="small" @click="handleSyncItem(row)">
              {{ t('accountManage.syncNow') }}
            </BaseButton>
          </template>
        </ElTableColumn>
      </ElTable>
      <template #footer>
        <BaseButton @click="syncDialogVisible = false">{{ t('common.close') }}</BaseButton>
      </template>
    </ElDialog>
  </ContentWrap>
</template>
