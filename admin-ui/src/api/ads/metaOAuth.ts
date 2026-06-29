import request from '@/axios'
import type { MetaOAuthCallbackInput, MetaOAuthCallbackOutput } from './types'

/** Meta OAuth 授权回调：POST /api/ads/meta_oauth/callback */
export const metaOAuthCallbackApi = (data: MetaOAuthCallbackInput) => {
  return request.post<MetaOAuthCallbackOutput>({
    url: '/api/ads/meta_oauth/callback',
    data
  })
}
