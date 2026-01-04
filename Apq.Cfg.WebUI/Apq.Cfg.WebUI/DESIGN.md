# Apq.Cfg.WebUI 设计文档

## 一、项目概述

### 1.1 项目定位

Apq.Cfg.WebUI 是 Apq.Cfg 配置组件库的 Web 管理界面，提供集中化的配置管理能力。它作为配置管理中心，可以连接和管理多个使用 Apq.Cfg 的应用程序的配置。

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
| 本机配置 | 管理 WebUI 自身的配置 |

### 1.3 技术栈

```
┌─────────────────────────────────────────────────────────────┐
│                        前端 (Vue 3)                          │
├─────────────────────────────────────────────────────────────┤
│  Vue 3.5  │  TypeScript 5.9  │  Element Plus 2.13  │  Vite 7 │
│  Vue Router 4.6  │  Pinia 3.0  │  Axios 1.13  │  Sass       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    后端 (ASP.NET Core)                       │
├─────────────────────────────────────────────────────────────┤
│  .NET 8.0 / 10.0 LTS  │  Apq.Cfg  │  Apq.Cfg.WebApi         │
└─────────────────────────────────────────────────────────────┘
```

---

## 二、系统架构

### 2.1 设计原则

- **服务端无状态**：服务端不保存任何应用列表或用户数据
- **客户端存储**：应用端点信息（包括认证凭据）保存在浏览器 localStorage
- **客户端直连**：前端直接访问各应用的配置 API，无需服务端代理

### 2.2 整体架构图

```
┌──────────────────────────────────────────────────────────────────┐
│                           用户浏览器                              │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │                    Vue 3 前端 (SPA)                         │  │
│  │  ┌──────────┐  ┌──────────┐  ┌──────────────────────────┐  │  │
│  │  │ 应用管理  │  │ 配置视图  │  │      本机配置视图         │  │  │
│  │  └──────────┘  └──────────┘  └──────────────────────────┘  │  │
│  │       │                                                     │  │
│  │       ▼                                                     │  │
│  │  ┌──────────────┐                                           │  │
│  │  │ localStorage │  ← 应用列表、认证信息                      │  │
│  │  └──────────────┘                                           │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
        │                                           │
        │ 本机配置 API                               │ 直接访问远程应用
        ▼                                           ▼
┌────────────────────┐                 ┌──────────────────────────────┐
│  Apq.Cfg.WebUI     │                 │       远程应用 (多个)         │
│  (本项目后端)       │                 │  ┌────────┐ ┌────────┐       │
│  ┌──────────────┐  │                 │  │ 应用 A │ │ 应用 B │ ...   │
│  │Apq.Cfg.WebApi│  │                 │  │/apqcfg │ │/apqcfg │       │
│  │ /api/apqcfg  │  │                 │  └────────┘ └────────┘       │
│  └──────────────┘  │                 └──────────────────────────────┘
└────────────────────┘
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
│  │ 2. 根据操作类型选择目标：                                     ││
│  │    - 本机配置 → WebUI 后端 /api/apqcfg/*                     ││
│  │    - 远程应用 → 直接访问应用的 /api/apqcfg/*                  ││
│  │ 3. 添加认证头（ApiKey 或 JWT Bearer）                        ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
                    │                           │
                    ▼                           ▼
        ┌───────────────────┐       ┌───────────────────┐
        │  WebUI 后端        │       │  远程应用          │
        │  /api/apqcfg/*    │       │  /api/apqcfg/*    │
        │  (本机配置)        │       │  (应用配置)        │
        └───────────────────┘       └───────────────────┘
```

---

## 三、目录结构

```
Apq.Cfg.WebUI/
├── Apq.Cfg.WebUI.sln                    # 解决方案文件
├── Directory.Build.props                 # MSBuild 属性
└── Apq.Cfg.WebUI/                       # 主项目
    ├── Apq.Cfg.WebUI.csproj             # 项目文件
    ├── Program.cs                        # 应用入口（仅 SPA 托管 + 本机配置 API）
    ├── README.md                         # 使用文档
    ├── DESIGN.md                         # 设计文档（本文件）
    │
    ├── Properties/                       # 项目属性
    │   └── launchSettings.json          # 启动配置
    │
    ├── appsettings.json                  # ASP.NET Core 配置
    ├── config.json                       # Apq.Cfg 配置（本机配置）
    │
    ├── Dockerfile                        # Docker 构建文件
    ├── Dockerfile.cn                     # 国内镜像加速版
    ├── buildDockerImage_u24_docker.ps1  # 远程构建脚本
    │
    ├── ClientApp/                        # Vue 前端项目
    │   ├── index.html                   # HTML 入口
    │   ├── package.json                 # 依赖配置
    │   ├── vite.config.ts               # Vite 配置
    │   ├── tsconfig.json                # TypeScript 配置
    │   └── src/
    │       ├── main.ts                  # 应用入口
    │       ├── App.vue                  # 根组件
    │       ├── api/                     # API 接口层
    │       │   └── config.ts            # 配置 API（直接访问远程应用）
    │       ├── layouts/                 # 布局组件
    │       │   └── MainLayout.vue       # 主布局
    │       ├── router/                  # 路由配置
    │       │   └── index.ts             # 路由定义
    │       ├── stores/                  # 状态管理
    │       │   └── apps.ts              # 应用状态（localStorage 持久化）
    │       ├── types/                   # 类型定义
    │       │   └── index.ts             # TypeScript 类型
    │       ├── utils/                   # 工具函数
    │       │   ├── request.ts           # Axios 封装
    │       │   └── storage.ts           # localStorage 封装
    │       └── views/                   # 页面视图
    │           ├── HomeView.vue         # 首页（应用管理）
    │           ├── ConfigView.vue       # 配置详情页
    │           └── SelfConfigView.vue   # 本机配置页
    │
    └── wwwroot/                          # 前端构建输出
```

