# CfgBuilder API

`CfgBuilder` 是配置构建器，用于组合多个配置源并创建配置实例。

## 构造函数

```csharp
public CfgBuilder()
```

创建一个新的配置构建器实例。

## 方法

### AddSource

```csharp
public CfgBuilder AddSource(ICfgSource source, bool optional = false)
```

添加自定义配置源。

**参数：**
- `source`: 配置源实例
- `optional`: 是否可选，默认 `false`

**返回：** 当前构建器实例（支持链式调用）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddSource(new MyCustomSource())
    .Build();
```

### AddJsonFile

```csharp
public CfgBuilder AddJsonFile(string path, bool optional = false, bool reloadOnChange = false)
```

添加 JSON 文件配置源。

**参数：**
- `path`: 文件路径
- `optional`: 是否可选
- `reloadOnChange`: 是否在文件变更时重载

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddJsonFile("config.local.json", optional: true, reloadOnChange: true)
    .Build();
```

### AddInMemory

```csharp
public CfgBuilder AddInMemory(IDictionary<string, string> data)
```

添加内存配置源。

**参数：**
- `data`: 配置键值对字典

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddInMemory(new Dictionary<string, string>
    {
        ["Database:Host"] = "localhost",
        ["Database:Port"] = "5432"
    })
    .Build();
```

### AddEnvironmentVariables

```csharp
public CfgBuilder AddEnvironmentVariables(string prefix = null)
```

添加环境变量配置源。

**参数：**
- `prefix`: 环境变量前缀（可选）

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddEnvironmentVariables("MYAPP_")
    .Build();
```

### ConfigureReload

```csharp
public CfgBuilder ConfigureReload(Action<DynamicReloadOptions> configure)
```

配置动态重载选项。

**参数：**
- `configure`: 配置委托

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", reloadOnChange: true)
    .ConfigureReload(options =>
    {
        options.DebounceDelay = 500;
        options.Strategy = ReloadStrategy.Debounced;
    })
    .Build();
```

### ConfigureEncoding

```csharp
public CfgBuilder ConfigureEncoding(Action<EncodingOptions> configure)
```

配置文件编码选项。

**参数：**
- `configure`: 配置委托

**示例：**
```csharp
var cfg = new CfgBuilder()
    .ConfigureEncoding(options =>
    {
        options.DefaultEncoding = Encoding.UTF8;
        options.AddMapping(".ini", Encoding.GetEncoding("GBK"));
    })
    .AddJsonFile("config.json")
    .Build();
```

### Build

```csharp
public ICfgRoot Build()
```

构建配置实例。

**返回：** `ICfgRoot` 配置根实例

**示例：**
```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .Build();
```

### BuildAsync

```csharp
public Task<ICfgRoot> BuildAsync(CancellationToken cancellationToken = default)
```

异步构建配置实例。

**返回：** `Task<ICfgRoot>` 配置根实例

**示例：**
```csharp
var cfg = await new CfgBuilder()
    .AddConsul("http://localhost:8500", "myapp/config")
    .BuildAsync();
```

## 完整示例

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    // 基础配置
    .AddJsonFile("config.json")
    // 环境特定配置
    .AddJsonFile($"config.{environment}.json", optional: true)
    // 本地覆盖
    .AddJsonFile("config.local.json", optional: true, reloadOnChange: true)
    // 远程配置
    .AddConsul("http://consul:8500", "myapp/config", optional: true)
    // 环境变量
    .AddEnvironmentVariables("MYAPP_")
    // 配置重载
    .ConfigureReload(options =>
    {
        options.DebounceDelay = 1000;
    })
    .Build();
```

## 下一步

- [ICfgRoot API](/api/icfg-root) - 配置根接口
- [ICfgSection API](/api/icfg-section) - 配置节接口
