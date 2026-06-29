import request from '@/axios'
import type { GetSysLogErrorListInput, SysLogErrorPagedResult } from './types'

/**
 * 错误日志分页查询
 * POST /api/system/log_error/list
 */
export const getSysLogErrorListApi = (data: GetSysLogErrorListInput) => {
  return request.post<SysLogErrorPagedResult>({
    url: '/api/system/log_error/list',
    data
  })
}
