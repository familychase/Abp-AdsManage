<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElTag, ElProgress } from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { Search } from '@/components/Search'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'

const { t } = useI18n()

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const mockList = Array.from({ length: 20 }, (_, i) => ({
      id: `analysis-${i + 1}`,
      keyword: `关键词 ${i + 1}`,
      searchVolume: Math.floor(Math.random() * 100000),
      competition: Math.floor(Math.random() * 100),
      ctr: (Math.random() * 20).toFixed(2),
      conversionRate: (Math.random() * 15).toFixed(2),
      cost: (Math.random() * 5000).toFixed(2),
      trend: i % 3 === 0 ? '上升' : i % 3 === 1 ? '稳定' : '下降',
      updateTime: `2025-0${(i % 9) + 1}-${String((i % 28) + 1).padStart(2, '0')} 10:00:00`
    }))
    const start = (unref(currentPage) - 1) * unref(pageSize)
    return {
      list: mockList.slice(start, start + unref(pageSize)),
      total: mockList.length
    }
  },
  fetchDelApi: async () => {
    return true
  }
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

const searchParams = ref({})
const setSearchParams = (params: any) => {
  searchParams.value = params
  getList()
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
    field: 'keyword',
    label: '关键词',
    search: {
      component: 'Input',
      componentProps: { placeholder: '请输入关键词' }
    }
  },
  {
    field: 'searchVolume',
    label: '搜索量',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'competition',
    label: '竞争度',
    search: { hidden: true },
    form: { hidden: true },
    table: {
      slots: {
        default: (data: any) => {
          return <ElProgress percentage={data.row.competition} stroke-width={8} />
        }
      }
    }
  },
  {
    field: 'ctr',
    label: '点击率(%)',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'conversionRate',
    label: '转化率(%)',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'cost',
    label: '花费',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'trend',
    label: '趋势',
    search: {
      component: 'Select',
      componentProps: {
        options: [
          { value: '上升', label: '上升' },
          { value: '稳定', label: '稳定' },
          { value: '下降', label: '下降' }
        ]
      }
    },
    table: {
      slots: {
        default: (data: any) => {
          const typeMap: Record<string, string> = {
            上升: 'success',
            稳定: 'default',
            下降: 'danger'
          }
          return <ElTag type={(typeMap[data.row.trend] || 'info') as any}>{data.row.trend}</ElTag>
        }
      }
    }
  },
  {
    field: 'updateTime',
    label: '更新时间',
    search: { hidden: true },
    form: { hidden: true }
  }
])

const { allSchemas } = useCrudSchemas(crudSchemas)
</script>

<template>
  <ContentWrap>
    <Search :schema="allSchemas.searchSchema" @search="setSearchParams" @reset="setSearchParams" />
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
