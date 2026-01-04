<template>
  <el-container class="main-layout">
    <el-header class="header">
      <div class="header-content">
        <div class="logo-title" @click="goHome">
          <img src="/logo.svg" alt="Logo" class="logo" />
          <h1 class="title">Apq.Cfg 配置管理中心</h1>
        </div>
        <span class="storage-notice">数据仅存于浏览器本地，配置请求直连应用API，不经服务器中转</span>
      </div>
    </el-header>
    <el-main class="main">
      <router-view />
    </el-main>
  </el-container>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goHome() {
  router.push('/')
}
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
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.logo-title {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  user-select: none;
}

.logo-title:hover .title {
  color: #409eff;
}

.logo {
  width: 36px;
  height: 36px;
}

.title {
  font-size: 18px;
  margin: 0;
  transition: color 0.2s;
}

.storage-notice {
  font-size: 12px;
  color: #c0c4cc;
  margin-left: 20px;
}

.main {
  padding: 20px;
  background: #f5f7fa;
}
</style>
