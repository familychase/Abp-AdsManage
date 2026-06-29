<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElTag, ElInput } from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'
import { getPageListApi } from '@/api/page'

const { t } = useI18n()

// 筛选条件
const searchKeyword = ref('')

// 平台类型映射
const platformLabels: Record<number, string> = {
  1: 'Google',
  2: 'Facebook',
  3: 'TikTok',
  4: 'LinkedIn'
}

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const res = await getPageListApi({
      page: unref(currentPage), // API 从 1 起始
      pageSize: unref(pageSize),
      filter: searchKeyword.value || undefined
    })
    return {
      list: res.data.items,
      total: res.data.totalCount
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

const { t: tPage } = useI18n('pageManage')

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
    type: 'index',
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'pageNo',
    label: tPage('pageNo'),
    search: { hidden: true },
    table: { minWidth: '160px' }
  },
  {
    field: 'pageName',
    label: tPage('pageName'),
    search: { hidden: true },
    table: { minWidth: '180px' }
  },
  {
    field: 'category',
    label: tPage('category'),
    search: { hidden: true },
    table: { minWidth: '100px' }
  },
  {
    field: 'platform',
    label: tPage('platform'),
    search: { hidden: true },
    table: {
      minWidth: '100px',
      slots: {
        default: (data: any) => {
          return <ElTag>{platformLabels[data.row.platform] ?? data.row.platform}</ElTag>
        }
      }
    }
  },
  {
    field: 'lastSyncTime',
    label: tPage('lastSyncTime'),
    search: { hidden: true },
    table: { minWidth: '160px' }
  },
  {
    field: 'creationTime',
    label: tPage('creationTime'),
    search: { hidden: true },
    table: { minWidth: '160px' }
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
          :placeholder="t('pageManage.searchPlaceholder')"
          clearable
          style="width: 200px"
          @keyup.enter="handleSearch"
        />
        <BaseButton type="primary" @click="handleSearch">{{ t('common.query') }}</BaseButton>
        <BaseButton @click="handleReset">
          {{ t('common.reset') }}
        </BaseButton>
      </div>
    </div>

    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{ total }"
      @register="tableRegister"
    />
  </ContentWrap>
</template>
