# Apq.Cfg.WebUI 设计方案

## 概述

Apq.Cfg.WebUI 是一个**独立的配置管理工具**，用于连接多个使用 Apq.Cfg.WebApi 的应用，可视化查看和管理它们的配置内容。

## 定位

```
┌─────────────────────┐
│   Apq.Cfg.WebUI     │  ← 独立部署的配置管理工具
│   (配置管理中心)     │
└─────────┬───────────┘
          │
          │ HTTP 请求
          ▼
┌─────────────────────┐     ┌─────────────────────┐     ┌─────────────────────┐
│   应用 A            │     │   应用 B            │     │   应用 C            │
│   + Apq.Cfg.WebApi  │     │   + Apq.Cfg.WebApi  │     │   + Apq.Cfg.WebApi  │
└─────────────────────┘     └─────────────────────┘     └─────────────────────┘
```

## 功能特性

- **多应用管理**：连接多个 WebApi 端点，统一管理
- 树形结构展示配置
- 搜索/过滤配置
- 编辑配置值
- 批量操作
- 导出配置
- 支持 API Key / JWT 认证
- 连接配置持久化

## 技术栈

- **后端**：ASP.NET Core 8.0/10.0
- **前端**：Vue 3 + Element Plus + TypeScript
- **构建**：Vite 5

## 发布方式

- Docker 镜像
- 独立可执行文件（self-contained）
- dotnet tool（可选）

## 项目结构

```
Apq.Cfg.WebUI/
├── Apq.Cfg.WebUI.csproj          # .NET Web 项目文件
├── Program.cs                     # 入口
├── appsettings.json
├── Properties/
│   └── launchSettings.json
├── Controllers/
│   ├── AppsController.cs          # 应用管理 API
│   └── ProxyController.cs         # API 代理（解决跨域）
├── Models/
│   ├── AppEndpoint.cs             # 应用端点模型
│   └── AppConnection.cs           # 连接配置模型
├── Services/
│   ├── IAppService.cs             # 应用管理服务接口
│   ├── AppService.cs              # 应用管理服务实现
│   └── ConfigProxyService.cs      # 配置代理服务
├── Data/
│   └── apps.json                  # 应用列表持久化（或用 SQLite）
├── ClientApp/                     # Vue 前端源码
│   ├── package.json
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── index.html
│   └── src/
│       ├── main.ts
│       ├── App.vue
│       ├── api/
│       │   ├── apps.ts            # 应用管理 API
│       │   └── config.ts          # 配置 API（通过代理）
│       ├── components/
│       │   ├── AppList.vue        # 应用列表
│       │   ├── AppCard.vue        # 应用卡片
│       │   ├── ConfigTree.vue     # 配置树
│       │   ├── ConfigEditor.vue   # 编辑器
│       │   └── ConnectionDialog.vue
│       ├── views/
│       │   ├── HomeView.vue       # 首页（应用列表）
│       │   └── ConfigView.vue     # 配置详情
│       ├── stores/
│       │   ├── apps.ts            # 应用列表状态
│       │   └── config.ts          # 当前配置状态
│       ├── types/
│       │   └── index.ts
│       └── router/
│           └── index.ts
├── wwwroot/                       # 前端构建输出
└── Dockerfile
```

---

## 后端设计

### 项目文件 (Apq.Cfg.WebUI.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
        <RootNamespace>Apq.Cfg.WebUI</RootNamespace>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <SpaRoot>ClientApp\</SpaRoot>
        <SpaProxyServerUrl>http://localhost:5173</SpaProxyServerUrl>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.SpaServices.Extensions"
                          Version="$(MicrosoftAspNetCoreVersion)" />
    </ItemGroup>

    <!-- 发布时包含前端构建输出 -->
    <Target Name="PublishRunWebpack" AfterTargets="ComputeFilesToPublish">
        <ItemGroup>
            <DistFiles Include="$(SpaRoot)dist\**" />
            <ResolvedFileToPublish Include="@(DistFiles->'%(FullPath)')"
                                   RelativePath="wwwroot\%(RecursiveDir)%(FileName)%(Extension)" />
        </ItemGroup>
    </Target>

