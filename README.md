# Apq.Cfg

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

.NET 统一配置组件库，支持多种配置格式和远程配置中心。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 特性

- **多格式支持**：JSON、INI、XML、YAML、TOML、HCL、Properties、Env
- **远程配置中心**：Consul、Etcd、Nacos、Apollo、Zookeeper、Vault
- **配置加密脱敏**：AES-GCM、ChaCha20、RSA、SM4 等多种算法
- **多层级合并**：高层级覆盖低层级，支持可写配置
- **热重载**：文件变更自动检测、防抖、增量更新
- **源生成器**：Native AOT 支持，零反射绑定
- **Web 管理**：RESTful API 和 Web 界面，支持远程配置管理

## 安装

```bash
dotnet add package Apq.Cfg
```

## 快速开始

```csharp
using Apq.Cfg;

// 构建配置
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile("config.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .Build();

// 读取配置
var host = cfg["Database:Host"];
var port = cfg.GetValue<int>("Database:Port");

// 使用配置节
var db = cfg.GetSection("Database");
var name = db["Name"];

// 修改并保存
cfg["App:LastRun"] = DateTime.Now.ToString();
await cfg.SaveAsync();
```

## 多格式支持示例

### HOCON/HCL 格式

```csharp
using Apq.Cfg;
using Apq.Cfg.Hcl;

var cfg = new CfgBuilder()
    .AddHclFile("config.hcl")
    .Build();

var host = cfg["database:host"];
```

### Properties 格式

```csharp
using Apq.Cfg;
using Apq.Cfg.Properties;

var cfg = new CfgBuilder()
    .AddPropertiesFile("config.properties")
    .Build();

var host = cfg["Database:Host"];
```

## Web API 集成

为应用添加配置管理 API：

```csharp
using Apq.Cfg;
using Apq.Cfg.WebApi;

var builder = WebApplication.CreateBuilder(args);

var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile("config.local.json", level: 5, writeable: true, isPrimaryWriter: true)
    .Build();

// 注册配置和 API 服务
builder.Services.AddSingleton<ICfgRoot>(cfg);
builder.Services.AddApqCfgWebApi(options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = "your-secret-key";
});

var app = builder.Build();
app.UseApqCfgWebApi();
app.MapApqCfgWebApi();
app.Run();
```

API 文档 UI 根据目标框架自动选择：
- .NET 8：Swagger UI (`/swagger`)
- .NET 10+：Scalar (`/scalar/v1`)

## 配置源层级

| 层级 | 用途 | 配置源 |
|------|------|--------|
| 0-99 | 本地文件 | Json, Ini, Xml, Yaml, Toml, Hcl, Properties |
| 100-199 | 远程存储 | Redis, Database |
| 200-299 | 配置中心 | Consul, Etcd, Nacos, Apollo, Zookeeper |
| 300-399 | 密钥管理 | Vault |
| 400+ | 环境变量 | Env, EnvironmentVariables |

## NuGet 包

| 包名 | 说明 |
|------|------|
| `Apq.Cfg` | 核心库（JSON、环境变量） |
| `Apq.Cfg.Ini` | INI 格式 |
| `Apq.Cfg.Xml` | XML 格式 |
| `Apq.Cfg.Yaml` | YAML 格式 |
| `Apq.Cfg.Toml` | TOML 格式 |
| `Apq.Cfg.Hcl` | HCL 格式 |
| `Apq.Cfg.Properties` | Properties 格式 |
| `Apq.Cfg.Env` | .env 文件 |
| `Apq.Cfg.Redis` | Redis 存储 |
| `Apq.Cfg.Database` | 数据库存储 |
| `Apq.Cfg.Consul` | Consul 配置中心 |
| `Apq.Cfg.Etcd` | Etcd 配置中心 |
| `Apq.Cfg.Nacos` | Nacos 配置中心 |
| `Apq.Cfg.Apollo` | Apollo 配置中心 |
| `Apq.Cfg.Zookeeper` | Zookeeper 配置中心 |
| `Apq.Cfg.Vault` | HashiCorp Vault |
| `Apq.Cfg.Crypto` | 配置加密脱敏 |
| `Apq.Cfg.SourceGenerator` | 源生成器 (Native AOT) |
| `Apq.Cfg.WebApi` | RESTful API 接口 |
| `Apq.Cfg.WebUI` | Web 管理界面 |

## 支持的框架

.NET 8.0 / 10.0 (LTS)

## 构建与测试

```bash
dotnet build
dotnet test
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com
- Gitee：https://gitee.com/apq/Apq.Cfg
