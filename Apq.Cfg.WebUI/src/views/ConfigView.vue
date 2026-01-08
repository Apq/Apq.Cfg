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
        <el-aside class="source-aside" :width="sourceCollapsed ? '64px' : '240px'">
          <el-menu
            ref="sourceMenuRef"
            :default-active="currentSource"
            :collapse="sourceCollapsed"
            class="source-menu"
            @select="handleSourceSelect"
          >
            <!-- 顶部操作栏 -->
            <div class="menu-header">
              <template v-if="!sourceCollapsed">
                <span class="menu-title">配置源</span>
                <el-button text size="small" @click="handleRefreshSources" :loading="refreshingSources">
                  <el-icon><Refresh /></el-icon>
                  刷新
                </el-button>
              </template>
              <el-button text @click="sourceCollapsed = !sourceCollapsed">
                <el-icon><Expand v-if="sourceCollapsed" /><Fold v-else /></el-icon>
              </el-button>
            </div>

            <!-- 合并后 -->
            <el-menu-item index="merged">
              <el-icon><Files /></el-icon>
              <template #title>合并后</template>
            </el-menu-item>

            <!-- 按层级分组 -->
            <el-sub-menu v-for="levelGroup in sourceLevelGroups" :key="levelGroup.level" :index="`level-${levelGroup.level}`">
              <template #title>
                <el-icon><Folder /></el-icon>
                <span>Level {{ levelGroup.level }}</span>
              </template>
              <el-menu-item
                v-for="source in levelGroup.sources"
                :key="source.value"
                :index="source.value"
              >
                <el-icon><Document /></el-icon>
                <template #title>
                  <span>{{ source.name }}</span>
                  <el-tag v-if="source.isPrimaryWriter" size="small" type="success" class="primary-tag">主写入</el-tag>
                  <el-tag v-else-if="!source.isWriteable" size="small" type="info" class="readonly-tag">只读</el-tag>
                </template>
              </el-menu-item>
            </el-sub-menu>
          </el-menu>
        </el-aside>

        <!-- 主内容区域 -->
        <div class="main-content">
          <el-row :gutter="16">
            <!-- 配置树列 -->
            <el-col :span="selectedNode ? 12 : 24">
              <el-card class="tree-card">
                <template #header>
                  <div class="card-header">
                    <div class="card-header-title">
                      <span>{{ currentSourceInfo.name }}</span>
                      <el-tag v-if="currentSourceInfo.level !== null" size="small" type="info">Level {{ currentSourceInfo.level }}</el-tag>
                      <el-tag v-if="currentSourceInfo.isPrimaryWriter" size="small" type="success">主写入</el-tag>
                    </div>
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
                      :disabled="selectedNode.isMasked || !currentSourceInfo.isWriteable"
                      :placeholder="selectedNode.isMasked ? '敏感值已脱敏' : (!currentSourceInfo.isWriteable ? '此配置源为只读' : '')"
                    />
                  </el-form-item>
                  <el-form-item v-if="selectedNode.isMasked">
                    <el-tag type="warning">此值已脱敏，无法编辑</el-tag>
                  </el-form-item>
                  <el-form-item v-else-if="!currentSourceInfo.isWriteable">
                    <el-tag type="info">此配置源为只读，无法编辑</el-tag>
                  </el-form-item>
                  <el-form-item>
                    <el-button type="primary" @click="handleUpdateValue" :disabled="selectedNode.isMasked || !currentSourceInfo.isWriteable">
                      更新
                    </el-button>
                    <el-button type="danger" @click="handleDeleteKey" :disabled="selectedNode.isMasked || !currentSourceInfo.isWriteable">
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
import { ref, computed, onMounted, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { MenuInstance } from 'element-plus'
import { ArrowLeft, Refresh, Check, ArrowDown, Search, Lock, Fold, Expand, Files, Folder, Document } from '@element-plus/icons-vue'
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
const sourceMenuRef = ref<MenuInstance | null>(null)

const filteredTreeData = computed(() => {
  if (!searchKey.value) return treeData.value
  return filterTree(treeData.value, searchKey.value.toLowerCase())
})

// 按层级分组配置源
const sourceLevelGroups = computed(() => {
  const levelMap = new Map<number, ConfigSourceDto[]>()

  for (const source of sources.value) {
    if (!levelMap.has(source.level)) {
      levelMap.set(source.level, [])
    }
    levelMap.get(source.level)!.push(source)
  }

  // 按层级排序
  const sortedLevels = Array.from(levelMap.keys()).sort((a, b) => a - b)
  return sortedLevels.map(level => ({
    level,
    sources: levelMap.get(level)!.map(source => ({
      value: `${source.level}/${source.name}`,
      name: source.name,
      isPrimaryWriter: source.isPrimaryWriter,
      isWriteable: source.isWriteable
    }))
  }))
})

// 展开所有配置源菜单
function expandAllSourceMenus() {
  nextTick(() => {
    if (sourceMenuRef.value) {
      sourceLevelGroups.value.forEach(group => {
        sourceMenuRef.value?.open(`level-${group.level}`)
      })
    }
  })
}

// 当前选中配置源的信息
const currentSourceInfo = computed(() => {
  if (currentSource.value === 'merged') {
    return { name: '合并后', level: null, isPrimaryWriter: false, isWriteable: true }
  }
  const [level, name] = currentSource.value.split('/')
  const source = sources.value.find(s => s.level === parseInt(level) && s.name === name)
  return {
    name: source?.name || name,
    level: parseInt(level),
    isPrimaryWriter: source?.isPrimaryWriter || false,
    isWriteable: source?.isWriteable || false
  }
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
      // 数据加载后展开所有菜单
      expandAllSourceMenus()
    }
  } catch (e) {
    console.error('Failed to load sources', e)
  }
}