</Project>
```

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加服务
builder.Services.AddControllers();
builder.Services.AddSingleton<IAppService, AppService>();
builder.Services.AddHttpClient<ConfigProxyService>();

// 开发环境启用 SPA 代理
builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "wwwroot";
});

var app = builder.Build();

app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

app.MapControllers();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
    }
});

app.Run();
```

### 模型

#### Models/AppEndpoint.cs

```csharp
namespace Apq.Cfg.WebUI.Models;

/// <summary>
/// 应用端点配置
/// </summary>
public class AppEndpoint
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// WebApi 地址（如 http://app-a:5000/api/config）
    /// </summary>
    public string Url { get; set; } = "";

    /// <summary>
    /// 认证方式
    /// </summary>
    public AuthType AuthType { get; set; } = AuthType.None;

    /// <summary>
    /// API Key（当 AuthType 为 ApiKey 时）
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// JWT Token（当 AuthType 为 JwtBearer 时）
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum AuthType
{
    None,
    ApiKey,
    JwtBearer
}
```

### 控制器

#### Controllers/AppsController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Apq.Cfg.WebUI.Models;
using Apq.Cfg.WebUI.Services;

namespace Apq.Cfg.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppsController : ControllerBase
{
    private readonly IAppService _appService;

    public AppsController(IAppService appService)
    {
        _appService = appService;
    }

    /// <summary>
    /// 获取所有应用
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AppEndpoint>>> GetAll()
    {
        var apps = await _appService.GetAllAsync();
        return Ok(apps);
    }

    /// <summary>
    /// 获取单个应用
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppEndpoint>> Get(string id)
    {
        var app = await _appService.GetByIdAsync(id);
        if (app == null) return NotFound();
        return Ok(app);
    }

