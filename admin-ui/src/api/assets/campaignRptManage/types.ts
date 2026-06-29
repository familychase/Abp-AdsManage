/** 广告系列报表项 */
export interface CampaignRptItem {
  id: number
  accountId: number
  accountName: string
  accountNo: string
  campaignNo: string
  campaignName: string
  reportDate: string
  platform: string
  spend: number
  clicks: number
  converts: number
  impressions: number
  cpc: number
}

/** 广告系列报表分页结果 */
export interface CampaignRptPagedResult {
  items: CampaignRptItem[]
  totalCount: number
}

/** 报表查询参数 */
export interface GetCampaignRptListInput {
  page: number
  pageSize: number
  accountNo?: string
  campaignNo?: string
  campaignName?: string
}
