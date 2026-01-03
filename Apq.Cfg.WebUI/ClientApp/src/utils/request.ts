import axios, { type AxiosInstance, type AxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'

// 动态获取基础路径，支持任意虚拟目录部署
// 从当前页面 URL 推断 API 基础路径
function getBaseUrl(): string {
  // 获取当前页面的路径，去掉最后的文件名或路由部分
  const pathname = window.location.pathname
  // 找到最后一个 / 的位置（排除 hash 路由的部分）
  // 例如：/apqcfg/ -> /apqcfg/
  // 例如：/apqcfg/self -> /apqcfg/
  // 例如：/ -> /

  // 如果路径以 index.html 结尾，去掉它
  let base = pathname.replace(/\/index\.html$/, '/')

  // 确保以 / 结尾
  if (!base.endsWith('/')) {
    // 去掉最后一段路由路径
    const lastSlash = base.lastIndexOf('/')
    base = base.substring(0, lastSlash + 1)
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
