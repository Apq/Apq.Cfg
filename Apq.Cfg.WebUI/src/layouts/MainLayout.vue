<template>
  <el-container class="main-layout">
    <el-header class="header">
      <div class="header-content">
        <h1 class="title" @click="goHome">Apq.Cfg 配置管理中心</h1>
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
}

.header-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.title {
  font-size: 18px;
  margin: 0;
  cursor: pointer;
  user-select: none;
}

.title:hover {
  color: #409eff;
}

.main {
  padding: 20px;
  background: #f5f7fa;
}
</style>
