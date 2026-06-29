import request from '@/axios'
import type { GetAdListInput, AdListResponse } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

export const getAdListApi = (data: GetAdListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<AdListResponse>({
    url: '/api/ads/ad/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}
