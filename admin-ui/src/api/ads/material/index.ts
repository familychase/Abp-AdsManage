import request from '@/axios'
import service from '@/axios/service'
import { useUserStoreWithOut } from '@/store/modules/user'
import { useLocaleStoreWithOut } from '@/store/modules/locale'
import type { GetMaterialListInput, MaterialListResponse } from './types'

const localeToAcceptLanguage: Record<string, string> = {
  'zh-CN': 'zh-Hans',
  en: 'en'
}

/**
 * 素材列表（分页）：POST /api/ads/material/list
 */
export const getMaterialListApi = (data: GetMaterialListInput) => {
  return request.post<IResponse<MaterialListResponse>>({
    url: '/api/ads/material/list',
    data
  })
}

/**
 * 删除素材：DELETE /api/ads/material/{id}
 */
export const deleteMaterialApi = (id: number) => {
  return request.delete<IResponse<unknown>>({
    url: `/api/ads/material/${id}`
  })
}

/**
 * 批量删除素材：POST /api/ads/material/batch_delete
 */
export const batchDeleteMaterialApi = (ids: number[]) => {
  return request.post<IResponse<unknown>>({
    url: '/api/ads/material/batch_delete',
    data: { ids }
  })
}

/**
 * 单文件上传素材：POST /api/ads/material/upload
 * - file: 素材文件 (binary)
 * - coverFile: 视频封面（可选）
 * - folderId: 文件夹ID（query 参数，可选）
 */
export const uploadMaterialApi = (
  file: File,
  options?: {
    coverFile?: File
    folderId?: number
    onProgress?: (percent: number) => void
  }
) => {
  const formData = new FormData()
  formData.append('file', file)
  if (options?.coverFile) {
    formData.append('coverFile', options.coverFile)
  }

  const params: Record<string, unknown> = {}
  if (options?.folderId) {
    params.folderId = options.folderId
  }

  const userStore = useUserStoreWithOut()
  const localeStore = useLocaleStoreWithOut()
  const lang = localeStore.getCurrentLocale?.lang || 'zh-CN'

  return service.request({
    url: '/api/ads/material/upload',
    method: 'post',
    data: formData,
    params,
    headers: {
      'Content-Type': 'multipart/form-data',
      'Accept-Language': localeToAcceptLanguage[lang] || lang,
      access_token: userStore.getToken ?? ''
    },
    onUploadProgress: (progressEvent) => {
      if (options?.onProgress && progressEvent.total) {
        const percent = (progressEvent.loaded / progressEvent.total) * 100
        options.onProgress(percent)
      }
    }
  })
}
