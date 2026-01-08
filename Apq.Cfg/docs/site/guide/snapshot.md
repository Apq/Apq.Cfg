# 配置快照导出

Apq.Cfg 提供了配置快照导出功能，支持将当前配置状态导出为多种格式，便于调试、备份和迁移。

## 基本用法

### 导出为 JSON

```csharp
using Apq.Cfg;
using Apq.Cfg.Snapshot;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .Build();

// 导出为 JSON（默认格式）
var json = cfg.ExportSnapshot();

// 导出为格式化的 JSON
var jsonIndented = cfg.ExportSnapshotAsJson(indented: true);
```

### 导出为环境变量格式

```csharp
// 导出为环境变量格式
var env = cfg.ExportSnapshotAsEnv();
// 输出: APP__NAME=MyApp
//       DATABASE__HOST=localhost

// 添加前缀
var envWithPrefix = cfg.ExportSnapshotAsEnv(prefix: "MYAPP_");
// 输出: MYAPP_APP__NAME=MyApp
//       MYAPP_DATABASE__HOST=localhost
```

### 导出为字典

```csharp
// 导出为扁平化字典
var dict = cfg.ExportSnapshotAsDictionary();

foreach (var (key, value) in dict)
{
    Console.WriteLine($"{key} = {value}");
}
// 输出: App:Name = MyApp
//       Database:Host = localhost
```

## 导出选项

使用 `ExportOptions` 类自定义导出行为：

```csharp
var options = new ExportOptions
{
    // 是否缩进格式化（仅 JSON）
    Indented = true,

    // 是否包含元数据（导出时间、键数量等）
    IncludeMetadata = true,

    // 是否对敏感值进行脱敏
    MaskSensitiveValues = true,

    // 仅包含指定的键（支持通配符 *）
    IncludeKeys = new[] { "App:*", "Database:Host" },

    // 排除指定的键（支持通配符 *）
    ExcludeKeys = new[] { "Secrets:*", "Database:Password" },

    // 环境变量格式的键前缀
    EnvPrefix = "MYAPP_"
};

// 使用内置导出器导出
var json = cfg.ExportSnapshot(SnapshotExporters.Json, options);
var env = cfg.ExportSnapshot(SnapshotExporters.Env, options);
```

### 使用构建器模式

```csharp
var json = cfg.ExportSnapshot(options =>
{
    options.IncludeMetadata = true;
    options.ExcludeKeys = new[] { "Secrets:*" };
});
```

## 导出格式

### JSON 格式

导出为嵌套的 JSON 结构：

```json
{
  "App": {
    "Name": "MyApp",
    "Version": "1.0.0"
  },
  "Database": {
    "Host": "localhost",
    "Port": "5432"
  }
}
```

启用元数据后：

```json
{
  "App": {
    "Name": "MyApp"
  },
  "__metadata": {
    "exportedAt": "2026-01-02T10:30:00.0000000Z",
    "format": "Apq.Cfg.Snapshot",
    "version": "1.0",
    "keyCount": 5
  }
}
```

### KeyValue 格式

导出为扁平的键值对格式：

```
App:Name=MyApp
App:Version=1.0.0
Database:Host=localhost
Database:Port=5432
```

### Env 格式

导出为环境变量格式（键名转换为大写，冒号替换为双下划线）：

```bash
APP__NAME=MyApp
APP__VERSION=1.0.0
DATABASE__HOST=localhost
DATABASE__PORT=5432
```

## 导出到文件

### 异步导出到文件

```csharp
// 导出到文件
await cfg.ExportSnapshotToFileAsync("config-snapshot.json");

// 使用自定义选项
await cfg.ExportSnapshotToFileAsync("config-snapshot.json", new ExportOptions
{
    Indented = true,
    MaskSensitiveValues = true
});
```

### 导出到流

```csharp
using var stream = new MemoryStream();
await cfg.ExportSnapshotAsync(stream, new ExportOptions
{
    MaskSensitiveValues = false
});
```

## 过滤配置

### 包含指定键

```csharp
// 仅导出 App 和 Database:Host
var filtered = cfg.ExportSnapshot(new ExportOptions
{
    IncludeKeys = new[] { "App:*", "Database:Host" }
});
```

### 排除指定键

```csharp
// 排除敏感配置
var safe = cfg.ExportSnapshot(new ExportOptions
{
    ExcludeKeys = new[] { "Secrets:*", "Database:Password", "Api:Key" }
});
```

### 通配符支持

- `*` 匹配任意字符
- `?` 匹配单个字符

```csharp
// 匹配所有以 Database 开头的键
IncludeKeys = new[] { "Database:*" }

// 匹配 App:Name 和 App:Version
IncludeKeys = new[] { "App:Name", "App:Version" }

// 排除所有密码和密钥
ExcludeKeys = new[] { "*:Password", "*:Secret", "*:Key" }
```

## 敏感值脱敏

默认情况下，导出会对敏感值进行脱敏处理：

