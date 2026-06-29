/** 字典分类 DTO */
export interface DictSortDto {
  id?: number
  /** 媒体平台（GOOGLE/META/TIKTOK/SNAPCHAT/KWAI/NONE 等） */
  platform?: string | null
  /** 类型（SYSTEM/MEDIA/NONE 等） */
  dictSortType?: string | null
  /** 字典分类名称 */
  dictSortName?: string | null
  /** 字典分类编号 */
  dictSortCode?: string | null
  /** 备注 */
  remarks?: string | null
  /** 排序 */
  sort?: number | null
  /** 状态（0=禁用 1=启用） */
  status?: number | null
  /** 子级字典项 */
  children?: DictItemDto[] | null
  /** 创建时间 */
  creationTime?: string | null
  /** 最后修改时间 */
  lastModificationTime?: string | null
}

/** 字典项 DTO */
export interface DictItemDto {
  id?: number
  /** 所属字典分类 ID */
  dictSortId?: number | null
  /** 字典项值 */
  dictItemValue?: string | null
  /** 字典项中文名称 */
  dictItemName?: string | null
  /** 字典项英文名称 */
  dictItemNameEN?: string | null
  /** 字典项编码 */
  dictItemCode?: string | null
  /** 备注 */
  remarks?: string | null
  /** 排序 */
  ordinal?: number | null
  /** 是否生产环境 */
  isProduction?: boolean | null
  /** 类型（NONE/SYSTEM/MEDIA 等） */
  itemType?: string | null
  /** 父级字典项 ID */
  parentId?: number | null
  /** 子级字典项 */
  children?: DictItemDto[] | null
  /** 创建时间 */
  creationTime?: string | null
  /** 最后修改时间 */
  lastModificationTime?: string | null
}

/** POST /api/system/dict/sort/list 请求参数 */
export interface DictSortListInput {
  /** 排序字段 */
  sorting?: string | null
  /** 当前页码，从 1 开始 */
  page: number
  /** 每页条数 */
  pageSize: number
  /** 字典名称/编码（模糊搜索） */
  filter?: string | null
}

/** 字典分类分页结果 */
export interface DictSortPagedResult {
  items: DictSortDto[]
  totalCount: number
}

/** POST /api/system/dict/sort 请求参数 */
export interface CreateDictSortInput {
  platform: string
  dictSortType: string
  dictSortCode: string
  dictSortName: string
  remarks?: string | null
}

/** PUT /api/system/dict/sort/{id} 请求参数 */
export interface UpdateDictSortInput {
  platform?: string | null
  dictSortType?: string | null
  dictSortCode?: string | null
  dictSortName?: string | null
  remarks?: string | null
}

/** POST /api/system/dict/item/list 请求参数 */
export interface DictItemListInput {
  /** 排序字段 */
  sorting?: string | null
  /** 当前页码，从 1 开始 */
  page: number
  /** 每页条数 */
  pageSize: number
  /** 字典分类 ID */
  dictSortId?: number | null
  /** 模糊搜索（字典项名称/编码/值） */
  filter?: string | null
}

/** 字典项分页结果 */
export interface DictItemPagedResult {
  items: DictItemDto[]
  totalCount: number
}

/** POST /api/system/dict/item 请求参数 */
export interface CreateDictItemInput {
  dictSortId: number
  parentId?: number | null
  dictItemCode?: string | null
  dictItemName: string
  dictItemNameEN?: string | null
  dictItemValue: string
  remarks?: string | null
  ordinal?: number | null
  itemType?: string | null
  isProduction?: boolean | null
}

/** PUT /api/system/dict/item/{id} 请求参数 */
export interface UpdateDictItemInput {
  dictSortId?: number | null
  parentId?: number | null
  dictItemCode?: string | null
  dictItemName?: string | null
  dictItemNameEN?: string | null
  dictItemValue?: string | null
  remarks?: string | null
  ordinal?: number | null
  itemType?: string | null
  isProduction?: boolean | null
}

/** POST /api/system/dict/item/web_list
 *  请求体: string[] (dictSortCode 列表)
 *  响应体: Record<string, DictItemDto[]> (按 dictSortCode 分组) */
export type DictWebListResult = Record<string, DictItemDto[]>

/** POST /api/system/dict/sort_item/add 批量新增字典分类+字典项 */
export interface BatchCreateSortItemInput {
  platform: string
  dictSortType: string
  dictSortCode: string
  dictSortName: string
  remarks?: string | null
  dictItems: CreateDictItemInput[]
}
