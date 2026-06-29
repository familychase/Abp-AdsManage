import type { PlatformType } from '@/api/ads/channel/types'

/** 国家列表查询参数 */
export interface CountryListInput {
  name?: string
  platform?: PlatformType
}

/** 语言列表查询参数 */
export interface LanguageListInput {
  name?: string
  platform?: PlatformType
}

/** API 返回的语言条目 */
export interface LanguageItem {
  id: number
  name: string
  code: string
  platform: string
}

/** API 返回的国家/地区条目（驼峰命名，与后端一致） */
export interface CountryItem {
  key: string
  name: string
  type: 'country_group' | 'country' | 'region' | 'city'
  countryCode?: string
  countryCodes?: string[]
  region?: string | null
  regionId?: number
}
