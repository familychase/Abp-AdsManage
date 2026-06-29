<script setup lang="tsx">
import { ContentWrap } from '@/components/ContentWrap'
import { useI18n } from '@/hooks/web/useI18n'
import { Table } from '@/components/Table'
import { ref, unref, nextTick, watch, reactive } from 'vue'
import { ElTree, ElInput, ElDivider, ElMessage, ElMessageBox, ElTag } from 'element-plus'
import { useTable } from '@/hooks/web/useTable'
import { Search } from '@/components/Search'
import Write from './components/Write.vue'
import { Dialog } from '@/components/Dialog'
import { CrudSchema, useCrudSchemas } from '@/hooks/web/useCrudSchemas'
import { BaseButton } from '@/components/Button'
// 后端 API
import { getDepartmentTreeApi } from '@/api/authorization/department/index.js'
import { getRoleListApi } from '@/api/authorization/role/index.js'
import {
  getUserListApi,
  getUserByIdApi,
  createUserApi,
  updateUserApi,
  deleteUserApi,
  resetUserPasswordApi
} from '@/api/authorization/user/index.js'
import type { SysUserDto, UserStatus } from '@/api/authorization/user/types.js'
import type { SysDepartmentTreeDto } from '@/api/authorization/department/types.js'

// 前端展示用的用户数据类型（映射后端字段）
interface UserTableItem {
  id: string
  userId: number
  username: string
  account: string
  email: string
  createTime: string
  role: string
  departmentId: number
  departmentName: string
  roleId: number
  roleName: string
  isAdmin: boolean
  isTeamAdmin: boolean
  status: string
  lastLoginTime: string
  aliasName: string
  phoneNumber: string
  creationTime: string
}

// 部门树节点类型
interface DeptTreeNode {
  id: number
  departmentName: string
  children?: DeptTreeNode[]
}

const { t } = useI18n()

// 后端用户数据转前端展示格式
const mapUserDto = (dto: SysUserDto): UserTableItem => ({
  id: String(dto.userId),
  userId: dto.userId,
  username: dto.userName || '',
  account: dto.userCode || '',
  email: dto.email || '',
  createTime: dto.creationTime || '',
  role: '',
  departmentId: dto.departmentId,
  departmentName: dto.departmentName || '',
  roleId: dto.roleId,
  roleName: dto.roleName || '',
  isAdmin: dto.isAdmin,
  isTeamAdmin: dto.isTeamAdmin,
  status: dto.status,
  creationTime: dto.creationTime || '',
  lastLoginTime: dto.lastLoginTime || '',
  aliasName: dto.aliasName || '',
  phoneNumber: dto.phoneNumber || ''
})

const { tableRegister, tableState, tableMethods } = useTable({
  fetchDataApi: async () => {
    const { pageSize, currentPage } = tableState
    const params = unref(searchParams)
    const res = await getUserListApi({
      page: unref(currentPage),
      pageSize: unref(pageSize),
      filterText: params.filterText || undefined,
      departmentId: unref(currentNodeKey) ? Number(unref(currentNodeKey)) : undefined,
      isAdmin: params.isAdmin === '1' ? true : params.isAdmin === '0' ? false : undefined,
      status: (params.status || undefined) as UserStatus | undefined
    })
    const items = res.data.items || []
    return {
      list: items.map(mapUserDto),
      total: res.data.totalCount || 0
    }
  },
  fetchDelApi: async () => {
    const idArr = unref(ids)
    let hasError = false
    for (const id of idArr) {
      try {
        await deleteUserApi(Number(id))
      } catch (error) {
        console.error(error)
        hasError = true
      }
    }
    if (hasError) {
      ElMessage.warning('部分数据删除失败')
    }
    return !hasError
  }
})
const { total, loading, dataList, pageSize, currentPage } = tableState
const { getList, delList } = tableMethods

