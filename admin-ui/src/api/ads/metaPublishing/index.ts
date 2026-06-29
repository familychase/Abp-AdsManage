import request from '@/axios'
import type { PositionTreeRequest, PositionTreeCategory } from './types'

/**
 * 获取 Meta 版位树：POST /api/ads/meta/publishing/position/trees
 *
 * 返回 Meta 广告版位的树级结构，第1层为版位分类（动态/快拍/视频插播等），
 * 第2层为具体版位，含 platform(parent_value) 和 placement value。
 * 用于广告组创建时的版位选择。
 *
 * @param data - 可选参数 task_code（任务编号）
 * @returns 响应 data 为 PositionTreeCategory[]
 */
export const getPositionTreesApi = (data?: PositionTreeRequest) => {
  return request.post<PositionTreeCategory[]>({
    url: '/api/ads/meta/publishing/position/trees',
    data: data || {}
  })
}
