# 动态配置重载设计方案

本文档详细说明 Apq.Cfg 动态配置重载功能的设计与实现。

## 1. 背景与目标

### 1.1 当前状态

`ICfgRoot.ToMicrosoftConfiguration()` 返回的 `IConfigurationRoot` 是一个静态快照。当底层配置源发生变化时，已返回的 `IConfigurationRoot` 实例不会自动更新。

### 1.2 设计目标

1. **动态更新**：当任意配置源变化时，`ToMicrosoftConfiguration()` 返回的配置能自动反映最新值
2. **性能优化**：避免任意源变化都触发整体重载，采用增量更新策略
3. **层级覆盖感知**：只有当最终合并值真正发生变化时才触发通知
4. **多源支持**：支持多个配置源同时存在的场景

## 2. 核心设计

### 2.1 架构概览

```
┌─────────────────────────────────────────────────────────────────┐
│                        MergedCfgRoot                            │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │ Source L1   │  │ Source L2   │  │ Source L3   │  ...        │
│  │ (低优先级)   │  │ (中优先级)   │  │ (高优先级)   │             │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘             │
│         │                │                │                     │
│         ▼                ▼                ▼                     │
│  ┌─────────────────────────────────────────────────────────────┤
│  │              ChangeCoordinator (变更协调器)                  │
│  │  - 收集各源的变更事件                                        │
│  │  - 防抖处理 (Debounce)                                       │
│  │  - 增量合并计算                                              │
│  │  - 最终值变化检测                                            │
│  └──────────────────────────┬──────────────────────────────────┤
│                             │                                   │
│                             ▼                                   │
│  ┌─────────────────────────────────────────────────────────────┤
│  │              MergedConfigurationProvider                     │
│  │  - 缓存最终合并值                                            │
│  │  - 提供 IChangeToken                                         │
│  │  - 仅在最终值变化时触发 Reload                               │
│  └─────────────────────────────────────────────────────────────┤
└─────────────────────────────────────────────────────────────────┘
```

### 2.2 核心组件

#### 2.2.1 ChangeCoordinator (变更协调器)

负责协调多个配置源的变更事件，实现智能合并和防抖。

```csharp
internal sealed class ChangeCoordinator : IDisposable
{
    private readonly ConcurrentDictionary<string, string?> _mergedSnapshot;
    private readonly List<(int Level, IConfigurationProvider Provider)> _providers;
    private readonly List<IDisposable> _changeTokenRegistrations;
    private readonly Timer _debounceTimer;
    private readonly object _lock = new();
    private readonly HashSet<int> _pendingChangeLevels;
    private readonly int _debounceMs;
    private volatile bool _disposed;

    public event Action<IReadOnlyDictionary<string, ConfigChange>>? OnMergedChanges;

    public ChangeCoordinator(
        IEnumerable<(int Level, IConfigurationProvider Provider)> providers,
        int debounceMs = 100,
        IReadOnlyDictionary<string, string?>? initialSnapshot = null)
    {
        // 支持传入初始快照以优化初始化性能
    }
}
```

#### 2.2.2 ConfigChange (配置变更记录)

```csharp
public readonly struct ConfigChange
{
    public string Key { get; init; }
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
    public ChangeType Type { get; init; }

    public override string ToString() =>
        $"[{Type}] {Key}: {OldValue ?? "(null)"} -> {NewValue ?? "(null)"}";
}

public enum ChangeType
{
    Added,
    Modified,
    Removed
}
```

#### 2.2.3 MergedConfigurationProvider

自定义的 `IConfigurationProvider` 实现，支持增量更新。

