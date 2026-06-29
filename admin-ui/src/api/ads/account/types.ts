import type { PlatformType } from '@/api/ads/channel/types'

/** 创建/更新广告账户 DTO（对齐 Swagger CreateUpdateAdsAccountDto） */
export interface CreateUpdateAdsAccountDto {
  accountNo?: string | null
  accountName: string
  accountState: number
  mediaState?: string | null
  balance?: number | null
  timezone?: string | null
  utcTimezoneOffset?: string | null
  platform: PlatformType
  ownerId: number
  ownerTeamId: number
  isManager?: boolean | null
  currency?: string | null
  isLimit?: boolean | null
  mediaDisableReason?: string | null
  mediaCreatedTime?: string | null
  accountRunningTime?: number | null
}

/** 广告账户 DTO（后端返回数据） */
export interface AdsAccountDto {
  id: number
  accountNo: string | null
  accountName: string | null
  accountState: number
  mediaState: string | null
  balance: number
  timezone: string | null
  utcTimezoneOffset: string | null
  platform: PlatformType
  ownerId: number
  ownerTeamId: number
  isManager: boolean
  currency: string | null
  isLimit: boolean
  mediaDisableReason: string | null
  mediaCreatedTime: string | null
  accountRunningTime: number
  creationTime: string | null
}

/** 分页查询广告账户列表请求参数（对齐 Swagger GetAdsAccountListInput） */
export interface GetAdsAccountListInput {
  sorting?: string | null
  page?: number | null
  pageSize?: number | null
  filterText?: string | null
  channelId?: string | null
  accountNo?: string | null
  accountName?: string | null
  platform?: PlatformType | null
  accountState?: number | null
}

/** 分页查询返回 */
export interface AdsAccountDtoPagedResult {
  items: AdsAccountDto[] | null
  totalCount: number
}
