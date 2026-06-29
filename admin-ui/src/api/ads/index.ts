import request from '@/axios'
import type {
  DuplicateSubmitInput,
  AdSetSubmitInput,
  GetDuplicateLogListInput,
  DuplicateLogDto,
  DuplicateLogPagedResult
} from './types'

/**
 * 广告系列复制发布：POST /api/ads/duplicate/campaign/internal
 */
export const duplicateCampaignInternal = (data: DuplicateSubmitInput) => {
  return request.post<IResponse<unknown>>({
    url: '/api/ads/duplicate/campaign/internal',
    data
  })
}

/**
 * 广告组复制发布：POST /api/ads/duplicate/adset/internal
 */
export const duplicateAdSetInternal = (data: AdSetSubmitInput) => {
  return request.post<IResponse<unknown>>({
    url: '/api/ads/duplicate/adset/internal',
    data
  })
}

/**
 * @deprecated 请使用 duplicateCampaignInternal
 */
export const duplicateInternal = (data: DuplicateSubmitInput) => {
  return duplicateCampaignInternal(data)
}

/**
 * 复制发布记录列表：GET /api/ads/duplicate/logging/list
 */
export const getDuplicateLogListApi = (params: GetDuplicateLogListInput) => {
  return request.get<DuplicateLogPagedResult>({
    url: '/api/ads/duplicate/logging/list',
    params
  })
}

/**
 * 广告系列复制发布记录：GET /api/ads/duplicate/campaign/logging/list
 */
export const getCampaignDuplicateLogListApi = (params: GetDuplicateLogListInput) => {
  return request.get<DuplicateLogPagedResult>({
    url: '/api/ads/duplicate/campaign/logging/list',
    params
  })
}

/**
 * 广告组复制发布记录：GET /api/ads/duplicate/adset/logging/list
 */
export const getAdSetDuplicateLogListApi = (params: GetDuplicateLogListInput) => {
  return request.get<DuplicateLogPagedResult>({
    url: '/api/ads/duplicate/adset/logging/list',
    params
  })
}

/**
 * 复制发布记录详情：GET /api/ads/duplicate/detail/{logId}
 */
export const getDuplicateDetailApi = (logId: number) => {
  return request.get<DuplicateLogDto>({
    url: `/api/ads/duplicate/detail/${logId}`
  })
}

/**
 * 删除复制发布记录：DELETE /api/ads/duplicate/{id}
 */
export const deleteDuplicateApi = (id: number) => {
  return request.delete<IResponse<unknown>>({
    url: `/api/ads/duplicate/${id}`
  })
}

/**
 * 批量删除广告系列：POST /api/ads/media/campaign/batch_delete
 * 返回 per-item 结果：{ campaignNo, success, errorMessage }
 */
export const batchDeleteCampaignsApi = (data: { accountNo: string; campaignNos: string[] }) => {
  return request.post<IResponse<BatchDeleteResultItem[]>>({
    url: '/api/ads/media/campaign/batch_delete',
    data
  })
}

/**
 * 批量删除广告组：POST /api/ads/media/adset/batch_delete
 * 返回 per-item 结果：{ adSetNo, success, errorMessage }
 */
export const batchDeleteAdSetsApi = (data: { accountNo: string; adSetNos: string[] }) => {
  return request.post<IResponse<BatchDeleteResultItem[]>>({
    url: '/api/ads/media/adset/batch_delete',
    data
  })
}

/** 批量删除 - 单条结果 */
export interface BatchDeleteResultItem {
  campaignNo: string
  success: boolean
  errorMessage?: string | null
}
