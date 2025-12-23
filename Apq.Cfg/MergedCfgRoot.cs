using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Apq.Cfg.Changes;
using Apq.Cfg.Internal;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 合并配置根实现
/// </summary>
internal sealed class MergedCfgRoot : ICfgRoot
{
    private readonly ConcurrentDictionary<int, (List<ICfgSource> Sources, IWritableCfgSource? Primary, ConcurrentDictionary<string, string?> Pending)> _levelData;
    private volatile bool _disposed;
    private readonly IConfigurationRoot _merged;
    private readonly Subject<ConfigChangeEvent> _configChangesSubject;
    private ChangeCoordinator? _coordinator;
    private IConfigurationRoot? _dynamicConfig;
    private readonly object _dynamicConfigLock = new();

    public MergedCfgRoot(IEnumerable<ICfgSource> sources)
    {
        var sortedSources = sources.OrderBy(s => s.Level).ThenBy(s => s.IsPrimaryWriter ? 1 : 0).ToList();
        _levelData = new ConcurrentDictionary<int, (List<ICfgSource>, IWritableCfgSource?, ConcurrentDictionary<string, string?>)>();
        _configChangesSubject = new Subject<ConfigChangeEvent>();

        foreach (var group in sortedSources.GroupBy(s => s.Level))
        {
            var list = group.ToList();
            var primary = list.LastOrDefault(s => s.IsPrimaryWriter && s is IWritableCfgSource) as IWritableCfgSource
                          ?? list.LastOrDefault(s => s.IsWriteable && s is IWritableCfgSource) as IWritableCfgSource;
            _levelData[group.Key] = (list, primary, new ConcurrentDictionary<string, string?>());
        }

        _merged = BuildMergedConfiguration();
    }

    public IObservable<ConfigChangeEvent> ConfigChanges => _configChangesSubject.AsObservable();

    public string? Get(string key)
    {
        // 先检查所有层级的 Pending（从高到低）
        foreach (var level in _levelData.Keys.OrderByDescending(k => k))
        {
            if (_levelData[level].Pending.TryGetValue(key, out var pendingValue))
                return pendingValue;
        }
        return _merged[key];
    }

    public T? Get<T>(string key) => _merged.GetValue<T>(key);

    public bool Exists(string key)
    {
        // 先检查所有层级的 Pending
        foreach (var level in _levelData.Keys)
        {
            if (_levelData[level].Pending.TryGetValue(key, out var pendingValue))
                return pendingValue != null;
        }
        return _merged[key] != null;
    }

    public void Remove(string key, int? targetLevel = null)
    {
        var level = targetLevel ?? _levelData.Keys.DefaultIfEmpty().Max();
        if (!_levelData.TryGetValue(level, out var data) || data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");
        data.Pending[key] = null;
    }

    public void Set(string key, string? value, int? targetLevel = null)
    {
        var level = targetLevel ?? _levelData.Keys.DefaultIfEmpty().Max();
        if (!_levelData.TryGetValue(level, out var data) || data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");
        data.Pending[key] = value;
    }

    public async Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default)
    {
        var level = targetLevel ?? _levelData.Keys.DefaultIfEmpty().Max();
        if (!_levelData.TryGetValue(level, out var data) || data.Pending.IsEmpty)
            return;

        if (data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");

        // 原子地获取并移除所有待保存的更改
        var changes = new Dictionary<string, string?>();
        foreach (var key in data.Pending.Keys.ToArray())
        {
            if (data.Pending.TryRemove(key, out var value))
                changes[key] = value;
        }

        if (changes.Count == 0)
            return;

        await data.Primary.ApplyChangesAsync(changes, cancellationToken).ConfigureAwait(false);

        // 保存后更新内存中的配置
        foreach (var (key, value) in changes)
            _merged[key] = value;
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

            // 直接使用已有的 _merged 配置的 providers
            var providers = new List<(int Level, IConfigurationProvider Provider)>();
            var providerIndex = 0;
            foreach (var level in _levelData.Keys.OrderBy(k => k))
            {
                var sourceCount = _levelData[level].Sources.Count;
                for (var i = 0; i < sourceCount; i++)
                {
                    // 从 _merged 中获取对应的 provider
                    if (providerIndex < _merged.Providers.Count())
                    {
                        var provider = _merged.Providers.ElementAt(providerIndex);
                        providers.Add((level, provider));
                        providerIndex++;
                    }
                }
            }

            // 从 _merged 配置中获取所有键值对作为初始快照
            var initialSnapshot = new Dictionary<string, string?>();
            CollectAllKeys(_merged, string.Empty, initialSnapshot);

            // 创建协调器
            _coordinator = new ChangeCoordinator(providers, options.DebounceMs, initialSnapshot);
            _coordinator.OnMergedChanges += changes =>
            {
                _configChangesSubject.OnNext(new ConfigChangeEvent
                {
                    Changes = changes,
                    Timestamp = DateTimeOffset.Now
                });
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
            var fullKey = string.IsNullOrEmpty(parentPath) ? child.Key : $"{parentPath}:{child.Key}";

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
        if (_disposed) return;
        _disposed = true;

        _coordinator?.Dispose();
        _configChangesSubject.OnCompleted();
        _configChangesSubject.Dispose();

        foreach (var (_, (sources, _, _)) in _levelData)
        foreach (var source in sources)
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
        if (_disposed) return;
        _disposed = true;

        _coordinator?.Dispose();
        _configChangesSubject.OnCompleted();
        _configChangesSubject.Dispose();

        foreach (var (_, (sources, _, _)) in _levelData)
        foreach (var source in sources)
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

    private IConfigurationRoot BuildMergedConfiguration()
    {
        var cb = new ConfigurationBuilder();
        foreach (var level in _levelData.Keys.OrderBy(k => k))
            foreach (var src in _levelData[level].Sources)
                cb.Add(src.BuildSource());
        return cb.Build();
    }
}