---

## 四、后端设计

后端职责非常简单：**托管 SPA 静态文件** + **提供本机配置 API**。

### 4.1 入口配置 (Program.cs)

```csharp
// 1. 构建 Apq.Cfg 配置（本机配置）
var apqCfgBuilder = new ApqCfgBuilder()
    .AddJson("config.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2. 注册本机配置 API
builder.Services.AddApqCfgWebApi(apqCfgBuilder, options => {
    options.EnableRead = true;
    options.EnableWrite = true;
    options.EnableDelete = true;
});

// 3. 配置 SPA 静态文件托管
app.UseSpaStaticFiles();
app.UseSpa(spa => {
    spa.Options.SourcePath = "ClientApp";
    if (env.IsDevelopment()) {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:38690");
    }
});
```

### 4.2 API 端点

后端仅提供本机配置 API，由 `Apq.Cfg.WebApi` 包提供：

| 方法 | 路径 | 描述 |
|------|------|------|
| GET | `/api/apqcfg/merged` | 获取合并后的配置 |
| GET | `/api/apqcfg/tree` | 获取配置树 |
| GET | `/api/apqcfg/sources` | 获取配置源列表 |
| GET | `/api/apqcfg/sources/{source}/tree` | 获取指定源的配置树 |
| GET | `/api/apqcfg/values/{key}` | 获取配置值 |
| PUT | `/api/apqcfg/values/{key}` | 设置配置值 |
| DELETE | `/api/apqcfg/values/{key}` | 删除配置值 |
| POST | `/api/apqcfg/save` | 保存配置到文件 |
| POST | `/api/apqcfg/reload` | 重载配置 |
| GET | `/api/apqcfg/export` | 导出配置（支持 JSON/ENV/KV 格式）|

---

## 五、前端设计

### 5.1 路由设计

```typescript
const routes = [
  {
    path: '/',
    component: MainLayout,
    children: [
      { path: '', name: 'home', component: HomeView },
      { path: 'self', name: 'selfConfig', component: SelfConfigView },
      { path: 'app/:id', name: 'config', component: ConfigView }
    ]
  }
]
```

### 5.2 数据模型

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

### 5.3 状态管理 (Pinia + localStorage)

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

### 5.4 配置 API 封装

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
    // 获取配置树
    getTree: () => instance.get('/tree'),

    // 获取配置源列表
    getSources: () => instance.get('/sources'),

    // 获取指定源的配置树
    getSourceTree: (source: string) => instance.get(`/sources/${source}/tree`),

    // 获取配置值
    getValue: (key: string) => instance.get(`/values/${encodeURIComponent(key)}`),

    // 设置配置值
    setValue: (key: string, value: string) =>
      instance.put(`/values/${encodeURIComponent(key)}`, { value }),

    // 删除配置值
    deleteValue: (key: string) =>
      instance.delete(`/values/${encodeURIComponent(key)}`),

    // 保存配置
    save: (source?: string) => instance.post('/save', { source }),

    // 重载配置
    reload: () => instance.post('/reload'),

    // 导出配置
    export: (format: 'json' | 'env' | 'kv') =>
      instance.get('/export', { params: { format } }),

    // 测试连接
    testConnection: () => instance.get('/merged')
  }
}
```

### 5.5 页面组件设计

#### HomeView（首页）

```
┌─────────────────────────────────────────────────────────────┐
│  Apq.Cfg 配置管理中心                        [本机配置] [添加] │
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

### 5.5 敏感值处理

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

## 六、部署架构

### 6.1 Docker 多阶段构建

```dockerfile
# 阶段 1: 构建前端
FROM node:24-alpine AS frontend
WORKDIR /build
COPY ClientApp/package*.json ./
RUN npm ci --silent
COPY ClientApp/ ./
RUN npm run build

# 阶段 2: 构建后端
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
COPY --from=frontend /wwwroot ./wwwroot
RUN dotnet publish -c Release -o /app

# 阶段 3: 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=backend /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Apq.Cfg.WebUI.dll"]
```

