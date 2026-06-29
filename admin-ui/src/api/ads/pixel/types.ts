/** 分页请求参数 */
export interface GetPixelListInput {
  sorting?: string | null
  page?: number
  pageSize?: number
  filterText?: string | null
  accountNo?: string | null
}

/** 像素列表项（匹配后端 PixelDto.cs） */
export interface PixelDto {
  /** 像素 ID */
  id: number
  /** Meta 像素编号 */
  pixelNo: string
  /** 像素名称 */
  pixelName: string
  /** 像素追踪代码（JS） */
  code?: string | null
  /** 关联账户数量 */
  accountCount: number
  /** 关联的账户编号列表 */
  associatedAccounts: string[]
  /** 最后同步时间 */
  lastSyncTime?: string | null
  /** 创建时间 */
  creationTime: string
}

/** 分页结果 */
export interface PixelPagedResult {
  items: PixelDto[]
  totalCount: number
}
