using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Apq.Cfg.Apollo;

/// <summary>
/// Apollo 配置源
/// </summary>
internal sealed class ApolloCfgSource : IWritableCfgSource, IDisposable
{
    private readonly ApolloCfgOptions _options;
    private readonly HttpClient _httpClient;
    private readonly HttpClient _longPollingClient;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly ConcurrentDictionary<string, long> _notificationIds;
    private readonly CancellationTokenSource _disposeCts;
    private volatile bool _disposed;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();
    private Task? _watchTask;

    public ApolloCfgSource(ApolloCfgOptions options, int level, bool isPrimaryWriter, string? name = null)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        Name = name ?? $"Apollo:{options.AppId}";
        _data = new ConcurrentDictionary<string, string?>();
        _notificationIds = new ConcurrentDictionary<string, long>();
        _disposeCts = new CancellationTokenSource();
        _reloadToken = new ConfigurationReloadToken();

        _httpClient = new HttpClient
        {
            Timeout = options.ConnectTimeout
        };

        _longPollingClient = new HttpClient
        {
            Timeout = options.LongPollingTimeout + TimeSpan.FromSeconds(10)
        };

        // 初始化通知 ID
        foreach (var ns in options.Namespaces)
        {
            _notificationIds[ns] = -1;
        }

        // 初始加载
        LoadDataAsync().GetAwaiter().GetResult();

        // 启动热重载监听
        if (options.EnableHotReload)
        {
            _watchTask = WatchForChangesAsync(_disposeCts.Token);
        }
    }

    /// <inheritdoc />
    public int Level { get; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string Type => nameof(ApolloCfgSource);

    /// <inheritdoc />
    public bool IsWriteable => false;

    /// <inheritdoc />
    public bool IsPrimaryWriter { get; }

    /// <inheritdoc />
    public int KeyCount => GetAllValues().Count();

    /// <inheritdoc />
    public int TopLevelKeyCount => GetAllValues()
        .Select(kv => kv.Key.Split(':')[0])
        .Distinct()
        .Count();

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        return _data.ToArray();
    }

    /// <summary>
    /// 释放资源，取消所有异步操作并释放 HTTP 客户端
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _disposeCts.Cancel();
        try { _watchTask?.Wait(TimeSpan.FromSeconds(2)); }
        catch { }

        _disposeCts.Dispose();
        _httpClient.Dispose();
        _longPollingClient.Dispose();
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.IConfigurationSource 实例</returns>
    /// <exception cref="ObjectDisposedException">当对象已释放时抛出</exception>
    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        return new ApolloConfigurationSource(this);
    }

    /// <summary>
    /// 应用配置更改（Apollo 不支持通过 API 写入配置）
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="NotSupportedException">始终抛出，因为 Apollo 不支持通过 API 写入配置</exception>
    public Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        // Apollo 不支持通过 API 写入配置
        throw new NotSupportedException("Apollo 配置源不支持写入操作，请通过 Apollo 管理界面修改配置");
    }

    private async Task LoadDataAsync()
    {
        try
        {
            _data.Clear();

            foreach (var ns in _options.Namespaces)
            {
                var config = await GetConfigAsync(ns).ConfigureAwait(false);
                if (config != null)
                {
                    foreach (var (key, value) in config)
                    {
                        // 如果有多个命名空间，使用命名空间作为前缀（application 除外）
                        var configKey = _options.Namespaces.Length > 1 && ns != "application"
                            ? $"{ns}:{key}"
                            : key;
                        _data[configKey] = value;
                    }
                }
            }
        }
        catch
        {
            // 连接失败时保持空数据
        }
    }

    private async Task<Dictionary<string, string?>?> GetConfigAsync(string namespaceName)
    {
        var url = BuildConfigUrl(namespaceName);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            AddAuthorizationHeader(request, url);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);

            var result = new Dictionary<string, string?>();

            // Apollo 返回格式: { "configurations": { "key": "value" }, ... }
            if (doc.RootElement.TryGetProperty("configurations", out var configurations))
            {
                foreach (var prop in configurations.EnumerateObject())
                {
                    var value = prop.Value.ValueKind == JsonValueKind.String
                        ? prop.Value.GetString()
                        : prop.Value.GetRawText();

                    // 将 . 分隔符转换为 : 分隔符
                    var key = prop.Name.Replace('.', ':');
                    result[key] = value;
                }
            }

            return result;
        }
        catch
        {
            return null;
        }
    }

    private string BuildConfigUrl(string namespaceName)
    {
        var metaServer = _options.MetaServer.TrimEnd('/');
        return $"{metaServer}/configs/{HttpUtility.UrlEncode(_options.AppId)}/{HttpUtility.UrlEncode(_options.Cluster)}/{HttpUtility.UrlEncode(namespaceName)}";
    }

    private async Task WatchForChangesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var hasChanges = await CheckForNotificationsAsync(cancellationToken).ConfigureAwait(false);
                if (hasChanges)
                {
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
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task<bool> CheckForNotificationsAsync(CancellationToken cancellationToken)
    {
        var notifications = _notificationIds.Select(kv => new
        {
            namespaceName = kv.Key,
            notificationId = kv.Value
        }).ToArray();

        var notificationsJson = JsonSerializer.Serialize(notifications);
        var metaServer = _options.MetaServer.TrimEnd('/');
        var url = $"{metaServer}/notifications/v2?" +
                  $"appId={HttpUtility.UrlEncode(_options.AppId)}&" +
                  $"cluster={HttpUtility.UrlEncode(_options.Cluster)}&" +
                  $"notifications={HttpUtility.UrlEncode(notificationsJson)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AddAuthorizationHeader(request, url);

        var response = await _longPollingClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        using var doc = JsonDocument.Parse(json);

        var hasChanges = false;
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (item.TryGetProperty("namespaceName", out var nsElement) &&
                item.TryGetProperty("notificationId", out var idElement))
            {
                var ns = nsElement.GetString();
                var id = idElement.GetInt64();
                if (ns != null)
                {
                    _notificationIds[ns] = id;
                    hasChanges = true;
                }
            }
        }

        return hasChanges;
    }

    private void AddAuthorizationHeader(HttpRequestMessage request, string url)
    {
        if (string.IsNullOrEmpty(_options.Secret)) return;

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var pathAndQuery = new Uri(url).PathAndQuery;
        var stringToSign = $"{timestamp}\n{pathAndQuery}";

        using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_options.Secret));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

        request.Headers.Add("Authorization", $"Apollo {_options.AppId}:{signature}");
        request.Headers.Add("Timestamp", timestamp);
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

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ApolloCfgSource));
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
    private sealed class ApolloConfigurationSource : IConfigurationSource
    {
        private readonly ApolloCfgSource _apolloSource;

        public ApolloConfigurationSource(ApolloCfgSource apolloSource)
        {
            _apolloSource = apolloSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ApolloConfigurationProvider(_apolloSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class ApolloConfigurationProvider : ConfigurationProvider
    {
        private readonly ApolloCfgSource _apolloSource;

        public ApolloConfigurationProvider(ApolloCfgSource apolloSource)
        {
            _apolloSource = apolloSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _apolloSource.GetAllKeys())
            {
                if (_apolloSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _apolloSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _apolloSource.GetReloadToken();
        }
    }
}
