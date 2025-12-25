using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Internal;

/// <summary>
/// 变更协调器，负责协调多个配置源的变更事件
/// </summary>
internal sealed class ChangeCoordinator : IDisposable
{
    private readonly ConcurrentDictionary<string, string?> _mergedSnapshot;
    private readonly ReadOnlyDictionary<string, string?> _snapshotWrapper; // 只读包装，避免每次复制
    private readonly List<(int Level, IConfigurationProvider Provider)> _providers;
    private readonly Dictionary<int, IConfigurationProvider> _providersByLevel; // 用于 O(1) 查找
    private readonly (int Level, IConfigurationProvider Provider)[] _providersDescending; // 缓存降序排列
    private readonly Dictionary<int, IDisposable> _changeTokenRegistrations; // 改用字典，按 level 存储
    private readonly Timer _debounceTimer;
    private readonly object _lock = new();
    private HashSet<int> _pendingChangeLevels;
    private HashSet<int> _processingChangeLevels; // 双缓冲：用于处理时交换
    private readonly int _debounceMs;
    private static readonly string[] s_emptyStringArray = Array.Empty<string>(); // 缓存空数组

    // 新增：配置选项
    private readonly DynamicReloadOptions _options;
    private readonly Queue<ConfigChangeEvent> _history; // 变更历史
    private readonly object _historyLock = new();
    private bool _hasPendingChanges; // Lazy 模式：标记是否有待处理变更

    // 对象池：复用 GetProviderKeys 中的集合
    [ThreadStatic]
    private static HashSet<string>? t_keysSet;
    [ThreadStatic]
    private static Queue<string>? t_pathsQueue;

    private int _disposed; // 改为 int 以支持 Interlocked

    /// <summary>
    /// 当合并后的配置发生变化时触发
    /// </summary>
    public event Action<IReadOnlyDictionary<string, ConfigChange>>? OnMergedChanges;

    /// <summary>
    /// 当重载发生错误时触发
    /// </summary>
    public event Action<ReloadErrorEvent>? OnReloadError;

    /// <summary>
    /// 当合并后的配置发生变化时触发（异步版本）
    /// </summary>
    public event Func<IReadOnlyDictionary<string, ConfigChange>, Task>? OnMergedChangesAsync;

