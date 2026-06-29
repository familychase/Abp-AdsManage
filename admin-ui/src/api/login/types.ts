/** 登录请求参数 */
export interface UserLoginType {
  userCode: string
  password: string
}

/** 登录成功后返回的用户信息（含 token） */
export interface LoginResponseType {
  token: string
  userCode: string
  username: string
  role: string
  roleId: string
  permissions?: string | string[]
}

/** 用户信息（与后端 /api/user/login 返回的 data 字段对齐，password 可选因为存储时不保留密码） */
export interface UserType {
  userCode: string
  username: string
  password?: string
  role: string
  roleId: string
  token?: string
  permissions?: string | string[]
}
