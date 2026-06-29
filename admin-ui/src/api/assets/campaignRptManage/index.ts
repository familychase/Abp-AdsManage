import request from '@/axios'
import { useUserStoreWithOut } from '@/store/modules/user'
import type { CampaignRptPagedResult, GetCampaignRptListInput } from './types'

/**
 * 查询广告系列报表
 */
export const getCampaignRptListApi = (data: GetCampaignRptListInput) => {
  const userStore = useUserStoreWithOut()
  return request.post<CampaignRptPagedResult>({
    url: '/api/ads/reports/campaign/list',
    data,
    headers: {
      access_token: userStore.getToken
    }
  })
}
