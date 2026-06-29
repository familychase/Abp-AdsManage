import request from '@/axios'
import type { UserType, LoginResponseType } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

// ─── 后端菜单节点（/api/system/user/self 返回的 menus 字段） ───
export interface BackendMenuMeta {
  hidden?: boolean
  alwaysShow?: boolean
  title?: string
  icon?: string
}

export interface BackendMenuNode {
  id: number
  parentId: number
  sort: number
  permissionCode?: string[]
  menuType: string // "DIRECTORY" | "MENU"
  children: BackendMenuNode[]
  path: string
  component: string // "#" 或 "/views/xxx/xxx.vue"
  name: string
  redirect: string
  meta: BackendMenuMeta
}

export interface SysUserInfo {
  userId: number
  userCode: string
  userName: string
  aliasName?: string
  departmentId: number
  departmentName?: string
  roleId: number
  roleName: string
  isAdmin: boolean
  isTeamAdmin: boolean
  email?: string
  phoneNumber?: string
}

export interface SelfInfoResponse {
  userInfo: SysUserInfo
  Permissions?: string[]
  menus: BackendMenuNode[]
}

// ─── 后端菜单 → 前端路由格式转换 ───
function convertMenuToRoute(node: BackendMenuNode): AppCustomRouteRecordRaw {
  // component 为 "#"（目录）时保持不变，否则去掉前导 / 和 .vue/.tsx 后缀
  let comp = node.component || '#'
  if (comp.startsWith('/')) {
    comp = comp.slice(1)
  }
  if (comp.endsWith('.vue')) {
    comp = comp.slice(0, -4)
  }
  if (comp.endsWith('.tsx')) {
    comp = comp.slice(0, -4)
  }

  return {
    path: node.path || '',
    name: node.name || `Route_${node.id}`,
    component: comp,
    redirect: node.redirect || '',
    meta: {
      title: node.meta?.title || node.name || '',
      icon: node.meta?.icon || '',
      hidden: node.meta?.hidden ?? false,
      alwaysShow: node.children && node.children.length > 0 ? true : undefined
    },
    children: node.children?.map(convertMenuToRoute) || []
  }
}

export function convertMenusToRoutes(menus: BackendMenuNode[]): AppCustomRouteRecordRaw[] {
  return menus.map(convertMenuToRoute)
}

// ─── API ───

/** 登录：POST /api/user/login */
export const loginApi = (data: UserType): Promise<IResponse<LoginResponseType>> => {
  return request.post({ url: '/api/user/login', data })
}

/** 退出登录：POST /api/user/login_out */
export const loginOutApi = (): Promise<IResponse> => {
  return request.post({ url: '/api/user/login_out' })
}

/** 获取用户列表 */
export const getUserListApi = ({ params }: AxiosConfig) => {
  return request.get<{
    code: string
    data: {
      list: UserType[]
      total: number
    }
  }>({ url: '/api/system/user/list', params })
}

/** 更新当前用户信息：PUT /api/system/user/{id}/self */
export const putSelfInfoApi = (
  id: number,
  data: { aliasName?: string; email?: string; phoneNumber?: string }
): Promise<IResponse> => {
  const userStore = useUserStoreWithOut()
  return request.put({
    url: `/api/system/user/${id}/self`,
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 获取当前用户信息及菜单路由：GET /api/system/user/self */
export const getSelfMenuApi = (): Promise<IResponse<SelfInfoResponse>> => {
  const userStore = useUserStoreWithOut()
  return request.get({
    url: '/api/system/user/self',
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 修改密码：POST /api/system/user/{id}/change_pswd */
export const changePasswordApi = (
  id: number,
  data: { oldPassword: string; newPassword: string; confirmPassword: string }
): Promise<IResponse> => {
  const userStore = useUserStoreWithOut()
  return request.post({
    url: `/api/system/user/${id}/change_pswd`,
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 获取角色路由 */
export const getTestRoleApi = (params: { roleName: string }): Promise<IResponse<string[]>> => {
  return request.get({ url: '/api/system/roles/list', params })
}
