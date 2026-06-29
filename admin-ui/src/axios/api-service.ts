import axios from 'axios'
import type { AxiosInstance } from 'axios'
import { ElMessage } from 'element-plus'
import { useUserStoreWithOut } from '@/store/modules/user'
import { useLocaleStoreWithOut } from '@/store/modules/locale'

const REQUEST_TIMEOUT = 60000
// 后端成功状态码
const BACKEND_SUCCESS_CODE = 0

// 从环境变量读取后端 API 地址，开发环境通过 Vite 代理则留空
const API_BASE_URL = import.meta.env.VITE_API_BASE_PATH || ''

const localeToAcceptLanguage: Record<string, string> = {
  'zh-CN': 'zh-Hans',
  en: 'en'
}

const apiService: AxiosInstance = axios.create({
  timeout: REQUEST_TIMEOUT,
  baseURL: API_BASE_URL
})

apiService.interceptors.request.use((config) => {
  const userStore = useUserStoreWithOut()
  const localeStore = useLocaleStoreWithOut()
  const lang = localeStore.getCurrentLocale?.lang || 'zh-CN'

  const token = userStore.getToken
  if (token) {
    config.headers['access_token'] = token
  }
  config.headers['Content-Type'] = 'application/json'
  config.headers['Accept-Language'] = localeToAcceptLanguage[lang] || lang
  return config
})

apiService.interceptors.response.use(
  (res) => {
    const body = res.data
    // 后端统一包装格式: { code: 0, message: "success", data: ... }
    if (body && typeof body === 'object' && 'code' in body) {
      if (body.code === BACKEND_SUCCESS_CODE) {
        return body.data as any
      }
      ElMessage.error(body.message || '请求失败')
      if (body.code === 401) {
        const userStore = useUserStoreWithOut()
        userStore.logout()
      }
      return Promise.reject(new Error(body.message || '请求失败'))
    }
    // 非包装格式直接返回
    return body
  },
  (error) => {
    if (error.response) {
      const status = error.response.status
      const message = error.response.data?.message || error.response.data?.title || error.message
      ElMessage.error(`请求失败(${status}): ${message}`)
      if (status === 401) {
        const userStore = useUserStoreWithOut()
        userStore.logout()
      }
    } else {
      ElMessage.error(error.message)
    }
    return Promise.reject(error)
  }
)

export default apiService
