import request from '@/axios'
import type { MediaAccountPagedResult } from './types'

/**
 * 媒体账户分页列表：GET /api/ads/media/account/page?accountNo=xxx
 */
export const getMediaAccountPageApi = (params?: { accountNo?: string }) => {
  return request.get<MediaAccountPagedResult>({
    url: '/api/ads/media/account/page',
    params
  })
}
