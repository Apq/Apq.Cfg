// 应用端点
export interface AppEndpoint {
  id: string
  name: string
  url: string
  authType: 'None' | 'ApiKey' | 'JwtBearer'
  apiKey?: string
  token?: string
  description?: string
  createdAt: string
}

// 配置树节点
export interface ConfigTreeNode {
  key: string
  value: string | null
  hasValue: boolean
  isMasked: boolean
  children: ConfigTreeNode[]
}

// 配置源信息（对应 Apq.Cfg.ConfigSourceInfo）
export interface ConfigSourceInfo {
  level: number
  name: string
  type: string
  isWriteable: boolean
  isPrimaryWriter: boolean
  keyCount: number
}

// API 响应
export interface ApiResponse<T> {
  success: boolean
  data?: T
  error?: string
}
