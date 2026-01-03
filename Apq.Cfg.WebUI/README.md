# Apq.Cfg.WebUI

Apq.Cfg 配置管理 Web 界面，用于集中管理多个应用的配置。

## 功能特性

- **多应用管理**：集中管理多个应用的配置
- **配置树视图**：树形结构展示配置，支持搜索
- **配置源切换**：查看合并后配置或单独配置源
- **实时编辑**：在线编辑配置值
- **敏感值脱敏**：自动识别并脱敏敏感配置
- **导出功能**：支持 JSON、ENV、Key-Value 格式导出
- **多种认证**：支持无认证、API Key、JWT Bearer

## 技术栈

- **后端**：ASP.NET Core 8.0/10.0
- **前端**：Vue 3 + TypeScript + Element Plus
- **构建**：Vite

## 快速开始

### 使用 Docker

```bash
docker build -t apq-cfg-webui .
docker run -p 8080:80 apq-cfg-webui
```

### 本地开发

```bash
# 安装前端依赖
cd ClientApp
npm install

# 启动前端开发服务器
npm run dev

# 启动后端
cd ..
dotnet run
```

### 生产构建

```bash
# 构建前端
cd ClientApp
npm run build

# 构建后端
cd ..
dotnet publish -c Release
```

## 配置

### 应用端点配置

WebUI 通过代理方式连接到各个应用的配置 API。应用端点信息存储在本地 JSON 文件中。

```json
{
    "id": "app-1",
    "name": "订单服务",
    "url": "http://localhost:5000/api/apqcfg",
    "authType": "ApiKey",
    "apiKey": "your-api-key",
    "description": "订单服务配置"
}
```

### 认证类型

| 类型 | 说明 |
|------|------|
| `None` | 无认证 |
| `ApiKey` | API Key 认证，通过 `X-API-Key` 请求头传递 |
| `JwtBearer` | JWT Bearer 认证，通过 `Authorization: Bearer <token>` 传递 |

## 项目结构

```
Apq.Cfg.WebUI/
├── ClientApp/                 # Vue 前端
│   ├── src/
│   │   ├── api/              # API 调用
│   │   ├── components/       # 组件
│   │   ├── stores/           # Pinia 状态管理
│   │   ├── types/            # TypeScript 类型
│   │   └── views/            # 页面视图
│   └── package.json
├── Controllers/               # API 控制器
│   ├── AppsController.cs     # 应用管理
│   └── ProxyController.cs    # 配置 API 代理
├── Services/                  # 服务
│   ├── AppService.cs         # 应用管理服务
│   └── ConfigProxyService.cs # 配置代理服务
├── Models/                    # 数据模型
│   └── AppEndpoint.cs        # 应用端点
├── Dockerfile                 # Docker 构建文件
└── Program.cs                 # 入口点
```

## API 端点

### 应用管理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/apps` | 获取所有应用 |
| GET | `/api/apps/{id}` | 获取单个应用 |
| POST | `/api/apps` | 添加应用 |
| PUT | `/api/apps/{id}` | 更新应用 |
| DELETE | `/api/apps/{id}` | 删除应用 |
| POST | `/api/apps/{id}/test` | 测试连接 |

### 配置代理

| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/proxy/{appId}/sources` | 获取配置源列表 |
| GET | `/api/proxy/{appId}/tree` | 获取配置树 |
| GET | `/api/proxy/{appId}/sources/{level}/{name}/tree` | 获取指定源配置树 |
| PUT | `/api/proxy/{appId}/values/{key}` | 设置配置值 |
| DELETE | `/api/proxy/{appId}/values/{key}` | 删除配置值 |
| POST | `/api/proxy/{appId}/save` | 保存配置 |
| POST | `/api/proxy/{appId}/reload` | 重新加载配置 |
| GET | `/api/proxy/{appId}/export` | 导出配置 |

## 截图

### 应用列表

应用卡片展示，支持添加、编辑、删除、测试连接。

### 配置管理

- 左侧：配置树，支持搜索
- 右侧：配置详情，支持编辑和删除
- 顶部：配置源切换、刷新、保存、导出

## 许可证

MIT License
