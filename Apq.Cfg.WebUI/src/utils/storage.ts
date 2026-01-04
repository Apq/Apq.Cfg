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
 * 导出格式
 */
export type ExportFormat = 'json' | 'csv'

/**
 * 下载文件
 */
function downloadFile(blob: Blob, filename: string): void {
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

/**
 * CSV 值转义（处理逗号、引号、换行）
 */
function escapeCsvValue(value: string | undefined | null): string {
  const str = value ?? ''
  if (str.includes(',') || str.includes('"') || str.includes('\n')) {
    return `"${str.replace(/"/g, '""')}"`
  }
  return str
}

/**
 * 解析 CSV 行（处理引号内的逗号）
 */
function parseCsvLine(line: string): string[] {
  const result: string[] = []
  let current = ''
  let inQuotes = false

  for (let i = 0; i < line.length; i++) {
    const char = line[i]
    if (char === '"') {
      if (inQuotes && line[i + 1] === '"') {
        current += '"'
        i++ // 跳过转义的引号
      } else {
        inQuotes = !inQuotes
      }
    } else if (char === ',' && !inQuotes) {
      result.push(current)
      current = ''
    } else {
      current += char
    }
  }
  result.push(current)
  return result
}

/**
 * 导出应用列表
 */
export function exportApps(format: ExportFormat = 'json'): void {
  const data = localStorage.getItem(STORAGE_KEY) || '[]'
  const apps = JSON.parse(data)
  const dateStr = new Date().toISOString().slice(0, 10)

  if (format === 'csv') {
    // CSV 导出（带 UTF-8 BOM，确保 Excel 正确显示中文）
    const headers = ['id', 'name', 'url', 'authType', 'apiKey', 'token', 'description']
    const rows = apps.map((app: Record<string, unknown>) =>
      headers.map(h => escapeCsvValue(String(app[h] ?? ''))).join(',')
    )
    const csv = [headers.join(','), ...rows].join('\n')
    const bom = '\uFEFF' // UTF-8 BOM
    const blob = new Blob([bom + csv], { type: 'text/csv;charset=utf-8' })
    downloadFile(blob, `apqcfg-apps-${dateStr}.csv`)
  } else {
    // JSON 导出
    const blob = new Blob([JSON.stringify(apps, null, 2)], { type: 'application/json' })
    downloadFile(blob, `apqcfg-apps-${dateStr}.json`)
  }
}

/**
 * 从文件导入应用列表（自动识别格式）
 */
export function importApps(file: File): Promise<void> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = (e) => {
      try {
        const text = e.target?.result as string
        const ext = file.name.split('.').pop()?.toLowerCase()
        let apps: unknown[]

        if (ext === 'csv') {
          // CSV 导入
          const lines = text.replace(/^\uFEFF/, '').trim().split('\n') // 移除 BOM
          if (lines.length < 2) {
            reject(new Error('CSV 文件为空或格式错误'))
            return
          }
          const [headerLine, ...dataLines] = lines
          const headers = headerLine.split(',')
          apps = dataLines.map(line => {
            const values = parseCsvLine(line)
            return Object.fromEntries(headers.map((h, i) => [h, values[i] ?? '']))
          })
        } else {
          // JSON 导入
          apps = JSON.parse(text)
        }

        if (Array.isArray(apps)) {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(apps))
          resolve()
        } else {
          reject(new Error('无效的应用列表格式'))
        }
      } catch {
        reject(new Error('文件解析失败'))
      }
    }
    reader.onerror = () => reject(new Error('文件读取失败'))
    reader.readAsText(file)
  })
}
