# Nacos 配置中心

Nacos 是阿里巴巴开源的动态服务发现、配置管理和服务管理平台。

## 安装

```bash
dotnet add package Apq.Cfg.Nacos
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Nacos` (200)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 200
.AddNacos(options => { ... })

// 指定自定义层级
.AddNacos(options => { ... }, level: 250)
```

## 快速开始

```csharp
using Apq.Cfg;
using Apq.Cfg.Nacos;

var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddNacos(options =>
    {
        options.ServerAddresses = "localhost:8848";
        options.Namespace = "public";
        options.DataId = "app-config";
        options.Group = "DEFAULT_GROUP";
        options.EnableHotReload = true;
    })  // 使用默认层级 200
    .Build();

// 读取配置
var value = cfg["Database:Host"];
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
| `EnableHotReload` | bool | false | 是否启用热重载 |
| `ReconnectIntervalMs` | int | 5000 | 重连间隔（毫秒） |

## 热重载

Nacos 配置源支持热重载，当 Nacos 中的配置发生变化时，会自动更新本地配置：

```csharp
using Apq.Cfg;
using Apq.Cfg.Nacos;
using Microsoft.Extensions.Primitives;

var cfg = new CfgBuilder()
    .AddNacos(options =>
    {
        options.ServerAddresses = "localhost:8848";
        options.DataId = "app-config";
        options.EnableHotReload = true;
    })  // 使用默认层级 200
    .Build();

// 方式1：使用 Rx 订阅配置变更
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});

// 方式2：使用 IChangeToken 监听变更
var msConfig = cfg.ToMicrosoftConfiguration();
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));
```

### 热重载特性

- **实时监听**：使用 Nacos SDK 的 `IListener` 接口监听配置变更
- **自动更新**：配置变更时自动解析并更新本地数据
- **变更通知**：通过 `ConfigChanges` 或 `IChangeToken` 通知订阅者
- **优雅关闭**：Dispose 时自动移除监听器

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
// 使用简化方法
.AddNacosJson("localhost:8848", "app-config", enableHotReload: true)

// 或使用完整配置
.AddNacos(options =>
{
    options.DataId = "app-config";
    options.DataFormat = NacosDataFormat.Json;
    options.EnableHotReload = true;
})
```

### Properties 模式

```properties
Database.Host=localhost
Database.Port=5432
App.Name=MyApp
```

```csharp
// 使用简化方法
.AddNacosProperties("localhost:8848", "app-config", enableHotReload: true)

// 或使用完整配置
.AddNacos(options =>
{
    options.DataId = "app-config";
    options.DataFormat = NacosDataFormat.Properties;
    options.EnableHotReload = true;
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
    options.EnableHotReload = true;
})
```

## 认证

### 用户名密码认证

```csharp
.AddNacos(options =>
{
    options.ServerAddresses = "localhost:8848";
    options.Username = "nacos";
    options.Password = "nacos";
    options.EnableHotReload = true;
})
```

### 阿里云 MSE 认证

```csharp
.AddNacos(options =>
{
    options.ServerAddresses = "mse-xxx.nacos.mse.aliyuncs.com:8848";
    options.AccessKey = "your-access-key";
    options.SecretKey = "your-secret-key";
    options.EnableHotReload = true;
})
```

## 多层级配置

Nacos 配置源可以与其他配置源组合使用：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")                              // 基础配置
    .AddJson("config.local.json", level: 1)              // 本地覆盖
    .AddNacos(options =>                                 // Nacos 远程配置（使用默认层级 200）
    {
        options.ServerAddresses = "nacos:8848";
        options.DataId = "myapp-config";
        options.EnableHotReload = true;
    })
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
        options.EnableHotReload = true;
    }, isPrimaryWriter: true)  // 使用默认层级 200
    .Build();

// 修改配置
cfg["App:Version"] = "2.0.0";
await cfg.SaveAsync();  // 发布到 Nacos
```

## 简化用法

```csharp
// 使用简化的扩展方法
var cfg = new CfgBuilder()
    .AddNacos("localhost:8848", "app-config", "DEFAULT_GROUP", enableHotReload: true)  // 使用默认层级 200
    .Build();

// JSON 格式简化方法
var cfg2 = new CfgBuilder()
    .AddNacosJson("localhost:8848", "app-config.json", enableHotReload: true)
    .Build();

// Properties 格式简化方法
var cfg3 = new CfgBuilder()
    .AddNacosProperties("localhost:8848", "app-config.properties", enableHotReload: true)
    .Build();
```

## 依赖

- [nacos-sdk-csharp](https://github.com/nacos-group/nacos-sdk-csharp) - Nacos 官方 .NET SDK

## 下一步

- [Apollo](/config-sources/apollo) - Apollo 配置中心
- [Consul](/config-sources/consul) - Consul 配置中心
