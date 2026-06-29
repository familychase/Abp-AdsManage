import type { PlatformType } from '@/api/ads/channel/types'

/** 获取广告组列表请求参数 */
export interface GetAdSetListInput {
  sorting?: string | null
  page?: number
  pageSize?: number
  accountIds?: number[] | null
  campaignIds?: number[] | null
  accountId?: number | null
  accountNo?: string | null
  campaignNo?: string | null
  adSetNo?: string | null
  platform?: PlatformType
  adSetName?: string | null
}

/** 广告组列表项 */
export interface AdSetListItem {
  adSetNo: string
  adSetName: string
  status?: string
  campaignNo?: string
  campaignName?: string
  accountNo?: string
  accountName?: string
  platform?: PlatformType
  dailyBudget?: number
  lifetimeBudget?: number
  startTime?: string
  endTime?: string
  createdTime?: string
  updatedTime?: string
}
