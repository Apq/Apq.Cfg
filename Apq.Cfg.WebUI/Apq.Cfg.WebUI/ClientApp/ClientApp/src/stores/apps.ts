import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { AppEndpoint } from '@/types'
import { appsApi } from '@/api/apps'

export const useAppsStore = defineStore('apps', () => {
  const apps = ref<AppEndpoint[]>([])
  const loading = ref(false)

  async function fetchApps() {
    loading.value = true
    try {
      apps.value = await appsApi.getAll()
    } finally {
      loading.value = false
    }
  }

  async function addApp(app: Partial<AppEndpoint>) {
    const created = await appsApi.add(app)
    apps.value.push(created)
    return created
  }

  async function updateApp(id: string, app: Partial<AppEndpoint>) {
    await appsApi.update(id, app)
    const index = apps.value.findIndex(a => a.id === id)
    if (index >= 0) {
      apps.value[index] = { ...apps.value[index], ...app }
    }
  }

  async function deleteApp(id: string) {
    await appsApi.delete(id)
    apps.value = apps.value.filter(a => a.id !== id)
  }

  async function testConnection(id: string) {
    const result = await appsApi.testConnection(id)
    return result.success
  }

  return {
    apps,
    loading,
    fetchApps,
    addApp,
    updateApp,
    deleteApp,
    testConnection
  }
})
