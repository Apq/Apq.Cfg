# Apq.Cfg.WebUI 设计文档

## 一、项目概述

### 1.1 项目定位

Apq.Cfg.WebUI 是 Apq.Cfg 配置组件库的 Web 管理界面，提供集中化的配置管理能力。它是一个**纯前端静态站点**，可以连接和管理多个使用 Apq.Cfg 的应用程序的配置。

### 1.2 核心功能

| 功能模块 | 描述 |
|---------|------|
| 多应用管理 | 管理多个应用端点，支持添加、编辑、删除、测试连接 |
| 配置树视图 | 以树形结构展示配置层级，支持搜索和过滤 |
| 实时编辑 | 在线编辑配置值，支持新增、修改、删除操作 |
| 配置源切换 | 查看合并后配置或单个配置源的配置 |
| 敏感值脱敏 | 自动识别并脱敏显示敏感配置（密码、密钥等） |
| 多格式导出 | 支持 JSON、ENV、KV 三种格式导出 |
| 认证支持 | 支持无认证、API Key、JWT Bearer 三种认证方式 |

### 1.3 技术栈

```
┌─────────────────────────────────────────────────────────────┐
│                   纯前端静态站点 (Vue 3)                      │
├─────────────────────────────────────────────────────────────┤
│  Vue 3.5  │  TypeScript 5.9  │  Element Plus 2.13  │  Vite 7 │
│  Vue Router 4.6  │  Pinia 3.0  │  Axios 1.13  │  Sass       │
└─────────────────────────────────────────────────────────────┘
```

---

## 二、系统架构

### 2.1 设计原则

- **纯静态站点**：无后端服务，可部署到任何静态文件托管服务
- **客户端存储**：应用端点信息（包括认证凭据）保存在浏览器 localStorage
- **客户端直连**：前端直接访问各应用的配置 API

### 2.2 整体架构图

```
┌──────────────────────────────────────────────────────────────────┐
│                           用户浏览器                              │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │                    Vue 3 前端 (SPA)                         │  │
│  │  ┌──────────────────┐  ┌──────────────────────────────┐    │  │
│  │  │     应用管理      │  │         配置视图              │    │  │
│  │  └──────────────────┘  └──────────────────────────────┘    │  │
│  │           │                                                 │  │
│  │           ▼                                                 │  │
│  │  ┌──────────────┐                                           │  │
│  │  │ localStorage │  ← 应用列表、认证信息                      │  │
│  │  └──────────────┘                                           │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
                                    │
                                    │ 直接访问远程应用
                                    ▼
                    ┌──────────────────────────────┐
                    │       远程应用 (多个)         │
                    │  ┌────────┐ ┌────────┐       │
                    │  │ 应用 A │ │ 应用 B │ ...   │
                    │  │/apqcfg │ │/apqcfg │       │
                    │  └────────┘ └────────┘       │
                    └──────────────────────────────┘
```

### 2.3 请求流程

```
┌─────────────────────────────────────────────────────────────────┐
│                         用户操作                                 │
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                       Vue 前端 (SPA)                             │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ 1. 从 localStorage 读取应用列表和认证信息                     ││
│  │ 2. 直接访问远程应用的 /api/apqcfg/*                          ││
│  │ 3. 添加认证头（ApiKey 或 JWT Bearer）                        ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                                │
                                ▼
                    ┌───────────────────┐
                    │     远程应用       │
                    │  /api/apqcfg/*    │
                    └───────────────────┘
```

---

## 三、目录结构

```
Apq.Cfg.WebUI/
├── README.md                         # 使用文档
├── DESIGN.md                         # 设计文档（本文件）
├── index.html                        # HTML 入口
├── package.json                      # 依赖配置
├── package-lock.json                 # 依赖锁定
├── vite.config.ts                    # Vite 配置
├── tsconfig.json                     # TypeScript 配置
├── tsconfig.node.json                # Node TypeScript 配置
├── src/                              # 源代码目录
│   ├── main.ts                       # 应用入口
│   ├── App.vue                       # 根组件
│   ├── api/                          # API 接口层
│   │   └── config.ts                 # 配置 API（直接访问远程应用）
│   ├── components/                   # 公共组件
│   ├── layouts/                      # 布局组件
│   │   └── MainLayout.vue            # 主布局
│   ├── router/                       # 路由配置
│   │   └── index.ts                  # 路由定义
│   ├── stores/                       # 状态管理
│   │   └── apps.ts                   # 应用状态（localStorage 持久化）
│   ├── types/                        # 类型定义
│   │   └── index.ts                  # TypeScript 类型
│   ├── utils/                        # 工具函数
│   │   ├── request.ts                # Axios 封装
│   │   └── storage.ts                # localStorage 封装
│   └── views/                        # 页面视图
│       ├── HomeView.vue              # 首页（应用管理）
│       └── ConfigView.vue            # 配置详情页
├── dist/                             # 构建输出（可直接部署）
└── node_modules/                     # 依赖包（不提交到 Git）
```

