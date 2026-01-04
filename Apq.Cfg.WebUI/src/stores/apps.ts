import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import type { AppEndpoint } from '@/types'
import { loadAppsFromStorage, saveAppsToStorage } from '@/utils/storage'
import { createConfigApi } from '@/api/config'

export const useAppsStore = defineStore('apps', () => {
  // 从 localStorage 初始化
  const apps = ref<AppEndpoint[]>(loadAppsFromStorage<AppEndpoint>())
  const loading = ref(false)

  // 监听变化，自动保存到 localStorage
  watch(apps, (value) => {
    saveAppsToStorage(value)
  }, { deep: true })

  /**
   * 添加应用
   */
  function addApp(app: Omit<AppEndpoint, 'id' | 'createdAt'>): AppEndpoint {
    const newApp: AppEndpoint = {
      ...app,
      id: crypto.randomUUID(),
      createdAt: new Date().toISOString()
    }
    apps.value.push(newApp)
    return newApp
  }

  /**
   * 更新应用
   */
  function updateApp(id: string, updates: Partial<AppEndpoint>): boolean {
    const index = apps.value.findIndex(a => a.id === id)
    if (index >= 0) {
      apps.value[index] = { ...apps.value[index], ...updates }
      return true
    }
    return false
  }

  /**
   * 删除应用
   */
  function deleteApp(id: string): boolean {
    const index = apps.value.findIndex(a => a.id === id)
    if (index >= 0) {
      apps.value.splice(index, 1)
      return true
    }
    return false
  }

  /**
   * 获取应用
   */
  function getApp(id: string): AppEndpoint | undefined {
    return apps.value.find(a => a.id === id)
  }

  /**
   * 测试连接
   */
  async function testConnection(app: AppEndpoint): Promise<boolean> {
    try {
      loading.value = true
      const api = createConfigApi(app)
      await api.testConnection()
      return true
    } catch {
      return false
    } finally {
      loading.value = false
    }
  }

  /**
   * 测试连接（通过 ID）
   */
  async function testConnectionById(id: string): Promise<boolean> {
    const app = getApp(id)
    if (!app) return false
    return testConnection(app)
  }

  return {
    apps,
    loading,
    addApp,
    updateApp,
    deleteApp,
    getApp,
    testConnection,
    testConnectionById
  }
})
