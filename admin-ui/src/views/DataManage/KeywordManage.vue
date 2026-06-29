<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElTag } from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { Search } from '@/components/Search'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'

const { t } = useI18n()

const ids = ref<string[]>([])

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const mockList = Array.from({ length: 30 }, (_, i) => ({
      id: `keyword-${i + 1}`,
      keywordId: `KW${String(10000 + i)}`,
      keyword: `关键词 ${i + 1}`,
      matchType: i % 3 === 0 ? '精确' : i % 3 === 1 ? '短语' : '广泛',
      status: i % 2 === 0 ? 1 : 0,
      bid: (Math.random() * 10).toFixed(2),
      impressions: Math.floor(Math.random() * 50000),
      clicks: Math.floor(Math.random() * 3000),
      createTime: `2025-0${(i % 9) + 1}-${String((i % 28) + 1).padStart(2, '0')} 09:00:00`
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
const { getList, delList } = tableMethods

const searchParams = ref({})
const setSearchParams = (params: any) => {
  searchParams.value = params
  getList()
}

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
    field: 'keyword',
    label: '关键词',
    search: {
      component: 'Input',
      componentProps: { placeholder: '请输入关键词' }
    }
  },
  {
    field: 'matchType',
    label: '匹配模式',
    search: {
      component: 'Select',
      componentProps: {
        options: [
          { value: '精确', label: '精确' },
          { value: '短语', label: '短语' },
          { value: '广泛', label: '广泛' }
        ]
      }
    },
    table: {
      slots: {
        default: (data: any) => {
          const typeMap: Record<string, 'primary' | 'success' | 'warning' | 'info'> = {
            精确: 'primary',
            短语: 'success',
            广泛: 'warning'
          }
          return <ElTag type={typeMap[data.row.matchType] ?? 'info'}>{data.row.matchType}</ElTag>
        }
      }
    }
  },
  {
    field: 'bid',
    label: '出价',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'impressions',
    label: '展示量',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'clicks',
    label: '点击量',
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'status',
    label: t('userDemo.status'),
    search: {
      component: 'Select',
      componentProps: {
        options: [
          { value: 1, label: t('userDemo.enable') },
          { value: 0, label: t('userDemo.disable') }
        ]
      }
    },
    table: {
      slots: {
        default: (data: any) => {
          return (
            <ElTag type={data.row.status === 1 ? 'success' : 'danger'}>
              {data.row.status === 1 ? t('userDemo.enable') : t('userDemo.disable')}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'createTime',
    label: t('tableDemo.displayTime'),
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'action',
    width: '200px',
    label: t('tableDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      slots: {
        default: (data: any) => {
          return (
            <>
              <BaseButton type="primary" size="small" onClick={() => {}}>
                {t('exampleDemo.edit')}
              </BaseButton>
              <BaseButton type="danger" size="small" onClick={() => delData(data.row)}>
                {t('exampleDemo.del')}
              </BaseButton>
            </>
          )
        }
      }
    }
  }
])

const { allSchemas } = useCrudSchemas(crudSchemas)

const delData = async (row: any) => {
  ids.value = [row.id]
  await delList(1)
}
</script>

<template>
  <ContentWrap>
    <Search :schema="allSchemas.searchSchema" @search="setSearchParams" @reset="setSearchParams" />
    <div class="mb-10px">
      <BaseButton type="primary">{{ t('exampleDemo.add') }}</BaseButton>
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
