import request from '@/axios'
import type { GetPixelListInput, PixelPagedResult } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/** 像素列表（分页） */
export const getPixelListApi = (data: GetPixelListInput) => {
  return request.post<PixelPagedResult>({
    url: '/api/ads/pixel/list',
    data,
    headers: authHeader()
  })
}
