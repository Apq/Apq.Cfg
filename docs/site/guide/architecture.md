# 架构设计

本文档详细说明 Apq.Cfg 的架构设计、核心组件和交互流程。

## 架构概览

```
┌─────────────────────────────────────────────────────────────────┐
│                        应用程序层                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────┐  │
│  │ cfg["key"]  │  │ cfg.SetValue()   │  │ cfg.ConfigChanges       │  │
│  │ cfg.GetSection() │ cfg.SaveAsync() │ (Rx Observable)    │  │
│  └──────┬──────┘  └──────┬──────┘  └───────────┬─────────────┘  │
└─────────┼────────────────┼─────────────────────┼────────────────┘
          │                │                     │
┌─────────▼────────────────▼─────────────────────▼────────────────┐
│                      ICfgRoot 接口                               │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │                   MergedCfgRoot                              ││
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  ││
│  │  │ 层级合并    │  │ 待保存队列  │  │ ChangeCoordinator   │  ││
│  │  │ (Level)     │  │ (Pending)   │  │ (变更协调器)        │  ││
│  │  └─────────────┘  └─────────────┘  └─────────────────────┘  ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
          │                │                     │
┌─────────▼────────────────▼─────────────────────▼────────────────┐
│                      配置源层 (ICfgSource)                       │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐   │
│  │ JSON    │ │ YAML    │ │ Consul  │ │ Nacos   │ │ Vault   │   │
│  │ Level:0 │ │ Level:1 │ │ Level:10│ │ Level:10│ │ Level:15│   │
│  └────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘ └────┬────┘   │
└───────┼──────────┼──────────┼──────────┼──────────┼─────────────┘
        │          │          │          │          │
┌───────▼──────────▼──────────▼──────────▼──────────▼─────────────┐
│                Microsoft.Extensions.Configuration                │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │              IConfigurationRoot (合并后的配置)               ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

## 核心组件

### 1. ICfgRoot 接口

配置根接口，是整个系统的入口点。

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 索引器
    string? this[string key] { get; set; }

    // 读取操作
    T? GetValue<T>(string key);
    bool Exists(string key);
    ICfgSection GetSection(string key);

    // 写入操作
    void SetValue(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken ct = default);

    // 批量操作
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);
    void GetMany(IEnumerable<string> keys, Action<string, string?> onValue);
    void SetManyValues(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);

    // 转换与事件
    IConfigurationRoot ToMicrosoftConfiguration();
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
```

### 2. ICfgSection 接口

配置节接口，提供对配置子树的访问。

```csharp
public interface ICfgSection
{
    string Path { get; }
    string? this[string key] { get; set; }
    T? GetValue<T>(string key);
    void SetValue(string key, string? value, int? targetLevel = null);
    ICfgSection GetSection(string key);
    IEnumerable<string> GetChildKeys();
}
```

### 3. ICfgSource 接口

配置源接口，所有配置源的基础抽象。

```csharp
public interface ICfgSource
{
    int Level { get; }              // 层级优先级
    bool IsWriteable { get; }       // 是否可写
    bool IsPrimaryWriter { get; }   // 是否为主写入源
    IConfigurationSource BuildSource();
}

public interface IWritableCfgSource : ICfgSource
{
    void SetValue(string key, string? value);
    void Remove(string key);
    Task SaveAsync(CancellationToken ct = default);
}
```

### 4. MergedCfgRoot 实现

核心实现类，负责多配置源的合并和管理。

```csharp
internal sealed class MergedCfgRoot : ICfgRoot
{
    // 按层级组织的配置源
    private readonly Dictionary<int, LevelData> _levelData;
    
    // 合并后的 Microsoft Configuration
    private readonly IConfigurationRoot _merged;
    
    // 变更协调器（处理热重载）
    private ChangeCoordinator? _coordinator;
    
    // 配置变更事件流
    private readonly Subject<ConfigChangeEvent> _configChangesSubject;
    
    // 缓存的层级数组（性能优化）
    private readonly int[] _levelsDescending;
    private readonly int[] _levelsAscending;
}
```

### 5. CfgBuilder 构建器

流式 API 构建器，用于创建配置实例。