```csharp
internal sealed class MergedConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly ChangeCoordinator _coordinator;
    private CancellationTokenSource _reloadTokenSource;
    private volatile bool _disposed;

    public event Action<ConfigChangeEvent>? OnConfigChanged;

    public override void Load()
    {
        // 从协调器获取快照数据
        Data.Clear();
        foreach (var (key, value) in _coordinator.GetSnapshot())
        {
            if (value != null)
                Data[key] = value;
        }
    }

    // 注意：使用 new 而非 override，因为基类方法非虚
    public new IChangeToken GetReloadToken()
    {
        return new CancellationChangeToken(_reloadTokenSource.Token);
    }

    private void OnCoordinatorChanges(IReadOnlyDictionary<string, ConfigChange> changes)
    {
        // 应用变更到 Data
        foreach (var (key, change) in changes)
        {
            if (change.Type == ChangeType.Removed)
                Data.Remove(key);
            else
                Data[key] = change.NewValue!;
        }

        // 触发 reload token
        TriggerReload();

        // 触发事件
        OnConfigChanged?.Invoke(new ConfigChangeEvent
        {
            Changes = changes,
            Timestamp = DateTimeOffset.Now
        });
    }

    private void TriggerReload()
    {
        var oldSource = Interlocked.Exchange(
            ref _reloadTokenSource,
            new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
    }
}
```

## 3. 变更检测流程

### 3.1 流程图

```
源变化事件
    │
    ▼
┌───────────────────┐
│ 记录变化的层级     │
│ _pendingChangeLevels.Add(level)
└─────────┬─────────┘
          │
          ▼
┌───────────────────┐
│ 重置防抖计时器     │
│ _debounceTimer.Change(_debounceMs)
└─────────┬─────────┘
          │
          │ (等待防抖窗口)
          ▼
┌───────────────────┐
│ 防抖窗口结束       │
│ ProcessPendingChanges()
└─────────┬─────────┘
          │
          ▼
┌───────────────────────────────────┐
│ 仅重新加载变化的层级              │
│ foreach (level in pendingLevels)  │
│   providers[level].Load()         │
└─────────┬─────────────────────────┘
          │
          ▼
┌───────────────────────────────────┐
│ 增量合并计算                      │
│ ComputeMergedChanges()            │
└─────────┬─────────────────────────┘
          │
          ▼
┌───────────────────────────────────┐
│ 比较最终合并值                    │
│ 只保留真正变化的键                │
└─────────┬─────────────────────────┘
          │
          ▼
┌───────────────────────────────────┐
│ 有变化？                          │
│ actualChanges.Count > 0           │
└─────────┬─────────────────────────┘
          │
    ┌─────┴─────┐
    │ Yes       │ No
    ▼           ▼
触发通知      静默忽略
```

### 3.2 增量合并算法

```csharp
private IReadOnlyDictionary<string, ConfigChange> ComputeMergedChanges(
    IReadOnlySet<int> changedLevels)
{
    var changes = new Dictionary<string, ConfigChange>();

    // 1. 收集所有可能受影响的键（变化层级的所有键 + 快照中的所有键）
    var affectedKeys = new HashSet<string>();

    // 添加变化层级的当前键
    foreach (var level in changedLevels)
    {
        var provider = _providers.FirstOrDefault(p => p.Level == level).Provider;
        if (provider != null)
        {
            foreach (var key in GetProviderKeys(provider))
                affectedKeys.Add(key);
        }
    }

    // 添加快照中的所有键（用于检测删除）
    foreach (var key in _mergedSnapshot.Keys)
        affectedKeys.Add(key);

    // 2. 对每个受影响的键，重新计算最终合并值
    foreach (var key in affectedKeys)
    {
        var oldValue = _mergedSnapshot.GetValueOrDefault(key);
        var newValue = ComputeFinalValue(key);

        // 3. 只有最终值真正变化时才记录
        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            var changeType = (oldValue, newValue) switch
            {
                (null, not null) => ChangeType.Added,
                (not null, null) => ChangeType.Removed,
                _ => ChangeType.Modified
            };

            changes[key] = new ConfigChange
            {
                Key = key,
                OldValue = oldValue,
                NewValue = newValue,
                Type = changeType
            };

            // 更新快照
            if (newValue == null)
                _mergedSnapshot.TryRemove(key, out _);
            else
                _mergedSnapshot[key] = newValue;
        }
    }

    return changes;
}

private string? ComputeFinalValue(string key)
{
    // 从高优先级到低优先级遍历，返回第一个非空值
    foreach (var (_, provider) in _providers.OrderByDescending(p => p.Level))
    {
        if (provider.TryGet(key, out var value) && value != null)
            return value;
    }
    return null;
}
```

