/**
 * 平台类型
 *
 * NONE   = 0  无
 * GOOGLE = 1  Google Ads
 * META   = 2  Meta (Facebook/Instagram)
 * TIKTOK = 3  TikTok
 */
export type PlatformType = 0 | 1 | 2 | 3

/** 渠道状态 */
export type ChannelStateType = 0 | 1 | 2

/** 审核类型 */
export type AuditType = 1 | 2 | 3

/** 创建 / 更新 渠道 DTO */
export interface CreateUpdateAdsChannelDto {
  channelName: string
  platform: PlatformType
  channelState: ChannelStateType
  auditType: AuditType
  managerId?: string | null
  isManager: boolean
  appKey?: string | null
  appSecret?: string | null
  accessToken?: string | null
  refreshToken?: string | null
  expired?: string | null
  mediaUserId?: string | null
}

/** 渠道 DTO（列表项 + 详情） */
export interface AdsChannelDto extends CreateUpdateAdsChannelDto {
  id: number
}

/** 渠道列表查询参数 */
export interface GetAdsChannelListInput {
  sorting?: string | null
  page?: number
  pageSize?: number
  filterText?: string | null
  platform?: PlatformType | null
  channelState?: ChannelStateType | null
}

/** 渠道列表分页结果 */
export interface AdsChannelPagedResult {
  items: AdsChannelDto[]
  totalCount: number
}
