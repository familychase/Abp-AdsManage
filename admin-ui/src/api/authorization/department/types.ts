// 通用分页结果
export interface PagedResultDto<T> {
  items: T[]
  totalCount: number
}

// 部门 DTO（对应后端 SysDepartmentDto）
export interface SysDepartmentDto {
  id: number
  deptName: string
  aliasName: string | null
  parentId: number
  parentName: string | null
  sort: number
  creationTime: string
  remark: string | null
}

/** 部门树节点（对应后端 /api/system/departments/tree 返回值） */
export interface SysDepartmentTreeDto {
  id: number
  deptName: string
  parentId: number
  children?: SysDepartmentTreeDto[]
}

// 部门列表查询参数（page 从 0 开始）
export interface GetSysDepartmentListInput {
  pageSize: number
  page: number
  filterText?: string | null
  parentId?: number | null
}

// 创建/更新部门 DTO（后端定义，无 status 字段）
export interface CreateUpdateSysDepartmentDto {
  parentId?: number
  deptName: string
  aliasName?: string | null
  sort?: number
  path?: string | null
  remark?: string | null
}
