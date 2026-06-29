import request from '@/axios'
import type {
  DictSortListInput,
  DictSortPagedResult,
  DictSortDto,
  CreateDictSortInput,
  UpdateDictSortInput,
  DictItemListInput,
  DictItemPagedResult,
  DictItemDto,
  CreateDictItemInput,
  UpdateDictItemInput,
  DictWebListResult,
  BatchCreateSortItemInput
} from './types'

// ══════════════════════════════════════════
//  字典分类 (Dict Sort)
// ══════════════════════════════════════════

/** 获取字典分类分页列表：POST /api/system/dict/sort/list */
export const getDictSortListApi = (data: DictSortListInput) => {
  return request.post<DictSortPagedResult>({
    url: '/api/system/dict/sort/list',
    data
  })
}

/** 获取单个字典分类：GET /api/system/dict/sort/{id} */
export const getDictSortApi = (id: number) => {
  return request.get<DictSortDto>({ url: `/api/system/dict/sort/${id}` })
}

/** 创建字典分类：POST /api/system/dict/sort */
export const createDictSortApi = (data: CreateDictSortInput) => {
  return request.post<DictSortDto>({
    url: '/api/system/dict/sort',
    data
  })
}

/** 更新字典分类：PUT /api/system/dict/sort/{id} */
export const updateDictSortApi = (id: number, data: UpdateDictSortInput) => {
  return request.put<DictSortDto>({
    url: `/api/system/dict/sort/${id}`,
    data
  })
}

/** 删除字典分类：DELETE /api/system/dict/sort/{id} */
export const deleteDictSortApi = (id: number) => {
  return request.delete({ url: `/api/system/dict/sort/${id}` })
}

// ══════════════════════════════════════════
//  字典项 (Dict Item)
// ══════════════════════════════════════════

/** 获取字典项分页列表：POST /api/system/dict/item/list */
export const getDictItemListApi = (data: DictItemListInput) => {
  return request.post<DictItemPagedResult>({
    url: '/api/system/dict/item/list',
    data
  })
}

/** 获取字典项树：GET /api/system/dict/item/tree/{dictSortId} */
export const getDictItemTreeApi = (dictSortId: number) => {
  return request.get<DictItemDto[]>({
    url: `/api/system/dict/item/tree/${dictSortId}`
  })
}

/** 获取单个字典项：GET /api/system/dict/item/{id} */
export const getDictItemApi = (id: number) => {
  return request.get<DictItemDto>({ url: `/api/system/dict/item/${id}` })
}

/** 创建字典项：POST /api/system/dict/item */
export const createDictItemApi = (data: CreateDictItemInput) => {
  return request.post<DictItemDto>({
    url: '/api/system/dict/item',
    data
  })
}

/** 更新字典项：PUT /api/system/dict/item/{id} */
export const updateDictItemApi = (id: number, data: UpdateDictItemInput) => {
  return request.put<DictItemDto>({
    url: `/api/system/dict/item/${id}`,
    data
  })
}

/** 删除字典项：DELETE /api/system/dict/item/{id} */
export const deleteDictItemApi = (id: number) => {
  return request.delete({ url: `/api/system/dict/item/${id}` })
}

/** 获取字典项 Web 列表：POST /api/system/dict/item/web_list
 *  @param sortCodes dictSortCode 数组 */
export const getDictItemWebListApi = (sortCodes: string[] = []) => {
  return request.post<DictWebListResult>({
    url: '/api/system/dict/item/web_list',
    data: sortCodes
  })
}

/** 批量新增字典分类+字典项：POST /api/system/dict/sort_item/add */
export const batchCreateSortItemApi = (data: BatchCreateSortItemInput) => {
  return request.post<IResponse>({
    url: '/api/system/dict/sort_item/add',
    data
  })
}

/** 查询字典项：GET /api/system/dict/item_list */
export const getDictItemQueryApi = (sortCode: string) => {
  return request.get<DictItemDto[]>({
    url: '/api/system/dict/item_list',
    params: { sort_code: sortCode }
  })
}
