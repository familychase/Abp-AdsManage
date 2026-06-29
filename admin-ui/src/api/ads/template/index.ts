import request from '@/axios'
import type {
  GetTemplateListInput,
  GetValidTemplateListInput,
  TemplateDto,
  TemplateDetail,
  TemplatePagedResult,
  TemplateStatistics,
  AdQuotaInfo,
  CreateTemplateInput,
  SaveTemplateInput,
  AdsPublishTemplateViewModel,
  AdsPublishTemplatePagedResult
} from './types'
import { toAdsPublishTemplateViewModel } from './types'

// ══════════════════════════════════════════
//  模板 CRUD
// ══════════════════════════════════════════

/** 模板列表：POST /api/ads/template/list */
export const getTemplateListApi = (data: GetTemplateListInput) => {
  return request.post<AdsPublishTemplatePagedResult>({
    url: '/api/ads/template/list',
    data
  })
}

/** 模板详情：GET /api/ads/template/{id} */
export const getTemplateDetailApi = (id: number) => {
  return request.get<TemplateDetail>({
    url: `/api/ads/template/${id}`
  })
}

/** 新增模板：POST /api/ads/template/add — 使用 AdsPublishTemplateViewModel */
export const addTemplateApi = (data: AdsPublishTemplateViewModel) => {
  return request.post<TemplateDto>({
    url: '/api/ads/template/add',
    data
  })
}

/** 编辑模板：POST /api/ads/template/edit — 使用 AdsPublishTemplateViewModel */
export const editTemplateApi = (data: AdsPublishTemplateViewModel) => {
  return request.post<TemplateDto>({
    url: '/api/ads/template/edit',
    data
  })
}

/** 新增模板（兼容旧 SaveTemplateInput，内部转换） */
export const addTemplateFromOldInputApi = (data: SaveTemplateInput) => {
  return addTemplateApi(toAdsPublishTemplateViewModel(data))
}

/** 编辑模板（兼容旧 SaveTemplateInput，内部转换） */
export const editTemplateFromOldInputApi = (data: SaveTemplateInput) => {
  return editTemplateApi(toAdsPublishTemplateViewModel(data))
}

/** 创建模板（简化）：POST /api/ads/template */
export const createTemplateApi = (data: CreateTemplateInput) => {
  return request.post<TemplateDto>({
    url: '/api/ads/template',
    data
  })
}

/** 删除模板（软删）：POST /api/ads/template/remove */
export const removeTemplateApi = (templateId: number) => {
  return request.post<IResponse>({
    url: '/api/ads/template/remove',
    data: { templateId }
  })
}

/** 删除模板：DELETE /api/ads/template/{id} */
export const deleteTemplateApi = (id: number) => {
  return request.delete<IResponse>({
    url: `/api/ads/template/${id}`
  })
}

/** 还原模板：POST /api/ads/template/restore */
export const restoreTemplateApi = (templateId: number) => {
  return request.post<IResponse>({
    url: '/api/ads/template/restore',
    data: { templateId }
  })
}

/** 复制模板：POST /api/ads/template/reproduction */
export const reproductionTemplateApi = (templateId: number) => {
  return request.post<IResponse>({
    url: '/api/ads/template/reproduction',
    data: { templateId }
  })
}

/** 复制并创建模板：POST /api/ads/template/{id}/copy */
export const copyAndCreateTemplateApi = (id: number) => {
  return request.post<IResponse>({
    url: `/api/ads/template/${id}/copy`
  })
}

// ══════════════════════════════════════════
//  模板统计 & 有效模板
// ══════════════════════════════════════════

/** 模板数量统计：POST /api/ads/template/statistics */
export const getTemplateStatisticsApi = (data?: { platform?: number }) => {
  return request.post<TemplateStatistics>({
    url: '/api/ads/template/statistics',
    data: data || {}
  })
}

/** 有效模板列表（发布选模板用）：POST /api/ads/template/valid/list */
export const getValidTemplateListApi = (data: GetValidTemplateListInput) => {
  return request.post<TemplatePagedResult>({
    url: '/api/ads/template/valid/list',
    data
  })
}

// ══════════════════════════════════════════
//  其他
// ══════════════════════════════════════════

/** 获取 AD 额度使用情况：GET /api/ads/template/quota */
export const getAdQuotaApi = () => {
  return request.get<AdQuotaInfo>({
    url: '/api/ads/template/quota'
  })
}

/** 批量建报告：POST /api/ads/template/batch-report */
export const batchBuildReportApi = (templateIds: number[]) => {
  return request.post<IResponse>({
    url: '/api/ads/template/batch-report',
    data: { templateIds }
  })
}

/** 快速创建模板（从近7日高消耗广告自动提取）：POST /api/ads/template/fast/add */
export const fastAddTemplateApi = () => {
  return request.post<IResponse>({
    url: '/api/ads/template/fast/add',
    data: {}
  })
}
