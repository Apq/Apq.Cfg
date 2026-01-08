<template>
  <div class="config-container">
    <el-header class="header">
      <div class="header-left">
        <el-button text @click="goBack">
          <el-icon><ArrowLeft /></el-icon>
          返回
        </el-button>
        <span class="app-name">{{ currentApp?.name || '加载中...' }}</span>
      </div>
      <div class="header-right">
      </div>
    </el-header>

    <el-main class="main" v-loading="loading">
      <div class="content-wrapper">
        <!-- 配置源侧边栏 -->
        <div class="source-sidebar" :class="{ collapsed: sourceCollapsed }">
          <div class="source-header">
            <span v-if="!sourceCollapsed" class="source-title">配置源</span>
            <div class="source-header-actions">
              <el-button
                v-if="!sourceCollapsed"
                text
                size="small"
                @click="handleRefreshSources"
                :loading="refreshingSources"
              >
                <el-icon><Refresh /></el-icon>
                刷新
              </el-button>
              <el-button
                text
                class="collapse-btn"
                @click="sourceCollapsed = !sourceCollapsed"
              >
                <el-icon><DArrowLeft v-if="!sourceCollapsed" /><DArrowRight v-else /></el-icon>
              </el-button>
            </div>
          </div>
          <div v-if="!sourceCollapsed" class="source-content">
            <el-tree
              :data="sourceTreeData"
              :props="{ label: 'label', children: 'children', disabled: 'disabled' }"
              node-key="value"
              default-expand-all
              highlight-current
              :current-node-key="currentSource"
              @node-click="handleSourceClick"
            >
              <template #default="{ data }">
                <span class="source-node">
                  <span>{{ data.label }}</span>
                  <el-tag v-if="data.isPrimaryWriter" size="small" type="success" style="margin-left: 8px">主写入</el-tag>
                </span>
              </template>
            </el-tree>
          </div>
        </div>

        <!-- 主内容区域 -->
        <div class="main-content">
          <el-row :gutter="16">
            <!-- 配置树列 -->
            <el-col :span="selectedNode ? 12 : 24">
              <el-card class="tree-card">
                <template #header>
                  <div class="card-header">
                    <span>配置树</span>
                    <div class="card-header-actions">
                      <el-input
                        v-model="searchKey"
                        placeholder="搜索配置..."
                        clearable
                        style="width: 120px"
                      >
                        <template #prefix>
                          <el-icon><Search /></el-icon>
                        </template>
                      </el-input>
                      <el-button size="small" @click="handleReload" :loading="reloading">
                        <el-icon><Refresh /></el-icon>
                        刷新
                      </el-button>
                      <el-button size="small" type="primary" @click="handleSave" :loading="saving">
                        <el-icon><Check /></el-icon>
                        保存
                      </el-button>
                      <el-dropdown size="small" @command="handleExport">
                        <el-button size="small">
                          导出
                          <el-icon class="el-icon--right"><ArrowDown /></el-icon>
                        </el-button>
                        <template #dropdown>
                          <el-dropdown-menu>
                            <el-dropdown-item command="json">导出 JSON</el-dropdown-item>
                            <el-dropdown-item command="env">导出 ENV</el-dropdown-item>
                            <el-dropdown-item command="kv">导出 Key-Value</el-dropdown-item>
                          </el-dropdown-menu>
                        </template>
                      </el-dropdown>
                    </div>
                  </div>
                </template>
                <el-tree
                  :data="filteredTreeData"
                  :props="{ label: 'key', children: 'children' }"
                  node-key="fullKey"
                  default-expand-all
                  highlight-current
                  @node-click="handleNodeClick"
                >
                  <template #default="{ data }">
                    <span class="tree-node">
                      <span>{{ data.key }}</span>
                      <el-icon v-if="data.isMasked" class="masked-icon"><Lock /></el-icon>
                    </span>
                  </template>
                </el-tree>
              </el-card>
            </el-col>

            <!-- 配置详情列 -->
            <el-col :span="12" v-if="selectedNode">
              <el-card class="detail-card">
                <template #header>
                  <span>配置详情</span>
                </template>
                <el-form label-width="80px">
                  <el-form-item label="键">
                    <el-input :value="selectedNode.fullKey" readonly />
                  </el-form-item>
                  <el-form-item label="值">
                    <el-input
                      v-model="editValue"
                      type="textarea"
                      :rows="4"
                      :disabled="selectedNode.isMasked"
                      :placeholder="selectedNode.isMasked ? '敏感值已脱敏' : ''"
                    />
                  </el-form-item>
                  <el-form-item v-if="selectedNode.isMasked">
                    <el-tag type="warning">此值已脱敏，无法编辑</el-tag>
                  </el-form-item>
                  <el-form-item>
                    <el-button type="primary" @click="handleUpdateValue" :disabled="selectedNode.isMasked">
                      更新
                    </el-button>
                    <el-button type="danger" @click="handleDeleteKey" :disabled="selectedNode.isMasked">
                      删除
                    </el-button>
                  </el-form-item>
                </el-form>
              </el-card>
            </el-col>
          </el-row>
        </div>
      </div>
    </el-main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { ArrowLeft, Refresh, Check, ArrowDown, Search, Lock, DArrowLeft, DArrowRight } from '@element-plus/icons-vue'
