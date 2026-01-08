<template>
  <div class="home-container">
    <div class="page-header">
      <div class="page-title">
        <h2>应用管理</h2>
      </div>
      <div class="header-actions">
        <el-dropdown @command="handleExport">
          <el-button>
            <el-icon><Download /></el-icon>
            导出
            <el-icon class="el-icon--right"><ArrowDown /></el-icon>
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="json">导出为 JSON</el-dropdown-item>
              <el-dropdown-item command="csv">导出为 CSV</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
        <el-button @click="showDropZone = !showDropZone">
          <el-icon><Upload /></el-icon>
          {{ showDropZone ? '收起' : '导入' }}
        </el-button>
        <el-button type="primary" @click="showAddDialog = true">
          <el-icon><Plus /></el-icon>
          添加应用
        </el-button>
      </div>
    </div>

    <!-- 拖放区域 -->
    <div
      v-if="showDropZone"
      class="drop-zone"
      :class="{ 'drop-zone-active': isDragging }"
      @dragover.prevent="onDragOver"
      @dragleave.prevent="onDragLeave"
      @drop.prevent="onDrop"
      @click="handleImport"
    >
      <el-icon :size="32"><Upload /></el-icon>
      <p>拖放文件到此处导入，或点击选择文件</p>
      <p class="drop-hint">支持 JSON、CSV 格式</p>
    </div>

    <el-row :gutter="20" v-loading="appsStore.loading">
        <el-col :xs="24" :sm="12" :md="8" :lg="6" v-for="app in appsStore.apps" :key="app.id">
          <el-card class="app-card" shadow="hover">
            <template #header>
              <div class="card-header">
                <span class="app-name">{{ app.name }}</span>
                <el-dropdown @command="handleCommand($event, app)">
                  <el-button text>
                    <el-icon><MoreFilled /></el-icon>
                  </el-button>
                  <template #dropdown>
                    <el-dropdown-menu>
                      <el-dropdown-item command="edit">编辑</el-dropdown-item>
                      <el-dropdown-item command="test">测试连接</el-dropdown-item>
                      <el-dropdown-item command="delete" divided>删除</el-dropdown-item>
                    </el-dropdown-menu>
                  </template>
                </el-dropdown>
              </div>
            </template>
            <div class="app-info">
              <p class="app-url">{{ app.url }}</p>
              <p class="app-auth">
                <el-tag size="small" :type="getAuthTagType(app.authType)">
                  {{ getAuthLabel(app.authType) }}
                </el-tag>
              </p>
              <p class="app-desc" v-if="app.description">{{ app.description }}</p>
            </div>
            <div class="app-actions">
              <el-button type="primary" @click="goToConfig(app.id)">查看配置</el-button>
            </div>
          </el-card>
        </el-col>

        <!-- 添加应用卡片 -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-card class="app-card add-card" shadow="hover" @click="showAddDialog = true">
            <div class="add-card-content">
              <el-icon :size="48"><Plus /></el-icon>
              <p>添加应用</p>
              <p class="add-hint">或点击上方"导入"按钮批量导入</p>
            </div>
          </el-card>
        </el-col>
      </el-row>

    <!-- 添加/编辑对话框 -->
    <el-dialog
      v-model="showAddDialog"
      :title="editingApp ? '编辑应用' : '添加应用'"
      width="500px"
      @close="resetForm"
    >
      <el-form :model="formData" label-width="100px">
        <el-form-item label="应用名称" required>
          <el-input v-model="formData.name" placeholder="如：订单服务" />
        </el-form-item>
        <el-form-item label="API 地址" required>
          <el-input v-model="formData.url" placeholder="如：http://localhost:5000/api/apqcfg" />
        </el-form-item>
        <el-form-item label="认证方式">
          <el-select v-model="formData.authType" style="width: 100%">
            <el-option label="无认证" :value="AuthType.None" />
            <el-option label="API Key" :value="AuthType.ApiKey" />
            <el-option label="JWT Bearer" :value="AuthType.JwtBearer" />
          </el-select>
        </el-form-item>
        <el-form-item label="API Key" v-if="formData.authType === AuthType.ApiKey">
          <el-input v-model="formData.apiKey" placeholder="输入 API Key" />
        </el-form-item>
        <el-form-item label="Token" v-if="formData.authType === AuthType.JwtBearer">
          <el-input v-model="formData.token" type="textarea" placeholder="输入 JWT Token" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="formData.description" type="textarea" placeholder="可选" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showAddDialog = false">取消</el-button>
        <el-button type="primary" @click="handleSave" :loading="saving">保存</el-button>
      </template>
    </el-dialog>

    <!-- 隐藏的文件输入 -->
    <input
      ref="fileInput"
      type="file"
      accept=".json,.csv"
      style="display: none"
      @change="handleFileChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, MoreFilled, Download, Upload, ArrowDown } from '@element-plus/icons-vue'