    /// <summary>
    /// 添加应用
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AppEndpoint>> Add([FromBody] AppEndpoint app)
    {
        var created = await _appService.AddAsync(app);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// 更新应用
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(string id, [FromBody] AppEndpoint app)
    {
        app.Id = id;
        var success = await _appService.UpdateAsync(app);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 删除应用
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var success = await _appService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    [HttpPost("{id}/test")]
    public async Task<ActionResult<bool>> TestConnection(string id)
    {
        var success = await _appService.TestConnectionAsync(id);
        return Ok(new { success });
    }
}
```

#### Controllers/ProxyController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using Apq.Cfg.WebUI.Services;

namespace Apq.Cfg.WebUI.Controllers;

/// <summary>
/// 配置 API 代理，解决跨域问题
/// </summary>
[ApiController]
[Route("api/proxy/{appId}")]
public class ProxyController : ControllerBase
{
    private readonly ConfigProxyService _proxyService;

    public ProxyController(ConfigProxyService proxyService)
    {
        _proxyService = proxyService;
    }

    /// <summary>
    /// 代理 GET 请求
    /// </summary>
    [HttpGet("{**path}")]
    public async Task<ActionResult> ProxyGet(string appId, string? path)
    {
        var result = await _proxyService.GetAsync(appId, path);
        return Content(result, "application/json");
    }

    /// <summary>
    /// 代理 PUT 请求
    /// </summary>
    [HttpPut("{**path}")]
    public async Task<ActionResult> ProxyPut(string appId, string? path, [FromBody] object? body)
    {
        var result = await _proxyService.PutAsync(appId, path, body);
        return Content(result, "application/json");
    }

    /// <summary>
    /// 代理 DELETE 请求
    /// </summary>
    [HttpDelete("{**path}")]
    public async Task<ActionResult> ProxyDelete(string appId, string? path)
    {
        var result = await _proxyService.DeleteAsync(appId, path);
        return Content(result, "application/json");
    }

    /// <summary>
    /// 代理 POST 请求
    /// </summary>
    [HttpPost("{**path}")]
    public async Task<ActionResult> ProxyPost(string appId, string? path, [FromBody] object? body)
    {
        var result = await _proxyService.PostAsync(appId, path, body);
        return Content(result, "application/json");
    }
}
```

### 服务

#### Services/IAppService.cs

```csharp
namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 应用管理服务接口
/// </summary>
public interface IAppService
{
    Task<List<AppEndpoint>> GetAllAsync();
    Task<AppEndpoint?> GetByIdAsync(string id);
    Task<AppEndpoint> AddAsync(AppEndpoint app);
    Task<bool> UpdateAsync(AppEndpoint app);
    Task<bool> DeleteAsync(string id);
    Task<bool> TestConnectionAsync(string id);
}
```

#### Services/AppService.cs

```csharp
using System.Text.Json;
using Apq.Cfg.WebUI.Models;

namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 应用管理服务实现（JSON 文件存储）
/// </summary>
public class AppService : IAppService
{
    private readonly string _dataFile;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<AppEndpoint>? _cache;

    public AppService(IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
    {
        _dataFile = Path.Combine(env.ContentRootPath, "Data", "apps.json");
        _httpClient = httpClientFactory.CreateClient();

        // 确保目录存在
        var dir = Path.GetDirectoryName(_dataFile);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public async Task<List<AppEndpoint>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_cache != null) return _cache;

            if (!File.Exists(_dataFile))
                return _cache = new List<AppEndpoint>();

            var json = await File.ReadAllTextAsync(_dataFile);
            _cache = JsonSerializer.Deserialize<List<AppEndpoint>>(json) ?? new();
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<AppEndpoint?> GetByIdAsync(string id)
    {
        var apps = await GetAllAsync();
        return apps.FirstOrDefault(a => a.Id == id);
    }

    public async Task<AppEndpoint> AddAsync(AppEndpoint app)
    {
        var apps = await GetAllAsync();
        app.Id = Guid.NewGuid().ToString("N");
        app.CreatedAt = DateTime.UtcNow;
        apps.Add(app);
        await SaveAsync(apps);
        return app;
    }

    public async Task<bool> UpdateAsync(AppEndpoint app)
    {
        var apps = await GetAllAsync();
        var index = apps.FindIndex(a => a.Id == app.Id);
        if (index < 0) return false;

        apps[index] = app;
        await SaveAsync(apps);
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var apps = await GetAllAsync();
        var removed = apps.RemoveAll(a => a.Id == id);
        if (removed == 0) return false;

        await SaveAsync(apps);
        return true;
    }

    public async Task<bool> TestConnectionAsync(string id)
    {
        var app = await GetByIdAsync(id);
        if (app == null) return false;

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, app.Url);

            // 添加认证头
            switch (app.AuthType)
            {
                case AuthType.ApiKey:
                    request.Headers.Add("X-Api-Key", app.ApiKey);
                    break;
                case AuthType.JwtBearer:
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", app.Token);
                    break;
            }

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task SaveAsync(List<AppEndpoint> apps)
    {
        _cache = apps;
        var json = JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_dataFile, json);
    }
}
```

#### Services/ConfigProxyService.cs

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Apq.Cfg.WebUI.Models;

namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 配置 API 代理服务
/// </summary>
public class ConfigProxyService
{
    private readonly HttpClient _httpClient;
    private readonly IAppService _appService;

    public ConfigProxyService(HttpClient httpClient, IAppService appService)
    {
        _httpClient = httpClient;
        _appService = appService;
    }

    public async Task<string> GetAsync(string appId, string? path)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Get, path);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string appId, string? path, object? body)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Put, path, body);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string appId, string? path, object? body)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Post, path, body);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string appId, string? path)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Delete, path);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(
        string appId, HttpMethod method, string? path, object? body = null)
    {
        var app = await _appService.GetByIdAsync(appId)
            ?? throw new InvalidOperationException($"App {appId} not found");

        var url = string.IsNullOrEmpty(path)
            ? app.Url
            : $"{app.Url.TrimEnd('/')}/{path}";

        var request = new HttpRequestMessage(method, url);

        // 添加认证头
        switch (app.AuthType)
        {
            case AuthType.ApiKey:
                request.Headers.Add("X-Api-Key", app.ApiKey);
                break;
            case AuthType.JwtBearer:
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", app.Token);
                break;
        }

        // 添加请求体
        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }
}
```

---

## 前端设计

### ClientApp/package.json

```json
{
  "name": "apq-cfg-webui-client",
  "version": "1.0.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc && vite build",
    "preview": "vite preview"
  },
  "dependencies": {
    "vue": "^3.4.0",
    "vue-router": "^4.2.0",
    "pinia": "^2.1.0",
    "element-plus": "^2.5.0",
    "axios": "^1.6.0",
    "@element-plus/icons-vue": "^2.3.0"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^5.0.0",
    "typescript": "^5.3.0",
    "vite": "^5.0.0",
    "vue-tsc": "^1.8.0",
    "sass": "^1.69.0"
  }
}
```

### ClientApp/vite.config.ts

```typescript
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  },
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  }
})
```

### 类型定义

#### ClientApp/src/types/index.ts

```typescript
// 应用端点
export interface AppEndpoint {
  id: string
  name: string
  url: string
  authType: 'None' | 'ApiKey' | 'JwtBearer'
  apiKey?: string
  token?: string
  description?: string
  createdAt: string
}

