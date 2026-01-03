<template>
  <el-container class="main-layout">
    <el-header class="header">
      <div class="header-content">
        <h1 class="title">Apq.Cfg 配置管理中心</h1>
        <el-menu
          mode="horizontal"
          :default-active="activeMenu"
          router
          class="nav-menu"
        >
          <el-menu-item index="/">应用管理</el-menu-item>
          <el-menu-item index="/self">本机配置</el-menu-item>
        </el-menu>
      </div>
    </el-header>
    <el-main class="main">
      <router-view />
    </el-main>
  </el-container>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const activeMenu = computed(() => {
  if (route.path === '/self') return '/self'
  if (route.path.startsWith('/app/')) return '/'
  return route.path
})
</script>

<style scoped>
.main-layout {
  min-height: 100vh;
}

.header {
  background: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 0 20px;
  height: 60px;
  line-height: 60px;
}

.header-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.title {
  font-size: 18px;
  margin: 0;
  margin-right: 40px;
  white-space: nowrap;
}

.nav-menu {
  border-bottom: none;
  flex: 1;
}

.nav-menu .el-menu-item {
  height: 60px;
  line-height: 60px;
}

.main {
  padding: 20px;
  background: #f5f7fa;
}
</style>
