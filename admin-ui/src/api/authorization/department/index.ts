import request from '@/axios'
import type {
  SysDepartmentDto,
  SysDepartmentTreeDto,
  PagedResultDto,
  GetSysDepartmentListInput,
  CreateUpdateSysDepartmentDto
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

// 获取部门列表（后端返回分页格式）
export const getDepartmentListApi = async (params?: GetSysDepartmentListInput) => {
  const res = await request.post<PagedResultDto<SysDepartmentDto>>({
    url: '/api/system/departments/list',
    data: params || { pageSize: 20, page: 1 },
    headers: authHeader()
  })
  return res.data
}

/** 获取部门树形结构（POST，FromBody 提交） */
export const getDepartmentTreeApi = async (params?: GetSysDepartmentListInput) => {
  const res = await request.post<SysDepartmentTreeDto[]>({
    url: '/api/system/departments/tree',
    data: params || { pageSize: 1000, page: 0 },
    headers: authHeader()
  })
  return res.data
}

// 获取单个部门
export const getDepartmentByIdApi = async (id: number) => {
  const res = await request.get<SysDepartmentDto>({
    url: `/api/system/departments/${id}`,
    headers: authHeader()
  })
  return res.data
}

// 创建部门
export const createDepartmentApi = async (data: CreateUpdateSysDepartmentDto) => {
  const res = await request.post<IResponse>({
    url: '/api/system/departments',
    data,
    headers: authHeader()
  })
  return res.data
}

// 更新部门
export const updateDepartmentApi = async (id: number, data: CreateUpdateSysDepartmentDto) => {
  const res = await request.put<IResponse>({
    url: `/api/system/departments/${id}`,
    data,
    headers: authHeader()
  })
  return res.data
}

// 删除部门
export const deleteDepartmentApi = async (id: number) => {
  const res = await request.delete<IResponse>({
    url: `/api/system/departments/${id}`,
    headers: authHeader()
  })
  return res.data
}
