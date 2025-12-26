using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Apq.Cfg.Sources;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Consul;

/// <summary>
/// Consul 配置源
/// </summary>
internal sealed class ConsulCfgSource : IWritableCfgSource, IDisposable
{
    private readonly ConsulCfgOptions _options;
    private readonly ConsulClient _client;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly CancellationTokenSource _watchCts;
    private ulong _lastIndex;
    private volatile bool _disposed;
    private Task? _watchTask;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();

    public ConsulCfgSource(ConsulCfgOptions options, int level, bool isPrimaryWriter)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        _data = new ConcurrentDictionary<string, string?>();
        _watchCts = new CancellationTokenSource();
        _reloadToken = new ConfigurationReloadToken();

        _client = new ConsulClient(config =>
        {
            config.Address = new Uri(options.Address);
            if (!string.IsNullOrEmpty(options.Token))
                config.Token = options.Token;
            if (!string.IsNullOrEmpty(options.Datacenter))
                config.Datacenter = options.Datacenter;
            config.WaitTime = options.WaitTime;
        });

        // 初始加载
        LoadDataAsync().GetAwaiter().GetResult();

        // 启动热重载监听
        if (options.EnableHotReload)
        {
            _watchTask = WatchForChangesAsync(_watchCts.Token);
        }
    }

    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _watchCts.Cancel();
        try { _watchTask?.Wait(TimeSpan.FromSeconds(2)); }
        catch { }

        _watchCts.Dispose();
        _client.Dispose();
    }

    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        return new ConsulConfigurationSource(this);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        foreach (var (key, value) in changes)
        {
            var consulKey = GetConsulKey(key);

            if (value is null)
            {
                await _client.KV.Delete(consulKey, cancellationToken).ConfigureAwait(false);
                _data.TryRemove(key, out _);
            }
            else
            {
                var pair = new KVPair(consulKey) { Value = Encoding.UTF8.GetBytes(value) };
                await _client.KV.Put(pair, cancellationToken).ConfigureAwait(false);
                _data[key] = value;
            }
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            switch (_options.DataFormat)
            {
                case ConsulDataFormat.KeyValue:
                    await LoadKeyValueDataAsync().ConfigureAwait(false);
                    break;
                case ConsulDataFormat.Json:
                    await LoadJsonDataAsync().ConfigureAwait(false);
                    break;
                case ConsulDataFormat.Yaml:
                    await LoadYamlDataAsync().ConfigureAwait(false);
                    break;
            }
        }
        catch
        {
            // 连接失败时保持空数据
        }
    }

    private async Task LoadKeyValueDataAsync()
    {
        var prefix = _options.KeyPrefix ?? "";
        var result = await _client.KV.List(prefix).ConfigureAwait(false);

        _lastIndex = result.LastIndex;

        if (result.Response == null) return;

        _data.Clear();
        foreach (var pair in result.Response)
        {
            if (pair.Value == null) continue;

            var key = pair.Key;
            // 去掉前缀
            if (!string.IsNullOrEmpty(prefix) && key.StartsWith(prefix))
                key = key.Substring(prefix.Length);

            // 将 Consul 的 / 分隔符转换为配置的 : 分隔符
            key = key.Replace('/', ':');

            var value = Encoding.UTF8.GetString(pair.Value);
            _data[key] = value;
        }
    }

    private async Task LoadJsonDataAsync()
    {
        var key = _options.SingleKey ?? _options.KeyPrefix ?? "config";
        var result = await _client.KV.Get(key).ConfigureAwait(false);

        _lastIndex = result.LastIndex;

        if (result.Response?.Value == null) return;

        var json = Encoding.UTF8.GetString(result.Response.Value);
        var flatData = FlattenJson(json);

        _data.Clear();
        foreach (var (k, v) in flatData)
        {
            _data[k] = v;
        }
    }

    private async Task LoadYamlDataAsync()
    {
        // YAML 支持需要额外依赖，这里简单处理为不支持
        // 如果需要 YAML 支持，可以引用 YamlDotNet
        var key = _options.SingleKey ?? _options.KeyPrefix ?? "config";
        var result = await _client.KV.Get(key).ConfigureAwait(false);

        _lastIndex = result.LastIndex;

        if (result.Response?.Value == null) return;

        // 暂时将整个 YAML 内容作为单个值存储
        var yaml = Encoding.UTF8.GetString(result.Response.Value);
        _data["_raw"] = yaml;
    }

    private async Task WatchForChangesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var prefix = _options.KeyPrefix ?? "";
                var queryOptions = new QueryOptions
                {
                    WaitIndex = _lastIndex,
                    WaitTime = _options.WaitTime
                };

                QueryResult<KVPair[]>? result;

                if (_options.DataFormat == ConsulDataFormat.KeyValue)
                {
                    result = await _client.KV.List(prefix, queryOptions, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var key = _options.SingleKey ?? _options.KeyPrefix ?? "config";
                    var singleResult = await _client.KV.Get(key, queryOptions, cancellationToken).ConfigureAwait(false);
                    result = new QueryResult<KVPair[]>
                    {
                        LastIndex = singleResult.LastIndex,
                        Response = singleResult.Response != null ? new[] { singleResult.Response } : Array.Empty<KVPair>()
                    };
                }

                if (result.LastIndex > _lastIndex)
                {
                    _lastIndex = result.LastIndex;
                    await LoadDataAsync().ConfigureAwait(false);
                    OnReload();
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // 连接失败，等待后重试
                try
                {
                    await Task.Delay(_options.ReconnectInterval, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private void OnReload()
    {
        ConfigurationReloadToken previousToken;
        lock (_reloadTokenLock)
        {
            previousToken = _reloadToken;
            _reloadToken = new ConfigurationReloadToken();
        }
        previousToken.OnReload();
    }

    private string GetConsulKey(string configKey)
    {
        // 将配置的 : 分隔符转换为 Consul 的 / 分隔符
        var consulKey = configKey.Replace(':', '/');
        var prefix = _options.KeyPrefix ?? "";
        return string.IsNullOrEmpty(prefix) ? consulKey : prefix + consulKey;
    }

    private static Dictionary<string, string?> FlattenJson(string json)
    {
        var result = new Dictionary<string, string?>();

        try
        {
            using var doc = JsonDocument.Parse(json);
            FlattenJsonElement(doc.RootElement, "", result);
        }
        catch
        {
            // JSON 解析失败
        }

        return result;
    }

    private static void FlattenJsonElement(JsonElement element, string prefix, Dictionary<string, string?> result)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";
                    FlattenJsonElement(property.Value, key, result);
                }
                break;

            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var key = $"{prefix}:{index}";
                    FlattenJsonElement(item, key, result);
                    index++;
                }
                break;

            case JsonValueKind.String:
                result[prefix] = element.GetString();
                break;

            case JsonValueKind.Number:
                result[prefix] = element.GetRawText();
                break;

            case JsonValueKind.True:
                result[prefix] = "true";
                break;

            case JsonValueKind.False:
                result[prefix] = "false";
                break;

            case JsonValueKind.Null:
                result[prefix] = null;
                break;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ConsulCfgSource));
    }

    internal IEnumerable<string> GetAllKeys() => _data.Keys;

    internal bool TryGetValue(string key, out string? value) => _data.TryGetValue(key, out value);

    internal IChangeToken GetReloadToken()
    {
        lock (_reloadTokenLock)
        {
            return _reloadToken;
        }
    }

    /// <summary>
    /// 内部配置源，用于集成到 Microsoft.Extensions.Configuration
    /// </summary>
    private sealed class ConsulConfigurationSource : IConfigurationSource
    {
        private readonly ConsulCfgSource _consulSource;

        public ConsulConfigurationSource(ConsulCfgSource consulSource)
        {
            _consulSource = consulSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConsulConfigurationProvider(_consulSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class ConsulConfigurationProvider : ConfigurationProvider
    {
        private readonly ConsulCfgSource _consulSource;

        public ConsulConfigurationProvider(ConsulCfgSource consulSource)
        {
            _consulSource = consulSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _consulSource.GetAllKeys())
            {
                if (_consulSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _consulSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _consulSource.GetReloadToken();
        }
    }
}