---

## 四、前端设计

### 4.1 路由设计

```typescript
const routes = [
  {
    path: '/',
    component: MainLayout,
    children: [
      { path: '', name: 'home', component: HomeView },
      { path: 'app/:id', name: 'config', component: ConfigView }
    ]
  }
]
```

### 4.2 数据模型

#### AppEndpoint（应用端点）

```typescript
interface AppEndpoint {
  id: string           // 唯一标识 (UUID)
  name: string         // 应用名称
  url: string          // WebApi 地址（如 http://localhost:5000/api/apqcfg）
  authType: AuthType   // 认证方式
  apiKey?: string      // API Key（authType 为 ApiKey 时使用）
  token?: string       // JWT Token（authType 为 JwtBearer 时使用）
  description?: string // 备注说明
  createdAt: string    // 创建时间 (ISO 8601)
}

enum AuthType {
  None = 0,      // 无认证
  ApiKey = 1,    // API Key 认证
  JwtBearer = 2  // JWT Bearer 认证
}
```

### 4.3 状态管理 (Pinia + localStorage)

```typescript
// stores/apps.ts
const STORAGE_KEY = 'apqcfg-apps'

export const useAppsStore = defineStore('apps', () => {
  // 从 localStorage 初始化
  const apps = ref<AppEndpoint[]>(loadFromStorage())

  // 监听变化，自动保存到 localStorage
  watch(apps, (value) => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(value))
  }, { deep: true })

  function loadFromStorage(): AppEndpoint[] {
    const data = localStorage.getItem(STORAGE_KEY)
    return data ? JSON.parse(data) : []
  }

  function addApp(app: Omit<AppEndpoint, 'id' | 'createdAt'>) {
    const newApp: AppEndpoint = {
      ...app,
      id: crypto.randomUUID(),
      createdAt: new Date().toISOString()
    }
    apps.value.push(newApp)
    return newApp
  }

  function updateApp(id: string, updates: Partial<AppEndpoint>) {
    const index = apps.value.findIndex(a => a.id === id)
    if (index >= 0) {
      apps.value[index] = { ...apps.value[index], ...updates }
    }
  }

  function deleteApp(id: string) {
    apps.value = apps.value.filter(a => a.id !== id)
  }

  function getApp(id: string) {
    return apps.value.find(a => a.id === id)
  }

  return { apps, addApp, updateApp, deleteApp, getApp }
})
```

### 4.4 配置 API 封装

前端直接访问远程应用的配置 API，根据应用的认证配置添加相应的请求头：

```typescript
// api/config.ts
import axios from 'axios'
import type { AppEndpoint, AuthType } from '@/types'

export function createConfigApi(app: AppEndpoint) {
  const instance = axios.create({
    baseURL: app.url,
    timeout: 30000
  })

  // 添加认证头
  instance.interceptors.request.use(config => {
    switch (app.authType) {
      case AuthType.ApiKey:
        config.headers['X-Api-Key'] = app.apiKey
        break
      case AuthType.JwtBearer:
        config.headers['Authorization'] = `Bearer ${app.token}`
        break
    }
    return config
  })

  return {
    getTree: () => instance.get('/tree'),
    getSources: () => instance.get('/sources'),
    getSourceTree: (source: string) => instance.get(`/sources/${source}/tree`),
    getValue: (key: string) => instance.get(`/values/${encodeURIComponent(key)}`),
    setValue: (key: string, value: string) =>
      instance.put(`/values/${encodeURIComponent(key)}`, { value }),
    deleteValue: (key: string) =>
      instance.delete(`/values/${encodeURIComponent(key)}`),
    save: (source?: string) => instance.post('/save', { source }),
    reload: () => instance.post('/reload'),
    export: (format: 'json' | 'env' | 'kv') =>
      instance.get('/export', { params: { format } }),
    testConnection: () => instance.get('/merged')
  }
}
```

