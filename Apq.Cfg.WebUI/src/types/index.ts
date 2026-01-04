// 认证类型枚举
export enum AuthType {
  None = 0,      // 无认证
  ApiKey = 1,    // API Key 认证
  JwtBearer = 2  // JWT Bearer 认证
}

// 应用端点
export interface AppEndpoint {
  id: string           // 唯一标识 (UUID)
  name: string         // 应用名称
  url: string          // WebApi 地址（如 http://localhost:5000/api/apqcfg）
  authType: AuthType   // 认证方式
  apiKey?: string      // API Key（authType 为 ApiKey 时使用）
  token?: string       // JWT Token（authType 为 JwtBearer 时使用）
  description?: string // 备注说明
  createdAt: string    // 创建时间 (ISO 8601)
}

// 配置树节点
export interface ConfigTreeNode {
  key: string
  value: string | null
  hasValue: boolean
  isMasked: boolean
  children: ConfigTreeNode[]
}

// 配置源 DTO
export interface ConfigSourceDto {
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