// 配置树节点
export interface ConfigTreeNode {
  key: string
  value: string | null
  hasValue: boolean
  isMasked: boolean
  children: ConfigTreeNode[]
}

// API 响应
export interface ApiResponse<T> {
  success: boolean
  data?: T
  error?: string
}
```

### API 封装

#### ClientApp/src/utils/request.ts

```typescript
import axios, { type AxiosInstance, type AxiosRequestConfig } from 'axios'
import { ElMessage } from 'element-plus'

const instance: AxiosInstance = axios.create({
  baseURL: '/',
  timeout: 30000
})

// 响应拦截器
instance.interceptors.response.use(
  response => response.data,
  error => {
    const message = error.response?.data?.error || error.message || '请求失败'
    ElMessage.error(message)
    return Promise.reject(error)
  }
)

export default {
  get<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return instance.get(url, config)
  },
  post<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return instance.post(url, data, config)
  },
  put<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return instance.put(url, data, config)
  },
  delete<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return instance.delete(url, config)
  }
}
```

#### ClientApp/src/api/apps.ts

```typescript
import request from '@/utils/request'
import type { AppEndpoint } from '@/types'

export const appsApi = {
  getAll(): Promise<AppEndpoint[]> {
    return request.get('/api/apps')
  },

  getById(id: string): Promise<AppEndpoint> {
    return request.get(`/api/apps/${id}`)
  },

  add(app: Partial<AppEndpoint>): Promise<AppEndpoint> {
    return request.post('/api/apps', app)
  },

  update(id: string, app: Partial<AppEndpoint>): Promise<void> {
    return request.put(`/api/apps/${id}`, app)
  },

  delete(id: string): Promise<void> {
    return request.delete(`/api/apps/${id}`)
  },

  testConnection(id: string): Promise<{ success: boolean }> {
    return request.post(`/api/apps/${id}/test`)
  }
}
```

#### ClientApp/src/api/config.ts

```typescript
import request from '@/utils/request'
import type { ApiResponse, ConfigTreeNode } from '@/types'

// 通过代理访问目标应用的配置 API
export const createConfigApi = (appId: string) => ({
  getAll(): Promise<ApiResponse<Record<string, string | null>>> {
    return request.get(`/api/proxy/${appId}`)
  },

  getTree(): Promise<ApiResponse<ConfigTreeNode>> {
    return request.get(`/api/proxy/${appId}/tree`)
  },

  getValue(key: string): Promise<ApiResponse<any>> {
    return request.get(`/api/proxy/${appId}/keys/${encodeURIComponent(key)}`)
  },

  setValue(key: string, value: string | null): Promise<ApiResponse<boolean>> {
    return request.put(`/api/proxy/${appId}/keys/${encodeURIComponent(key)}`, value)
  },

  delete(key: string): Promise<ApiResponse<boolean>> {
    return request.delete(`/api/proxy/${appId}/keys/${encodeURIComponent(key)}`)
  },

  save(): Promise<ApiResponse<boolean>> {
    return request.post(`/api/proxy/${appId}/save`)
  },

  reload(): Promise<ApiResponse<boolean>> {
    return request.post(`/api/proxy/${appId}/reload`)
  },

  export(format: string = 'json'): Promise<string> {
    return request.get(`/api/proxy/${appId}/export/${format}`, {
      responseType: 'text'
    })
  }
})
```

---

## 页面设计

### 首页（应用列表）

```
┌─────────────────────────────────────────────────────────────────┐
│  Apq.Cfg 配置管理中心                          [+ 添加应用]     │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │ 📦 应用 A       │  │ 📦 应用 B       │  │ 📦 应用 C       │  │
│  │                 │  │                 │  │                 │  │
│  │ http://app-a... │  │ http://app-b... │  │ http://app-c... │  │
│  │ 🔑 API Key      │  │ 🔓 无认证       │  │ 🎫 JWT          │  │
│  │                 │  │                 │  │                 │  │
│  │ [查看] [编辑]   │  │ [查看] [编辑]   │  │ [查看] [编辑]   │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 配置详情页

