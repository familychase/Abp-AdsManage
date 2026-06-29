import request from '@/axios'
import type { CountryListInput, CountryItem, LanguageListInput, LanguageItem } from './types'
import { useUserStoreWithOut } from '@/store/modules/user'

/**
 * 获取国家/地区列表
 */
export const getCountryListApi = (params?: CountryListInput) => {
  const userStore = useUserStoreWithOut()
  return request.get<CountryItem[]>({
    url: '/api/ads/audience/country/list',
    params,
    headers: { access_token: userStore.getToken }
  })
}

/**
 * 获取语言列表
 */
export const getLanguageListApi = (params?: LanguageListInput) => {
  const userStore = useUserStoreWithOut()
  return request.get<LanguageItem[]>({
    url: '/api/ads/audience/language/list',
    params,
    headers: { access_token: userStore.getToken }
  })
}
