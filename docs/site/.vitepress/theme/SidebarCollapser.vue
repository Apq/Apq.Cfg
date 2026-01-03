<script setup lang="ts">
import { watch, onMounted, nextTick } from 'vue'
import { useRoute } from 'vitepress'

const route = useRoute()

// 根据当前路径判断应该展开哪个组
function getActiveGroup(path: string): string | null {
  if (path.includes('/api/net10.0/')) return 'net10.0'
  if (path.includes('/api/net8.0/')) return 'net8.0'
  if (path.includes('/api/netstandard2.0/')) return 'netstandard2.0'
  return null
}

// 收起非当前组的侧边栏
function collapseOtherGroups(activeGroup: string | null) {
  nextTick(() => {
    // 查找所有可折叠的侧边栏组
    const groups = document.querySelectorAll('.VPSidebarItem.level-0.collapsible')

    groups.forEach((group) => {
      const titleEl = group.querySelector('.VPSidebarItem > .item > .text')
      if (!titleEl) return

      const title = titleEl.textContent || ''
      const isCurrentGroup =
        (activeGroup === 'net10.0' && title.includes('net10.0')) ||
        (activeGroup === 'net8.0' && title.includes('net8.0')) ||
        (activeGroup === 'netstandard2.0' && title.includes('netstandard2.0'))

      // 如果不是当前组且已展开，则收起
      if (!isCurrentGroup && !group.classList.contains('collapsed')) {
        const caretBtn = group.querySelector('.caret')
        if (caretBtn instanceof HTMLElement) {
          caretBtn.click()
        }
      }

      // 如果是当前组且已收起，则展开
      if (isCurrentGroup && group.classList.contains('collapsed')) {
        const caretBtn = group.querySelector('.caret')
        if (caretBtn instanceof HTMLElement) {
          caretBtn.click()
        }
      }
    })
  })
}

// 监听路由变化
watch(
  () => route.path,
  (newPath) => {
    const activeGroup = getActiveGroup(newPath)
    if (activeGroup) {
      collapseOtherGroups(activeGroup)
    }
  }
)

// 首次加载时也执行
onMounted(() => {
  const activeGroup = getActiveGroup(route.path)
  if (activeGroup) {
    // 延迟执行，确保 DOM 已渲染
    setTimeout(() => {
      collapseOtherGroups(activeGroup)
    }, 100)
  }
})
</script>

<template>
  <div style="display: none;"></div>
</template>
