import request from '@/axios'
import { useUserStoreWithOut } from '@/store/modules/user'
import type {
  AdsAccountDto,
  AdsAccountDtoPagedResult,
  GetAdsAccountListInput,
  CreateUpdateAdsAccountDto
} from './types'

/** 分页查询广告账户列表：POST /api/ads/account/list */
export const getAccountListApi = (data: GetAdsAccountListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<AdsAccountDtoPagedResult>({
    url: '/api/ads/account/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 获取单个广告账户：GET /api/ads/account/{id} */
export const getAccountByIdApi = (id: number) => {
  const userStore = useUserStoreWithOut()
  return request.get<AdsAccountDto>({
    url: `/api/ads/account/${id}`,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 创建广告账户：POST /api/ads/account */
export const createAccountApi = (data: CreateUpdateAdsAccountDto) => {
  const userStore = useUserStoreWithOut()
  return request.post<AdsAccountDto>({
    url: '/api/ads/account',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 更新广告账户：PUT /api/ads/account/{id} */
export const updateAccountApi = (id: number, data: CreateUpdateAdsAccountDto) => {
  const userStore = useUserStoreWithOut()
  return request.put<AdsAccountDto>({
    url: `/api/ads/account/${id}`,
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/** 删除广告账户：DELETE /api/ads/account/{id} */
export const deleteAccountApi = (id: number) => {
  const userStore = useUserStoreWithOut()
  return request.delete({
    url: `/api/ads/account/${id}`,
    headers: {
      access_token: userStore.getToken
    }
  })
}