import { useAppsStore } from '@/stores/apps'
import { AuthType } from '@/types'
import type { AppEndpoint } from '@/types'
import { exportApps, importApps, type ExportFormat } from '@/utils/storage'

const router = useRouter()
const appsStore = useAppsStore()

const showAddDialog = ref(false)
const editingApp = ref<AppEndpoint | null>(null)
const saving = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)
const showDropZone = ref(false)

const formData = ref({
  name: '',
  url: '',
  authType: AuthType.None,
  apiKey: '',
  token: '',
  description: ''
})

// 生成不重复的默认应用名称
function generateDefaultAppName(): string {
  const baseName = '新应用'
  const existingNames = new Set(appsStore.apps.map(app => app.name))

  if (!existingNames.has(baseName)) {
    return baseName
  }

  let index = 1
  while (existingNames.has(`${baseName} ${index}`)) {
    index++
  }
  return `${baseName} ${index}`
}

// 监听对话框打开，设置默认值
watch(showAddDialog, (visible) => {
  if (visible && !editingApp.value) {
    // 新增模式，设置默认应用名称
    formData.value.name = generateDefaultAppName()
  }
})

function getAuthTagType(authType: AuthType) {
  switch (authType) {
    case AuthType.ApiKey: return 'warning'
    case AuthType.JwtBearer: return 'success'
    default: return 'info'
  }
}

function getAuthLabel(authType: AuthType) {
  switch (authType) {
    case AuthType.ApiKey: return 'API Key'
    case AuthType.JwtBearer: return 'JWT'
    default: return '无认证'
  }
}

function goToConfig(id: string) {
  router.push(`/app/${id}`)
}

function handleCommand(command: string, app: AppEndpoint) {
  switch (command) {
    case 'edit':
      editingApp.value = app
      formData.value = {
        name: app.name,
        url: app.url,
        authType: app.authType,
        apiKey: app.apiKey || '',
        token: app.token || '',
        description: app.description || ''
      }
      showAddDialog.value = true
      break
    case 'test':
      testConnection(app)
      break
    case 'delete':
      deleteApp(app)
      break
  }
}

async function testConnection(app: AppEndpoint) {
  try {
    const success = await appsStore.testConnection(app)
    if (success) {
      ElMessage.success('连接成功')
    } else {
      ElMessage.error('连接失败')
    }
  } catch {
    ElMessage.error('连接失败')
  }
}

async function deleteApp(app: AppEndpoint) {
  try {
    await ElMessageBox.confirm(`确定要删除应用 "${app.name}" 吗？`, '确认删除', {
      type: 'warning'
    })
    appsStore.deleteApp(app.id)
    ElMessage.success('删除成功')
  } catch {
    // 用户取消
  }
}

