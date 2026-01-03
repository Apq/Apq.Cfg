<template>
  <div class="self-config-container">
    <div class="page-header">
      <h2>本机配置</h2>
      <div class="header-actions">
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
    </div>

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

    <div class="main-content" v-loading="loading">
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
                  <el-tag v-if="data.hasValue" size="small" type="info" style="margin-left: 5px">
                    {{ truncateValue(data.value) }}
                  </el-tag>
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
                />
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="handleUpdateValue">
                  更新
                </el-button>
              </el-form-item>
            </el-form>
          </el-card>

          <el-empty v-else description="点击左侧配置项查看详情" />
        </el-col>
      </el-row>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { selfConfigApi, type ConfigSource, type ConfigTreeNode } from '@/api/selfConfig'

interface TreeNodeData extends ConfigTreeNode {
  fullKey: string
  hasValue?: boolean
}

const loading = ref(false)
const saving = ref(false)
const reloading = ref(false)

const sources = ref<ConfigSource[]>([])
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

function truncateValue(value?: string): string {
  if (!value) return ''
  return value.length > 20 ? value.substring(0, 20) + '...' : value
}

onMounted(async () => {
  await loadSources()
  await loadConfig()
})

async function loadSources() {
  try {
    const res = await selfConfigApi.getSources()
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
      res = await selfConfigApi.getMergedTree()
    } else {
      const [level, name] = currentSource.value.split('/')
      res = await selfConfigApi.getSourceTree(parseInt(level), name)
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
    const hasValue = node.value !== undefined && node.value !== null
    return {
      ...node,
      fullKey,
      hasValue,
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
    const res = await selfConfigApi.setValue(selectedNode.value.fullKey, editValue.value)

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

async function handleSave() {
  saving.value = true
  try {
    const res = await selfConfigApi.save()
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
    const res = await selfConfigApi.reload()
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
    const content = await selfConfigApi.export(format as 'json' | 'env' | 'kv')

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
</script>

<style scoped>
.self-config-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  font-size: 20px;
}

.header-actions {
  display: flex;
  gap: 10px;
}

.source-tabs {
  background: #fff;
  padding: 10px 15px;
  border-radius: 4px;
  margin-bottom: 20px;
}

.main-content {
  flex: 1;
}

.tree-card, .detail-card {
  height: calc(100vh - 280px);
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
</style>
