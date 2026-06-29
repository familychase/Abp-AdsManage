/** POST /api/system/log_error/list 请求参数 */
export interface GetSysLogErrorListInput {
  /** 排序字段，如 "CreationTime DESC" */
  sorting?: string | null
  /** 当前页码，从 1 开始 */
  page: number
  /** 每页条数 */
  pageSize: number
  /** 日志级别（如 Error / Warning / Info / Debug） */
  level?: string | null
  /** 日志器名称 */
  logger?: string | null
  /** 请求路径（模糊匹配） */
  requestPath?: string | null
  /** 关键字（模糊匹配消息/异常） */
  keyword?: string | null
}

/** POST /api/system/log_error/list 返回单条错误日志 */
export interface SysLogErrorDto {
  id?: number
  /** 日志级别 */
  level?: string | null
  /** 日志器（Logger 名称） */
  logger?: string | null
  /** 日志消息 */
  message?: string | null
  /** 请求路径 */
  requestPath?: string | null
  /** HTTP 方法 */
  method?: string | null
  /** 状态码 */
  statusCode?: number | null
  /** 用户编号 */
  userId?: number | null
  /** 用户名 */
  userName?: string | null
  /** 客户端 IP */
  clientIp?: string | null
  /** 异常类型 */
  exceptionType?: string | null
  /** 异常消息 */
  exceptionMessage?: string | null
  /** 异常堆栈 */
  exceptionStackTrace?: string | null
  /** 浏览器/UserAgent */
  userAgent?: string | null
  /** 创建时间 */
  creationTime?: string | null
}

/** 分页结果 */
export interface SysLogErrorPagedResult {
  items: SysLogErrorDto[]
  totalCount: number
}
