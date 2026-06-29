import request from '@/axios'
import type {
  CreateAdsSyncScheduleInput,
  GetAdsSyncScheduleListInput,
  AdsSyncScheduleItem
} from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/** 创建同步调度：POST /api/system/sync_schedule */
export const createSyncScheduleApi = (data: CreateAdsSyncScheduleInput) => {
  return request.post<IResponse>({
    url: '/api/system/sync_schedule',
    data,
    headers: authHeader()
  })
}

export interface SyncScheduleListResponse {
  items: AdsSyncScheduleItem[]
  totalCount: number
}

/** 获取同步调度列表：POST /api/system/sync_schedule/list */
export const getSyncScheduleListApi = async (params: GetAdsSyncScheduleListInput) => {
  const res: IResponse<SyncScheduleListResponse> = await request.post({
    url: '/api/system/sync_schedule/list',
    data: params,
    headers: authHeader()
  })
  return res
}

/** 推送执行同步调度：POST /api/system/sync_schedule/{id}/push */
export const pushSyncScheduleApi = (id: number) => {
  return request.post<IResponse>({
    url: `/api/system/sync_schedule/${id}/push`,
    headers: authHeader()
  })
}
