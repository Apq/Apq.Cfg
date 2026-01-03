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
        <el-button @click="handleReload" :loading="reloading">
          <el-icon><Refresh /></el-icon>
          刷新
        </el-button>
        <el-button type="primary" @click="handleSave" :loading="saving">
          <el-icon><Check /></el-icon>
          保存
        </el-button>
        <el-dropdown @command="handleExport">
          <el-button>
            导出
            <el-icon class="el-icon--right"><ArrowDown /></el-icon>
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="json">JSON</el-dropdown-item>
              <el-dropdown-item command="env">ENV</el-dropdown-item>
              <el-dropdown-item command="kv">Key-Value</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-header>

    <div class="source-tabs">
      <el-radio-group v-model="currentSource" @change="loadConfig">
        <el-radio-button value="merged">合并后</el-radio-button>
        <el-radio-button
          v-for="source in sources"
          :key="`${source.level}-${source.name}`"
          :value="`${source.level}/${source.name}`"
        >
          {{ source.name }} ({{ source.level }})
          <el-tag v-if="source.isPrimaryWriter" size="small" type="success" style="margin-left: 5px">主写入</el-tag>
        </el-radio-button>
      </el-radio-group>
    </div>

    <el-main class="main" v-loading="loading">
      <el-row :gutter="20">
        <el-col :span="10">
          <el-card class="tree-card">
            <template #header>
              <div class="card-header">
                <span>配置树</span>
                <el-input
                  v-model="searchKey"
                  placeholder="搜索配置..."
                  clearable
                  style="width: 200px"
                >
                  <template #prefix>
                    <el-icon><Search /></el-icon>
                  </template>
                </el-input>
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

        <el-col :span="14">
          <el-card class="detail-card" v-if="selectedNode">
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

          <el-empty v-else description="点击左侧配置项查看详情" />
        </el-col>
      </el-row>
    </el-main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useAppsStore } from '@/stores/apps'
import { createConfigApi } from '@/api/config'
import type { ConfigTreeNode, ConfigSourceDto, AppEndpoint } from '@/types'

interface TreeNodeData extends ConfigTreeNode {
  fullKey: string
}

const route = useRoute()
const router = useRouter()
const appsStore = useAppsStore()

const appId = computed(() => route.params.id as string)
const currentApp = ref<AppEndpoint | null>(null)
const configApi = computed(() => createConfigApi(appId.value))

const loading = ref(false)
const saving = ref(false)
const reloading = ref(false)

const sources = ref<ConfigSourceDto[]>([])
const currentSource = ref('merged')
const treeData = ref<TreeNodeData[]>([])
const searchKey = ref('')

const selectedNode = ref<TreeNodeData | null>(null)
const editValue = ref('')

const filteredTreeData = computed(() => {
  if (!searchKey.value) return treeData.value
  return filterTree(treeData.value, searchKey.value.toLowerCase())
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
  await appsStore.fetchApps()
  currentApp.value = appsStore.apps.find(a => a.id === appId.value) || null
  await loadSources()
  await loadConfig()
})

async function loadSources() {
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

async function handleUpdateValue() {
  if (!selectedNode.value) return

  try {
    let res
    if (currentSource.value === 'merged') {
      res = await configApi.value.setValue(selectedNode.value.fullKey, editValue.value)
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
  if (!selectedNode.value) return

  try {
    await ElMessageBox.confirm(`确定要删除配置 "${selectedNode.value.fullKey}" 吗？`, '确认删除', {
      type: 'warning'
    })

    let res
    if (currentSource.value === 'merged') {
      res = await configApi.value.deleteKey(selectedNode.value.fullKey)
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

async function handleExport(format: string) {
  try {
    let content: string
    if (currentSource.value === 'merged') {
      content = await configApi.value.export(format)
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

.source-tabs {
  background: #fff;
  padding: 10px 20px;
  border-bottom: 1px solid #ebeef5;
}

.main {
  flex: 1;
  padding: 20px;
}

.tree-card, .detail-card {
  height: calc(100vh - 200px);
  overflow: auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
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
