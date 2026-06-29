import request from '@/axios'
import type { PageListParams, PageListData } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

/**
 * 获取主页列表
 */
export const getPageListApi = (data: PageListParams) => {
  const userStore = useUserStoreWithOut()
  return request.post<PageListData>({
    url: '/api/ads/assets/page/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}
