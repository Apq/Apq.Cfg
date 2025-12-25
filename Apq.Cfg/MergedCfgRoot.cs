using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Apq.Cfg.Changes;
using Apq.Cfg.Internal;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 层级数据结构，避免元组解构开销
/// </summary>
internal sealed class LevelData
{
    public readonly List<ICfgSource> Sources;
    public readonly IWritableCfgSource? Primary;
    public readonly ConcurrentDictionary<string, string?> Pending;

    public LevelData(List<ICfgSource> sources, IWritableCfgSource? primary)
    {
        Sources = sources;
        Primary = primary;
        Pending = new ConcurrentDictionary<string, string?>();
    }
}

/// <summary>
/// 合并配置根实现
/// </summary>
internal sealed class MergedCfgRoot : ICfgRoot
{
    private readonly Dictionary<int, LevelData> _levelData;
    private int _disposed; // 改为 int 以支持 Interlocked
    private readonly IConfigurationRoot _merged;
    private readonly Subject<ConfigChangeEvent> _configChangesSubject;
    private readonly Subject<ReloadErrorEvent> _reloadErrorSubject;
    private ChangeCoordinator? _coordinator;
    private IConfigurationRoot? _dynamicConfig;
    private readonly object _dynamicConfigLock = new();
    private DynamicReloadOptions? _dynamicReloadOptions;

    // 缓存排序后的层级列表，避免每次 Get() 都排序
    private readonly int[] _levelsDescending;
    private readonly int[] _levelsAscending;
    private readonly IConfigurationProvider[] _providersArray;

    public MergedCfgRoot(IEnumerable<ICfgSource> sources)
    {
        var sortedSources = sources.OrderBy(s => s.Level).ThenBy(s => s.IsPrimaryWriter ? 1 : 0).ToList();
        var groups = sortedSources.GroupBy(s => s.Level).ToList();
        _levelData = new Dictionary<int, LevelData>(groups.Count);
        _configChangesSubject = new Subject<ConfigChangeEvent>();
        _reloadErrorSubject = new Subject<ReloadErrorEvent>();

        foreach (var group in groups)
        {
            var list = group.ToList();
            var primary = list.LastOrDefault(s => s.IsPrimaryWriter && s is IWritableCfgSource) as IWritableCfgSource
                          ?? list.LastOrDefault(s => s.IsWriteable && s is IWritableCfgSource) as IWritableCfgSource;
            _levelData[group.Key] = new LevelData(list, primary);
        }

        _merged = BuildMergedConfiguration();

        // 预先计算并缓存排序后的层级列表
        _levelsDescending = _levelData.Keys.OrderByDescending(k => k).ToArray();
        _levelsAscending = _levelData.Keys.OrderBy(k => k).ToArray();
        _providersArray = _merged.Providers.ToArray();
    }

    public IObservable<ConfigChangeEvent> ConfigChanges => _configChangesSubject.AsObservable();

    /// <summary>
    /// 重载错误事件（Rx 可观察序列）
    /// </summary>
    public IObservable<ReloadErrorEvent> ReloadErrors => _reloadErrorSubject.AsObservable();

    public string? Get(string key)
    {
        // Lazy 策略：访问前确保配置是最新的
        _coordinator?.EnsureLatest();

        // 使用缓存的降序层级数组，避免每次排序
        foreach (var level in _levelsDescending)
        {
            if (_levelData[level].Pending.TryGetValue(key, out var pendingValue))
                return pendingValue;
        }
        return _merged[key];
    }

    public T? Get<T>(string key)
    {
        // Lazy 策略：访问前确保配置是最新的
        _coordinator?.EnsureLatest();

        // 先检查 Pending 中是否有待保存的值，与 Get(string) 行为一致
        foreach (var level in _levelsDescending)
        {
            if (_levelData[level].Pending.TryGetValue(key, out var pendingValue))
            {
                if (pendingValue == null) return default;
                // Pending 中有值但还未保存，需要手动转换
                return ValueConverter.Convert<T>(pendingValue);
            }
        }
        return _merged.GetValue<T>(key);
    }

    public bool Exists(string key)
    {
        // Lazy 策略：访问前确保配置是最新的
        _coordinator?.EnsureLatest();

        // 使用缓存的层级数组
        foreach (var level in _levelsDescending)
        {
            if (_levelData[level].Pending.TryGetValue(key, out var pendingValue))
                return pendingValue != null;
        }
        return _merged[key] != null;
    }

    public ICfgSection GetSection(string key)
    {
        return new CfgSection(this, _merged, key);
    }

    public IEnumerable<string> GetChildKeys()
    {
        return _merged.GetChildren().Select(c => c.Key);
    }

