import request from '@/utils/request'
import type { AppEndpoint } from '@/types'

export const appsApi = {
  getAll(): Promise<AppEndpoint[]> {
    return request.get('/api/apps')
  },

  getById(id: string): Promise<AppEndpoint> {
    return request.get(`/api/apps/${id}`)
  },

  add(app: Partial<AppEndpoint>): Promise<AppEndpoint> {
    return request.post('/api/apps', app)
  },

  update(id: string, app: Partial<AppEndpoint>): Promise<void> {
    return request.put(`/api/apps/${id}`, app)
  },

  delete(id: string): Promise<void> {
    return request.delete(`/api/apps/${id}`)
  },

  testConnection(id: string): Promise<{ success: boolean }> {
    return request.post(`/api/apps/${id}/test`)
  }
}