```csharp
public class CfgBuilder
{
    private readonly List<ICfgSource> _sources = new();
    private readonly EncodingMappingConfig _encodingConfig = new();
    
    public CfgBuilder AddJson(string path, int level, ...);
    public CfgBuilder AddYaml(string path, int level, ...);
    public CfgBuilder AddConsul(Action<ConsulCfgOptions> configure, int level, ...);
    public CfgBuilder AddVault(Action<VaultCfgOptions> configure, int level, ...);
    
    public ICfgRoot Build();
}
```

## 层级合并机制

### 层级优先级

配置源按 `Level` 属性排序，数值越大优先级越高：

```
Level 0:  config.json          (基础配置)
Level 1:  config.{env}.json    (环境配置)
Level 2:  config.local.json    (本地覆盖)
Level 10: Consul/Nacos/Etcd         (远程配置)
Level 15: Vault                     (密钥配置)
Level 20: 环境变量                   (最高优先级)
```

### 合并算法

```csharp
// 索引器实现
public string? this[string key]
{
    get
    {
        // 1. 先检查待保存队列（从高层级到低层级）
        foreach (var level in _levelsDescending)
        {
            if (_levelData[level].Pending.TryGetValue(key, out var value))
                return value;
        }

        // 2. 从合并后的配置中获取
        return _merged[key];
    }
}
```

### 写入策略

```csharp
public void SetValue(string key, string? value, int? targetLevel = null)
{
    // 1. 确定目标层级
    var level = targetLevel ?? GetDefaultWriteLevel();
    
    // 2. 写入待保存队列
    _levelData[level].Pending[key] = value;
    
    // 3. 写入配置源（如果支持）
    if (_levelData[level].Primary is IWritableCfgSource writable)
    {
        writable.Set(key, value);
    }
}
```

## 热重载机制

### 变更协调器 (ChangeCoordinator)

```
┌─────────────────────────────────────────────────────────────┐
│                    ChangeCoordinator                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ 防抖处理    │  │ 增量更新    │  │ 变更通知            │  │
│  │ (Debounce)  │  │ (Diff)      │  │ (ConfigChanges)     │  │
│  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘  │
└─────────┼────────────────┼─────────────────────┼────────────┘
          │                │                     │
          ▼                ▼                     ▼
    文件变更事件      配置源重载           Rx Observable
```

### 热重载流程

1. **文件监听**：FileSystemWatcher 监听配置文件变更
2. **防抖处理**：多次快速变更合并为一次处理（默认 100ms）
3. **增量更新**：只重载发生变化的配置源
4. **差异计算**：比较新旧配置，生成变更事件
5. **通知订阅者**：通过 `ConfigChanges` 发布变更事件

```csharp
// 订阅配置变更
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## 编码处理流程

### 读取编码检测

```
┌─────────────────────────────────────────────────────────────┐
│                     编码检测流程                             │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ 用户指定    │ -> │ 编码映射    │ -> │ 缓存结果        │  │
│  │ (Specified) │    │ (Mapping)   │    │ (Cache)         │  │
│  └──────┬──────┘    └──────┬──────┘    └───────┬─────────┘  │
│         │                  │                   │            │
│         ▼                  ▼                   ▼            │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ BOM 检测    │ -> │ UTF.Unknown │ -> │ 回退编码        │  │
│  │             │    │ 库检测      │    │ (UTF-8)         │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 编码映射优先级

| 匹配类型 | 默认优先级 | 示例 |
|----------|------------|------|
| ExactPath | 100 | `/path/to/config.json` |
| Wildcard | 0 | `*.ps1` |
| Regex | 0 | `logs[/\\].*\.log$` |
| 内置 PowerShell | -100 | `*.ps1`, `*.psm1` |

::: tip 深入了解
更多编码处理细节请参阅 [编码处理流程](/guide/encoding-workflow)。
:::

## 加密脱敏机制

Apq.Cfg 提供完整的配置加密和脱敏功能，采用接口抽象和依赖注入实现解耦：

- **IValueTransformer**：值转换器接口，用于读取时解密、写入时加密
- **IValueMasker**：值脱敏器接口，用于日志输出时隐藏敏感信息
- **ICryptoProvider**：加密提供者接口，支持多种加密算法

核心库不依赖任何加密扩展包，加密功能通过独立的 `Apq.Cfg.Crypto` 包提供。

::: tip 深入了解
更多加密脱敏设计细节请参阅 [加密脱敏设计](/guide/encryption-masking-design)。
使用指南请参阅 [加密脱敏](/guide/encryption-masking)。
:::