    public void Remove(string key, int? targetLevel = null)
    {
        var level = targetLevel ?? (_levelsDescending.Length > 0 ? _levelsDescending[0] : throw new InvalidOperationException("没有配置源"));
        if (!_levelData.TryGetValue(level, out var data) || data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");
        data.Pending[key] = null;
    }

    public void Set(string key, string? value, int? targetLevel = null)
    {
        var level = targetLevel ?? (_levelsDescending.Length > 0 ? _levelsDescending[0] : throw new InvalidOperationException("没有配置源"));
        if (!_levelData.TryGetValue(level, out var data) || data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");
        data.Pending[key] = value;
    }

    public async Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default)
    {
        var level = targetLevel ?? (_levelsDescending.Length > 0 ? _levelsDescending[0] : throw new InvalidOperationException("没有配置源"));
        if (!_levelData.TryGetValue(level, out var data) || data.Pending.IsEmpty)
            return;

        if (data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");

        // 原子地获取并移除所有待保存的更改
        // 使用快照遍历避免 ToArray() 分配，预估容量减少扩容
        var changes = new Dictionary<string, string?>(data.Pending.Count);
        foreach (var kvp in data.Pending)
        {
            if (data.Pending.TryRemove(kvp.Key, out var value))
                changes[kvp.Key] = value;
        }

        if (changes.Count == 0)
            return;

        await data.Primary.ApplyChangesAsync(changes, cancellationToken).ConfigureAwait(false);

        // 保存后更新内存中的配置
        foreach (var (key, value) in changes)
            _merged[key] = value;
    }

    /// <summary>
    /// 手动触发配置重载（用于 Manual 和 Lazy 策略）
    /// </summary>
    public void Reload()
    {
        _coordinator?.Reload();
    }

    /// <summary>
    /// 检查是否有待处理的配置变更
    /// </summary>
    public bool HasPendingChanges => _coordinator?.HasPendingChanges ?? false;

    /// <summary>
    /// 获取配置变更历史记录
    /// </summary>
    public IReadOnlyList<ConfigChangeEvent> GetChangeHistory()
    {
        return _coordinator?.GetHistory() ?? Array.Empty<ConfigChangeEvent>();
    }

    /// <summary>
    /// 清空配置变更历史记录
    /// </summary>
    public void ClearChangeHistory()
    {
        _coordinator?.ClearHistory();
    }

    public IConfigurationRoot ToMicrosoftConfiguration() => _merged;

    public IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options)
    {
        options ??= new DynamicReloadOptions();

        if (!options.EnableDynamicReload)
            return _merged;

        lock (_dynamicConfigLock)
        {
            if (_dynamicConfig != null)
                return _dynamicConfig;

            _dynamicReloadOptions = options;

            // 使用缓存的 providers 数组和层级数组，避免 LINQ 开销
            var providers = new List<(int Level, IConfigurationProvider Provider)>();
            var providerIndex = 0;
            foreach (var level in _levelsAscending)
            {
                var sourceCount = _levelData[level].Sources.Count;
                for (var i = 0; i < sourceCount; i++)
                {
                    // 使用缓存的数组进行索引访问
                    if (providerIndex < _providersArray.Length)
                    {
                        providers.Add((level, _providersArray[providerIndex]));
                        providerIndex++;
                    }
                }
            }

            // 从 _merged 配置中获取所有键值对作为初始快照
            var initialSnapshot = new Dictionary<string, string?>();
            CollectAllKeys(_merged, string.Empty, initialSnapshot);

            // 创建协调器，传递完整选项
            _coordinator = new ChangeCoordinator(providers, options.DebounceMs, initialSnapshot, options);
            _coordinator.OnMergedChanges += changes =>
            {
                _configChangesSubject.OnNext(new ConfigChangeEvent(changes));
            };
            _coordinator.OnReloadError += errorEvent =>
            {
                _reloadErrorSubject.OnNext(errorEvent);
            };

            // 创建动态配置
            var builder = new ConfigurationBuilder();
            builder.Add(new MergedConfigurationSource(_coordinator));
            _dynamicConfig = builder.Build();

            return _dynamicConfig;
        }
    }

    private static void CollectAllKeys(IConfiguration config, string parentPath, Dictionary<string, string?> result)
    {
        foreach (var child in config.GetChildren())
        {
            // 优化字符串拼接：使用 string.Concat 避免插值分配
            var fullKey = string.IsNullOrEmpty(parentPath) ? child.Key : string.Concat(parentPath, ":", child.Key);

            // 如果有值，添加到结果
            if (child.Value != null)
            {
                result[fullKey] = child.Value;
            }

            // 递归处理子节点
            CollectAllKeys(child, fullKey, result);
        }
    }

    public void Dispose()
    {
        // 使用 Interlocked 确保原子性，避免竞态条件
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        _coordinator?.Dispose();
        _configChangesSubject.OnCompleted();
        _configChangesSubject.Dispose();
        _reloadErrorSubject.OnCompleted();
        _reloadErrorSubject.Dispose();

        foreach (var levelData in _levelData.Values)
        foreach (var source in levelData.Sources)
        {
            try
            {
                if (source is IDisposable disposable)
                    disposable.Dispose();
            }
            catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        // 使用 Interlocked 确保原子性，避免竞态条件
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        _coordinator?.Dispose();
        _configChangesSubject.OnCompleted();
        _configChangesSubject.Dispose();
        _reloadErrorSubject.OnCompleted();
        _reloadErrorSubject.Dispose();

        foreach (var levelData in _levelData.Values)
        foreach (var source in levelData.Sources)
        {
            try
            {
                if (source is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else if (source is IDisposable disposable)
                    disposable.Dispose();
            }
            catch { }
        }
    }

    private IConfigurationRoot BuildMergedConfiguration(int[]? levelsAscending = null)
    {
        var cb = new ConfigurationBuilder();
        // 使用传入的排序数组或直接排序（初始化时缓存尚未创建）
        IEnumerable<int> levels = levelsAscending != null
            ? levelsAscending
            : _levelData.Keys.OrderBy(k => k);
        foreach (var level in levels)
            foreach (var src in _levelData[level].Sources)
                cb.Add(src.BuildSource());
        return cb.Build();
    }
}