    public ChangeCoordinator(
        IEnumerable<(int Level, IConfigurationProvider Provider)> providers,
        int debounceMs = 100,
        IReadOnlyDictionary<string, string?>? initialSnapshot = null,
        DynamicReloadOptions? options = null)
    {
        _options = options ?? new DynamicReloadOptions();
        _debounceMs = debounceMs;
        _providers = providers.OrderBy(p => p.Level).ToList();
        _providersByLevel = _providers.ToDictionary(p => p.Level, p => p.Provider);
        _providersDescending = _providers.OrderByDescending(p => p.Level).ToArray();
        _mergedSnapshot = new ConcurrentDictionary<string, string?>();
        _snapshotWrapper = new ReadOnlyDictionary<string, string?>(_mergedSnapshot);
        _changeTokenRegistrations = new Dictionary<int, IDisposable>(_providers.Count);
        _pendingChangeLevels = new HashSet<int>();
        _processingChangeLevels = new HashSet<int>(); // 初始化双缓冲集合
        _debounceTimer = new Timer(OnDebounceElapsed, null, Timeout.Infinite, Timeout.Infinite);
        _history = new Queue<ConfigChangeEvent>();

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
                _changeTokenRegistrations[currentLevel] = registration;
            }
        }
    }

    private void ResubscribeToProvider(int level, IConfigurationProvider provider)
    {
        if (Volatile.Read(ref _disposed) != 0) return;

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
                if (Volatile.Read(ref _disposed) == 0)
                {
                    // 替换旧的 registration，避免内存泄漏
                    _changeTokenRegistrations[level] = registration;
                }
                else
                {
                    registration.Dispose();
                }
            }
        }
    }

    private void OnSourceChanged(int level)
    {
        if (Volatile.Read(ref _disposed) != 0) return;

        lock (_lock)
        {
            _pendingChangeLevels.Add(level);

            // 根据重载策略决定行为
            switch (_options.Strategy)
            {
                case ReloadStrategy.Eager:
                    // 立即重载（防抖后）
                    _debounceTimer.Change(_debounceMs, Timeout.Infinite);
                    break;

                case ReloadStrategy.Lazy:
                    // 标记有待处理变更，访问时再处理
                    _hasPendingChanges = true;
                    break;

                case ReloadStrategy.Manual:
                    // 不自动处理，等待手动调用 Reload()
                    _hasPendingChanges = true;
                    break;
            }
        }
    }

    /// <summary>
    /// 手动触发重载（用于 Manual 和 Lazy 策略）
    /// </summary>
    public void Reload()
    {
        if (Volatile.Read(ref _disposed) != 0) return;

        HashSet<int> levelsToProcess;
        lock (_lock)
        {
            if (_pendingChangeLevels.Count == 0) return;
            levelsToProcess = new HashSet<int>(_pendingChangeLevels);
            _pendingChangeLevels.Clear();
            _hasPendingChanges = false;
        }

        ProcessPendingChangesWithErrorHandling(levelsToProcess);
    }

    /// <summary>
    /// 检查是否有待处理的变更（用于 Lazy 策略）
    /// </summary>
    public bool HasPendingChanges => _hasPendingChanges;

    /// <summary>
    /// 确保配置是最新的（用于 Lazy 策略，访问前调用）
    /// </summary>
    public void EnsureLatest()
    {
        if (_options.Strategy == ReloadStrategy.Lazy && _hasPendingChanges)
        {
            Reload();
        }
    }

    private void OnDebounceElapsed(object? state)
    {
        if (Volatile.Read(ref _disposed) != 0) return;

        HashSet<int> levelsToProcess;
        lock (_lock)
        {
            if (_pendingChangeLevels.Count == 0) return;
            // 双缓冲交换：避免每次分配新 HashSet
            levelsToProcess = _pendingChangeLevels;
            _pendingChangeLevels = _processingChangeLevels;
            _processingChangeLevels = levelsToProcess;
        }

        ProcessPendingChangesWithErrorHandling(levelsToProcess);

        // 处理完成后清空，为下次交换做准备
        lock (_lock)
        {
            levelsToProcess.Clear();
        }
    }

    private void ProcessPendingChangesWithErrorHandling(HashSet<int> changedLevels)
    {
        // 保存旧快照用于回滚
        Dictionary<string, string?>? oldSnapshot = null;
        if (_options.RollbackOnError)
        {
            oldSnapshot = new Dictionary<string, string?>(_mergedSnapshot);
        }

        try
        {
            ProcessPendingChanges(changedLevels);
        }
        catch (Exception ex)
        {
            var rolledBack = false;

            // 回滚到旧快照
            if (_options.RollbackOnError && oldSnapshot != null)
            {
                _mergedSnapshot.Clear();
                foreach (var (key, value) in oldSnapshot)
                {
                    if (value != null)
                        _mergedSnapshot[key] = value;
                }
                rolledBack = true;
            }

            // 触发错误事件
            var errorEvent = new ReloadErrorEvent(changedLevels, ex, rolledBack);
            OnReloadError?.Invoke(errorEvent);
        }
    }

    private void ProcessPendingChanges(IReadOnlySet<int> changedLevels)
    {
        // 重新加载变化的层级，使用字典 O(1) 查找
        foreach (var level in changedLevels)
        {
            if (_providersByLevel.TryGetValue(level, out var provider))
                provider.Load();
        }

        // 计算增量变更
        var changes = ComputeMergedChanges(changedLevels);

        if (changes.Count > 0)
        {
            // 添加到历史记录
            if (_options.HistorySize > 0)
            {
                var historyEvent = new ConfigChangeEvent(changes);
                AddToHistory(historyEvent);
            }

            // 触发同步事件
            OnMergedChanges?.Invoke(changes);

            // 触发异步事件
            var asyncHandler = OnMergedChangesAsync;
            if (asyncHandler != null)
            {
                // 在后台执行异步处理，不阻塞当前线程
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await asyncHandler(changes).ConfigureAwait(false);
                    }
                    catch
                    {
                        // 忽略异步处理器的异常
                    }
                });
            }
        }
    }

    private IReadOnlyDictionary<string, ConfigChange> ComputeMergedChanges(IReadOnlySet<int> changedLevels)
    {
        // 1. 收集所有可能受影响的键（变化层级的所有键 + 快照中的所有键）
        // 预估容量：快照大小 + 一些额外空间
        var affectedKeys = new HashSet<string>(_mergedSnapshot.Count + 16);

        // 添加变化层级的当前键，使用字典 O(1) 查找
        // 直接收集到 affectedKeys，避免中间数组分配
        foreach (var level in changedLevels)
        {
            if (_providersByLevel.TryGetValue(level, out var provider))
            {
                CollectProviderKeys(provider, affectedKeys);
            }
        }

        // 添加快照中的所有键（用于检测删除）
        foreach (var key in _mergedSnapshot.Keys)
            affectedKeys.Add(key);

        // 预估变更数量：通常只有少量键会变化
        var changes = new Dictionary<string, ConfigChange>(Math.Min(affectedKeys.Count, 32));

        // 获取键前缀过滤器
        var filters = _options.KeyPrefixFilters;
        var hasFilters = filters != null && filters.Count > 0;

        // 2. 对每个受影响的键，重新计算最终合并值
        foreach (var key in affectedKeys)
        {
            // 应用键前缀过滤器
            if (hasFilters && !MatchesAnyPrefix(key, filters!))
                continue;

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

                changes[key] = new ConfigChange(key, oldValue, newValue, changeType);

                // 更新快照
                if (newValue == null)
                    _mergedSnapshot.TryRemove(key, out _);
                else
                    _mergedSnapshot[key] = newValue;
            }
        }

        return changes;
    }

    private static bool MatchesAnyPrefix(string key, IReadOnlyList<string> prefixes)
    {
        foreach (var prefix in prefixes)
        {
            if (key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private string? ComputeFinalValue(string key)
    {
        // 使用缓存的降序数组，避免每次排序
        foreach (var (_, provider) in _providersDescending)
        {
            if (provider.TryGet(key, out var value) && value != null)
                return value;
        }
        return null;
    }

    /// <summary>
    /// 收集 Provider 的所有键到目标集合（避免分配新集合）
    /// </summary>
    private static void CollectProviderKeys(IConfigurationProvider provider, HashSet<string> targetKeys)
    {
        // 使用 ThreadStatic 复用 Queue，避免每次分配
        var pendingPaths = t_pathsQueue ??= new Queue<string>(32);
        pendingPaths.Clear();
        pendingPaths.Enqueue(string.Empty);

        while (pendingPaths.Count > 0)
        {
            var parentPath = pendingPaths.Dequeue();

            // GetChildKeys 返回的是相对于 parentPath 的子键名称
            var childKeys = provider.GetChildKeys(s_emptyStringArray, parentPath);

            foreach (var childKey in childKeys)
            {
                // 优化字符串拼接：避免不必要的分配
                var fullKey = string.IsNullOrEmpty(parentPath)
                    ? childKey
                    : string.Concat(parentPath, ":", childKey);

                // 尝试获取值
                if (provider.TryGet(fullKey, out _))
                {
                    targetKeys.Add(fullKey);
                }

                // 将此路径加入待处理队列，以便检查其子键
                pendingPaths.Enqueue(fullKey);
            }
        }
    }

    private static IEnumerable<string> GetProviderKeys(IConfigurationProvider provider)
    {
        // 使用 ThreadStatic 复用 HashSet，避免每次分配
        var keys = t_keysSet ??= new HashSet<string>(64);
        keys.Clear();

        CollectProviderKeys(provider, keys);

        // 返回副本，因为 keys 会被复用
        return keys.ToArray();
    }

    /// <summary>
    /// 获取当前合并快照（只读视图，无复制开销）
    /// </summary>
    public IReadOnlyDictionary<string, string?> GetSnapshot() => _snapshotWrapper;

    /// <summary>
    /// 获取变更历史记录
    /// </summary>
    public IReadOnlyList<ConfigChangeEvent> GetHistory()
    {
        lock (_historyLock)
        {
            return _history.ToArray();
        }
    }

    /// <summary>
    /// 清空变更历史记录
    /// </summary>
    public void ClearHistory()
    {
        lock (_historyLock)
        {
            _history.Clear();
        }
    }

    private void AddToHistory(ConfigChangeEvent changeEvent)
    {
        if (_options.HistorySize <= 0) return;

        lock (_historyLock)
        {
            _history.Enqueue(changeEvent);

            // 保持历史记录在限制范围内
            while (_history.Count > _options.HistorySize)
            {
                _history.Dequeue();
            }
        }
    }

    public void Dispose()
    {
        // 使用 Interlocked 确保原子性，避免竞态条件
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        _debounceTimer.Dispose();

        lock (_lock)
        {
            foreach (var registration in _changeTokenRegistrations.Values)
            {
                try { registration.Dispose(); }
                catch { }
            }
            _changeTokenRegistrations.Clear();
        }
    }
}