### 6.2 反向代理支持

WebUI 支持任意虚拟目录部署，无需配置：

```
┌─────────────────────────────────────────────────────────────┐
│                        Nginx / Traefik                       │
├─────────────────────────────────────────────────────────────┤
│  /              → WebUI (http://webui:80)                   │
│  /apqcfg/       → WebUI (http://webui:80)                   │
│  /admin/config/ → WebUI (http://webui:80)                   │
└─────────────────────────────────────────────────────────────┘
```

**实现原理**：
1. 前端使用相对路径构建 (`base: './'`)
2. 运行时动态检测 `window.location.pathname`
3. 后端处理 `X-Forwarded-*` 头

---

## 七、安全设计

### 7.1 客户端存储安全

应用端点信息（包括认证凭据）存储在浏览器 localStorage：

| 数据 | 存储位置 | 说明 |
|------|---------|------|
| 应用列表 | localStorage | 包含 URL、认证方式、凭据 |
| ApiKey | localStorage | 明文存储（浏览器本地） |
| JWT Token | localStorage | 明文存储（浏览器本地） |

**安全考虑**：
- 数据仅存储在用户本地浏览器，不上传到服务器
- 每个用户管理自己的应用列表，天然隔离
- 建议在可信环境下使用，避免在公共电脑上保存敏感凭据
- 可考虑添加导出/导入功能，方便用户备份和迁移

### 7.2 远程应用认证

前端直接访问远程应用时，根据配置添加认证头：

| 认证方式 | 请求头 | 适用场景 |
|---------|--------|---------|
| None | 无 | 内网环境、开发测试 |
| ApiKey | `X-Api-Key: {key}` | 简单场景、服务间调用 |
| JwtBearer | `Authorization: Bearer {token}` | 生产环境、用户认证 |

### 7.3 CORS 要求

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

### 7.4 敏感值脱敏

前端自动识别敏感配置并脱敏显示：

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

## 八、扩展点

### 8.1 添加新的认证方式

1. 在 `AuthType` 枚举中添加新类型
2. 在 `createConfigApi` 的拦截器中处理新的认证头
3. 在前端表单中添加对应的输入字段

### 8.2 添加新的导出格式

1. 在 `Apq.Cfg.WebApi` 中实现新的导出格式
2. 在前端导出对话框中添加新选项

### 8.3 数据导出/导入

可添加应用列表的导出/导入功能，方便用户备份和迁移：

```typescript
// 导出应用列表
function exportApps() {
  const data = localStorage.getItem(STORAGE_KEY)
  const blob = new Blob([data || '[]'], { type: 'application/json' })
  // 下载文件...
}

// 导入应用列表
function importApps(file: File) {
  const reader = new FileReader()
  reader.onload = (e) => {
    const apps = JSON.parse(e.target?.result as string)
    localStorage.setItem(STORAGE_KEY, JSON.stringify(apps))
  }
  reader.readAsText(file)
}
```

---

## 九、开发指南

### 9.1 本地开发

```bash
# 1. 启动后端
cd Apq.Cfg.WebUI/Apq.Cfg.WebUI
dotnet run -f net10.0

# 2. 启动前端（新终端）
cd Apq.Cfg.WebUI/Apq.Cfg.WebUI/ClientApp
npm install
npm run dev

# 3. 访问
# 前端开发服务器: http://localhost:38690
# 后端 API: http://localhost:38678
```

### 9.2 构建发布

```bash
# 构建前端
cd ClientApp && npm run build

# 构建后端（包含前端）
dotnet publish -c Release -o ./publish -f net10.0
```

### 9.3 Docker 构建

```bash
# 从解决方案根目录构建
docker build -f Apq.Cfg.WebUI/Apq.Cfg.WebUI/Dockerfile -t apqcfg-webui .

# 国内环境使用加速版
docker build -f Apq.Cfg.WebUI/Apq.Cfg.WebUI/Dockerfile.cn \
    --build-arg DOCKER_MIRROR=docker.m.daocloud.io \
    -t apqcfg-webui .
```

---

## 十、版本历史

| 版本 | 日期 | 变更说明 |
|------|------|---------|
| 1.0.0 | 2025-01 | 初始版本，支持多应用管理、配置编辑、导出 |

---

## 十一、参考资料

- [Apq.Cfg 在线文档](https://apq-cfg.vercel.app/)
- [Apq.Cfg Gitee 仓库](https://gitee.com/apq/Apq.Cfg)
- [Vue 3 文档](https://vuejs.org/)
- [Element Plus 文档](https://element-plus.org/)
- [ASP.NET Core 文档](https://docs.microsoft.com/aspnet/core)