const crudSchemas = reactive<CrudSchema[]>([
  {
    field: 'index',
    label: t('userDemo.index'),
    form: { hidden: true },
    search: { hidden: true },
    detail: { hidden: true },
    table: { type: 'index' }
  },
  // ---- 搜索条件（按渲染顺序） ----
  {
    field: 'filterText',
    label: '关键字',
    search: {
      component: 'Input',
      componentProps: {
        placeholder: '账号/姓名/手机号'
      }
    },
    form: { hidden: true },
    table: { hidden: true },
    detail: { hidden: true }
  },
  // ---- 编辑/新增弹窗字段（按排序） ----
  {
    field: 'account',
    label: '用户账号',
    search: { hidden: true },
    form: { component: 'Input' }
  },
  {
    field: 'username',
    label: '用户名',
    search: { hidden: true },
    form: { component: 'Input' }
  },
  {
    field: 'aliasName',
    label: '别名',
    search: { hidden: true },
    form: { component: 'Input' },
    table: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'email',
    label: t('userDemo.email'),
    search: { hidden: true },
    form: { component: 'Input' }
  },
  {
    field: 'phoneNumber',
    label: '手机号',
    search: { hidden: true },
    form: { component: 'Input' },
    table: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'status',
    label: '状态',
    search: {
      component: 'Select',
      componentProps: {
        options: [
          { label: '全部', value: '' },
          { label: '正常', value: 'ACTIVE' },
          { label: '禁用', value: 'DISABLED' }
        ],
        clearable: true
      }
    },
    form: {
      component: 'Select',
      componentProps: {
        options: [
          { label: '全部', value: '' },
          { label: '正常', value: 'ACTIVE' },
          { label: '禁用', value: 'DISABLED' }
        ],
        clearable: true
      }
    },
    table: {
      width: 100,
      slots: {
        default: (data: any) => {
          return (
            <ElTag type={data.row.status === 'ACTIVE' ? 'success' : 'danger'}>
              {data.row.status === 'ACTIVE' ? '正常' : '禁用'}
            </ElTag>
          )
        }
      }
    },
    detail: { hidden: true }
  },
  {
    field: 'departmentId',
    label: t('userDemo.department'),
    search: { hidden: true },
    detail: { hidden: true },
    form: {
      component: 'TreeSelect',
      componentProps: {
        nodeKey: 'id',
        props: { label: 'departmentName' },
        checkStrictly: true
      },
      optionApi: async () => {
        try {
          const res = await getDepartmentTreeApi()
          const treeData: SysDepartmentTreeDto[] = Array.isArray(res) ? res : []
          const mapNode = (node: SysDepartmentTreeDto): any => ({
            id: node.id,
            departmentName: node.deptName,
            children: node.children?.map(mapNode)
          })
          return treeData.map(mapNode)
        } catch (error) {
          console.error(error)
          ElMessage.error('获取部门列表失败')
          return []
        }
      }
    },
    table: { hidden: true }
  },
  {
    field: 'roleId',
    label: '所属角色',
    search: { hidden: true },
    form: {
      component: 'Select',
      componentProps: {
        clearable: true
      },
      optionApi: async () => {
        try {
          const res = await getRoleListApi()
          return (
            res.data?.list?.map((v: any) => ({
              label: v.roleName,
              value: v.id
            })) || []
          )
        } catch (error) {
          console.error(error)
          ElMessage.error('获取角色列表失败')
          return []
        }
      }
    },
    table: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'creationTime',
    label: '创建时间',
    search: { hidden: true },
    form: { component: 'Input', componentProps: { disabled: true } },
    table: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'lastLoginTime',
    label: '最后登录时间',
    search: { hidden: true },
    form: { component: 'Input', componentProps: { disabled: true } },
    table: { hidden: true },
    detail: { hidden: true }
  },
  {
    field: 'isAdmin',
    label: '超级管理员',
    search: {
      component: 'Select',
      componentProps: {
        options: [
          { label: '全部', value: '' },
          { label: '是', value: '1' },
          { label: '否', value: '0' }
        ],
        clearable: true
      }
    },
    form: {
      component: 'Switch',
      value: false,
      componentProps: {
        activeText: '是',
        inactiveText: '否'
      }
    },
    table: {
      width: 120,
      slots: {
        default: (data: any) => {
          return (
            <ElTag type={data.row.isAdmin ? 'warning' : 'info'}>
              {data.row.isAdmin ? '是' : '否'}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'isTeamAdmin',
    label: '团队管理员',
    search: { hidden: true },
    form: {
      component: 'Switch',
      value: false,
      componentProps: {
        activeText: '是',
        inactiveText: '否'
      }
    },
    table: {
      width: 120,
      slots: {
        default: (data: any) => {
          return (
            <ElTag type={data.row.isTeamAdmin ? 'warning' : 'info'}>
              {data.row.isTeamAdmin ? '是' : '否'}
            </ElTag>
          )
        }
      }
    }
  },
  {
    field: 'departmentName',
    label: t('userDemo.departmentName'),
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'createTime',
    label: t('userDemo.createTime'),
    search: { hidden: true },
    form: { hidden: true }
  },
  {
    field: 'action',
    label: t('userDemo.action'),
    search: { hidden: true },
    form: { hidden: true },
    detail: { hidden: true },
    table: {
      width: 260,
      slots: {
        default: (data: any) => {
          const row = data.row as UserTableItem
          return (
            <>
              <BaseButton size="small" type="primary" onClick={() => action(row, 'edit')}>
                {t('exampleDemo.edit')}
              </BaseButton>
              <BaseButton size="small" type="warning" onClick={() => resetPswd(row)}>
                重置密码
              </BaseButton>
              <BaseButton size="small" type="danger" onClick={() => delData(row)}>
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

const searchParams = ref<{
  filterText?: string
  isAdmin?: string
  status?: string
}>({})
const setSearchParams = (params: any) => {
  currentPage.value = 1
  searchParams.value = params
  getList()
}

const treeEl = ref<typeof ElTree>()

const currentNodeKey = ref('')
const departmentList = ref<DeptTreeNode[]>([])
const fetchDepartment = async () => {
  try {
    const res = await getDepartmentTreeApi()
    const treeData: SysDepartmentTreeDto[] = Array.isArray(res) ? res : []
    // 递归转换后端树节点 -> 前端树节点
    const mapNode = (node: SysDepartmentTreeDto): DeptTreeNode => ({
      id: node.id,
      departmentName: node.deptName,
      children: node.children?.map(mapNode)
    })
    departmentList.value = treeData.map(mapNode)
    currentNodeKey.value = departmentList.value[0]?.id?.toString() || ''
    await nextTick()
    unref(treeEl)?.setCurrentKey(currentNodeKey.value)
  } catch (error) {
    console.error(error)
    ElMessage.error('获取部门列表失败')
  }
}
fetchDepartment()

const currentDepartment = ref('')
watch(
  () => currentDepartment.value,
  (val) => {
    unref(treeEl)!.filter(val)
  }
)

const currentChange = (data: DeptTreeNode) => {
  // if (data.children) return
  currentNodeKey.value = String(data.id)
  currentPage.value = 1
  getList()
}

const filterNode = (value: string, data: DeptTreeNode) => {
  if (!value) return true
  return data.departmentName.includes(value)
}

const dialogVisible = ref(false)
const dialogTitle = ref('')

const currentRow = ref<UserTableItem>()
const actionType = ref('')

const AddAction = () => {
  dialogTitle.value = t('exampleDemo.add')
  currentRow.value = undefined
  dialogVisible.value = true
  actionType.value = ''
}

const delLoading = ref(false)
const ids = ref<string[]>([])

const delData = async (row: UserTableItem) => {
  ids.value = [row.id]
  delLoading.value = true
  await delList(unref(ids).length).finally(() => {
    delLoading.value = false
  })
}

const resetPswd = async (row: UserTableItem) => {
  try {
    await ElMessageBox.confirm(`确认重置用户「${row.username}」的密码？`, '重置密码', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    })
    await resetUserPasswordApi(row.userId)
    ElMessage.success('密码重置成功')
    getList()
  } catch (error: any) {
    if (error?.toString()?.includes('cancel') || error?.toString()?.includes('close')) {
      return
    }
    console.error(error)
    ElMessage.error('密码重置失败')
  }
}

const action = async (row: UserTableItem, type: string) => {
  actionType.value = type
  if (type === 'edit') {
    try {
      const res = await getUserByIdApi(row.userId)
      // res = { code, message, data: SysUserDto }
      const user = res.data
      currentRow.value = {
        ...row,
        userId: user.userId,
        account: user.userCode || '',
        username: user.userName || '',
        aliasName: user.aliasName || '',
        email: user.email || '',
        phoneNumber: user.phoneNumber || '',
        departmentId: user.departmentId,
        departmentName: user.departmentName || '',
        roleId: user.roleId,
        roleName: user.roleName || '',
        isAdmin: user.isAdmin,
        isTeamAdmin: user.isTeamAdmin,
        status: user.status,
        creationTime: user.creationTime || '',
        lastLoginTime: user.lastLoginTime || ''
      }
      dialogTitle.value = t('exampleDemo.edit')
    } catch (error) {
      console.error(error)
      ElMessage.error('获取用户信息失败')
      return
    }
  } else {
    currentRow.value = { ...row }
  }
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
      // 将前端表单数据转为后端 DTO
      const dto = {
        userCode: formData.account || formData.userCode,
        userName: formData.username || formData.userName,
        aliasName: formData.aliasName || null,
        email: formData.email || null,
        phoneNumber: formData.phoneNumber || null,
        departmentId: formData.departmentId || formData.department?.id || 0,
        roleId: formData.roleId || 0,
        isAdmin: formData.isAdmin || false,
        isTeamAdmin: formData.isTeamAdmin || false
      }
      if (actionType.value === 'edit' && currentRow.value) {
        await updateUserApi(Number(currentRow.value.id), dto)
        ElMessage.success('修改成功')
      } else {
        await createUserApi(dto)
        ElMessage.success('新增成功')
      }
      currentPage.value = 1
      getList()
      dialogVisible.value = false
    } catch (error) {
      console.error(error)
      ElMessage.error('保存失败，请稍后重试')
    } finally {
      saveLoading.value = false
    }
  }
}
</script>

<template>
  <div class="flex w-100% h-100%">
    <ContentWrap class="w-250px">
      <div class="flex justify-center items-center">
        <div class="flex-1">{{ t('userDemo.departmentList') }}</div>
        <ElInput
          v-model="currentDepartment"
          class="flex-[2]"
          :placeholder="t('userDemo.searchDepartment')"
          clearable
        />
      </div>
      <ElDivider />
      <ElTree
        ref="treeEl"
        :data="departmentList"
        default-expand-all
        :expand-on-click-node="false"
        node-key="id"
        :current-node-key="currentNodeKey"
        :props="{
          label: 'departmentName'
        }"
        :filter-node-method="filterNode"
        @current-change="currentChange"
      >
        <template #default="{ data }">
          <div
            :title="data.departmentName"
            class="whitespace-nowrap overflow-ellipsis overflow-hidden"
          >
            {{ data.departmentName }}
          </div>
        </template>
      </ElTree>
    </ContentWrap>
    <ContentWrap class="flex-[3] ml-20px">
      <Search
        :schema="allSchemas.searchSchema"
        @reset="setSearchParams"
        @search="setSearchParams"
      />

      <div class="mb-10px">
        <BaseButton type="primary" @click="AddAction">{{ t('exampleDemo.add') }}</BaseButton>
      </div>
      <Table
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :columns="allSchemas.tableColumns"
        :data="dataList"
        :loading="loading"
        @register="tableRegister"
        :pagination="{
          total
        }"
      />
    </ContentWrap>

    <Dialog v-model="dialogVisible" :title="dialogTitle">
      <Write ref="writeRef" :form-schema="allSchemas.formSchema" :current-row="currentRow" />

      <template #footer>
        <BaseButton type="primary" :loading="saveLoading" @click="save">
          {{ t('exampleDemo.save') }}
        </BaseButton>
        <BaseButton @click="dialogVisible = false">{{ t('dialogDemo.close') }}</BaseButton>
      </template>
    </Dialog>
  </div>
</template>
