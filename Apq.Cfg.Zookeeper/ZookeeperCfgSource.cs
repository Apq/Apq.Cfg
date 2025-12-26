using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using org.apache.zookeeper;

namespace Apq.Cfg.Zookeeper;

/// <summary>
/// Zookeeper 配置源
/// </summary>
internal sealed class ZookeeperCfgSource : IWritableCfgSource, IDisposable
{
    private readonly ZookeeperCfgOptions _options;
    private ZooKeeper? _client;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly CancellationTokenSource _disposeCts;
    private volatile bool _disposed;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();
    private readonly SemaphoreSlim _connectLock = new(1, 1);
    private readonly ZookeeperWatcher _watcher;

    public ZookeeperCfgSource(ZookeeperCfgOptions options, int level, bool isPrimaryWriter)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        _data = new ConcurrentDictionary<string, string?>();
        _disposeCts = new CancellationTokenSource();
        _reloadToken = new ConfigurationReloadToken();
        _watcher = new ZookeeperWatcher(this);

        // 初始连接和加载
        ConnectAndLoadAsync().GetAwaiter().GetResult();
    }

    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _disposeCts.Cancel();
        _disposeCts.Dispose();

        try
        {
            _client?.closeAsync().GetAwaiter().GetResult();
        }
        catch { }

        _connectLock.Dispose();
    }

    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        return new ZookeeperConfigurationSource(this);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        if (_client == null || _client.getState() != ZooKeeper.States.CONNECTED)
        {
            await EnsureConnectedAsync().ConfigureAwait(false);
        }

        foreach (var (key, value) in changes)
        {
            var path = GetZookeeperPath(key);

            if (value is null)
            {
                // 删除节点
                try
                {
                    await _client!.deleteAsync(path).ConfigureAwait(false);
                }
                catch (KeeperException.NoNodeException)
                {
                    // 节点不存在，忽略
                }
                _data.TryRemove(key, out _);
            }
            else
            {
                var data = Encoding.UTF8.GetBytes(value);

                try
                {
                    // 尝试更新
                    await _client!.setDataAsync(path, data).ConfigureAwait(false);
                }
                catch (KeeperException.NoNodeException)
                {
                    // 节点不存在，创建
                    await EnsurePathExistsAsync(path).ConfigureAwait(false);
                    await _client!.setDataAsync(path, data).ConfigureAwait(false);
                }

                _data[key] = value;
            }
        }
    }

    private async Task ConnectAndLoadAsync()
    {
        await EnsureConnectedAsync().ConfigureAwait(false);
        await LoadDataAsync().ConfigureAwait(false);
    }

    private async Task EnsureConnectedAsync()
    {
        await _connectLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_client != null && _client.getState() == ZooKeeper.States.CONNECTED)
                return;

            _client?.closeAsync().GetAwaiter().GetResult();

            var sessionTimeout = (int)_options.SessionTimeout.TotalMilliseconds;
            _client = new ZooKeeper(
                _options.ConnectionString,
                sessionTimeout,
                _watcher);

            // 等待连接
            var connectTimeout = _options.ConnectTimeout;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (_client.getState() != ZooKeeper.States.CONNECTED)
            {
                if (stopwatch.Elapsed > connectTimeout)
                {
                    throw new TimeoutException($"连接 Zookeeper 超时: {_options.ConnectionString}");
                }
                await Task.Delay(100).ConfigureAwait(false);
            }

            // 添加认证
            if (!string.IsNullOrEmpty(_options.AuthScheme) && !string.IsNullOrEmpty(_options.AuthInfo))
            {
                _client.addAuthInfo(_options.AuthScheme, Encoding.UTF8.GetBytes(_options.AuthInfo));
            }
        }
        finally
        {
            _connectLock.Release();
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            switch (_options.DataFormat)
            {
                case ZookeeperDataFormat.KeyValue:
                    await LoadKeyValueDataAsync().ConfigureAwait(false);
                    break;
                case ZookeeperDataFormat.Json:
                    await LoadJsonDataAsync().ConfigureAwait(false);
                    break;
            }
        }
        catch (KeeperException.NoNodeException)
        {
            // 根节点不存在，保持空数据
        }
        catch
        {
            // 连接失败时保持空数据
        }
    }

    private async Task LoadKeyValueDataAsync()
    {
        var rootPath = NormalizePath(_options.RootPath);
        _data.Clear();

        await LoadNodeRecursiveAsync(rootPath, "").ConfigureAwait(false);
    }

    private async Task LoadNodeRecursiveAsync(string path, string keyPrefix)
    {
        if (_client == null) return;

        try
        {
            // 读取当前节点数据
            var dataResult = await _client.getDataAsync(path, _options.EnableHotReload ? _watcher : null)
                .ConfigureAwait(false);

            if (dataResult.Data != null && dataResult.Data.Length > 0)
            {
                var value = Encoding.UTF8.GetString(dataResult.Data);
                if (!string.IsNullOrEmpty(keyPrefix))
                {
                    _data[keyPrefix] = value;
                }
            }

            // 获取子节点
            var childrenResult = await _client.getChildrenAsync(path, _options.EnableHotReload ? _watcher : null)
                .ConfigureAwait(false);

            if (childrenResult.Children != null)
            {
                foreach (var child in childrenResult.Children)
                {
                    var childPath = path == "/" ? $"/{child}" : $"{path}/{child}";
                    var childKey = string.IsNullOrEmpty(keyPrefix) ? child : $"{keyPrefix}:{child}";
                    await LoadNodeRecursiveAsync(childPath, childKey).ConfigureAwait(false);
                }
            }
        }
        catch (KeeperException.NoNodeException)
        {
            // 节点不存在
        }
    }

    private async Task LoadJsonDataAsync()
    {
        var nodePath = string.IsNullOrEmpty(_options.SingleNode)
            ? NormalizePath(_options.RootPath)
            : NormalizePath(_options.RootPath + "/" + _options.SingleNode);

        try
        {
            var dataResult = await _client!.getDataAsync(nodePath, _options.EnableHotReload ? _watcher : null)
                .ConfigureAwait(false);

            if (dataResult.Data == null || dataResult.Data.Length == 0) return;

            var json = Encoding.UTF8.GetString(dataResult.Data);
            var flatData = FlattenJson(json);

            _data.Clear();
            foreach (var (k, v) in flatData)
            {
                _data[k] = v;
            }
        }
        catch (KeeperException.NoNodeException)
        {
            // 节点不存在
        }
    }

    private string GetZookeeperPath(string configKey)
    {
        // 将配置的 : 分隔符转换为 Zookeeper 的 / 分隔符
        var zkPath = configKey.Replace(':', '/');
        var rootPath = NormalizePath(_options.RootPath);
        return rootPath == "/" ? $"/{zkPath}" : $"{rootPath}/{zkPath}";
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "/";
        if (!path.StartsWith("/")) path = "/" + path;
        if (path.Length > 1 && path.EndsWith("/")) path = path.TrimEnd('/');
        return path;
    }

    private async Task EnsurePathExistsAsync(string path)
    {
        if (_client == null) return;

        var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "";

        foreach (var part in parts.Take(parts.Length - 1))
        {
            currentPath += "/" + part;
            try
            {
                await _client.createAsync(currentPath, Array.Empty<byte>(),
                    ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
            }
            catch (KeeperException.NodeExistsException)
            {
                // 节点已存在
            }
        }

        // 创建最终节点
        try
        {
            await _client.createAsync(path, Array.Empty<byte>(),
                ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
        }
        catch (KeeperException.NodeExistsException)
        {
            // 节点已存在
        }
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
        if (_disposed) throw new ObjectDisposedException(nameof(ZookeeperCfgSource));
    }

    internal void OnNodeChanged()
    {
        if (_disposed) return;

        // 重新加载数据
        Task.Run(async () =>
        {
            try
            {
                await EnsureConnectedAsync().ConfigureAwait(false);
                await LoadDataAsync().ConfigureAwait(false);
                OnReload();
            }
            catch
            {
                // 忽略重载错误
            }
        });
    }

    internal void OnSessionExpired()
    {
        if (_disposed) return;

        // 会话过期，重新连接
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_options.ReconnectInterval).ConfigureAwait(false);
                await ConnectAndLoadAsync().ConfigureAwait(false);
                OnReload();
            }
            catch
            {
                // 忽略重连错误
            }
        });
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
    /// Zookeeper 事件监听器
    /// </summary>
    private sealed class ZookeeperWatcher : Watcher
    {
        private readonly ZookeeperCfgSource _source;

        public ZookeeperWatcher(ZookeeperCfgSource source)
        {
            _source = source;
        }

        public override Task process(WatchedEvent @event)
        {
            switch (@event.getState())
            {
                case Watcher.Event.KeeperState.Expired:
                    _source.OnSessionExpired();
                    break;

                case Watcher.Event.KeeperState.SyncConnected:
                    if (@event.get_Type() != Event.EventType.None)
                    {
                        // 节点变更事件
                        _source.OnNodeChanged();
                    }
                    break;
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 内部配置源，用于集成到 Microsoft.Extensions.Configuration
    /// </summary>
    private sealed class ZookeeperConfigurationSource : IConfigurationSource
    {
        private readonly ZookeeperCfgSource _zkSource;

        public ZookeeperConfigurationSource(ZookeeperCfgSource zkSource)
        {
            _zkSource = zkSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ZookeeperConfigurationProvider(_zkSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class ZookeeperConfigurationProvider : ConfigurationProvider
    {
        private readonly ZookeeperCfgSource _zkSource;

        public ZookeeperConfigurationProvider(ZookeeperCfgSource zkSource)
        {
            _zkSource = zkSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _zkSource.GetAllKeys())
            {
                if (_zkSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _zkSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _zkSource.GetReloadToken();
        }
    }
}