## 4. 层级覆盖逻辑

### 4.1 问题场景

假设有三层配置：
- Level 1 (低): `{ "Timeout": "30" }`
- Level 2 (中): `{ "Timeout": "60" }`
- Level 3 (高): `{ "Timeout": "90" }`

最终合并值：`Timeout = 90`

**场景1**: Level 1 的 Timeout 从 30 改为 45
- Level 3 仍然覆盖，最终值仍为 90
- **不应触发变更通知**

**场景2**: Level 3 的 Timeout 从 90 改为 120
- 最终值从 90 变为 120
- **应触发变更通知**

**场景3**: Level 3 删除 Timeout 配置
- 最终值从 90 变为 60 (Level 2 的值)
- **应触发变更通知**

### 4.2 实现保证

通过 `ComputeFinalValue()` 方法，始终从高优先级向低优先级查找，确保：
1. 只有最终合并值变化时才触发通知
2. 低层级的变化如果被高层级覆盖，不会产生通知

## 5. 性能优化策略

### 5.1 防抖 (Debouncing)

```csharp
private void OnSourceChanged(int level)
{
    if (_disposed) return;

    lock (_lock)
    {
        _pendingChangeLevels.Add(level);
        _debounceTimer.Change(_debounceMs, Timeout.Infinite);
    }
}
```

**优点**：
- 批量文件保存时，多次快速变化合并为一次处理
- 减少不必要的重复计算

### 5.2 增量加载

只重新加载发生变化的配置源，而非全部重载：

```csharp
private void ProcessPendingChanges(IReadOnlySet<int> changedLevels)
{
    // 重新加载变化的层级
    foreach (var level in changedLevels)
    {
        var provider = _providers.FirstOrDefault(p => p.Level == level).Provider;
        provider?.Load();
    }

    // 计算增量变更
    var changes = ComputeMergedChanges(changedLevels);

    if (changes.Count > 0)
        OnMergedChanges?.Invoke(changes);
}
```

### 5.3 快照缓存

维护最终合并值的快照，避免每次查询都遍历所有层级：

```csharp
private readonly ConcurrentDictionary<string, string?> _mergedSnapshot;
```

### 5.4 初始快照优化

支持传入初始快照，避免重复计算已有配置的合并值：

```csharp
public ChangeCoordinator(
    IEnumerable<(int Level, IConfigurationProvider Provider)> providers,
    int debounceMs = 100,
    IReadOnlyDictionary<string, string?>? initialSnapshot = null)
{
    // 初始化快照
    if (initialSnapshot != null)
    {
        foreach (var (key, value) in initialSnapshot)
        {
            if (value != null)
                _mergedSnapshot[key] = value;
        }
    }
    else
    {
        InitializeSnapshot();
    }
}
```

## 6. API 设计

### 6.1 新增接口

```csharp
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // ... 现有方法 ...

    /// <summary>
    /// 转换为 Microsoft Configuration（静态快照）
    /// </summary>
    IConfigurationRoot ToMicrosoftConfiguration();

    /// <summary>
    /// 转换为支持动态重载的 Microsoft Configuration
    /// </summary>
    /// <param name="options">动态重载选项，为 null 时使用默认选项</param>
    IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options);

    /// <summary>
    /// 获取配置变更的可观察序列
    /// </summary>
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}

public sealed class DynamicReloadOptions
{
    /// <summary>
    /// 防抖时间窗口（毫秒），默认 100ms
    /// </summary>
    public int DebounceMs { get; set; } = 100;

    /// <summary>
    /// 是否启用动态重载，默认 true
    /// </summary>
    public bool EnableDynamicReload { get; set; } = true;
}

public sealed class ConfigChangeEvent
{
    public IReadOnlyDictionary<string, ConfigChange> Changes { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}
```