### 4.5 页面组件设计

#### HomeView（首页）

```
┌─────────────────────────────────────────────────────────────┐
│  Apq.Cfg 配置管理中心                                  [添加] │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐          │
│  │   应用 A    │  │   应用 B    │  │   应用 C    │          │
│  │  订单服务   │  │  用户服务   │  │  支付服务   │          │
│  │ ────────── │  │ ────────── │  │ ────────── │          │
│  │ URL: ...   │  │ URL: ...   │  │ URL: ...   │          │
│  │ 认证: API  │  │ 认证: JWT  │  │ 认证: 无   │          │
│  │ ────────── │  │ ────────── │  │ ────────── │          │
│  │ [测试][管理]│  │ [测试][管理]│  │ [测试][管理]│          │
│  └─────────────┘  └─────────────┘  └─────────────┘          │
└─────────────────────────────────────────────────────────────┘
```

#### ConfigView（配置详情页）

```
┌─────────────────────────────────────────────────────────────┐
│  ← 返回  订单服务配置                    [刷新] [保存] [导出] │
├─────────────────────────────────────────────────────────────┤
│  配置源: [合并后 ▼]  搜索: [________________]               │
├──────────────────────┬──────────────────────────────────────┤
│  配置树              │  配置详情                            │
│  ├─ Database         │  ┌────────────────────────────────┐  │
│  │  ├─ Host          │  │ 键: Database:Host              │  │
│  │  ├─ Port          │  │ 值: [localhost          ]      │  │
│  │  └─ Password ●    │  │ 来源: config.json              │  │
│  ├─ Redis            │  │ ──────────────────────────────│  │
│  │  └─ Connection    │  │ [保存] [删除]                  │  │
│  └─ Logging          │  └────────────────────────────────┘  │
│     └─ LogLevel      │                                      │
└──────────────────────┴──────────────────────────────────────┘
```

### 4.6 敏感值处理

前端自动识别以下关键词并脱敏显示：

```typescript
const sensitiveKeywords = [
  'password', 'secret', 'key', 'token', 'credential',
  'connectionstring', 'apikey', 'accesskey', 'privatekey'
]

function isSensitive(key: string): boolean {
  const lowerKey = key.toLowerCase()
  return sensitiveKeywords.some(k => lowerKey.includes(k))
}

function maskValue(value: string): string {
  if (value.length <= 4) return '****'
  return value.substring(0, 2) + '****' + value.substring(value.length - 2)
}
```

---

## 五、部署

### 5.1 构建

```bash
cd Apq.Cfg.WebUI/Apq.Cfg.WebUI/ClientApp
npm install
npm run build
```

构建输出到 `Apq.Cfg.WebUI/dist/` 目录。

### 5.2 部署方式

作为纯静态站点，可以部署到：

- **Nginx / Apache** - 直接托管静态文件
- **GitHub Pages / GitLab Pages** - 免费静态托管
- **Vercel / Netlify** - 现代静态站点托管
- **阿里云 OSS / 腾讯云 COS** - 对象存储静态网站托管
- **任何 HTTP 服务器** - 只需托管 dist 目录下的文件

### 5.3 Nginx 配置示例

```nginx
server {
    listen 80;
    server_name webui.example.com;
    root /var/www/apqcfg-webui;
    index index.html;

    # SPA 路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 缓存静态资源
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

### 5.4 虚拟目录部署

WebUI 使用相对路径构建 (`base: './'`)，支持部署到任意虚拟目录：

```
http://example.com/                    # 根目录
http://example.com/apqcfg/             # 虚拟目录
http://example.com/admin/config/       # 多级虚拟目录
```

---

## 六、安全设计

### 6.1 客户端存储安全

应用端点信息（包括认证凭据）存储在浏览器 localStorage：

| 数据 | 存储位置 | 说明 |
|------|---------|------|
| 应用列表 | localStorage | 包含 URL、认证方式、凭据 |
| ApiKey | localStorage | 明文存储（浏览器本地） |
| JWT Token | localStorage | 明文存储（浏览器本地） |

**安全考虑**：
- 数据仅存储在用户本地浏览器，不上传到任何服务器
- 每个用户管理自己的应用列表，天然隔离
- 建议在可信环境下使用，避免在公共电脑上保存敏感凭据

### 6.2 远程应用认证

前端直接访问远程应用时，根据配置添加认证头：

| 认证方式 | 请求头 | 适用场景 |
|---------|--------|---------|
| None | 无 | 内网环境、开发测试 |
| ApiKey | `X-Api-Key: {key}` | 简单场景、服务间调用 |
| JwtBearer | `Authorization: Bearer {token}` | 生产环境、用户认证 |

### 6.3 CORS 要求

由于前端直接访问远程应用，**远程应用必须配置 CORS** 允许 WebUI 的来源：

```csharp
// 远程应用需要配置 CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://your-webui-domain")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 七、扩展点

