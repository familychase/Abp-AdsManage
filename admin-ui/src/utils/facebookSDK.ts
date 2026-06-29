declare global {
  interface Window {
    FB: any
    fbAsyncInit: () => void
    __fbInitDone?: boolean
  }
}

export interface FBLoginResponse {
  accessToken: string
  expiresIn: number
  reauthorize_required_in?: number
  signedRequest: string
  userID: string
  grantedScopes?: string
}

export interface FBUserInfo {
  id: string
  name: string
  email: string
  picture?: {
    data: {
      url: string
      width: number
      height: number
    }
  }
}

export interface FBLoginStatusResponse {
  status: 'connected' | 'not_authorized' | 'unknown'
  authResponse: FBLoginResponse | null
}

export class FacebookAuthError extends Error {
  code: string
  constructor(code: string, message: string) {
    super(message)
    this.code = code
    this.name = 'FacebookAuthError'
  }
}

let isSDKLoaded = false

const onSDKReady = (): Promise<void> => {
  return new Promise((resolve) => {
    if (isSDKLoaded) {
      resolve()
      return
    }
    // SDK 可能已经在 fbAsyncInit 中初始化完毕，
    // addEventListener 加在事件触发之后将永久不 resolve
    // 注意：仅 window.FB 存在不代表 FB.init() 已完成，
    // 必须确认 __fbInitDone 标记为真
    if (window.__fbInitDone) {
      isSDKLoaded = true
      resolve()
      return
    }
    window.addEventListener(
      'fb-sdk-ready',
      () => {
        isSDKLoaded = true
        resolve()
      },
      { once: true }
    )
  })
}

const waitForSDK = async (): Promise<void> => {
  if (isSDKLoaded && window.__fbInitDone) {
    return
  }
  // 轮询等待 SDK 完全可用
  let retries = 0
  const maxRetries = 50
  while (retries < maxRetries) {
    await onSDKReady()
    if (window.__fbInitDone) {
      return
    }
    retries++
    await new Promise((r) => setTimeout(r, 200))
  }
  throw new Error('Facebook SDK 加载超时')
}

/**
 * 调起 Facebook 登录授权弹窗
 * @param scope 权限范围，默认 public_profile,email
 */
export const loginWithFacebook = (scope = 'public_profile,email'): Promise<FBLoginResponse> => {
  return new Promise((resolve, reject) => {
    waitForSDK()
      .then(() => {
        const loginOptions: Record<string, any> = {
          scope,
          return_scopes: true,
          auth_type: 'rerequest' // 强制重新授权，避免缓存 session 静默返回
        }
        window.FB.login((response: any) => {
          if (response.authResponse) {
            resolve(response.authResponse as FBLoginResponse)
            return
          }
          // 细化 status 判断
          if (response.status === 'not_authorized') {
            reject(new FacebookAuthError('NOT_AUTHORIZED', '用户未授权应用'))
            return
          }
          if (response.status === 'unknown') {
            reject(
              new FacebookAuthError('UNKNOWN_STATUS', '无法确定授权状态，用户可能未登录 Facebook')
            )
            return
          }
          // 没有 authResponse 也没有明确 status → 用户取消
          reject(new FacebookAuthError('USER_CANCELLED', '用户取消了授权'))
        }, loginOptions)
      })
      .catch((err) => {
        if (err?.message?.includes('超时')) {
          reject(new FacebookAuthError('SDK_TIMEOUT', 'Facebook SDK 加载超时'))
        } else {
          reject(new FacebookAuthError('SDK_LOAD_FAILED', err?.message || 'Facebook SDK 加载失败'))
        }
      })
  })
}

/**
 * 获取 Facebook 用户信息
 */
export const getFacebookUserInfo = (fields = 'id,name,email,picture'): Promise<FBUserInfo> => {
  return new Promise((resolve, reject) => {
    waitForSDK()
      .then(() => {
        window.FB.api('/me', { fields }, (response: any) => {
          if (response && !response.error) {
            resolve(response as FBUserInfo)
          } else {
            reject(new Error(response?.error?.message || '获取用户信息失败'))
          }
        })
      })
      .catch(reject)
  })
}

/**
 * 检查 Facebook 登录状态
 */
export const getFacebookLoginStatus = (): Promise<FBLoginStatusResponse> => {
  return new Promise((resolve, reject) => {
    waitForSDK()
      .then(() => {
        window.FB.getLoginStatus((response: any) => {
          resolve(response as FBLoginStatusResponse)
        })
      })
      .catch(reject)
  })
}

/**
 * Facebook 登出
 */
export const logoutFacebook = (): Promise<void> => {
  return new Promise((resolve) => {
    waitForSDK()
      .then(() => {
        window.FB.logout(() => {
          resolve()
        })
      })
      .catch(() => {
        resolve()
      })
  })
}