### 6.2 使用示例

```csharp
// 构建配置
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 1, writeable: false, reloadOnChange: true)
    .AddJson("config.local.json", level: 2, writeable: true, reloadOnChange: true)
    .AddEnvironmentVariables(level: 3, prefix: "APP_")
    .Build();

// 获取支持动态重载的 Microsoft Configuration
var msConfig = cfg.ToMicrosoftConfiguration(new DynamicReloadOptions
{
    DebounceMs = 200
});

// 方式1：使用 IChangeToken
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));

// 方式2：使用 Rx 订阅
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

## 7. 实现文件

### 7.1 新增文件

| 文件路径 | 说明 |
|----------|------|
| `Apq.Cfg/Changes/ChangeType.cs` | 配置变更类型枚举 |
| `Apq.Cfg/Changes/ConfigChange.cs` | 配置变更记录结构 |
| `Apq.Cfg/Changes/ConfigChangeEvent.cs` | 配置变更事件 |
| `Apq.Cfg/Changes/DynamicReloadOptions.cs` | 动态重载选项 |
| `Apq.Cfg/Internal/ChangeCoordinator.cs` | 变更协调器 |
| `Apq.Cfg/Internal/MergedConfigurationProvider.cs` | 合并配置提供者 |
| `Apq.Cfg/Internal/MergedConfigurationSource.cs` | 合并配置源 |
| `tests/.../DynamicReloadTests.cs` | 单元测试（9个测试用例） |

### 7.2 修改文件

| 文件路径 | 修改内容 |
|----------|----------|
| `Apq.Cfg/ICfgRoot.cs` | 添加 `ToMicrosoftConfiguration(DynamicReloadOptions?)` 和 `ConfigChanges` |
| `Apq.Cfg/MergedCfgRoot.cs` | 实现动态重载功能 |
| `Apq.Cfg/Apq.Cfg.csproj` | 添加 `System.Reactive` 依赖 |

## 8. 注意事项

### 8.1 线程安全

- `ChangeCoordinator` 内部使用锁保护共享状态
- `ConcurrentDictionary` 用于快照存储
- 事件触发在独立线程，避免阻塞源变更检测
- 使用 `volatile` 标记 `_disposed` 字段

### 8.2 内存管理

- 正确实现 `IDisposable`，释放所有 `IChangeToken` 注册
- 取消订阅所有事件处理器
- 停止防抖计时器
- `MergedCfgRoot` 释放时调用 `_configChangesSubject.OnCompleted()`

### 8.3 异常处理

- 单个源加载失败不应影响其他源
- 变更处理异常应记录日志但不中断流程：
  ```csharp
  try
  {
      ProcessPendingChanges(levelsToProcess);
  }
  catch
  {
      // 忽略处理异常，避免中断后续变更检测
  }
  ```

### 8.4 技术限制

- `MergedConfigurationProvider.GetReloadToken()` 使用 `new` 关键字而非 `override`，因为 `ConfigurationProvider` 基类的该方法不是虚方法

## 9. 总结

本方案通过以下机制解决了动态配置重载的核心问题：

| 问题 | 解决方案 |
|------|----------|
| 性能下降 | 防抖 + 增量加载 + 快照缓存 + 初始快照优化 |
| 层级覆盖 | 最终值比较，只通知真正变化 |
| 多源支持 | ChangeCoordinator 统一协调 |
| API 兼容 | 保持现有接口，新增可选参数 |

该设计在保持 API 简洁的同时，提供了高效、准确的动态配置重载能力。

## 下一步

- [动态重载](/guide/dynamic-reload) - 动态重载基础用法
- [架构设计](/guide/architecture) - 整体架构设计
- [性能优化](/guide/performance) - 性能调优指南
