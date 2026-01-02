# 配置模板与变量替换

Apq.Cfg 支持在配置值中使用变量引用，实现配置的动态组合和复用。

> **与微软配置组件的对比**：`Microsoft.Extensions.Configuration` 不支持变量替换功能。这是 Apq.Cfg 的差异化特性，特别适用于避免配置重复、动态组合路径等场景。

## 基本用法

### 引用其他配置

```csharp
// config.json
{
    "App": {
        "Name": "MyApp",
        "LogPath": "${App:Name}/logs",
        "DataPath": "${App:Name}/data"
    }
}
```

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .Build();

// 使用 GetResolved 获取解析后的值
var logPath = cfg.GetResolved("App:LogPath");
// 返回: "MyApp/logs"

var dataPath = cfg.GetResolved("App:DataPath");
// 返回: "MyApp/data"
```

### 引用环境变量

使用 `${ENV:变量名}` 语法引用环境变量：

```csharp
// config.json
{
    "Paths": {
        "Home": "${ENV:USERPROFILE}",
        "Temp": "${ENV:TEMP}",
        "AppData": "${ENV:APPDATA}/MyApp"
    }
}
```

```csharp
var homePath = cfg.GetResolved("Paths:Home");
// 返回: "C:\Users\username"
```

### 引用系统属性

使用 `${SYS:属性名}` 语法引用系统属性：

```csharp
// config.json
{
    "System": {
        "Machine": "${SYS:MachineName}",
        "User": "${SYS:UserName}",
        "LogFile": "logs/${SYS:MachineName}_${SYS:Today}.log"
    }
}
```

```csharp
var logFile = cfg.GetResolved("System:LogFile");
// 返回: "logs/SERVER01_2026-01-02.log"
```

#### 支持的系统属性

| 属性名 | 说明 |
|--------|------|
| `MachineName` | 计算机名 |
| `UserName` | 当前用户名 |
| `UserDomainName` | 用户域名 |
| `OSVersion` | 操作系统版本 |
| `ProcessId` | 当前进程 ID |
| `CurrentDirectory` | 当前工作目录 |
| `SystemDirectory` | 系统目录 |
| `ProcessorCount` | 处理器数量 |
| `Is64BitProcess` | 是否 64 位进程 |
| `Is64BitOperatingSystem` | 是否 64 位操作系统 |
| `CLRVersion` | CLR 版本 |
| `Now` | 当前时间（ISO 8601 格式） |
| `UtcNow` | 当前 UTC 时间 |
| `Today` | 当前日期（yyyy-MM-dd） |

## 嵌套引用

变量可以嵌套引用，支持多层解析：

```csharp
// config.json
{
    "App": {
        "Name": "MyApp",
        "Version": "1.0.0"
    },
    "Paths": {
        "Base": "${ENV:APPDATA}/${App:Name}",
        "Data": "${Paths:Base}/data/${App:Version}"
    }
}
```

```csharp
var dataPath = cfg.GetResolved("Paths:Data");
// 返回: "C:\Users\username\AppData\Roaming\MyApp/data/1.0.0"
```

## 解析模板字符串

除了获取配置值，还可以直接解析任意模板字符串：

```csharp
var template = "Application ${App:Name} v${App:Version} running on ${SYS:MachineName}";
var result = cfg.ResolveVariables(template);
// 返回: "Application MyApp v1.0.0 running on SERVER01"
```

## 类型转换

`GetResolved<T>` 方法支持将解析后的值转换为指定类型：

```csharp
// config.json
{
    "Settings": {
        "BasePort": "8080",
        "Port": "${Settings:BasePort}"
    }
}
```

```csharp
var port = cfg.GetResolved<int>("Settings:Port");
// 返回: 8080 (int)
```

## 批量获取

```csharp
var keys = new[] { "App:LogPath", "App:DataPath", "Paths:Home" };
var values = cfg.GetManyResolved(keys);

foreach (var (key, value) in values)
{
    Console.WriteLine($"{key} = {value}");
}
```

## 自定义解析选项

### 修改变量语法

```csharp
var options = new VariableResolutionOptions
{
    VariablePrefix = "#{",    // 默认 "${"
    VariableSuffix = "}#",    // 默认 "}"
    PrefixSeparator = "."     // 默认 ":"
};
options.Resolvers.Add(VariableResolvers.Config);

// 使用自定义语法: #{App.Name}#
var result = cfg.GetResolved("Key", options);
```

### 控制递归深度

```csharp
var options = new VariableResolutionOptions
{
    MaxRecursionDepth = 5  // 默认 10
};
```

### 未解析变量的处理

```csharp
var options = new VariableResolutionOptions
{
    // Keep: 保留原始表达式（默认）
    // Empty: 替换为空字符串
    // Throw: 抛出异常
    UnresolvedBehavior = UnresolvedVariableBehavior.Throw
};
```

## 通过 CfgBuilder 配置

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .ConfigureVariableResolution(options =>
    {
        options.MaxRecursionDepth = 5;
        options.UnresolvedBehavior = UnresolvedVariableBehavior.Empty;
    })
    .Build();
```

### 添加自定义解析器

```csharp
// 自定义解析器
public class CustomResolver : IVariableResolver
{
    public string? Prefix => "CUSTOM";

    public string? Resolve(string variableName, ICfgRoot cfg)
    {
        return variableName switch
        {
            "Timestamp" => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            "Guid" => Guid.NewGuid().ToString(),
            _ => null
        };
    }
}

// 注册自定义解析器
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddVariableResolver(new CustomResolver())
    .Build();

// 使用: ${CUSTOM:Timestamp}
```

## 循环引用检测

模板引擎会自动检测循环引用并抛出异常：

```csharp
// config.json - 循环引用
{
    "A": "${B}",
    "B": "${A}"
}
```

```csharp
// 抛出 InvalidOperationException: 检测到循环引用: A
cfg.GetResolved("A");
```

## API 参考

### 扩展方法

| 方法 | 说明 |
|------|------|
| `GetResolved(key)` | 获取解析变量后的配置值 |
| `GetResolved<T>(key)` | 获取解析后的值并转换类型 |
| `GetResolved(key, options)` | 使用自定义选项获取解析后的值 |
| `TryGetResolved(key, out value)` | 尝试获取解析后的值 |
| `TryGetResolved<T>(key, out value)` | 尝试获取解析后的值并转换类型 |
| `GetManyResolved(keys)` | 批量获取解析后的值 |
| `ResolveVariables(template)` | 解析模板字符串中的变量 |
| `ResolveVariables(template, options)` | 使用自定义选项解析模板 |

### VariableResolutionOptions 属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `VariablePrefix` | `string` | `"${"` | 变量前缀 |
| `VariableSuffix` | `string` | `"}"` | 变量后缀 |
| `PrefixSeparator` | `string` | `":"` | 前缀分隔符 |
| `MaxRecursionDepth` | `int` | `10` | 最大递归深度 |
| `UnresolvedBehavior` | `enum` | `Keep` | 未解析变量的处理方式 |
| `CacheResults` | `bool` | `true` | 是否缓存解析结果 |
| `InvalidateCacheOnChange` | `bool` | `true` | 配置变更时清除缓存 |
| `Resolvers` | `IList` | 内置解析器 | 变量解析器列表 |

### 内置解析器

| 解析器 | 前缀 | 说明 |
|--------|------|------|
| `VariableResolvers.Config` | 无 | 引用其他配置键 |
| `VariableResolvers.Environment` | `ENV` | 引用环境变量 |
| `VariableResolvers.System` | `SYS` | 引用系统属性 |
