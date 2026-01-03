import axios, { type AxiosInstance, type AxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'

// 动态获取基础路径，支持任意虚拟目录部署
// 从当前页面 URL 推断 API 基础路径
function getBaseUrl(): string {
  const pathname = window.location.pathname

  // 如果路径以 index.html 结尾，去掉它
  let base = pathname.replace(/\/index\.html$/, '/')

  // 检查是否是已知的路由路径，如果是则去掉
  const knownRoutes = ['/self', '/app/']
  for (const route of knownRoutes) {
    const idx = base.indexOf(route)
    if (idx >= 0) {
      // idx === 0 表示从根目录访问，base 应该是 /
      // idx > 0 表示有虚拟目录前缀，base 应该是前缀部分
      base = idx === 0 ? '/' : base.substring(0, idx + 1)
      break
    }
  }

  // 确保以 / 结尾
  if (!base.endsWith('/')) {
    base += '/'
  }

  return base
}

const instance: AxiosInstance = axios.create({
  baseURL: getBaseUrl(),
  timeout: 30000
})

// 响应拦截器
instance.interceptors.response.use(
  response => response.data,
  error => {
    const message = error.response?.data?.error || error.message || '请求失败'
    ElMessage.error(message)
    return Promise.reject(error)
  }
)

export default {
  get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return instance.get(url, config)
  },
  post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return instance.post(url, data, config)
  },
  put<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return instance.put(url, data, config)
  },
  delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return instance.delete(url, config)
  }
}