## 配置源实现

### 文件配置源基类

```csharp
public abstract class FileCfgSourceBase : ICfgSource, IWritableCfgSource
{
    protected readonly string FilePath;
    protected readonly EncodingOptions EncodingOptions;
    protected readonly bool ReloadOnChange;
    
    // 编码检测
    protected Encoding DetectEncoding();
    
    // 文件监听
    protected void SetupFileWatcher();
    
    // 抽象方法（子类实现）
    protected abstract void ParseContent(string content);
    protected abstract string SerializeContent();
}
```

### 远程配置源基类

```csharp
public abstract class RemoteCfgSourceBase : ICfgSource, IWritableCfgSource
{
    protected readonly bool EnableHotReload;
    protected readonly TimeSpan ReconnectInterval;
    
    // 连接管理
    protected abstract Task ConnectAsync();
    protected abstract Task DisconnectAsync();
    
    // 热重载
    protected abstract void SetupWatcher();
    protected abstract void OnConfigChanged(Dictionary<string, string?> newData);
}
```

## 依赖注入集成

### 服务注册

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApqCfg(
        this IServiceCollection services,
        Action<CfgBuilder> configure)
    {
        var builder = new CfgBuilder();
        configure(builder);
        var cfgRoot = builder.Build();
        
        services.AddSingleton<ICfgRoot>(cfgRoot);
        services.AddSingleton<IConfiguration>(sp => 
            sp.GetRequiredService<ICfgRoot>().ToMicrosoftConfiguration());
        
        return services;
    }
    
    public static IServiceCollection ConfigureApqCfg<TOptions>(
        this IServiceCollection services,
        string sectionPath) where TOptions : class, new()
    {
        services.AddOptions<TOptions>()
            .Configure<ICfgRoot>((options, cfg) =>
            {
                var section = cfg.GetSection(sectionPath);
                // 绑定配置到选项对象
            });
        
        return services;
    }
}
```

## 源生成器

### 编译时代码生成

```csharp
[CfgSection("config")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public int Port { get; set; }
}

// 生成的代码
public partial class AppConfig
{
    public static AppConfig BindFrom(ICfgSection section)
    {
        return new AppConfig
        {
            Name = section["Name"],
            Port = section.GetValue<int>("Port")
        };
    }
}
```

### 支持的类型

- **简单类型**：string, int, long, bool, double, decimal, DateTime, Guid, 枚举
- **集合类型**：T[], List\<T\>, HashSet\<T\>, Dictionary\<TKey, TValue\>
- **复杂类型**：嵌套的 [CfgSection] 标记类

## 线程安全

### 并发控制策略

1. **读取操作**：无锁，使用不可变数据结构
2. **写入操作**：ConcurrentDictionary 存储待保存数据
3. **保存操作**：每个配置源独立锁
4. **热重载**：防抖 + 原子替换

```csharp
// 批量读取减少锁竞争
public void GetMany(IEnumerable<string> keys, Action<string, string?> onValue)
{
    _coordinator?.EnsureLatest();
    
    foreach (var key in keys)
    {
        var value = GetInternal(key);
        onValue(key, value);
    }
}
```

## 扩展点

### 自定义配置源

实现 `ICfgSource` 或 `IWritableCfgSource` 接口：

```csharp
public class CustomCfgSource : ICfgSource, IWritableCfgSource
{
    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }
    
    public IConfigurationSource BuildSource() => new CustomConfigurationSource(this);
    
    public void SetValue(string key, string? value) { /* ... */ }
    public void Remove(string key) { /* ... */ }
    public Task SaveAsync(CancellationToken ct) { /* ... */ }
}
```

### 自定义编码映射

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        config.AddReadMapping("*.xml", EncodingMappingType.Wildcard, Encoding.UTF8);
        config.AddWriteMapping("**/*.log", EncodingMappingType.Wildcard, Encoding.Unicode);
    })
    .Build();
```

## 下一步

- [加密脱敏](/guide/encryption-masking) - 配置加密与脱敏功能
- [扩展开发](/guide/extension) - 了解如何开发自定义配置源
- [性能优化](/guide/performance) - 性能调优指南
- [最佳实践](/guide/best-practices) - 最佳实践指南
