# Apq.Cfg.Samples - 功能示例

本示例项目演示了 Apq.Cfg 配置库的完整功能，包含 8 个独立的示例，覆盖所有主要特性。

## 运行示例

```bash
cd Samples/Apq.Cfg.Samples
dotnet run
```

## 功能覆盖

| 示例 | 功能模块 | 覆盖的 API |
|------|----------|-----------|
| 示例 1 | 基础用法 | `CfgBuilder`, `AddJson`, `AddEnvironmentVariables`, `Get`, `Set`, `Remove`, `Exists`, `SaveAsync`, `ToMicrosoftConfiguration` |
| 示例 2 | 多格式支持 | `AddIni`, `AddXml`, `AddYaml`, `AddToml`, 混合格式层级覆盖 |
| 示例 3 | 配置节 | `GetSection`, `GetChildKeys`, 嵌套配置访问 |
| 示例 4 | 批量操作 | `GetMany`, `GetMany<T>`, `SetMany` |
| 示例 5 | 类型转换 | `Get<T>` 支持 int, long, double, decimal, bool, DateTime, Guid, Enum |
| 示例 6 | 动态重载 | `DynamicReloadOptions`, `IChangeToken`, `ConfigChanges` (Rx) |
| 示例 7 | 依赖注入 | `AddApqCfg`, `ConfigureApqCfg<T>`, `IOptions<T>` |
| 示例 8 | 编码映射 | `WithEncodingConfidenceThreshold`, `WithEncodingDetectionLogging`, `AddReadEncodingMapping`, `AddWriteEncodingMappingWildcard` |

## 示例详情

### 示例 1: 基础用法 - JSON 配置与层级覆盖

演示核心功能：
- 创建多层级配置（基础配置 + 本地覆盖配置 + 环境变量）
- `level` 参数控制优先级（数值越大优先级越高）
- `writeable` 参数控制配置源是否可写
- `isPrimaryWriter` 指定默认写入目标
- `targetLevel` 参数指定写入到特定层级

```csharp
var cfg = new CfgBuilder()
    .AddJson(configPath, level: 0, writeable: false)
    .AddJson(localConfigPath, level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "MYAPP_")
    .Build();

// 读取（自动从最高优先级获取）
var value = cfg.Get("App:Name");

// 写入（需指定 targetLevel，因为最高层级环境变量不可写）
cfg.Set("App:LastRun", DateTime.Now.ToString(), targetLevel: 1);
await cfg.SaveAsync(targetLevel: 1);
```

### 示例 2: 多格式支持

支持的配置格式：
- **JSON** - 内置支持
- **INI** - 需引用 `Apq.Cfg.Ini`
- **XML** - 需引用 `Apq.Cfg.Xml`
- **YAML** - 需引用 `Apq.Cfg.Yaml`
- **TOML** - 需引用 `Apq.Cfg.Toml`

```csharp
// 混合多种格式，通过 level 控制优先级
using var cfg = new CfgBuilder()
    .AddIni(iniPath, level: 0)
    .AddYaml(yamlPath, level: 1)
    .AddToml(tomlPath, level: 2, isPrimaryWriter: true)
    .Build();
```

### 示例 3: 配置节与子键枚举

```csharp
// 获取配置节简化嵌套访问
var dbSection = cfg.GetSection("Database");
var primarySection = dbSection.GetSection("Primary");
var host = primarySection.Get("Host");

// 枚举子键
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine(key);  // 输出: Primary, Replica
}
```

### 示例 4: 批量操作

```csharp
// 批量获取
var keys = new[] { "Settings:Theme", "Settings:Language" };
var values = cfg.GetMany(keys);

// 批量获取并转换类型
var intValues = cfg.GetMany<int>(new[] { "Settings:FontSize" });

// 批量设置
cfg.SetMany(new Dictionary<string, string?>
{
    ["Settings:Theme"] = "light",
    ["Settings:FontSize"] = "16"
});
```

### 示例 5: 类型转换

支持的类型：
- 基本类型：`int`, `long`, `double`, `decimal`, `bool`
- 日期时间：`DateTime`, `DateTimeOffset`
- 其他：`Guid`, `Enum`, 可空类型

```csharp
var intVal = cfg.Get<int>("Types:IntValue");
var dateVal = cfg.Get<DateTime>("Types:DateValue");
var enumVal = cfg.Get<LogLevel>("Types:EnumValue");
var nullableVal = cfg.Get<int?>("Types:NotExist");  // 返回 null
```

### 示例 6: 动态配置重载

```csharp
// 启用文件变更监听
var cfg = new CfgBuilder()
    .AddJson(configPath, reloadOnChange: true)
    .Build();

// 配置重载选项
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 100,           // 防抖时间
    EnableDynamicReload = true,  // 启用动态重载
    Strategy = ReloadStrategy.Eager,  // 立即重载
    RollbackOnError = true,      // 错误时回滚
    HistorySize = 5              // 保留历史记录数
});

// 方式1: 使用 IChangeToken
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));

// 方式2: 使用 Rx 订阅
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

### 示例 7: 依赖注入集成

```csharp
var services = new ServiceCollection();

// 注册配置服务
services.AddApqCfg(cfg => cfg
    .AddJson(configPath, writeable: true, isPrimaryWriter: true));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");
services.ConfigureApqCfg<LoggingOptions>("Logging");

// 使用
var provider = services.BuildServiceProvider();
var cfgRoot = provider.GetRequiredService<ICfgRoot>();
var dbOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
```

### 示例 8: 编码映射配置

```csharp
var cfg = new CfgBuilder()
    // 设置编码检测置信度阈值
    .WithEncodingConfidenceThreshold(0.7f)
    // 编码检测日志回调
    .WithEncodingDetectionLogging(result =>
    {
        Console.WriteLine($"编码: {result.Encoding.EncodingName}");
        Console.WriteLine($"置信度: {result.Confidence:P0}");
        Console.WriteLine($"方法: {result.Method}");
    })
    // 为特定文件指定读取编码
    .AddReadEncodingMapping(configPath, Encoding.UTF8, priority: 100)
    // 为通配符匹配的文件指定写入编码
    .AddWriteEncodingMappingWildcard("*.json", new UTF8Encoding(false), priority: 50)
    // 使用正则表达式匹配
    .AddReadEncodingMappingRegex(@"config.*\.json$", Encoding.UTF8, priority: 80)
    .AddJson(configPath)
    .Build();
```

## 项目依赖

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Apq.Cfg\Apq.Cfg.csproj" />
  <ProjectReference Include="..\..\Apq.Cfg.Ini\Apq.Cfg.Ini.csproj" />
  <ProjectReference Include="..\..\Apq.Cfg.Xml\Apq.Cfg.Xml.csproj" />
  <ProjectReference Include="..\..\Apq.Cfg.Yaml\Apq.Cfg.Yaml.csproj" />
  <ProjectReference Include="..\..\Apq.Cfg.Toml\Apq.Cfg.Toml.csproj" />
</ItemGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.1" />
</ItemGroup>
```

## 注意事项

1. **层级与可写性**：当最高优先级的配置源不可写时（如环境变量），调用 `Set()` 和 `SaveAsync()` 需要显式指定 `targetLevel` 参数。

2. **isPrimaryWriter**：只能有一个配置源设置为 `isPrimaryWriter: true`，它将作为默认的写入目标。

3. **reloadOnChange**：启用后会监听文件变更，配合 `DynamicReloadOptions` 可以实现配置热更新。

4. **编码检测**：默认使用 UTF.Unknown 库进行编码检测，可通过映射规则强制指定编码。
