// 用户状态：'ACTIVE'=正常, 'DISABLED'=禁用
export type UserStatus = 'ACTIVE' | 'DISABLED'

// 用户 DTO（来自后端 SysUserDto）
export interface SysUserDto {
  userId: number
  userCode: string | null
  userName: string | null
  aliasName: string | null
  departmentId: number
  departmentName: string | null
  roleId: number
  roleName: string | null
  isAdmin: boolean
  isTeamAdmin: boolean
  email: string | null
  phoneNumber: string | null
  lastLoginTime: string | null
  creationTime: string | null
  status: UserStatus
}
// 分页结果
export interface SysUserDtoPagedResult {
  items: SysUserDto[] | null
  totalCount: number
}

// 用户列表查询参数
export interface GetSysUserListInput {
  page?: number | null
  pageSize?: number | null
  sorting?: string | null
  filterText?: string | null
  departmentId?: number | null
  isAdmin?: boolean | null
  status?: UserStatus | null
}

// 创建/更新用户 DTO
export interface CreateUpdateSysUserDto {
  userCode: string
  userName: string
  aliasName?: string | null
  email?: string | null
  phoneNumber?: string | null
  departmentId: number
  roleId: number
  isAdmin: boolean
  isTeamAdmin: boolean
}
