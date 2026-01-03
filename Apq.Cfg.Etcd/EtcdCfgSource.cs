using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Apq.Cfg.Sources;
using dotnet_etcd;
using Etcdserverpb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Etcd;

/// <summary>
/// Etcd 配置源
/// </summary>
internal sealed class EtcdCfgSource : IWritableCfgSource, IDisposable
{
    private readonly EtcdCfgOptions _options;
    private readonly EtcdClient _client;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly CancellationTokenSource _watchCts;
    private volatile bool _disposed;
    private Task? _watchTask;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();

    public EtcdCfgSource(EtcdCfgOptions options, int level, bool isPrimaryWriter, string? name = null)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        Name = name ?? $"Etcd:{options.KeyPrefix ?? "/config/"}";
        _data = new ConcurrentDictionary<string, string?>();
        _watchCts = new CancellationTokenSource();
        _reloadToken = new ConfigurationReloadToken();

        // 创建 Etcd 客户端
        var connectionString = string.Join(",", options.Endpoints);
        _client = new EtcdClient(connectionString);

        // 认证
        if (!string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password))
        {
            _client.Authenticate(new AuthenticateRequest
            {
                Name = options.Username,
                Password = options.Password
            });
        }

        // 初始加载
        LoadDataAsync().GetAwaiter().GetResult();

        // 启动热重载监听
        if (options.EnableHotReload)
        {
            _watchTask = WatchForChangesAsync(_watchCts.Token);
        }
    }

    public int Level { get; }

    /// <inheritdoc />
    public string Name { get; set; }

    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        return _data.ToArray();
    }

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
        return new EtcdConfigurationSource(this);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        foreach (var (key, value) in changes)
        {
            var etcdKey = GetEtcdKey(key);

            if (value is null)
            {
                await _client.DeleteAsync(etcdKey).ConfigureAwait(false);
                _data.TryRemove(key, out _);
            }
            else
            {
                await _client.PutAsync(etcdKey, value).ConfigureAwait(false);
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
                case EtcdDataFormat.KeyValue:
                    await LoadKeyValueDataAsync().ConfigureAwait(false);
                    break;
                case EtcdDataFormat.Json:
                    await LoadJsonDataAsync().ConfigureAwait(false);
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
        var prefix = _options.KeyPrefix ?? "/config/";
        var response = await _client.GetRangeAsync(prefix).ConfigureAwait(false);

        _data.Clear();
        foreach (var kv in response.Kvs)
        {
            var key = kv.Key.ToStringUtf8();
            // 去掉前缀
            if (!string.IsNullOrEmpty(prefix) && key.StartsWith(prefix))
                key = key.Substring(prefix.Length);

            // 将 Etcd 的 / 分隔符转换为配置的 : 分隔符
            key = key.Replace('/', ':');

            var value = kv.Value.ToStringUtf8();
            _data[key] = value;
        }
    }

    private async Task LoadJsonDataAsync()
    {
        var key = _options.SingleKey ?? _options.KeyPrefix ?? "/config";
        var response = await _client.GetAsync(key).ConfigureAwait(false);

        if (response.Kvs.Count == 0) return;

        var json = response.Kvs[0].Value.ToStringUtf8();
        var flatData = FlattenJson(json);

        _data.Clear();
        foreach (var (k, v) in flatData)
        {
            _data[k] = v;
        }
    }

    private async Task WatchForChangesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var prefix = _options.KeyPrefix ?? "/config/";

                if (_options.DataFormat == EtcdDataFormat.KeyValue)
                {
                    // 监听前缀下的所有键变更
                    _client.WatchRange(prefix, async (response) =>
                    {
                        if (response.Events.Count > 0)
                        {
                            await LoadDataAsync().ConfigureAwait(false);
                            OnReload();
                        }
                    }, cancellationToken: cancellationToken);
                }
                else
                {
                    var key = _options.SingleKey ?? _options.KeyPrefix ?? "/config";
                    _client.Watch(key, async (response) =>
                    {
                        if (response.Events.Count > 0)
                        {
                            await LoadDataAsync().ConfigureAwait(false);
                            OnReload();
                        }
                    }, cancellationToken: cancellationToken);
                }

                // 等待取消
                await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
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

    private string GetEtcdKey(string configKey)
    {
        // 将配置的 : 分隔符转换为 Etcd 的 / 分隔符
        var etcdKey = configKey.Replace(':', '/');
        var prefix = _options.KeyPrefix ?? "/config/";
        return string.IsNullOrEmpty(prefix) ? etcdKey : prefix + etcdKey;
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
        if (_disposed) throw new ObjectDisposedException(nameof(EtcdCfgSource));
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
    private sealed class EtcdConfigurationSource : IConfigurationSource
    {
        private readonly EtcdCfgSource _etcdSource;

        public EtcdConfigurationSource(EtcdCfgSource etcdSource)
        {
            _etcdSource = etcdSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EtcdConfigurationProvider(_etcdSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class EtcdConfigurationProvider : ConfigurationProvider
    {
        private readonly EtcdCfgSource _etcdSource;

        public EtcdConfigurationProvider(EtcdCfgSource etcdSource)
        {
            _etcdSource = etcdSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _etcdSource.GetAllKeys())
            {
                if (_etcdSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _etcdSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _etcdSource.GetReloadToken();
        }
    }
}
