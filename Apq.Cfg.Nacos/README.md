# Apq.Cfg.Nacos

[![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Nacos)](https://www.nuget.org/packages/Apq.Cfg.Nacos)

Nacos 配置中心支持，为 Apq.Cfg 提供从 Nacos 读取配置的能力，使用官方 SDK。

## 安装

```bash
dotnet add package Apq.Cfg.Nacos
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.Nacos;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddNacos(options =>
    {
        options.ServerAddresses = "localhost:8848";
        options.Namespace = "public";
        options.DataId = "app-config";
        options.Group = "DEFAULT_GROUP";
    }, level: 10)
    .Build();

// 读取配置
var value = cfg.Get("Database:Host");
```

## 配置选项

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `ServerAddresses` | string | `localhost:8848` | Nacos 服务地址，多个地址用逗号分隔 |
| `Namespace` | string | `public` | 命名空间 ID |
| `DataId` | string | `""` | 配置的 DataId |
| `Group` | string | `DEFAULT_GROUP` | 配置分组 |
| `Username` | string? | null | 用户名（可选） |
| `Password` | string? | null | 密码（可选） |
| `AccessKey` | string? | null | Access Key（阿里云 MSE） |
| `SecretKey` | string? | null | Secret Key（阿里云 MSE） |
| `ConnectTimeoutMs` | int | 10000 | 连接超时时间（毫秒） |
| `DataFormat` | NacosDataFormat | Json | 配置数据格式 |

## 数据格式

### JSON 模式（默认）

```json
{
  "Database": {
    "Host": "localhost",
    "Port": 5432
  },
  "App": {
    "Name": "MyApp"
  }
}
```

```csharp
.AddNacos(options =>
{
    options.DataId = "app-config";
    options.DataFormat = NacosDataFormat.Json;
})
```

### Properties 模式

```properties
Database.Host=localhost
Database.Port=5432
App.Name=MyApp
```

```csharp
.AddNacos(options =>
{
    options.DataId = "app-config";
    options.DataFormat = NacosDataFormat.Properties;
})
```

### YAML 模式

```yaml
Database:
  Host: localhost
  Port: 5432
App:
  Name: MyApp
```

```csharp
.AddNacos(options =>
{
    options.DataId = "app-config";
    options.DataFormat = NacosDataFormat.Yaml;
})
```

> 注意：YAML 模式目前将整个内容作为 `_raw` 键存储，完整解析需要额外依赖。

## 认证

### 用户名密码认证

```csharp
.AddNacos(options =>
{
    options.ServerAddresses = "localhost:8848";
    options.Username = "nacos";
    options.Password = "nacos";
})
```

### 阿里云 MSE 认证

```csharp
.AddNacos(options =>
{
    options.ServerAddresses = "mse-xxx.nacos.mse.aliyuncs.com:8848";
    options.AccessKey = "your-access-key";
    options.SecretKey = "your-secret-key";
})
```

## 多层级配置

Nacos 配置源可以与其他配置源组合使用，通过 `level` 参数控制优先级：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // 基础配置
    .AddJson("config.local.json", level: 1)     // 本地覆盖
    .AddNacos(options =>                         // Nacos 远程配置（最高优先级）
    {
        options.ServerAddresses = "nacos:8848";
        options.DataId = "myapp-config";
    }, level: 10)
    .Build();
```

## 配置写入

Nacos 配置源支持写入操作，可以将配置修改发布到 Nacos：

```csharp
var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = "localhost:8848";
        options.DataId = "app-config";
    }, level: 0, isPrimaryWriter: true)
    .Build();

// 修改配置
cfg.Set("App:Version", "2.0.0");
await cfg.SaveAsync();  // 发布到 Nacos
```

## 简化用法

```csharp
// 使用简化的扩展方法
var cfg = new CfgBuilder()
    .AddNacos("localhost:8848", "app-config", "DEFAULT_GROUP", level: 10)
    .Build();
```

## 依赖

- [nacos-sdk-csharp](https://github.com/nacos-group/nacos-sdk-csharp) - Nacos 官方 .NET SDK
