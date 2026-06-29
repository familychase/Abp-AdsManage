import request from '@/axios'
import type {
  CampaignDetailInput,
  CampaignDetailResponse,
  GetCampaignListInput,
  CampaignListResponse,
  CampaignBatchDeleteInput,
  CampaignBatchDeleteResult
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

/**
 * 获取广告系列列表
 */
export const getCampaignListApi = (data: GetCampaignListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<CampaignListResponse>({
    url: '/api/ads/campaign/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}

/**
 * 获取广告系列详情
 */
export const getCampaignDetailApi = (data: CampaignDetailInput) => {
  return request.get<CampaignDetailResponse>({
    url: '/api/ads/media/campaign/detail',
    params: data
  })
}

/**
 * 批量删除广告系列：POST /api/ads/media/campaign/batch_delete
 */
export const campaignBatchDeleteApi = (data: CampaignBatchDeleteInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<CampaignBatchDeleteResult>({
    url: '/api/ads/media/campaign/batch_delete',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}
