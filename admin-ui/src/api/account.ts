import request from '@/axios'

export interface AuthSaveParams {
  platform: string
  accessToken: string
  userId: string
  userName: string
  userEmail: string
  avatar?: string
  expiresIn: number
  channelId?: string | number
}

/** 保存授权信息 */
export const saveAuthApi = (data: AuthSaveParams): Promise<IResponse> => {
  return request.post({ url: '/api/account/save-auth', data })
}

/** 获取授权用户列表 */
export const getAuthListApi = (
  params?: Record<string, any>
): Promise<
  IResponse<{
    list: any[]
    total: number
  }>
> => {
  return request.get({ url: '/api/account/auth-list', params })
}