import { useAppsStore } from '@/stores/apps'
import { createConfigApi } from '@/api/config'
import type { ConfigTreeNode, ConfigSourceDto } from '@/types'

interface TreeNodeData extends ConfigTreeNode {
  fullKey: string
}

const route = useRoute()
const router = useRouter()
const appsStore = useAppsStore()

const appId = computed(() => route.params.id as string)
const currentApp = computed(() => appsStore.getApp(appId.value))
const configApi = computed(() => {
  const app = currentApp.value
  if (!app) return null
  return createConfigApi(app)
})

const loading = ref(false)
const saving = ref(false)
const reloading = ref(false)
const refreshingSources = ref(false)

const sources = ref<ConfigSourceDto[]>([])
const currentSource = ref('merged')
const treeData = ref<TreeNodeData[]>([])
const searchKey = ref('')

const selectedNode = ref<TreeNodeData | null>(null)
const editValue = ref('')
const sourceCollapsed = ref(false)

const filteredTreeData = computed(() => {
  if (!searchKey.value) return treeData.value
  return filterTree(treeData.value, searchKey.value.toLowerCase())
})

// 按层级分组配置源（树形结构，合并后作为根节点）
const sourceTreeData = computed(() => {
  const levelMap = new Map<number, ConfigSourceDto[]>()

  for (const source of sources.value) {
    if (!levelMap.has(source.level)) {
      levelMap.set(source.level, [])
    }
    levelMap.get(source.level)!.push(source)
  }

  // 按层级排序，生成树形数据
  const sortedLevels = Array.from(levelMap.keys()).sort((a, b) => a - b)
  const levelNodes = sortedLevels.map(level => ({
    value: `level-${level}`,
    label: `Level ${level}`,
    disabled: true,
    children: levelMap.get(level)!.map(source => ({
      value: `${source.level}/${source.name}`,
      label: source.name,
      isPrimaryWriter: source.isPrimaryWriter
    }))
  }))

  // 合并后作为根节点
  return [{
    value: 'merged',
    label: '合并后',
    children: levelNodes
  }]
})

function filterTree(nodes: TreeNodeData[], keyword: string): TreeNodeData[] {
  return nodes.reduce((acc: TreeNodeData[], node) => {
    const matchesKey = node.fullKey.toLowerCase().includes(keyword)
    const filteredChildren = node.children ? filterTree(node.children as TreeNodeData[], keyword) : []

    if (matchesKey || filteredChildren.length > 0) {
      acc.push({
        ...node,
        children: filteredChildren.length > 0 ? filteredChildren : node.children
      } as TreeNodeData)
    }
    return acc
  }, [])
}

onMounted(async () => {
  if (!currentApp.value) {
    ElMessage.error('应用不存在')
    router.push('/')
    return
  }
  await loadSources()
  await loadConfig()
})

async function loadSources() {
  if (!configApi.value) return
  try {
    const res = await configApi.value.getSources()
    if (res.success && res.data) {
      sources.value = res.data
    }
  } catch (e) {
    console.error('Failed to load sources', e)
  }
}

async function loadConfig() {
  if (!configApi.value) return
  loading.value = true
  selectedNode.value = null
  try {
    let res
    if (currentSource.value === 'merged') {
      res = await configApi.value.getMergedTree()
    } else {
      const [level, name] = currentSource.value.split('/')
      res = await configApi.value.getSourceTree(parseInt(level), name)
    }

    if (res.success && res.data) {
      treeData.value = processTree(res.data.children || [], '')
    }
  } catch (e) {
    ElMessage.error('加载配置失败')
  } finally {
    loading.value = false
  }
}

function processTree(nodes: ConfigTreeNode[], parentKey: string): TreeNodeData[] {
  return nodes.map(node => {
    const fullKey = parentKey ? `${parentKey}:${node.key}` : node.key
    return {
      ...node,
      fullKey,
      children: node.children ? processTree(node.children, fullKey) : []
    } as TreeNodeData
  })
}

