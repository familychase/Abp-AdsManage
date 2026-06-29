/** GET /api/ads/media/account/page 返回的媒体账户项 */
export interface MediaAccountDto {
  /** 页面编号（后端字段名保持与旧 PageItem 一致） */
  pageNo: string
  /** 页面名称 */
  pageName: string
  /** 账户编号 */
  accountNo: string
  /** 平台 */
  platform: number
  /** 分类 */
  category?: string
  /** 最后同步时间 */
  lastSyncTime?: string
  /** 创建时间 */
  creationTime?: string
}

/** GET /api/ads/media/account/page 分页结果 */
export interface MediaAccountPagedResult {
  items: MediaAccountDto[]
  totalCount: number
}