```
┌─────────────────────────────────────────────────────────────────┐
│  ← 返回  │  应用 A                    [刷新] [保存] [导出▼]     │
├─────────────────────────────────────────────────────────────────┤
│ ┌───────────────────┐ ┌───────────────────────────────────────┐ │
│ │ 🔍 搜索配置...     │ │ 配置详情                              │ │
│ ├───────────────────┤ │                                       │ │
│ │ ▼ App             │ │ 键: App:Name                          │ │
│ │   ├─ Name         │ │                                       │ │
│ │   ├─ Version      │ │ 值:                                   │ │
│ │   └─ Debug        │ │ ┌───────────────────────────────────┐ │ │
│ │ ▼ Database        │ │ │ MyApp                             │ │ │
│ │   ├─ Host         │ │ └───────────────────────────────────┘ │ │
│ │   ├─ Port         │ │                                       │ │
│ │   └─ Password 🔒  │ │ [保存] [取消]                         │ │
│ │ ▼ Logging         │ │                                       │ │
│ │   └─ Level        │ │                                       │ │
│ └───────────────────┘ └───────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## 部署方式

### Docker 部署

#### Dockerfile

```dockerfile
# 构建前端
FROM node:20-alpine AS frontend
WORKDIR /app/ClientApp
COPY ClientApp/package*.json ./
RUN npm ci
COPY ClientApp/ ./
RUN npm run build

# 构建后端
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
COPY --from=frontend /app/wwwroot ./wwwroot
RUN dotnet publish -c Release -o /app/publish

# 运行时
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=backend /app/publish ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Apq.Cfg.WebUI.dll"]
```

#### docker-compose.yml

```yaml
version: '3.8'
services:
  webui:
    build: .
    ports:
      - "8080:80"
    volumes:
      - ./data:/app/Data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
```

### 独立可执行文件

```bash
# 构建前端
cd ClientApp
npm ci && npm run build
cd ..

# 发布 .NET 应用（self-contained）
dotnet publish -c Release -r win-x64 --self-contained -o ./publish

# 或 Linux
dotnet publish -c Release -r linux-x64 --self-contained -o ./publish
```

---

## 实现步骤

### 第一阶段：后端基础

1. 创建 ASP.NET Core Web 项目
2. 实现 AppEndpoint 模型
3. 实现 IAppService 和 AppService（JSON 文件存储）
4. 实现 AppsController
5. 实现 ConfigProxyService
6. 实现 ProxyController

### 第二阶段：前端基础

7. 初始化 Vue 3 + Vite 项目
8. 安装 Element Plus、Pinia、Vue Router
9. 配置 TypeScript 和路径别名
10. 实现类型定义
11. 实现 API 封装

### 第三阶段：前端页面

12. 实现 AppList 组件（应用列表）
13. 实现 ConnectionDialog 组件（添加/编辑应用）
14. 实现 HomeView（首页）
15. 实现 ConfigTree 组件
16. 实现 ConfigEditor 组件
17. 实现 ConfigView（配置详情页）

### 第四阶段：集成和部署

18. 配置 SPA 集成
19. 编写 Dockerfile
20. 测试 Docker 部署
21. 编写 README 文档
