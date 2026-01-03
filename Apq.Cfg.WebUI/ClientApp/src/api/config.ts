import request from '@/utils/request'
import type { ApiResponse, ConfigTreeNode, ConfigSourceInfo } from '@/types'

// 通过代理访问目标应用的配置 API
export const createConfigApi = (appId: string) => ({
  // ========== 合并后配置（Merged）==========

  getMerged(): Promise<ApiResponse<Record<string, string | null>>> {
    return request.get(`/api/proxy/${appId}/merged`)
  },

  getMergedTree(): Promise<ApiResponse<ConfigTreeNode>> {
    return request.get(`/api/proxy/${appId}/merged/tree`)
  },

  getMergedValue(key: string): Promise<ApiResponse<any>> {
    return request.get(`/api/proxy/${appId}/merged/keys/${encodeURIComponent(key)}`)
  },

  // ========== 合并前配置（Sources）==========

  getSources(): Promise<ApiResponse<ConfigSourceInfo[]>> {
    return request.get(`/api/proxy/${appId}/sources`)
  },

  getSourceConfig(level: number, name: string): Promise<ApiResponse<Record<string, string | null>>> {
    return request.get(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}`)
  },

  getSourceTree(level: number, name: string): Promise<ApiResponse<ConfigTreeNode>> {
    return request.get(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}/tree`)
  },

  getSourceValue(level: number, name: string, key: string): Promise<ApiResponse<any>> {
    return request.get(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`)
  },

  // ========== 写入操作 ==========

  setValue(key: string, value: string | null): Promise<ApiResponse<boolean>> {
    return request.put(`/api/proxy/${appId}/keys/${encodeURIComponent(key)}`, value)
  },

  setSourceValue(level: number, name: string, key: string, value: string | null): Promise<ApiResponse<boolean>> {
    return request.put(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`, value)
  },

  deleteKey(key: string): Promise<ApiResponse<boolean>> {
    return request.delete(`/api/proxy/${appId}/keys/${encodeURIComponent(key)}`)
  },

  deleteSourceKey(level: number, name: string, key: string): Promise<ApiResponse<boolean>> {
    return request.delete(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}/keys/${encodeURIComponent(key)}`)
  },

  // ========== 管理操作 ==========

  save(): Promise<ApiResponse<boolean>> {
    return request.post(`/api/proxy/${appId}/save`)
  },

  reload(): Promise<ApiResponse<boolean>> {
    return request.post(`/api/proxy/${appId}/reload`)
  },

  export(format: string = 'json'): Promise<string> {
    return request.get(`/api/proxy/${appId}/export/${format}`, {
      responseType: 'text'
    })
  },

  exportSource(level: number, name: string, format: string = 'json'): Promise<string> {
    return request.get(`/api/proxy/${appId}/sources/${level}/${encodeURIComponent(name)}/export/${format}`, {
      responseType: 'text'
    })
  }
})
