# API 参考

本节包含 Apq.Cfg 所有公开 API 的详细文档。

> 文档基于 .NET 10.0 生成，API 与 .NET 8.0 版本完全兼容。

## 核心接口

| 接口 | 描述 |
|------|------|
| [ICfgRoot](/api/icfg-root) | 配置根接口，主入口点 |
| [ICfgSection](/api/icfg-section) | 配置节接口 |

## 核心类

| 类 | 描述 |
|----|------|
| [CfgBuilder](/api/cfg-builder) | 流式 API 构建器 |

## 扩展方法

| 分类 | 描述 |
|------|------|
| [扩展方法](/api/extensions) | 各种配置源的扩展方法 |

## 快速参考

### 创建配置

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### 读取值

```csharp
// 字符串值
string? value = cfg["App:Name"];

// 类型化值
int port = cfg.GetValue<int>("App:Port");

// 检查是否存在
bool exists = cfg.Exists("App:Name");

// 获取节
ICfgSection section = cfg.GetSection("Database");
```

### 写入值

```csharp
// 设置值
cfg.SetValue("App:Name", "NewName");

// 删除值
cfg.Remove("App:TempKey");

// 保存更改
await cfg.SaveAsync();
```

### 批量操作

```csharp
// 批量读取
var values = cfg.GetMany(new[] { "Key1", "Key2", "Key3" });

// 批量写入
cfg.SetManyValues(new Dictionary<string, string?>
{
    ["Key1"] = "Value1",
    ["Key2"] = "Value2"
});
```

### 配置变更

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}");
    }
});
```

## 下一步

- [CfgBuilder](/api/cfg-builder) - 构建器 API 参考
- [ICfgRoot](/api/icfg-root) - 根接口参考
- [ICfgSection](/api/icfg-section) - 节接口参考
