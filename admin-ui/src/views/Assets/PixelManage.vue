<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElInput } from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'
import { getPixelListApi } from '@/api/ads/pixel'
import type { PixelDto } from '@/api/ads/pixel/types'

const { t } = useI18n()

// 搜索条件
const searchKeyword = ref('')

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const res = await getPixelListApi({
      page: unref(currentPage),
      pageSize: unref(pageSize),
      filterText: searchKeyword.value || undefined
    })
    const items = res.data.items || []
    return {
      list: items,
      total: res.data.totalCount || 0
    }
  },
  fetchDelApi: async () => {
    return true
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

const handleSearch = () => {
  currentPage.value = 1
  getList()
}

const handleReset = () => {
  searchKeyword.value = ''
  handleSearch()
}

const crudSchemas = reactive<CrudSchema[]>([
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'pixelName',
    label: t('pixelManage.pixelName'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (data: any) => {
          const row: PixelDto = data.row
          return (
            <div>
              <div style={{ fontWeight: 500 }}>{row.pixelName || '-'}</div>
              <div style={{ color: '#999', fontSize: '12px' }}>{row.pixelNo || '-'}</div>
            </div>
          )
        }
      }
    }
  },
  {
    field: 'pixelNo',
    label: t('pixelManage.accountNo'),
    search: { hidden: true },
    table: {
      minWidth: '140px',
      slots: {
        default: (data: any) => {
          const row: PixelDto = data.row
          return <span>{row.pixelNo || '-'}</span>
        }
      }
    }
  },
  {
    field: 'lastSyncTime',
    label: t('pixelManage.mediaUser'),
    search: { hidden: true },
    table: {
      minWidth: '180px',
      slots: {
        default: (data: any) => {
          const row: PixelDto = data.row
          return <span>{row.lastSyncTime || '-'}</span>
        }
      }
    }
  },
  {
    field: 'accountCount',
    label: t('pixelManage.adAccount'),
    search: { hidden: true },
    table: {
      slots: {
        default: (data: any) => {
          const row: PixelDto = data.row
          return <span>{t('pixelManage.itemCount', { count: row.accountCount ?? 0 })}</span>
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('pixelManage.creationTime'),
    search: { hidden: true },
    table: {
      minWidth: '160px',
      slots: {
        default: (data: any) => {
          const row: PixelDto = data.row
          return <span>{row.creationTime || '-'}</span>
        }
      }
    }
  }
])

const { allSchemas } = useCrudSchemas(crudSchemas)
</script>

<template>
  <ContentWrap>
    <div
      style="
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 10px;
        margin-bottom: 16px;
      "
    >
      <div style="display: flex; align-items: center; flex-wrap: wrap; gap: 10px">
        <ElInput
          v-model="searchKeyword"
          :placeholder="t('pixelManage.searchKeywordPlaceholder')"
          clearable
          style="width: 220px"
          @keyup.enter="handleSearch"
          @clear="handleSearch"
        />
        <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
        <BaseButton @click="handleReset">{{ t('common.reset') }}</BaseButton>
      </div>
    </div>

    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      style="width: 100%"
      table-layout="auto"
      @register="tableRegister"
    />
  </ContentWrap>
</template>
