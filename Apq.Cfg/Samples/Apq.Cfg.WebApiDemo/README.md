# Apq.Cfg.WebApiDemo

这是一个演示 Apq.Cfg 配置系统的 Web API 项目，展示了多层级、多文件、多种源类型的配置管理能力。

## 功能特性

- 🔧 **多格式配置支持**：JSON、YAML、TOML、XML、INI、ENV
- 📊 **多层级配置合并**：通过 level 参数控制配置优先级
- 🌐 **RESTful API**：提供完整的配置读写 API
- 📖 **OpenAPI 文档**：内置 Scalar API 文档界面
- 🔐 **API Key 认证**：保护写入操作

## 项目依赖

| 项目 | 说明 |
|------|------|
| Apq.Cfg.WebApi | 配置管理 Web API 核心 |
| Apq.Cfg.Yaml | YAML 配置源支持 |
| Apq.Cfg.Toml | TOML 配置源支持 |
| Apq.Cfg.Ini | INI 配置源支持 |
| Apq.Cfg.Xml | XML 配置源支持 |
| Apq.Cfg.Env | ENV 环境变量文件支持 |

## 配置层级

项目演示了多层级配置的合并策略（level 越高优先级越高）：

| Level | 配置类型 | 说明 |
|-------|----------|------|
| 0 | 基础配置 | JSON, YAML, TOML, XML, INI 格式的基础配置 |
| 1 | WebApi 配置 | Apq.Cfg WebApi 的配置选项 |
| 5 | 功能开关 | Feature Flags 配置 |
| 10 | 环境配置 | 环境特定的 .env 文件 |
| 15 | 本地覆盖 | 本地开发覆盖配置（可写） |

## 快速开始

### 1. 运行项目

```bash
cd Apq.Cfg/Samples/Apq.Cfg.WebApiDemo
dotnet run
```

### 2. 访问应用

- **首页**: http://localhost:5000/
- **API 文档**: http://localhost:5000/scalar/v1
- **配置 API**: http://localhost:5000/api/apqcfg/merged

## API 端点

### 读取配置

```bash
# 获取合并后的所有配置
GET /api/apqcfg/merged

# 获取配置树结构
GET /api/apqcfg/merged/tree

# 获取单个配置值
GET /api/apqcfg/merged/keys/App:Name

# 获取配置节
GET /api/apqcfg/merged/sections/Database

# 查看所有配置源
GET /api/apqcfg/sources
```

### 写入配置

写入操作需要 API Key 认证：

```bash
# 设置配置值
PUT /api/apqcfg/merged/keys/Local:Debug
Header: X-Api-Key: demo-api-key-12345
Body: "true"
```

## 目录结构

```
Apq.Cfg.WebApiDemo/
├── Program.cs              # 应用入口，配置构建逻辑
├── appsettings.json        # ASP.NET Core 默认配置
├── config/
│   ├── apqcfg.json         # WebApi 配置
│   ├── local.json          # 本地覆盖配置（可写）
│   ├── base/               # 基础配置（多种格式）
│   │   ├── app.json
│   │   ├── database.yaml
│   │   ├── cache.toml
│   │   ├── services.xml
│   │   └── security.ini
│   ├── features/           # 功能开关配置
│   │   └── feature-flags.json
│   └── env/                # 环境特定配置
│       ├── development.env
│       ├── staging.env
│       └── production.env
└── Properties/
    └── launchSettings.json
```

## 配置示例

### WebApi 配置 (config/apqcfg.json)

```json
{
  "ApqCfg": {
    "WebApi": {
      "Enabled": true,
      "RoutePrefix": "/api/apqcfg",
      "Authentication": "ApiKey",
      "ApiKey": "demo-api-key-12345",
      "EnableCors": true,
      "OpenApiEnabled": true
    }
  }
}
```

### 本地覆盖配置 (config/local.json)

```json
{
  "Local": {
    "Comment": "本地开发覆盖配置，此文件不应提交到版本控制",
    "Debug": true
  }
}
```

## 核心代码

```csharp
// 构建多层级、多源配置
var cfg = new CfgBuilder()
    // Level 0: 基础配置（多种格式）
    .AddJson("config/base/app.json", level: 0)
    .AddYaml("config/base/database.yaml", level: 0)
    .AddToml("config/base/cache.toml", level: 0)
    .AddXml("config/base/services.xml", level: 0)
    .AddIni("config/base/security.ini", level: 0)
    
    // Level 1: WebApi 配置
    .AddJson("config/apqcfg.json", level: 1)
    
    // Level 5: 功能开关配置
    .AddJson("config/features/feature-flags.json", level: 5)
    
    // Level 10: 环境特定配置
    .AddEnv($"config/env/{environment.ToLower()}.env", level: 10, optional: true)
    
    // Level 15: 本地覆盖配置（可写）
    .AddJson("config/local.json", level: 15, writeable: true, isPrimaryWriter: true)
    
    .Build();

// 注册 WebApi 服务
builder.Services.AddApqCfgWebApi(cfg);

// 使用中间件和端点
app.UseApqCfgWebApi();
app.MapApqCfgWebApi();
```

## 相关文档

- [Apq.Cfg 快速入门](../../docs/site/guide/quick-start.md)
- [WebApi 使用指南](../../docs/site/guide/webapi.md)
- [配置源选择指南](../../docs/site/guide/source-selection.md)
- [多源配置合并](../../docs/site/guide/config-merge.md)
