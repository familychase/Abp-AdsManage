import request from '@/axios'

// 获取所有字典
export const getDictApi = () => {
  return request.get({ url: '/api/dict/list' })
}

// 获取某个字典
export const getDictOneApi = async () => {
  return request.get({ url: '/api/dict/one' })
}
