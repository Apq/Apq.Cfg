import DefaultTheme from 'vitepress/theme'
import type { Theme } from 'vitepress'
import { h } from 'vue'
import SidebarCollapser from './SidebarCollapser.vue'
import './custom.css'

export default {
  extends: DefaultTheme,
  Layout() {
    return h(DefaultTheme.Layout, null, {
      // 在布局的末尾插入侧边栏控制组件
      'layout-bottom': () => h(SidebarCollapser)
    })
  }
} satisfies Theme
