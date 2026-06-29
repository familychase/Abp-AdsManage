/** 广告系列复制发布请求参数 */
export interface DuplicateSubmitInput {
  accountNo: string
  campaignNo: string
  pageNo: string
  copyNumber: number
  channelId?: number
}

/** 广告组复制发布请求参数 */
export interface AdSetSubmitInput {
  channelId: number
  accountNo: string
  pageNo: string
  copyNumber: number
  adSetNo: string
}
/** POST /api/ads/meta_oauth/callback 请求参数 */
export interface MetaOAuthCallbackInput {
  code: string
  appId: string
  redirectUri: string
  channelName?: string | null
  email?: string | null
  isManager: boolean
}

/** POST /api/ads/meta_oauth/callback 返回结果 */
export interface MetaOAuthCallbackOutput {
  channelId: number
  channelName: string
  mediaUserId: string
  platform: number
}

/** POST /api/system/sync_schedule 请求参数 */
export interface CreateAdsSyncScheduleInput {
  actionType: number
  jobName: string
  platform: number
  resourceId: string
  resourceType: string
  nextPublishTime: string
}

/** POST /api/system/sync_schedule/list 请求参数 */
export interface GetAdsSyncScheduleListInput {
  page?: number
  pageSize?: number
  filterText?: string
  resourceId?: string
  resourceType?: string
  platform?: number
  actionType?: number
}

/** 复制来源枚举 */
export type DuplicateSource = 'ADS_MANAGEMENT' | 'STRATEGY' | 'MINI_PROGRAM'

/** 复制状态枚举 */
export type DuplicateState = 'PENDING' | 'SUCCESS' | 'FAILED' | 'IN_PROGRESS'

/** GET /api/ads/duplicate/logging/list 查询参数 */
export interface GetDuplicateLogListInput {
  DuplicateSource?: DuplicateSource
  State?: DuplicateState
  AdObjectNo?: string
  AccountNo?: string
  DuplicateAccountNo?: string
  Page?: number
  PageSize?: number
  SkipCount?: number
  MaxResultCount?: number
  Sorting?: string
}

/** 复制发布记录（列表项） */
export interface DuplicateLogDto {
  id: number
  logId: number
  duplicateSource?: DuplicateSource
  state?: DuplicateState
  adObjectNo?: string
  accountNo?: string
  duplicateAccountNo?: string
  copyNumber?: number
  channelId?: number
  channelName?: string
  campaignNo?: string
  pageNo?: string
  duplicateContent?: string
  errorMessage?: string
  message?: string
  creationTime?: string
  /** 计划时间 */
  scheduleTime?: string
  /** 结束时间 */
  endTime?: string
}

/** 复制发布记录分页结果 */
export interface DuplicateLogPagedResult {
  items: DuplicateLogDto[]
  totalCount: number
}

/** 同步调度记录（列表项） */
export interface AdsSyncScheduleItem {
  id: number
  actionType: number
  resourceId: string
  resourceType: string
  platform: number
  jobName: string
  extendingData?: string
  level?: number
  isAudience?: boolean
  linkDate?: string
  nextPublishTime?: string
}
