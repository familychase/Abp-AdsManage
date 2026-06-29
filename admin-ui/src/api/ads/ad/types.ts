/** 广告列表项 */
export interface AdListItem {
  adId: number
  adNo: string
  adName: string
  mediaState: string
  creativeNo: string | null
  pageNo: string
  campaignNo: string
  adSetNo: string
  accountNo: string
  mediaCreateTime: string | null
  creationTime: string | null
}

/** 广告列表查询参数 */
export interface GetAdListInput {
  page: number
  pageSize: number
  accountIds?: number[]
  campaignIds?: string[]
  adSetIds?: string[]
  accountId?: number
  accountNo?: string
  campaignNo?: string
  adSetNo?: string
  adNo?: string
  adName?: string
}

/** 广告列表响应 */
export interface AdListResponse {
  totalCount: number
  items: AdListItem[]
}
