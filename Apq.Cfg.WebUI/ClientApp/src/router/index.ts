import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/HomeView.vue')
    },
    {
      path: '/app/:id',
      name: 'config',
      component: () => import('@/views/ConfigView.vue')
    }
  ]
})

export default router
