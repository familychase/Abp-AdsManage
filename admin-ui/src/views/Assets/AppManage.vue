<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ElSelect, ElOption, ElInput, ElTag } from 'element-plus'
import { ref, reactive, unref } from 'vue'
import { useTable } from '@/hooks/web/useTable'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'

const { t } = useI18n()

const searchType = ref('')
const searchUser = ref('')
const searchName = ref('')

const appTypes = ['Android', 'iOS', 'Web', '小程序']
const authUsers = ['Facebook_用户A', 'Google_用户B', 'TikTok_用户C']

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const platforms = ['Facebook', 'Google', 'TikTok', 'LinkedIn']
    const mockList = Array.from({ length: 15 }, (_, i) => ({
      id: `app-${i + 1}`,
      name: `应用 ${i + 1}`,
      appId: `APP${String(20000 + i)}`,
      type: appTypes[i % 4],
      authUser: authUsers[i % 3],
      platform: platforms[i % 4],
      updateTime: `2025-0${(i % 9) + 1}-${String((i % 28) + 1).padStart(2, '0')} ${String(8 + (i % 10)).padStart(2, '0')}:00:00`
    }))
    const start = (unref(currentPage) - 1) * unref(pageSize)
    return {
      list: mockList.slice(start, start + unref(pageSize)),
      total: mockList.length
    }
  },
  fetchDelApi: async () => true
})

const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList } = tableMethods

const handleSearch = () => {
  currentPage.value = 1
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
    field: 'name',
    label: t('appManage.appName'),
    search: { hidden: true },
    table: {
      width: '200px',
      slots: {
        default: (data: any) => (
          <div>
            <div style="font-weight:500">{data.row.name}</div>
            <div style="color:#999;font-size:12px">ID: {data.row.appId}</div>
          </div>
        )
      }
    }
  },
  {
    field: 'authUser',
    label: t('appManage.mediaUser'),
    search: { hidden: true },
    table: {
      slots: {
        default: (data: any) => <ElTag type="success">{data.row.authUser}</ElTag>
      }
    }
  },
  {
    field: 'updateTime',
    label: t('appManage.updateTime'),
    search: { hidden: true }
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
        <ElSelect
          v-model="searchType"
          :placeholder="t('appManage.appType')"
          clearable
          style="width: 150px"
          @change="handleSearch"
        >
          <ElOption v-for="item in appTypes" :key="item" :label="item" :value="item" />
        </ElSelect>
        <ElSelect
          v-model="searchUser"
          :placeholder="t('appManage.mediaUser')"
          clearable
          style="width: 160px"
          @change="handleSearch"
        >
          <ElOption v-for="item in authUsers" :key="item" :label="item" :value="item" />
        </ElSelect>
        <ElInput
          v-model="searchName"
          :placeholder="t('appManage.nameOrCode')"
          clearable
          style="width: 180px"
          @change="handleSearch"
        />
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
