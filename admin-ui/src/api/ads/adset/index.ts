import request from '@/axios'
import type { AdSetListItem, GetAdSetListInput } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

/**
 * 获取广告组列表：POST /api/ads/adset/list
 */
export const getAdSetListApi = (data: GetAdSetListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<{ items: AdSetListItem[]; totalCount: number }>({
    url: '/api/ads/adset/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}
