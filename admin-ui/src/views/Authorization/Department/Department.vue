<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { Search } from '@/components/Search'
import { Dialog } from '@/components/Dialog'
import { useI18n } from '@/hooks/web/useI18n'

import { Table } from '@/components/Table'
import {
  getDepartmentListApi,
  createDepartmentApi,
  updateDepartmentApi,
  deleteDepartmentApi as deleteSystemDepartmentApi
} from '@/api/authorization/department/index.js'
import type { SysDepartmentDto } from '@/api/authorization/department/types.js'
import { useTable } from '@/hooks/web/useTable'
import { ref, unref, reactive } from 'vue'
import Write from './components/Write.vue'
import Detail from './components/Detail.vue'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'

// 前端展示用的部门数据类型
interface DepartmentTableItem {
  id: string | number
  departmentName: string
  parentName: string
  creationTime: string
  remark: string
  deptName?: string
  parentId?: number
  aliasName?: string
  sort?: number
}

// 部门名称缓存，用于将 parentId 解析为 parentName
let deptNameMap: Record<number, string> = {}

const buildDeptNameMap = (items: SysDepartmentDto[]) => {
  deptNameMap = {}
  for (const item of items) {
    deptNameMap[item.id] = item.deptName
  }
}

// 后端部门数据映射到前端展示
const mapDeptDto = (dto: SysDepartmentDto): DepartmentTableItem => ({
  id: dto.id,
  departmentName: dto.deptName,
  parentName: dto.parentName ? deptNameMap[dto.parentId] : '-',
  parentId: dto.parentId,
  creationTime: dto.creationTime,
  remark: dto.remark || '',
  deptName: dto.deptName,
  aliasName: dto.aliasName || '',
  sort: dto.sort
})

const ids = ref<string[]>([])

/** 从扁平列表中递归收集某节点及其所有子节点 ID */
const collectSubtreeIds = (items: any[], parentId: number): number[] => {
  const ids: number[] = [parentId]
  for (const item of items) {
    if (item.parentId === parentId) {
      ids.push(...collectSubtreeIds(items, item.id))
    }
  }
  return ids
}

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { currentPage, pageSize } = tableState
    const res = await getDepartmentListApi({
      pageSize: unref(pageSize),
      page: unref(currentPage) - 1, // 后端 page 从 0 开始
      filterText: unref(searchParams).deptName || undefined,
      parentId: unref(searchParams).parentId || undefined
    })
    const items = res?.items || []
    buildDeptNameMap(items)
    return {
      list: items.map(mapDeptDto),
      total: res?.totalCount || 0
    }
  },
  fetchDelApi: async () => {
    const idArr = unref(ids)
    for (const id of idArr) {
      await deleteSystemDepartmentApi(Number(id))
    }
    return true
  }
})
const { loading, dataList, total, currentPage, pageSize } = tableState
const { getList, delList } = tableMethods

const searchParams = ref<Record<string, any>>({})
const setSearchParams = (params: any) => {
  searchParams.value = params
  getList()
}

const { t } = useI18n()

const crudSchemas = reactive<CrudSchema[]>([
  {
    field: 'index',
    label: t('tableDemo.index'),
    type: 'index',
    search: {
      hidden: true
    },
    form: {
      hidden: true
    },
    detail: {
      hidden: true
    }
  },
  {
    field: 'deptName',
    label: t('userDemo.departmentName'),
    form: {
      component: 'Input',
      componentProps: {
        placeholder: '请输入部门名称'
      }
    },
    detail: {
      slots: {
        default: (data: any) => {
          return <>{data.departmentName}</>
        }
      }
    }
  },
  {
    field: 'parentName',
    label: '上级部门名称',
    search: {
      hidden: true
    },
    table: {
      slots: {
        default: (data: any) => {
          return <>{data.row.parentName}</>
        }
      }
    },
    form: {
      component: 'TreeSelect',
      componentProps: {
        nodeKey: 'id',
        props: {
          label: 'deptName'
        },
        clearable: true,
        placeholder: '请选择上级部门（为空则创建顶级部门）'
      },
      optionApi: async () => {
        const res = await getDepartmentListApi()
        const list = res?.items || []
        let filtered = list
        // 编辑时排除本部门及其所有子部门
        if (actionType.value === 'edit' && currentRow.value) {
          const excludeIds = collectSubtreeIds(list, Number(currentRow.value.id))
          filtered = list.filter((dept) => !excludeIds.includes(dept.id))
        }
        return filtered.map((dept) => ({
          id: dept.id,
          deptName: dept.deptName,
          parentId: dept.parentId
        }))
      }
    },
    detail: {
      hidden: true
    }
  },
  {
    field: 'aliasName',
    label: '别名',
    search: {
      hidden: true
    },
    form: {
      component: 'Input',
      componentProps: {
        placeholder: '请输入别名（选填）'
      }
    },
    detail: {
      slots: {
        default: (data: any) => {
          return <>{data.aliasName || '-'}</>
        }
      }
    }
  },
  {
    field: 'sort',
    label: '排序',
    search: {
      hidden: true
    },
    table: {
      width: 80
    },
    form: {
      component: 'InputNumber',
      componentProps: {
        min: 0,
        max: 9999,
        placeholder: '排序号'
      }
    },
    detail: {
      slots: {
        default: (data: any) => {
          return <>{data.sort ?? 0}</>
        }
      }
    }
  },
  {
    field: 'creationTime',
    label: t('tableDemo.displayTime'),
    search: {
      hidden: true
    },
    form: {
      hidden: true
    }
  },
  {
    field: 'remark',
    label: t('userDemo.remark'),
    search: {
      hidden: true
    },
    form: {
      component: 'Input',
      componentProps: {
        type: 'textarea',
        rows: 5
      },
      colProps: {
        span: 24
      }
    },
    detail: {
      slots: {
        default: (data: any) => {
          return <>{data.remark}</>
        }
      }
    }
  },
  {
    field: 'action',
    width: '260px',
    label: t('tableDemo.action'),
    search: {
      hidden: true
    },
    form: {
      hidden: true
    },
    detail: {
      hidden: true
    },
    table: {
      slots: {
        default: (data: any) => {
          return (
            <>
              <BaseButton size="small" type="primary" onClick={() => action(data.row, 'edit')}>
                {t('exampleDemo.edit')}
              </BaseButton>
              <BaseButton size="small" type="success" onClick={() => action(data.row, 'detail')}>
                {t('exampleDemo.detail')}
              </BaseButton>
              <BaseButton size="small" type="danger" onClick={() => delData(data.row)}>
                {t('exampleDemo.del')}
              </BaseButton>
            </>
          )
        }
      }
    }
  }
])

