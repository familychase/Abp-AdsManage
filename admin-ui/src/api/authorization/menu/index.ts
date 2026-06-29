import request from '@/axios'
import type {
  SysMenuNode,
  GetSysMenuListInput,
  CreateUpdateSysMenuDto,
  SysMenuPagedResult
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

// ── 统一添加 token 到请求头 ──
function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/** ── 获取菜单列表（树形结构）── */
export const getMenuListApi = async (params?: GetSysMenuListInput) => {
  const res = await request.post<SysMenuPagedResult>({
    url: '/api/system/menus/list',
    data: params || { page: 1, pageSize: 999 },
    headers: authHeader()
  })
  return {
    data: {
      list: transformMenuTree(res.data?.items || [])
    }
  }
}

/** ── 获取菜单树（用于角色分配）── */
export const getMenuTreeApi = async () => {
  const res = await request.post<unknown>({
    url: '/api/system/menus/tree',
    data: { page: 0, pageSize: 0 },
    headers: authHeader()
  })
  return res.data as any[]
}

/** ── 获取菜单树形列表（用于菜单管理页面展示）── */
export const getMenuTreeListApi = async () => {
  const data = await getMenuTreeApi()
  return transformMenuTree(data)
}

/** ── 获取菜单树形数据（用于父级下拉选择 TreeSelect）── */
export const getMenuTreeForSelectApi = async (menuType?: string) => {
  const res = await request.post<unknown>({
    url: '/api/system/menus/tree',
    data: { page: 0, pageSize: 0, menuType },
    headers: authHeader()
  })
  return transformMenuTree(res.data as any[])
}

/** ── 获取单个菜单详情 ── */
export const getMenuDetailApi = (id: number) => {
  return request.get<SysMenuNode>({
    url: `/api/system/menus/${id}`,
    headers: authHeader()
  })
}

/** ── 新增菜单 ── */
export const createMenuApi = (data: CreateUpdateSysMenuDto) => {
  return request.post<IResponse>({
    url: '/api/system/menus',
    data,
    headers: authHeader()
  })
}

/** ── 更新菜单 ── */
export const updateMenuApi = (id: number, data: CreateUpdateSysMenuDto) => {
  return request.put<IResponse>({
    url: `/api/system/menus/${id}`,
    data,
    headers: authHeader()
  })
}

/** ── 删除菜单 ── */
export const deleteMenuApi = (id: number) => {
  return request.delete<IResponse>({
    url: `/api/system/menus/${id}`,
    headers: authHeader()
  })
}

// ══════════════════════════════════════════
//  后端 → 前端 数据转换
// ══════════════════════════════════════════

function transformMenuNode(node: SysMenuNode): any {
  return {
    id: node.id,
    parentId: node.parentId,
    title: node.name, // 根级 title，供 TreeSelect 的 label 使用
    nameEn: node.nameEn,
    meta: {
      title: node.name,
      icon: node.icon || node.iconName || undefined,
      hidden: !node.visible,
      activeMenu: undefined,
      alwaysShow: false,
      noCache: false,
      breadcrumb: true,
      affix: false,
      noTagsView: false,
      canTo: false
    },
    component: node.componentPath || '#',
    name: node.componentName,
    path: node.route || node.menuPath || '',
    permissionCode: node.permissionCode,
    status: 1,
    type: node.menuType,
    sort: node.sort,
    children: node.children ? transformMenuTree(node.children) : undefined
  }
}

function transformMenuTree(nodes: SysMenuNode[]): any[] {
  return (nodes || []).map(transformMenuNode)
}

// ══════════════════════════════════════════
//  前端 → 后端 数据转换
// ══════════════════════════════════════════

/** 将前端表单数据转换为后端 CreateUpdateSysMenuDto */
export function toCreateUpdateSysMenuDto(formData: any): CreateUpdateSysMenuDto {
  const dto: CreateUpdateSysMenuDto = {
    name: formData.meta?.title || '',
    nameEn: formData.meta?.nameEn || null,
    menuType: formData.type,
    parentId: formData.parentId || 0,
    route: formData.path || null,
    icon: formData.meta?.icon || null,
    iconName: null,
    menuPath: null,
    componentName: formData.name || null,
    componentPath: formData.component || null,
    permissionCode: Array.isArray(formData.permissionCode)
      ? formData.permissionCode
          .map((p: string) => (p.includes(':') ? p : `${formData.name || 'unknown'}:${p}`))
          .join(',') || null
      : formData.permissionCode || null,
    sort: formData.sort ?? 0,
    visible: formData.meta?.hidden !== true, // hidden true → visible false
    remark: null
  }
  return dto
}
