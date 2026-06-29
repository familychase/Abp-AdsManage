import request from '@/axios'
import type {
  SysRoleDto,
  PagedResultDto,
  GetSysRoleListInput,
  CreateUpdateSysRoleDto
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/** 获取角色列表（分页） */
export const getRoleListApi = async (params?: GetSysRoleListInput) => {
  const res = await request.post<PagedResultDto<SysRoleDto>>({
    url: '/api/system/roles/list',
    data: params || { page: 1, pageSize: 999 },
    headers: authHeader()
  })
  // 转换后端分页格式为前端兼容格式
  const items = (res.data?.items || []).map(mapRoleDto)
  return {
    data: {
      list: items,
      total: res.data?.totalCount || 0
    }
  }
}

/** 获取单个角色详情 */
export const getRoleByIdApi = async (id: number) => {
  const res = await request.get<SysRoleDto>({
    url: `/api/system/roles/${id}`,
    headers: authHeader()
  })
  return mapRoleDto(res.data)
}

/** 新增角色 */
export const createRoleApi = (data: CreateUpdateSysRoleDto) => {
  return request.post<IResponse>({
    url: '/api/system/roles',
    data,
    headers: authHeader()
  })
}

/** 更新角色 */
export const updateRoleApi = (id: number, data: CreateUpdateSysRoleDto) => {
  return request.put<IResponse>({
    url: `/api/system/roles/${id}`,
    data,
    headers: authHeader()
  })
}

/** 删除角色 */
export const deleteRoleApi = (id: number) => {
  return request.delete<IResponse>({
    url: `/api/system/roles/${id}`,
    headers: authHeader()
  })
}

// ── 后端 DTO → 前端展示格式 ──
function mapRoleDto(dto: SysRoleDto): any {
  return {
    id: dto.id,
    roleName: dto.name,
    status: dto.status ?? 1, // 后端可能无 status，默认启用
    creationTime: dto.creationTime ?? '',
    remark: dto.remark ?? '',
    sort: dto.sort ?? 0,
    menuIds: dto.menuIds || []
  }
}
