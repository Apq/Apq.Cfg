using System.Collections.Concurrent;
using Apq.Cfg.Sources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.AuthMethods.AppRole;

namespace Apq.Cfg.Vault;

/// <summary>
/// Vault 配置源
/// </summary>
internal sealed class VaultCfgSource : IWritableCfgSource, IDisposable
{
    private readonly VaultCfgOptions _options;
    private readonly IVaultClient _client;
    private readonly ConcurrentDictionary<string, string?> _data;
    private readonly CancellationTokenSource _watchCts;
    private volatile bool _disposed;
    private Task? _watchTask;
    private ConfigurationReloadToken _reloadToken;
    private readonly object _reloadTokenLock = new();

    public VaultCfgSource(VaultCfgOptions options, int level, bool isPrimaryWriter, string? name = null)
    {
        _options = options;
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
        Name = name ?? $"Vault:{options.Path ?? "config"}";
        _data = new ConcurrentDictionary<string, string?>();
        _watchCts = new CancellationTokenSource();
        _reloadToken = new ConfigurationReloadToken();

        _client = CreateVaultClient(options);

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
    }

    public IConfigurationSource BuildSource()
    {
        ThrowIfDisposed();
        return new VaultConfigurationSource(this);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();

        if (_options.KvVersion == 2)
        {
            await ApplyChangesV2Async(changes, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await ApplyChangesV1Async(changes, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ApplyChangesV2Async(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        var path = _options.Path ?? "config";
        var enginePath = _options.EnginePath ?? "secret";

        // 获取当前数据
        Dictionary<string, object> data;
        try
        {
            var currentData = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: path,
                mountPoint: enginePath).ConfigureAwait(false);
            data = currentData?.Data?.Data != null
                ? new Dictionary<string, object>(currentData.Data.Data)
                : new Dictionary<string, object>();
        }
        catch
        {
            data = new Dictionary<string, object>();
        }

        // 应用变更
        foreach (var (key, value) in changes)
        {
            var vaultKey = key.Replace(':', '_'); // Vault 通常使用下划线
            if (value is null)
            {
                data.Remove(vaultKey);
            }
            else
            {
                data[vaultKey] = value;
            }
            _data[key] = value;
        }

        // 写回 Vault
        await _client.V1.Secrets.KeyValue.V2.WriteSecretAsync(
            path: path,
            data: data,
            mountPoint: enginePath).ConfigureAwait(false);
    }

    private async Task ApplyChangesV1Async(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        var path = _options.Path ?? "config";
        var enginePath = _options.EnginePath ?? "secret";

        // 获取当前数据
        Dictionary<string, object> data;
        try
        {
            var currentData = await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync(
                path: path,
                mountPoint: enginePath).ConfigureAwait(false);
            data = currentData?.Data != null
                ? new Dictionary<string, object>(currentData.Data)
                : new Dictionary<string, object>();
        }
        catch
        {
            data = new Dictionary<string, object>();
        }

        // 应用变更
        foreach (var (key, value) in changes)
        {
            var vaultKey = key.Replace(':', '_');
            if (value is null)
            {
                data.Remove(vaultKey);
            }
            else
            {
                data[vaultKey] = value;
            }
            _data[key] = value;
        }

        // 写回 Vault
        await _client.V1.Secrets.KeyValue.V1.WriteSecretAsync(
            path: path,
            values: data,
            mountPoint: enginePath).ConfigureAwait(false);
    }

    private async Task LoadDataAsync()
    {
        try
        {
            if (_options.KvVersion == 2)
            {
                await LoadDataV2Async().ConfigureAwait(false);
            }
            else
            {
                await LoadDataV1Async().ConfigureAwait(false);
            }
        }
        catch
        {
            // 连接失败时保持空数据
        }
    }

    private async Task LoadDataV2Async()
    {
        var path = _options.Path ?? "config";
        var enginePath = _options.EnginePath ?? "secret";

        var secret = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: path,
            mountPoint: enginePath).ConfigureAwait(false);

        if (secret?.Data?.Data == null) return;

        _data.Clear();
        foreach (var kvp in secret.Data.Data)
        {
            // 将 Vault 的 _ 分隔符转换为配置的 : 分隔符
            var configKey = kvp.Key.Replace('_', ':');
            _data[configKey] = kvp.Value?.ToString();
        }
    }

    private async Task LoadDataV1Async()
    {
        var path = _options.Path ?? "config";
        var enginePath = _options.EnginePath ?? "secret";

        var secret = await _client.V1.Secrets.KeyValue.V1.ReadSecretAsync(
            path: path,
            mountPoint: enginePath).ConfigureAwait(false);

        if (secret?.Data == null) return;

        _data.Clear();
        foreach (var kvp in secret.Data)
        {
            var configKey = kvp.Key.Replace('_', ':');
            _data[configKey] = kvp.Value?.ToString();
        }
    }

    private async Task WatchForChangesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 简单的轮询实现
                await Task.Delay(_options.PollInterval, cancellationToken).ConfigureAwait(false);

                var oldHash = GetDataHash();
                await LoadDataAsync().ConfigureAwait(false);
                var newHash = GetDataHash();

                if (oldHash != newHash)
                {
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

    private string GetDataHash()
    {
        return string.Join("|", _data.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
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

    private IVaultClient CreateVaultClient(VaultCfgOptions options)
    {
        IAuthMethodInfo? authMethod = null;

        switch (options.AuthMethod)
        {
            case VaultAuthMethod.Token:
                if (string.IsNullOrEmpty(options.Token))
                    throw new ArgumentException("Token is required for Token authentication", nameof(options));
                authMethod = new TokenAuthMethodInfo(options.Token);
                break;

            case VaultAuthMethod.UserPass:
                if (string.IsNullOrEmpty(options.Username) || string.IsNullOrEmpty(options.Password))
                    throw new ArgumentException("Username and Password are required for UserPass authentication", nameof(options));
                authMethod = new UserPassAuthMethodInfo(options.Username, options.Password);
                break;

            case VaultAuthMethod.AppRole:
                if (string.IsNullOrEmpty(options.RoleId) || string.IsNullOrEmpty(options.RoleSecret))
                    throw new ArgumentException("RoleId and RoleSecret are required for AppRole authentication", nameof(options));
                authMethod = new AppRoleAuthMethodInfo(options.RoleId, options.RoleSecret);
                break;
        }

        var vaultClientSettings = new VaultClientSettings(options.Address, authMethod);

        if (!string.IsNullOrEmpty(options.Namespace))
        {
            vaultClientSettings.Namespace = options.Namespace;
        }

        return new VaultClient(vaultClientSettings);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VaultCfgSource));
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
    private sealed class VaultConfigurationSource : IConfigurationSource
    {
        private readonly VaultCfgSource _vaultSource;

        public VaultConfigurationSource(VaultCfgSource vaultSource)
        {
            _vaultSource = vaultSource;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VaultConfigurationProvider(_vaultSource);
        }
    }

    /// <summary>
    /// 内部配置提供程序
    /// </summary>
    private sealed class VaultConfigurationProvider : ConfigurationProvider
    {
        private readonly VaultCfgSource _vaultSource;

        public VaultConfigurationProvider(VaultCfgSource vaultSource)
        {
            _vaultSource = vaultSource;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in _vaultSource.GetAllKeys())
            {
                if (_vaultSource.TryGetValue(key, out var value))
                {
                    Data[key] = value;
                }
            }
        }

        public override bool TryGet(string key, out string? value)
        {
            return _vaultSource.TryGetValue(key, out value);
        }

        public new IChangeToken GetReloadToken()
        {
            return _vaultSource.GetReloadToken();
        }
    }
}
