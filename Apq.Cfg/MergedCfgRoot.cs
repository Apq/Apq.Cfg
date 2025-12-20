using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 合并配置根实现
/// </summary>
internal sealed class MergedCfgRoot : ICfgRoot
{
    private readonly Dictionary<int, (List<ICfgSource> Sources, IWritableCfgSource? Primary, Dictionary<string, string?> Pending)> _levelData;
    private volatile bool _disposed;
    private readonly IConfigurationRoot _merged;

    public MergedCfgRoot(IEnumerable<ICfgSource> sources)
    {
        var sortedSources = sources.OrderBy(s => s.Level).ThenBy(s => s.IsPrimaryWriter ? 1 : 0).ToList();
        _levelData = new Dictionary<int, (List<ICfgSource>, IWritableCfgSource?, Dictionary<string, string?>)>();

        foreach (var group in sortedSources.GroupBy(s => s.Level))
        {
            var list = group.ToList();
            var primary = list.LastOrDefault(s => s.IsPrimaryWriter && s is IWritableCfgSource) as IWritableCfgSource
                          ?? list.LastOrDefault(s => s.IsWriteable && s is IWritableCfgSource) as IWritableCfgSource;
            _levelData[group.Key] = (list, primary, new Dictionary<string, string?>());
        }

        _merged = BuildMergedConfiguration();
    }

    public string? Get(string key) => _merged[key];

    public T? Get<T>(string key) => _merged.GetValue<T>(key);

    public bool Exists(string key) => _merged[key] != null;

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
        if (!_levelData.TryGetValue(level, out var data) || data.Pending.Count == 0)
            return;

        if (data.Primary == null)
            throw new InvalidOperationException($"层级 {level} 没有可写的配置源");

        var changes = new Dictionary<string, string?>(data.Pending);
        data.Pending.Clear();
        await data.Primary.ApplyChangesAsync(changes, cancellationToken).ConfigureAwait(false);
    }

    public IConfigurationRoot ToMicrosoftConfiguration() => _merged;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

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