async function loadConfig(keepSelection = false) {
  if (!configApi.value) return
  loading.value = true
  const previousSelectedKey = keepSelection ? selectedNode.value?.fullKey : null
  if (!keepSelection) {
    selectedNode.value = null
  }
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

      // 如果需要保持选中状态，重新查找并选中节点
      if (previousSelectedKey) {
        const findNode = (nodes: TreeNodeData[]): TreeNodeData | null => {
          for (const node of nodes) {
            if (node.fullKey === previousSelectedKey) return node
            if (node.children) {
              const found = findNode(node.children as TreeNodeData[])
              if (found) return found
            }
          }
          return null
        }
        const foundNode = findNode(treeData.value)
        if (foundNode) {
          selectedNode.value = foundNode
          editValue.value = foundNode.value || ''
        }
      }
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

// 选择配置源菜单项
function handleSourceSelect(index: string) {
  // 忽略 level 分组项
  if (index.startsWith('level-')) return
  currentSource.value = index
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
      // 保存到文件
      const saveRes = await configApi.value.save()
      if (saveRes.success) {
        ElMessage.success('更新成功')
        await loadConfig(true)
      } else {
        ElMessage.error(saveRes.error || '保存失败')
      }
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
      // 保存到文件
      const saveRes = await configApi.value.save()
      if (saveRes.success) {
        ElMessage.success('删除成功')
        selectedNode.value = null
        await loadConfig()
      } else {
        ElMessage.error(saveRes.error || '保存失败')
      }
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

.source-aside {
  background: #fff;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  overflow: auto;
  transition: width 0.3s;
}

.source-menu {
  height: auto;
  min-height: 100%;
  border-right: none;
  overflow-x: auto;
}

.source-menu:not(.el-menu--collapse) {
  width: 240px;
}

.menu-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 20px;
  border-bottom: 1px solid var(--el-menu-border-color);
}

.menu-title {
  font-weight: 500;
  font-size: 14px;
  color: #303133;
}

.primary-tag {
  margin-left: 8px;
}

.readonly-tag {
  margin-left: 8px;
}

.main-content {
  flex: 1;
  min-width: 0;
  overflow: auto;
}

.tree-card, .detail-card {
  height: calc(100vh - 140px);
}

.tree-card :deep(.el-card__body),
.detail-card :deep(.el-card__body) {
  overflow: auto;
  height: calc(100% - 60px);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-header-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 500;
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