async function handleSave() {
  if (!formData.value.name || !formData.value.url) {
    ElMessage.warning('请填写必填项')
    return
  }

  saving.value = true
  try {
    // 构造临时应用对象用于测试连接
    const tempApp: AppEndpoint = {
      id: editingApp.value?.id || crypto.randomUUID(),
      name: formData.value.name,
      url: formData.value.url,
      authType: formData.value.authType,
      apiKey: formData.value.apiKey || undefined,
      token: formData.value.token || undefined,
      description: formData.value.description || undefined,
      createdAt: editingApp.value?.createdAt || new Date().toISOString()
    }

    // 先测试连接
    const connected = await appsStore.testConnection(tempApp)
    if (!connected) {
      ElMessage.error('无法连接到服务，请检查 API 地址和认证信息')
      return
    }

    // 连接成功，保存应用
    if (editingApp.value) {
      appsStore.updateApp(editingApp.value.id, formData.value)
      ElMessage.success('更新成功')
    } else {
      appsStore.addApp(formData.value)
      ElMessage.success('添加成功')
    }
    showAddDialog.value = false
    resetForm()
  } catch {
    ElMessage.error('操作失败')
  } finally {
    saving.value = false
  }
}

function resetForm() {
  editingApp.value = null
  formData.value = {
    name: '',
    url: '',
    authType: AuthType.None,
    apiKey: '',
    token: '',
    description: ''
  }
}

function handleExport(format: ExportFormat) {
  exportApps(format)
  ElMessage.success(`导出 ${format.toUpperCase()} 成功`)
}

function handleImport() {
  fileInput.value?.click()
}

async function handleFileChange(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  await processImportFile(file)
  // 清空文件输入
  input.value = ''
}

// 拖放事件处理
function onDragOver(event: DragEvent) {
  if (event.dataTransfer?.types.includes('Files')) {
    isDragging.value = true
  }
}

function onDragLeave() {
  isDragging.value = false
}

async function onDrop(event: DragEvent) {
  isDragging.value = false
  const file = event.dataTransfer?.files[0]
  if (!file) return

  // 检查文件类型
  const ext = file.name.split('.').pop()?.toLowerCase()
  if (ext !== 'json' && ext !== 'csv') {
    ElMessage.warning('仅支持 JSON 和 CSV 格式文件')
    return
  }

  await processImportFile(file)
}

async function processImportFile(file: File) {
  try {
    await importApps(file)
    // 重新加载应用列表
    window.location.reload()
  } catch (error: unknown) {
    const message = error instanceof Error ? error.message : '导入失败'
    ElMessage.error(message)
  }
}
</script>

<style scoped>
.home-container {
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

.drop-zone {
  border: 2px dashed #dcdfe6;
  border-radius: 8px;
  padding: 20px;
  text-align: center;
  color: #909399;
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: 20px;
}

.drop-zone:hover {
  border-color: #409eff;
  color: #409eff;
}

.drop-zone-active {
  border-color: #409eff;
  background: rgba(64, 158, 255, 0.1);
  color: #409eff;
}

.drop-zone p {
  margin: 8px 0 0;
  font-size: 14px;
}

.drop-zone .drop-hint {
  font-size: 12px;
  color: #c0c4cc;
}

.drop-zone-active .drop-hint {
  color: #79bbff;
}

.app-card {
  margin-bottom: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.app-name {
  font-weight: bold;
  font-size: 16px;
}

.app-info {
  margin-bottom: 15px;
}

.app-url {
  color: #909399;
  font-size: 12px;
  word-break: break-all;
  margin: 5px 0;
}

.app-auth {
  margin: 10px 0;
}

.app-desc {
  color: #606266;
  font-size: 13px;
  margin: 5px 0;
}

.app-actions {
  text-align: center;
}

.add-card {
  cursor: pointer;
  border: 2px dashed #dcdfe6;
  background: transparent;
}

.add-card:hover {
  border-color: #409eff;
  background: rgba(64, 158, 255, 0.05);
}

.add-card :deep(.el-card__body) {
  padding: 0;
}

.add-card-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 180px;
  color: #909399;
}

.add-card:hover .add-card-content {
  color: #409eff;
}

.add-card-content p {
  margin: 12px 0 0;
  font-size: 14px;
}

.add-card-content .add-hint {
  margin-top: 16px;
  font-size: 12px;
  color: #c0c4cc;
}
</style>
