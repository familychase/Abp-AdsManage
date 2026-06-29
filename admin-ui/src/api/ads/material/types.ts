/** 素材类型（与后端 MaterialType 枚举对齐） */
export type MaterialType = 'IMAGE' | 'VIDEO'

/** 素材列表项（与后端 /api/ads/material/list 响应对齐） */
export interface MaterialItem {
  /** 素材ID */
  id: number
  /** 素材名称 */
  materialName: string
  /** 素材编号 */
  materialNo: string
  /** 素材URL */
  materialUrl: string
  /** 封面URL（视频封面） */
  coverUrl: string | null
  /** 素材类型 */
  materialType: MaterialType
  /** 文件大小（字节） */
  fileSize: number | null
  /** 视频时长（秒，仅视频有值） */
  duration: number | null
  /** 文件夹ID */
  folderId: number
  /** 创建时间 */
  creationTime: string
}

/** 素材列表查询参数（POST /api/ads/material/list） */
export interface GetMaterialListInput {
  page: number
  pageSize: number
  /** 排序 */
  sorting?: string | null
  /** 过滤关键词（搜索 materialName） */
  filterText?: string | null
  /** 素材类型 */
  materialType?: MaterialType | null
  /** 文件夹ID */
  folderId?: number | null
}

/** 素材列表响应 */
export interface MaterialListResponse {
  totalCount: number
  items: MaterialItem[]
}
