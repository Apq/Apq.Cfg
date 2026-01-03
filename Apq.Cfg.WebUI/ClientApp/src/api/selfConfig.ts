import request from '@/utils/request'

export interface ConfigSource {
  level: number
  name: string
  type: string
  writeable: boolean
  isPrimaryWriter: boolean
  path?: string
}

export interface ConfigTreeNode {
  key: string
  value?: string
  children?: ConfigTreeNode[]
}

export interface ApiResponse<T> {
  success: boolean
  data: T
  error?: string
  code?: string
}

export const selfConfigApi = {
  /**
   * 获取配置源列表
   */
  getSources(): Promise<ApiResponse<ConfigSource[]>> {
    return request.get('/api/apqcfg/sources')
  },

  /**
   * 获取合并后的配置树
   */
  getMergedTree(): Promise<ApiResponse<ConfigTreeNode>> {
    return request.get('/api/apqcfg/merged/tree')
  },

  /**
   * 获取指定配置源的配置树
   */
  getSourceTree(level: number, name: string): Promise<ApiResponse<ConfigTreeNode>> {
    return request.get(`/api/apqcfg/sources/${level}/${encodeURIComponent(name)}/tree`)
  },

  /**
   * 获取合并后的所有配置
   */
  getMergedConfig(): Promise<ApiResponse<Record<string, string | null>>> {
    return request.get('/api/apqcfg/merged')
  },

  /**
   * 设置配置值
   */
  setValue(key: string, value: string | null): Promise<ApiResponse<boolean>> {
    return request.put(`/api/apqcfg/keys/${key}`, value, {
      headers: { 'Content-Type': 'application/json' }
    })
  },

  /**
   * 保存配置
   */
  save(): Promise<ApiResponse<boolean>> {
    return request.post('/api/apqcfg/save')
  },

  /**
   * 重新加载配置
   */
  reload(): Promise<ApiResponse<boolean>> {
    return request.post('/api/apqcfg/reload')
  },

  /**
   * 导出配置
   */
  export(format: 'json' | 'env' | 'kv'): Promise<string> {
    return request.get(`/api/apqcfg/export/${format}`, {
      responseType: 'text'
    })
  }
}
