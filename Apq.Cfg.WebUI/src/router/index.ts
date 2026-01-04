import { createRouter, createWebHistory } from 'vue-router'

// 动态获取基础路径，支持任意虚拟目录部署
function getBasePath(): string {
  // 从当前 URL 推断基础路径
  const pathname = window.location.pathname

  // 如果路径以 index.html 结尾，去掉它
  let base = pathname.replace(/\/index\.html$/, '/')

  // 检查是否是已知的路由路径，如果是则去掉
  const knownRoutes = ['/app/']
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

const router = createRouter({
  // 动态获取基础路径，支持任意虚拟目录
  history: createWebHistory(getBasePath()),
  routes: [
    {
      path: '/',
      component: () => import('@/layouts/MainLayout.vue'),
      children: [
        {
          path: '',
          name: 'home',
          component: () => import('@/views/HomeView.vue')
        },
        {
          path: 'app/:id',
          name: 'config',
          component: () => import('@/views/ConfigView.vue')
        }
      ]
    }
  ]
})

export default router
