<script setup lang="tsx">
import { reactive, ref, unref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getMenuTreeListApi,
  getMenuDetailApi,
  createMenuApi,
  updateMenuApi,
  deleteMenuApi,
  toCreateUpdateSysMenuDto
} from '@/api/authorization/menu/index.js'
import { useTable } from '@/hooks/web/useTable'
import { useI18n } from '@/hooks/web/useI18n'
import { Table, TableColumn } from '@/components/Table'
import { ElTag } from 'element-plus'
import { Icon } from '@/components/Icon'
import { Search } from '@/components/Search'
import { FormSchema } from '@/components/Form'
import { ContentWrap } from '@/components/ContentWrap'
import Write from './components/Write.vue'
import { Dialog } from '@/components/Dialog'
import { BaseButton } from '@/components/Button'

const { t } = useI18n()

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const list = await getMenuTreeListApi()
    return {
      list: list || []
    }
  }
})

const { dataList, loading } = tableState
const { getList } = tableMethods

const tableColumns = reactive<TableColumn[]>([
  {
    field: 'index',
    label: t('userDemo.index'),
    type: 'index'
  },
  {
    field: 'meta.title',
    label: t('menu.menuName'),
    slots: {
      default: (data: any) => {
        const title = data.row.meta.title
        return <>{title}</>
      }
    }
  },
  {
    field: 'meta.icon',
    label: t('menu.icon'),
    slots: {
      default: (data: any) => {
        const icon = data.row.meta.icon
        if (icon) {
          return (
            <>
              <Icon icon={icon} />
            </>
          )
        } else {
          return null
        }
      }
    }
  },
  // {
  //   field: 'meta.permission',
  //   label: t('menu.permission'),
  //   slots: {
  //     default: (data: any) => {
  //       const permission = data.row.meta.permission
  //       return permission ? <>{permission.join(', ')}</> : null
  //     }
  //   }
  // },
  {
    field: 'component',
    label: t('menu.component'),
    slots: {
      default: (data: any) => {
        const component = data.row.component
        return <>{component === '#' ? '顶级目录' : component === '##' ? '子目录' : component}</>
      }
    }
  },
  {
    field: 'path',
    label: t('menu.path')
  },
  {
    field: 'status',
    label: t('menu.status'),
    slots: {
      default: (data: any) => {
        return (
          <>
            <ElTag type={data.row.status === 0 ? 'danger' : 'success'}>
              {data.row.status === 1 ? t('userDemo.enable') : t('userDemo.disable')}
            </ElTag>
          </>
        )
      }
    }
  },
  {
    field: 'action',
    label: t('userDemo.action'),
    width: 200,
    slots: {
      default: (data: any) => {
        const row = data.row
        if (row.parentId === -1) return null
        return (
          <>
            <BaseButton size="small" type="primary" onClick={() => handleEdit(row)}>
              {t('exampleDemo.edit')}
            </BaseButton>
            <BaseButton size="small" type="danger" onClick={() => handleDelete(row)}>
              {t('exampleDemo.del')}
            </BaseButton>
          </>
        )
      }
    }
  }
])

const searchSchema = reactive<FormSchema[]>([
  {
    field: 'meta.title',
    label: t('menu.menuName'),
    component: 'Input'
  }
])

const searchParams = ref({})
const setSearchParams = (data: any) => {
  searchParams.value = data
  getList()
}

const dialogVisible = ref(false)
const dialogTitle = ref('')

const currentRow = ref()

const writeRef = ref<ComponentRef<typeof Write>>()

const saveLoading = ref(false)

const handleEdit = async (row: any) => {
  dialogTitle.value = t('exampleDemo.edit')
  try {
    const res = await getMenuDetailApi(row.id)
    const detail = res.data
    currentRow.value = {
      id: detail.id,
      parentId: detail.parentId,
      type: detail.menuType,
      meta: {
        title: detail.name,
        nameEn: detail.nameEn,
        icon: detail.icon || detail.iconName || undefined,
        hidden: !detail.visible
      },
      component: detail.componentPath || '#',
      name: detail.componentName,
      path: detail.route || detail.menuPath || '',
      permissionCode: detail.permissionCode,
      sort: detail.sort
    }
  } catch {
    currentRow.value = row
  }
  dialogVisible.value = true
}

const AddAction = () => {
  dialogTitle.value = t('exampleDemo.add')
  currentRow.value = undefined
  dialogVisible.value = true
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认删除该菜单？', '提示', {
      confirmButtonText: '确认',
      cancelButtonText: '取消',
      type: 'warning'
    })
    await deleteMenuApi(row.id)
    ElMessage.success('删除成功')
    getList()
  } catch {
    // 取消或失败不处理
  }
}

const save = async () => {
  const write = unref(writeRef)
  const formData = await write?.submit()
  if (formData) {
    saveLoading.value = true
    try {
      const dto = toCreateUpdateSysMenuDto(formData)
      if (currentRow.value?.id) {
        await updateMenuApi(currentRow.value.id, dto)
      } else {
        await createMenuApi(dto)
      }
      ElMessage.success('保存成功')
      dialogVisible.value = false
      getList()
    } catch {
      // 错误由拦截器处理
    } finally {
      saveLoading.value = false
    }
  }
}
</script>

<template>
  <ContentWrap>
    <Search :schema="searchSchema" @reset="setSearchParams" @search="setSearchParams" />
    <div class="mb-10px">
      <BaseButton type="primary" @click="AddAction">{{ t('exampleDemo.add') }}</BaseButton>
    </div>
    <Table
      :columns="tableColumns"
      default-expand-all
      node-key="id"
      :data="dataList"
      :loading="loading"
      @register="tableRegister"
    />
  </ContentWrap>

  <Dialog v-model="dialogVisible" :title="dialogTitle">
    <Write ref="writeRef" :current-row="currentRow" />

    <template #footer>
      <BaseButton type="primary" :loading="saveLoading" @click="save">
        {{ t('exampleDemo.save') }}
      </BaseButton>
      <BaseButton @click="dialogVisible = false">{{ t('dialogDemo.close') }}</BaseButton>
    </template>
  </Dialog>
</template>
