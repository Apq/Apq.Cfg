using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Nacos.V2;
using Nacos.V2.DependencyInjection;

namespace Apq.Cfg.Nacos;

/// <summary>
/// Nacos 配置源，使用官方 SDK，支持热重载
/// </summary>
internal sealed class NacosCfgSource : IWritableCfgSource, IDisposable
{
    private readonly NacosCfgOptions _options;
    private readonly INacosConfigService _configService;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly ServiceProvider _serviceProvider;
    private readonly NacosConfigListener? _listener;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();
    private volatile bool _disposed;

    public NacosCfgSource(NacosCfgOptions options, int level, bool isPrimaryWriter)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        _data = new ConcurrentDictionary<string, string?>();
        _reloadToken = new ConfigurationReloadToken();

        // 创建 Nacos 配置服务
        var services = new ServiceCollection();
        ConfigureNacosServices(services, options);
        _serviceProvider = services.BuildServiceProvider();
        _configService = _serviceProvider.GetRequiredService<INacosConfigService>();

        // 初始加载
        InitializeAsync().GetAwaiter().GetResult();

        // 启用热重载监听
        if (options.EnableHotReload)
        {
            _listener = new NacosConfigListener(this);
            AddListenerAsync().GetAwaiter().GetResult();
        }
    }

    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    private static void ConfigureNacosServices(IServiceCollection services, NacosCfgOptions options)
    {
        var serverAddresses = options.ServerAddresses
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Select(s => s.StartsWith("http://") || s.StartsWith("https://") ? s : $"http://{s}")
            .ToList();

        services.AddNacosV2Config(x =>
        {
            x.ServerAddresses = serverAddresses;
            x.Namespace = options.Namespace == "public" ? "" : options.Namespace;
            x.UserName = options.Username ?? "";
            x.Password = options.Password ?? "";
            x.AccessKey = options.AccessKey ?? "";
            x.SecretKey = options.SecretKey ?? "";
            x.DefaultTimeOut = options.ConnectTimeoutMs;
        });
    }

    private async Task InitializeAsync()
    {
        try
        {
            var content = await _configService.GetConfig(
                _options.DataId,
                _options.Group,
                _options.ConnectTimeoutMs).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(content))
            {
                ParseContent(content);
            }
        }
        catch
        {
            // 连接失败时保持空数据
        }
    }

    private async Task AddListenerAsync()
    {
        try
        {
            if (_listener != null)
            {
                await _configService.AddListener(
                    _options.DataId,
                    _options.Group,
                    _listener).ConfigureAwait(false);
            }
        }
        catch
        {
            // 添加监听器失败，忽略
        }
    }

    private async Task RemoveListenerAsync()
    {
        try
        {
            if (_listener != null)
            {
                await _configService.RemoveListener(
                    _options.DataId,
                    _options.Group,
                    _listener).ConfigureAwait(false);
            }
        }
        catch
        {
            // 移除监听器失败，忽略
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // 移除监听器
        if (_listener != null)
        {
            RemoveListenerAsync().GetAwaiter().GetResult();
        }

        // 关闭服务
        try
        {
            _configService.ShutDown().GetAwaiter().GetResult();
        }
        catch
        {
            // 忽略关闭失败
        }

        _serviceProvider.Dispose();
    }

    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        return new NacosConfigurationSource(this);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        // 更新本地数据
        foreach (var (key, value) in changes)
        {
            if (value is null)
            {
                _data.TryRemove(key, out _);
            }
            else
            {
                _data[key] = value;
            }
        }

        // 将数据序列化并发布到 Nacos
        var content = SerializeData();
        var configType = _options.DataFormat switch
        {
            NacosDataFormat.Json => "json",
            NacosDataFormat.Yaml => "yaml",
            NacosDataFormat.Properties => "properties",
            _ => "json"
        };

        await _configService.PublishConfig(_options.DataId, _options.Group, content, configType)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 处理配置变更（由监听器调用）
    /// </summary>
    internal void OnConfigChanged(string content)
    {
        if (_disposed) return;

        try
        {
            ParseContent(content);
            OnReload();
        }
        catch
        {
            // 解析失败，保持原有数据
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

    private void ParseContent(string content)
    {
        _data.Clear();

        switch (_options.DataFormat)
        {
            case NacosDataFormat.Json:
                ParseJsonContent(content);
                break;
            case NacosDataFormat.Yaml:
                ParseYamlContent(content);
                break;
            case NacosDataFormat.Properties:
                ParsePropertiesContent(content);
                break;
        }
    }

    private void ParseJsonContent(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            FlattenJsonElement(doc.RootElement, "", _data);
        }
        catch
        {
            // JSON 解析失败
        }
    }

    private void ParseYamlContent(string yaml)
    {
        // YAML 支持需要额外依赖，这里简单处理
        // 将整个 YAML 内容作为单个值存储
        _data["_raw"] = yaml;
    }

    private void ParsePropertiesContent(string properties)
    {
        var lines = properties.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                continue;

            var eqIndex = trimmed.IndexOf('=');
            if (eqIndex <= 0) continue;

            var key = trimmed.Substring(0, eqIndex).Trim();
            var value = trimmed.Substring(eqIndex + 1).Trim();

            // 将 . 分隔符转换为 : 分隔符
            key = key.Replace('.', ':');
            _data[key] = value;
        }
    }

    private string SerializeData()
    {
        switch (_options.DataFormat)
        {
            case NacosDataFormat.Json:
                return SerializeToJson();
            case NacosDataFormat.Properties:
                return SerializeToProperties();
            default:
                return SerializeToJson();
        }
    }

    private string SerializeToJson()
    {
        var root = new Dictionary<string, object?>();

        foreach (var (key, value) in _data)
        {
            SetNestedValue(root, key.Split(':'), value);
        }

        return JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
    }

    private static void SetNestedValue(Dictionary<string, object?> root, string[] keys, string? value)
    {
        var current = root;
        for (var i = 0; i < keys.Length - 1; i++)
        {
            var key = keys[i];
            if (!current.TryGetValue(key, out var next) || next is not Dictionary<string, object?> nextDict)
            {
                nextDict = new Dictionary<string, object?>();
                current[key] = nextDict;
            }
            current = nextDict;
        }
        current[keys[^1]] = value;
    }

    private string SerializeToProperties()
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in _data)
        {
            // 将 : 分隔符转换为 . 分隔符
            var propKey = key.Replace(':', '.');
            sb.AppendLine($"{propKey}={value}");
        }
        return sb.ToString();
    }

    private static void FlattenJsonElement(JsonElement element, string prefix, ConcurrentDictionary<string, string?> result)
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
        if (_disposed) throw new ObjectDisposedException(nameof(NacosCfgSource));
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
    /// Nacos 配置监听器
    /// </summary>
    private sealed class NacosConfigListener : IListener
    {
        private readonly NacosCfgSource _source;

        public NacosConfigListener(NacosCfgSource source)
        {
            _source = source;
        }

        public void ReceiveConfigInfo(string configInfo)
        {
            _source.OnConfigChanged(configInfo);
        }
    }

    /// <summary>
    /// 内部配置源，用于集成到 Microsoft.Extensions.Configuration
    /// </summary>
    private sealed class NacosConfigurationSource : IConfigurationSource
    {
        private readonly NacosCfgSource _nacosSource;

        public NacosConfigurationSource(NacosCfgSource nacosSource)
        {
            _nacosSource = nacosSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new NacosConfigurationProvider(_nacosSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class NacosConfigurationProvider : ConfigurationProvider
    {
        private readonly NacosCfgSource _nacosSource;

        public NacosConfigurationProvider(NacosCfgSource nacosSource)
        {
            _nacosSource = nacosSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _nacosSource.GetAllKeys())
            {
                if (_nacosSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _nacosSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _nacosSource.GetReloadToken();
        }
    }
}