function handleNodeClick(data: TreeNodeData) {
  if (data.hasValue) {
    selectedNode.value = data
    editValue.value = data.value || ''
  }
}

// 点击配置源树节点
function handleSourceClick(data: { value: string; disabled?: boolean }) {
  if (data.disabled) return
  currentSource.value = data.value
  loadConfig()
}

async function handleUpdateValue() {
  if (!selectedNode.value || !configApi.value) return

  try {
    let res
    if (currentSource.value === 'merged') {
      res = await configApi.value.setMergedValue(selectedNode.value.fullKey, editValue.value)
    } else {
      const [level, name] = currentSource.value.split('/')
      res = await configApi.value.setSourceValue(parseInt(level), name, selectedNode.value.fullKey, editValue.value)
    }

    if (res.success) {
      ElMessage.success('更新成功')
      await loadConfig()
    } else {
      ElMessage.error(res.error || '更新失败')
    }
  } catch {
    ElMessage.error('更新失败')
  }
}

async function handleDeleteKey() {
  if (!selectedNode.value || !configApi.value) return

  try {
    await ElMessageBox.confirm(`确定要删除配置 "${selectedNode.value.fullKey}" 吗？`, '确认删除', {
      type: 'warning'
    })

    let res
    if (currentSource.value === 'merged') {
      res = await configApi.value.deleteMergedKey(selectedNode.value.fullKey)
    } else {
      const [level, name] = currentSource.value.split('/')
      res = await configApi.value.deleteSourceKey(parseInt(level), name, selectedNode.value.fullKey)
    }

    if (res.success) {
      ElMessage.success('删除成功')
      selectedNode.value = null
      await loadConfig()
    } else {
      ElMessage.error(res.error || '删除失败')
    }
  } catch {
    // 用户取消
  }
}

async function handleSave() {
  if (!configApi.value) return
  saving.value = true
  try {
    const res = await configApi.value.save()
    if (res.success) {
      ElMessage.success('保存成功')
    } else {
      ElMessage.error(res.error || '保存失败')
    }
  } catch {
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

async function handleReload() {
  if (!configApi.value) return
  reloading.value = true
  try {
    const res = await configApi.value.reload()
    if (res.success) {
      ElMessage.success('刷新成功')
      await loadConfig()
    } else {
      ElMessage.error(res.error || '刷新失败')
    }
  } catch {
    ElMessage.error('刷新失败')
  } finally {
    reloading.value = false
  }
}

async function handleRefreshSources() {
  refreshingSources.value = true
  try {
    await loadSources()
    ElMessage.success('配置源刷新成功')
  } catch {
    ElMessage.error('配置源刷新失败')
  } finally {
    refreshingSources.value = false
  }
}

async function handleExport(format: string) {
  if (!configApi.value) return
  try {
    let content: string
    if (currentSource.value === 'merged') {
      content = await configApi.value.exportMerged(format)
    } else {
      const [level, name] = currentSource.value.split('/')
      content = await configApi.value.exportSource(parseInt(level), name, format)
    }

    // 下载文件
    const blob = new Blob([content], { type: 'text/plain' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `config.${format === 'kv' ? 'txt' : format}`
    a.click()
    URL.revokeObjectURL(url)
  } catch {
    ElMessage.error('导出失败')
  }
}

function goBack() {
  router.push('/')
}
</script>

<style scoped>
.config-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #fff;
  padding: 0 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-right {
  display: flex;
  gap: 10px;
}

.app-name {
  font-size: 18px;
  font-weight: bold;
}

.main {
  flex: 1;
  padding: 20px;
}

.content-wrapper {
  display: flex;
  gap: 16px;
  height: calc(100vh - 140px);
}

.source-sidebar {
  width: 240px;
  min-width: 240px;
  background: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease, min-width 0.3s ease;
  overflow: hidden;
}

.source-sidebar.collapsed {
  width: 48px;
  min-width: 48px;
}

.source-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  border-bottom: 1px solid #ebeef5;
  min-height: 48px;
  box-sizing: border-box;
}

.source-header-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}

.source-sidebar.collapsed .source-header {
  justify-content: center;
  padding: 12px 8px;
}

.source-title {
  font-weight: 500;
  font-size: 14px;
  color: #303133;
}

.collapse-btn {
  padding: 4px;
}

.source-content {
  flex: 1;
  overflow: auto;
  padding: 8px 0;
}

.main-content {
  flex: 1;
  min-width: 0;
}

.tree-card, .detail-card {
  height: calc(100vh - 140px);
  overflow: auto;
}

.source-node {
  display: flex;
  align-items: center;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.tree-node {
  display: flex;
  align-items: center;
  gap: 5px;
}

.masked-icon {
  color: #e6a23c;
}
</style>
