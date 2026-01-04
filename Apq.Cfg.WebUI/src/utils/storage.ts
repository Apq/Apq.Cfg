/**
 * localStorage 封装工具
 */

const STORAGE_KEY = 'apqcfg-apps'

/**
 * 从 localStorage 加载应用列表
 */
export function loadAppsFromStorage<T>(): T[] {
  try {
    const data = localStorage.getItem(STORAGE_KEY)
    return data ? JSON.parse(data) : []
  } catch {
    return []
  }
}

/**
 * 保存应用列表到 localStorage
 */
export function saveAppsToStorage<T>(apps: T[]): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(apps))
}

/**
 * 导出应用列表为 JSON 文件
 */
export function exportApps(): void {
  const data = localStorage.getItem(STORAGE_KEY) || '[]'
  const blob = new Blob([data], { type: 'application/json' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `apqcfg-apps-${new Date().toISOString().slice(0, 10)}.json`
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

/**
 * 从 JSON 文件导入应用列表
 */
export function importApps(file: File): Promise<void> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = (e) => {
      try {
        const apps = JSON.parse(e.target?.result as string)
        if (Array.isArray(apps)) {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(apps))
          resolve()
        } else {
          reject(new Error('无效的应用列表格式'))
        }
      } catch {
        reject(new Error('JSON 解析失败'))
      }
    }
    reader.onerror = () => reject(new Error('文件读取失败'))
    reader.readAsText(file)
  })
}
