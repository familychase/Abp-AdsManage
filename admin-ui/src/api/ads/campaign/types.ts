/** 广告系列列表项（来自 /api/ads/campaign/list） */
export interface CampaignListItem {
  campaignId: number
  campaignNo: string
  campaignName: string
  mediaState: string
  budgetType: string | null
  budget: number
  objective: string | null
  accountNo: string
  mediaCreateTime: string | null
  creationTime: string | null
}

/** 广告系列列表查询参数 */
export interface GetCampaignListInput {
  page: number
  pageSize: number
  accountIds?: number[]
  accountId?: number
  accountNo?: string
  campaignNo?: string
  campaignName?: string
}

/** 广告系列列表响应 */
export interface CampaignListResponse {
  totalCount: number
  items: CampaignListItem[]
}

/** 广告系列详情查询参数 */
export interface CampaignDetailInput {
  accountNo: string
  campaignNo: string
}

/** 广告系列详情响应 */
export interface CampaignDetailResponse {
  campaignNo: string
  campaignName: string
  status: string
  dailyBudget: string
  lifetimeBudget: string
  objective: string
  startTime: string
  endTime: string
  createdTime: string
  message?: string | null
}

/** POST /api/ads/media/campaign/batch_delete 请求参数 */
export interface CampaignBatchDeleteInput {
  campaignNos: string[]
}

/** 批量删除单条结果 */
export interface CampaignBatchDeleteItem {
  campaignNo: string
  success: boolean
  errorMessage?: string | null
}

/** 批量删除响应 */
export interface CampaignBatchDeleteResult {
  items: CampaignBatchDeleteItem[]
}