### 7.1 添加新的认证方式

1. 在 `AuthType` 枚举中添加新类型
2. 在 `createConfigApi` 的拦截器中处理新的认证头
3. 在前端表单中添加对应的输入字段

### 7.2 数据导出/导入

可添加应用列表的导出/导入功能，方便用户备份和迁移。

#### 支持格式

| 格式 | 导出 | 导入 | 说明 |
|------|------|------|------|
| JSON | ✅ | ✅ | 默认格式，保留完整结构 |
| CSV | ✅ | ✅ | 表格格式，Excel/WPS 可直接打开 |

#### 实现示例

```typescript
// JSON 导出
function exportAsJson(apps: AppConfig[]) {
  const blob = new Blob([JSON.stringify(apps, null, 2)], { type: 'application/json' })
  downloadFile(blob, 'apps.json')
}

// CSV 导出（带 UTF-8 BOM，确保 Excel 正确显示中文）
function exportAsCsv(apps: AppConfig[]) {
  const headers = ['name', 'baseUrl', 'authType', 'apiKey']
  const rows = apps.map(app => headers.map(h => escapeCsvValue(app[h] ?? '')).join(','))
  const csv = [headers.join(','), ...rows].join('\n')
  const bom = '\uFEFF' // UTF-8 BOM
  const blob = new Blob([bom + csv], { type: 'text/csv;charset=utf-8' })
  downloadFile(blob, 'apps.csv')
}

// CSV 值转义（处理逗号、引号、换行）
function escapeCsvValue(value: string): string {
  if (value.includes(',') || value.includes('"') || value.includes('\n')) {
    return `"${value.replace(/"/g, '""')}"`
  }
  return value
}

// 通用下载函数
function downloadFile(blob: Blob, filename: string) {
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  a.click()
  URL.revokeObjectURL(url)
}

// 导入（自动识别格式）
async function importApps(file: File): Promise<AppConfig[]> {
  const ext = file.name.split('.').pop()?.toLowerCase()
  const text = await file.text()

  if (ext === 'json') {
    return JSON.parse(text)
  }

  if (ext === 'csv') {
    const lines = text.replace(/^\uFEFF/, '').trim().split('\n') // 移除 BOM
    const [header, ...rows] = lines
    const keys = header.split(',')
    return rows.map(row => {
      const values = parseCsvLine(row)
      return Object.fromEntries(keys.map((k, i) => [k, values[i] ?? '']))
    }) as AppConfig[]
  }

  throw new Error(`不支持的文件格式: ${ext}`)
}

// 解析 CSV 行（处理引号内的逗号）
function parseCsvLine(line: string): string[] {
  const result: string[] = []
  let current = ''
  let inQuotes = false

  for (const char of line) {
    if (char === '"') {
      inQuotes = !inQuotes
    } else if (char === ',' && !inQuotes) {
      result.push(current)
      current = ''
    } else {
      current += char
    }
  }
  result.push(current)
  return result
}
```

---

## 八、开发指南

### 8.1 本地开发

```bash
cd Apq.Cfg.WebUI
npm install
npm run dev

# 访问 http://localhost:38690
```

### 8.2 构建发布

```bash
npm run build
# 输出到 dist/ 目录
```

### 8.3 预览构建结果

```bash
npm run preview
```

---

## 九、版本历史

| 版本 | 日期 | 变更说明 |
|------|------|---------|
| 2.0.0 | 2026-01 | 重构为纯静态站点，移除后端依赖 |
| 1.0.0 | 2025-01 | 初始版本，前后端分离架构 |

---

## 十、参考资料

- [Apq.Cfg 在线文档](https://apq-cfg.vercel.app/)
- [Apq.Cfg Gitee 仓库](https://gitee.com/apq/Apq.Cfg)
- [Vue 3 文档](https://vuejs.org/)
- [Element Plus 文档](https://element-plus.org/)
