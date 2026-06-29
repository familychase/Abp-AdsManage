// ══════════════════════════════════════════
//  Meta 发布 — TypeScript 类型定义
//  对应 Swagger MetaPublishing 标签
// ══════════════════════════════════════════

/** 版位树请求参数（对应 Swagger PositionTreeRequest） */
export interface PositionTreeRequest {
  /** 任务编号（可选） */
  task_code?: string | null
}

/** 版位树子节点（叶子 — 对应单个版位） */
export interface PositionTreeItem {
  /** 版位显示名称（如 "Facebook 动态消息"） */
  name: string
  /** 所属平台（facebook / instagram / messenger / audience_network） */
  parent_value: string
  /** 版位值（如 "feed", "story"） */
  value: string
  /** 是否禁用 */
  disabled: boolean
}

/** 版位树分类（第1层 — 版位类别分组） */
export interface PositionTreeCategory {
  /** 分类名称（如 "动态", "快拍和Reels"） */
  name: string
  /** 该分类下的版位列表 */
  children: PositionTreeItem[]
}

/** 版位树 API 响应 */
export interface PositionTreeResponse {
  code: number
  message: string
  data: PositionTreeCategory[]
}
