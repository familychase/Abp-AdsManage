// SysMenuType: DIRECTORY=目录, MENU=菜单, BUTTON=按钮
export type SysMenuType = 'DIRECTORY' | 'MENU' | 'BUTTON'

// 菜单树节点（来自后端 /api/system/menus/list 响应）
export interface SysMenuNode {
  id: number
  parentId: number
  name: string
  nameEn?: string | null
  route?: string | null
  icon?: string | null
  iconName?: string | null
  menuType: SysMenuType
  permissionCode?: string | null
  componentName?: string | null
  componentPath?: string | null
  menuPath?: string | null
  sort: number
  visible: boolean
  remark?: string | null
  children?: SysMenuNode[]
}

// 菜单列表查询参数
export interface GetSysMenuListInput {
  page?: number | null
  pageSize?: number | null
  sorting?: string | null
  filterText?: string | null
  menuType?: SysMenuType | null
  parentId?: number | null
}

// 创建 / 更新菜单 DTO
export interface CreateUpdateSysMenuDto {
  parentId?: number | null
  name: string
  nameEn?: string | null
  route?: string | null
  icon?: string | null
  iconName?: string | null
  menuType: SysMenuType
  permissionCode?: string | null
  componentName?: string | null
  componentPath?: string | null
  menuPath?: string | null
  sort?: number
  visible?: boolean
  remark?: string | null
}

// 分页结果
export interface SysMenuPagedResult {
  items: SysMenuNode[]
  totalCount: number
}
