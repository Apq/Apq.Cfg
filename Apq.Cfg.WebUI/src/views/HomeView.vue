<template>
  <div class="home-container">
    <div class="page-header">
      <h2>应用管理</h2>
      <div class="header-actions">
        <el-button @click="handleExport">
          <el-icon><Download /></el-icon>
          导出
        </el-button>
        <el-button @click="handleImport">
          <el-icon><Upload /></el-icon>
          导入
        </el-button>
        <el-button type="primary" @click="showAddDialog = true">
          <el-icon><Plus /></el-icon>
          添加应用
        </el-button>
      </div>
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

        <el-col :xs="24" :sm="12" :md="8" :lg="6" v-if="appsStore.apps.length === 0 && !appsStore.loading">
          <el-empty description="暂无应用，点击上方按钮添加" />
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
      accept=".json"
      style="display: none"
      @change="handleFileChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, MoreFilled, Download, Upload } from '@element-plus/icons-vue'
import { useAppsStore } from '@/stores/apps'
import { AuthType } from '@/types'
import type { AppEndpoint } from '@/types'
import { exportApps, importApps } from '@/utils/storage'

const router = useRouter()
const appsStore = useAppsStore()

const showAddDialog = ref(false)
const editingApp = ref<AppEndpoint | null>(null)
const saving = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const formData = ref({
  name: '',
  url: '',
  authType: AuthType.None,
  apiKey: '',
  token: '',
  description: ''
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

function handleSave() {
  if (!formData.value.name || !formData.value.url) {
    ElMessage.warning('请填写必填项')
    return
  }

  saving.value = true
  try {
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

function handleExport() {
  exportApps()
  ElMessage.success('导出成功')
}

function handleImport() {
  fileInput.value?.click()
}

async function handleFileChange(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  try {
    await importApps(file)
    // 重新加载应用列表（刷新页面以重新从 localStorage 加载）
    window.location.reload()
  } catch (error: any) {
    ElMessage.error(error.message || '导入失败')
  } finally {
    // 清空文件输入
    input.value = ''
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
</style>
