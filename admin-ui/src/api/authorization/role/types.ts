// 角色 DTO（对应后端 SysRolesDto）
export interface SysRoleDto {
  id: number
  name: string
  sort: number
  remark: string | null
  // 后端可能返回更多字段
  status?: number
  creationTime: string
  menuIds?: number[]
}

// 分页结果
export interface PagedResultDto<T> {
  items: T[]
  totalCount: number
}

// 角色列表查询参数
export interface GetSysRoleListInput {
  page?: number | null
  pageSize?: number | null
  sorting?: string | null
  filterText?: string | null
}

// 创建/更新角色 DTO（后端定义）
export interface CreateUpdateSysRoleDto {
  name: string
  sort?: number
  remark?: string | null
}
