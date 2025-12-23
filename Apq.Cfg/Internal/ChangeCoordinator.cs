using System.Collections.Concurrent;
using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Internal;

/// <summary>
/// 变更协调器，负责协调多个配置源的变更事件
/// </summary>
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

    /// <summary>
    /// 当合并后的配置发生变化时触发
    /// </summary>
    public event Action<IReadOnlyDictionary<string, ConfigChange>>? OnMergedChanges;

    public ChangeCoordinator(
        IEnumerable<(int Level, IConfigurationProvider Provider)> providers,
        int debounceMs = 100,
        IReadOnlyDictionary<string, string?>? initialSnapshot = null)
    {
        _debounceMs = debounceMs;
        _providers = providers.OrderBy(p => p.Level).ToList();
        _mergedSnapshot = new ConcurrentDictionary<string, string?>();
        _changeTokenRegistrations = new List<IDisposable>();
        _pendingChangeLevels = new HashSet<int>();
        _debounceTimer = new Timer(OnDebounceElapsed, null, Timeout.Infinite, Timeout.Infinite);

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

        // 订阅各源的变更事件
        SubscribeToChanges();
    }

    private void InitializeSnapshot()
    {
        // 收集所有键
        var allKeys = new HashSet<string>();
        foreach (var (_, provider) in _providers)
        {
            foreach (var key in GetProviderKeys(provider))
                allKeys.Add(key);
        }

        // 计算每个键的最终合并值
        foreach (var key in allKeys)
        {
            var value = ComputeFinalValue(key);
            if (value != null)
                _mergedSnapshot[key] = value;
        }
    }

    private void SubscribeToChanges()
    {
        foreach (var (level, provider) in _providers)
        {
            var currentLevel = level; // 捕获变量
            var token = provider.GetReloadToken();
            if (token != null)
            {
                var registration = token.RegisterChangeCallback(_ =>
                {
                    OnSourceChanged(currentLevel);
                    // 重新订阅（因为 token 是一次性的）
                    ResubscribeToProvider(currentLevel, provider);
                }, null);
                _changeTokenRegistrations.Add(registration);
            }
        }
    }

    private void ResubscribeToProvider(int level, IConfigurationProvider provider)
    {
        if (_disposed) return;

        var token = provider.GetReloadToken();
        if (token != null)
        {
            var registration = token.RegisterChangeCallback(_ =>
            {
                OnSourceChanged(level);
                ResubscribeToProvider(level, provider);
            }, null);

            lock (_lock)
            {
                if (!_disposed)
                    _changeTokenRegistrations.Add(registration);
            }
        }
    }

    private void OnSourceChanged(int level)
    {
        if (_disposed) return;

        lock (_lock)
        {
            _pendingChangeLevels.Add(level);
            _debounceTimer.Change(_debounceMs, Timeout.Infinite);
        }
    }

    private void OnDebounceElapsed(object? state)
    {
        if (_disposed) return;

        HashSet<int> levelsToProcess;
        lock (_lock)
        {
            if (_pendingChangeLevels.Count == 0) return;
            levelsToProcess = new HashSet<int>(_pendingChangeLevels);
            _pendingChangeLevels.Clear();
        }

        try
        {
            ProcessPendingChanges(levelsToProcess);
        }
        catch
        {
            // 忽略处理异常，避免中断后续变更检测
        }
    }

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

    private IReadOnlyDictionary<string, ConfigChange> ComputeMergedChanges(IReadOnlySet<int> changedLevels)
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

    private static IEnumerable<string> GetProviderKeys(IConfigurationProvider provider)
    {
        var keys = new HashSet<string>();
        var pendingPaths = new Queue<string>();
        pendingPaths.Enqueue(string.Empty);

        while (pendingPaths.Count > 0)
        {
            var parentPath = pendingPaths.Dequeue();

            // GetChildKeys 返回的是相对于 parentPath 的子键名称
            var childKeys = provider.GetChildKeys(Enumerable.Empty<string>(), parentPath);

            foreach (var childKey in childKeys)
            {
                var fullKey = string.IsNullOrEmpty(parentPath)
                    ? childKey
                    : $"{parentPath}:{childKey}";

                // 尝试获取值
                if (provider.TryGet(fullKey, out _))
                {
                    keys.Add(fullKey);
                }

                // 将此路径加入待处理队列，以便检查其子键
                pendingPaths.Enqueue(fullKey);
            }
        }

        return keys;
    }

    /// <summary>
    /// 获取当前合并快照
    /// </summary>
    public IReadOnlyDictionary<string, string?> GetSnapshot() =>
        new Dictionary<string, string?>(_mergedSnapshot);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _debounceTimer.Dispose();

        lock (_lock)
        {
            foreach (var registration in _changeTokenRegistrations)
            {
                try { registration.Dispose(); }
                catch { }
            }
            _changeTokenRegistrations.Clear();
        }
    }
}
