import request from '@/axios'
import type {
  CreateUpdateAdsChannelDto,
  GetAdsChannelListInput,
  AdsChannelPagedResult,
  AdsChannelDto
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/** 渠道列表（分页） */
export const getChannelListApi = (params?: GetAdsChannelListInput) => {
  return request.post<AdsChannelPagedResult>({
    url: '/api/ads/channel/list',
    data: params || { page: 1, pageSize: 999 },
    headers: authHeader()
  })
}

/** 创建渠道（授权成功后插入） */
export const createChannelApi = (data: CreateUpdateAdsChannelDto) => {
  return request.post<AdsChannelDto>({
    url: '/api/ads/channel',
    data,
    headers: authHeader()
  })
}

/** 更新渠道（触发同步） */
export const updateChannelApi = (id: number, data: CreateUpdateAdsChannelDto) => {
  return request.put<IResponse>({
    url: `/api/ads/channel/${id}`,
    data,
    headers: authHeader()
  })
}

/** 删除渠道 */
export const deleteChannelApi = (id: number) => {
  return request.delete<IResponse>({
    url: `/api/ads/channel/${id}`,
    headers: authHeader()
  })
}
