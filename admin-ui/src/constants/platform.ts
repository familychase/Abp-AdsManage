import type { PlatformType } from '@/api/ads/channel/types'

/**
 * 媒体平台标签映射（与后端 PlatformType 枚举对齐）
 *
 * NONE   = 0  无
 * GOOGLE = 1  Google Ads
 * META   = 2  Meta (Facebook/Instagram)
 * TIKTOK = 3  TikTok
 */
export const PLATFORM_LABELS: Record<PlatformType, string> = {
  0: '无',
  1: 'Google',
  2: 'META',
  3: 'TikTok'
}

/** 平台下拉选项（适用于 ElSelect / FormSchema.componentProps.options） */
export const PLATFORM_OPTIONS = Object.entries(PLATFORM_LABELS).map(([value, label]) => ({
  value: Number(value) as PlatformType,
  label
}))