```csharp
// 启用脱敏（默认）
var masked = cfg.ExportSnapshot(new ExportOptions
{
    MaskSensitiveValues = true
});
// 输出: Database:Password = sec***123

// 禁用脱敏（仅用于调试）
var unmasked = cfg.ExportSnapshot(new ExportOptions
{
    MaskSensitiveValues = false
});
// 输出: Database:Password = secret123
```

## 静态方法

也可以直接使用 `ConfigExporter` 静态类：

```csharp
using Apq.Cfg.Snapshot;

// 导出为字符串
var json = ConfigExporter.Export(cfg);

// 导出为字典
var dict = ConfigExporter.ExportToDictionary(cfg);

// 导出到文件
await ConfigExporter.ExportToFileAsync(cfg, "snapshot.json");

// 导出到流
await ConfigExporter.ExportAsync(cfg, stream);
```

## 自定义导出器

使用 `SnapshotExporter` 委托可以创建自定义导出格式：

```csharp
using Apq.Cfg.Snapshot;

// 使用 Lambda 表达式创建自定义导出器
var yaml = cfg.ExportSnapshot((data, ctx) =>
{
    var sb = new StringBuilder();

    if (ctx.IncludeMetadata)
    {
        sb.AppendLine($"# Exported at: {ctx.ExportedAt:O}");
        sb.AppendLine($"# Key count: {ctx.KeyCount}");
        sb.AppendLine();
    }

    foreach (var (key, value) in data.OrderBy(x => x.Key))
    {
        // 简单的 YAML 格式
        sb.AppendLine($"{key}: \"{value}\"");
    }

    return sb.ToString();
});

// 导出到文件
await cfg.ExportSnapshotToFileAsync((data, ctx) =>
{
    // 自定义格式逻辑
    return string.Join("\n", data.Select(x => $"{x.Key}={x.Value}"));
}, "config.txt");
```

### ExportContext 属性

自定义导出器可以使用 `ExportContext` 获取导出上下文信息：

| 属性 | 类型 | 说明 |
|------|------|------|
| `ExportedAt` | `DateTime` | 导出时间（UTC） |
| `IncludeMetadata` | `bool` | 是否包含元数据 |
| `Indented` | `bool` | 是否缩进格式化 |
| `EnvPrefix` | `string?` | 环境变量前缀 |
| `KeyCount` | `int` | 配置键数量 |
| `Properties` | `IDictionary<string, object?>` | 自定义属性 |

### 内置导出器

| 导出器 | 说明 |
|--------|------|
| `SnapshotExporters.Json` | 嵌套 JSON 结构 |
| `SnapshotExporters.KeyValue` | 扁平键值对 |
| `SnapshotExporters.Env` | 环境变量格式 |

```csharp
// 直接使用内置导出器
var json = cfg.ExportSnapshot(SnapshotExporters.Json);
var kv = cfg.ExportSnapshot(SnapshotExporters.KeyValue);
var env = cfg.ExportSnapshot(SnapshotExporters.Env);
```

## 使用场景

### 调试配置问题

```csharp
// 在应用启动时导出配置快照
var snapshot = cfg.ExportSnapshotAsJson(maskSensitive: true);
logger.LogDebug("Configuration snapshot:\n{Snapshot}", snapshot);
```

### 配置备份

```csharp
// 定期备份配置
var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
await cfg.ExportSnapshotToFileAsync($"backup/config_{timestamp}.json", new ExportOptions
{
    Indented = true,
    MaskSensitiveValues = false  // 备份需要完整值
});
```

### 配置迁移

```csharp
// 导出为环境变量格式，用于容器部署
var envContent = cfg.ExportSnapshotAsEnv(prefix: "APP_");
await File.WriteAllTextAsync(".env.production", envContent);
```

### 配置比较

```csharp
// 导出两个环境的配置进行比较
var devConfig = devCfg.ExportSnapshotAsDictionary();
var prodConfig = prodCfg.ExportSnapshotAsDictionary();

var differences = devConfig.Keys
    .Where(k => !prodConfig.ContainsKey(k) || devConfig[k] != prodConfig[k])
    .ToList();
```

## API 参考

### ExportOptions 属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Indented` | `bool` | `true` | JSON 是否缩进 |
| `IncludeMetadata` | `bool` | `false` | 是否包含元数据 |
| `MaskSensitiveValues` | `bool` | `true` | 是否脱敏敏感值 |
| `IncludeKeys` | `string[]?` | `null` | 仅包含的键（支持通配符） |
| `ExcludeKeys` | `string[]?` | `null` | 排除的键（支持通配符） |
| `EnvPrefix` | `string?` | `null` | 环境变量格式的前缀 |

### 扩展方法

| 方法 | 说明 |
|------|------|
| `ExportSnapshot()` | 导出为字符串 |
| `ExportSnapshot(options)` | 使用选项导出 |
| `ExportSnapshot(configure)` | 使用构建器模式导出 |
| `ExportSnapshotAsJson()` | 导出为 JSON |
| `ExportSnapshotAsEnv()` | 导出为环境变量格式 |
| `ExportSnapshotAsDictionary()` | 导出为字典 |
| `ExportSnapshotToFileAsync()` | 导出到文件 |
| `ExportSnapshotAsync()` | 导出到流 |
