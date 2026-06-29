/** 分页请求参数 */
export interface PageListParams {
  page: number
  pageSize: number
  filter?: string
  channelId?: number
}

/** 列表项 */
export interface PageItem {
  pageNo: string
  pageName: string
  category: string
  accountNo: string
  platform: number
  lastSyncTime: string
  creationTime: string
}

/** 列表响应中的 data 字段 */
export interface PageListData {
  items: PageItem[]
  totalCount: number
}

/** 完整响应 */
export interface PageListResponse {
  code: number
  message: string
  data: PageListData
}
