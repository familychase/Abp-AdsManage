import request from '@/axios'
import { useUserStoreWithOut } from '@/store/modules/user'

function authHeader() {
  const userStore = useUserStoreWithOut()
  return { access_token: userStore.getToken }
}

/**
 * 触发广告结构同步（广告系列/广告组/广告）
 * 1. 创建同步调度记录
 * 2. 立即推送执行
 */
export const triggerAdStructureSyncApi = async (data: { accountNo: string; platform: number }) => {
  // 1. 创建同步调度
  const createRes: any = await request.post({
    url: '/api/system/sync_schedule',
    data: {
      actionType: 1, // MANUAL
      jobName: 'SyncAdAccountStructureIntegrationJob',
      platform: data.platform,
      resourceId: data.accountNo,
      resourceType: 2, // AD_ACCOUNT
      nextPublishTime: new Date().toISOString()
    },
    headers: authHeader()
  })

  const scheduleId = createRes.data?.id
  if (!scheduleId) {
    throw new Error('创建同步调度失败')
  }

  // 2. 推送执行
  return request.post<IResponse>({
    url: `/api/system/sync_schedule/${scheduleId}/push`,
    headers: authHeader()
  })
}