// @ts-ignore
const { allSchemas } = useCrudSchemas(crudSchemas)

const dialogVisible = ref(false)
const dialogTitle = ref('')

const currentRow = ref<DepartmentTableItem | null>(null)
const actionType = ref('')

const AddAction = () => {
  dialogTitle.value = t('exampleDemo.add')
  currentRow.value = null
  dialogVisible.value = true
  actionType.value = ''
}

const delLoading = ref(false)

const delData = async (row: DepartmentTableItem) => {
  ids.value = [String(row.id)]
  delLoading.value = true
  await delList(unref(ids).length).finally(() => {
    delLoading.value = false
  })
}

const action = (row: DepartmentTableItem, type: string) => {
  dialogTitle.value = t(type === 'edit' ? 'exampleDemo.edit' : 'exampleDemo.detail')
  actionType.value = type
  currentRow.value = row
  dialogVisible.value = true
}

const writeRef = ref<ComponentRef<typeof Write>>()

const saveLoading = ref(false)

const save = async () => {
  const write = unref(writeRef)
  const formData = await write?.submit()
  if (formData) {
    saveLoading.value = true
    try {
      // POST /api/system/departments 接口参数
      const dto = {
        parentId: formData.parentId || 0,
        deptName: formData.deptName || '',
        aliasName: formData.aliasName || null,
        sort: formData.sort ?? 0,
        path: null,
        remark: formData.remark || null
      }
      if (actionType.value === 'edit' && currentRow.value) {
        await updateDepartmentApi(Number(currentRow.value.id), dto)
      } else {
        await createDepartmentApi(dto)
      }
      dialogVisible.value = false
      currentPage.value = 1
      getList()
    } catch (error) {
      console.log(error)
    } finally {
      saveLoading.value = false
    }
  }
}
</script>

<template>
  <ContentWrap>
    <Search :schema="allSchemas.searchSchema" @search="setSearchParams" @reset="setSearchParams" />

    <div class="mb-10px">
      <BaseButton type="primary" @click="AddAction">{{ t('exampleDemo.add') }}</BaseButton>
    </div>

    <Table
      v-model:pageSize="pageSize"
      v-model:currentPage="currentPage"
      :columns="allSchemas.tableColumns"
      :data="dataList"
      :loading="loading"
      :pagination="{
        total: total
      }"
      @register="tableRegister"
    />
  </ContentWrap>

  <Dialog v-model="dialogVisible" :title="dialogTitle">
    <Write
      v-if="actionType !== 'detail'"
      ref="writeRef"
      :form-schema="allSchemas.formSchema"
      :current-row="currentRow"
    />

    <Detail
      v-if="actionType === 'detail'"
      :detail-schema="allSchemas.detailSchema"
      :current-row="currentRow"
    />

    <template #footer>
      <BaseButton
        v-if="actionType !== 'detail'"
        type="primary"
        :loading="saveLoading"
        @click="save"
      >
        {{ t('exampleDemo.save') }}
      </BaseButton>
      <BaseButton @click="dialogVisible = false">{{ t('dialogDemo.close') }}</BaseButton>
    </template>
  </Dialog>
</template>
