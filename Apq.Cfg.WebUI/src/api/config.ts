import axios, { type AxiosInstance } from 'axios'
import type { AppEndpoint, ApiResponse, ConfigTreeNode, ConfigSourceDto } from '@/types'
import { AuthType } from '@/types'

/**
 * 创建配置 API 实例
 * 前端直接访问远程应用的配置 API，根据应用的认证配置添加相应的请求头
 */
export function createConfigApi(app: AppEndpoint) {
  const instance: AxiosInstance = axios.create({
    baseURL: app.url.replace(/\/$/, ''), // 移除末尾斜杠
    timeout: 30000
  })

  // 添加认证头
  instance.interceptors.request.use(config => {
    switch (app.authType) {
      case AuthType.ApiKey:
        if (app.apiKey) {
          config.headers['X-Api-Key'] = app.apiKey
        }
        break
      case AuthType.JwtBearer:
        if (app.token) {
          config.headers['Authorization'] = `Bearer ${app.token}`
        }
        break
    }
    return config
  })

  // 响应拦截器 - 提取 data
  instance.interceptors.response.use(
    response => response.data,
    error => {
      const message = error.response?.data?.error || error.message || '请求失败'
      return Promise.reject(new Error(message))
    }
  )

  return {
    // ========== 合并后配置（Merged）==========

    getMerged(): Promise<ApiResponse<Record<string, string | null>>> {
      return instance.get('/merged')
    },

    getMergedTree(): Promise<ApiResponse<ConfigTreeNode>> {
      return instance.get('/merged/tree')
    },

    getMergedValue(key: string): Promise<ApiResponse<any>> {
      return instance.get(`/merged/keys/${encodeURIComponent(key)}`)
    },

    // ========== 合并前配置（Sources）==========

    getSources(): Promise<ApiResponse<ConfigSourceDto[]>> {
      return instance.get('/sources')
    },

    getSourceConfig(level: number, name: string): Promise<ApiResponse<Record<string, string | null>>> {
      return instance.get(`/sources/${level}/${encodeURIComponent(name)}`)
    },

    getSourceTree(level: number, name: string): Promise<ApiResponse<ConfigTreeNode>> {
      return instance.get(`/sources/${level}/${encodeURIComponent(name)}/tree`)
    },

    getSourceValue(level: number, name: string, key: string): Promise<ApiResponse<any>> {
      return instance.get(`/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`)
    },

    // ========== 写入操作 ==========

    setMergedValue(key: string, value: string | null): Promise<ApiResponse<boolean>> {
      return instance.put(`/merged/keys/${encodeURIComponent(key)}`, JSON.stringify(value), {
        headers: { 'Content-Type': 'application/json' }
      })
    },

    setSourceValue(level: number, name: string, key: string, value: string | null): Promise<ApiResponse<boolean>> {
      return instance.put(`/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`, JSON.stringify(value), {
        headers: { 'Content-Type': 'application/json' }
      })
    },

    deleteMergedKey(key: string): Promise<ApiResponse<boolean>> {
      return instance.delete(`/merged/keys/${encodeURIComponent(key)}`)
    },

    deleteSourceKey(level: number, name: string, key: string): Promise<ApiResponse<boolean>> {
      return instance.delete(`/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`)
    },

    // ========== 管理操作 ==========

    save(): Promise<ApiResponse<boolean>> {
      return instance.post('/save')
    },

    reload(): Promise<ApiResponse<boolean>> {
      return instance.post('/reload')
    },

    exportMerged(format: string = 'json'): Promise<string> {
      return instance.get(`/merged/export/${format}`, {
        responseType: 'text'
      })
    },

    exportSource(level: number, name: string, format: string = 'json'): Promise<string> {
      return instance.get(`/sources/${level}/${encodeURIComponent(name)}/export/${format}`, {
        responseType: 'text'
      })
    },

    // ========== 测试连接 ==========

    testConnection(): Promise<ApiResponse<Record<string, string | null>>> {
      return instance.get('/merged')
    }
  }
}

/**
 * 敏感值关键词
 */
const sensitiveKeywords = [
  'password', 'secret', 'key', 'token', 'credential',
  'connectionstring', 'apikey', 'accesskey', 'privatekey'
]

/**
 * 判断配置键是否为敏感值
 */
export function isSensitive(key: string): boolean {
  const lowerKey = key.toLowerCase()
  return sensitiveKeywords.some(k => lowerKey.includes(k))
}

/**
 * 脱敏显示敏感值
 */
export function maskValue(value: string): string {
  if (!value || value.length <= 4) return '****'
  return value.substring(0, 2) + '****' + value.substring(value.length - 2)
}
