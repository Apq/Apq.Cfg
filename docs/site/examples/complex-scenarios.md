# 复杂场景示例

本文档提供了 Apq.Cfg 在复杂场景下的使用示例，帮助开发者解决实际项目中的配置管理挑战。

## 1. 多环境部署配置

### 场景描述

企业级应用程序通常需要在多个环境（开发、测试、预发布、生产）之间部署，每个环境需要不同的配置值。

### 解决方案

```csharp
public static class ConfigurationBuilder
{
    public static ICfgRoot BuildConfiguration(string environment, string[]? args = null)
    {
        var builder = new CfgBuilder();

        // 层级 0：基础配置，所有环境共享（默认层级）
        builder.AddJson("config.json", optional: true);

        // 层级 1：环境特定配置
        builder.AddJson($"config.{environment}.json", level: 1, optional: true);

        // 层级 4：机器特定配置（可选）
        builder.AddJson($"config.{environment}.{Environment.MachineName}.json",
                     level: 4, optional: true);

        // 层级 200：远程配置中心（生产环境，使用默认层级）
        if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
        {
            builder.AddConsul(
                address: "https://consul.example.com",
                keyPrefix: "myapp/config/",
                enableHotReload: true);
        }

        // 层级 400：环境变量，可覆盖任何文件配置（默认层级）
        builder.AddEnvironmentVariables(prefix: "APP_");

        return builder.Build();
    }
}
```

### 部署脚本

```bash
#!/bin/bash

# 设置环境变量
export ASPNETCORE_ENVIRONMENT=Production
export APP_ConnectionStrings__DefaultConnection="Server=prod-db;Database=MyApp;..."

# 启动应用
dotnet MyApp.dll
```

## 2. 配置迁移场景

### 场景描述

当应用程序升级或配置结构发生变化时，需要将旧配置迁移到新格式。

### 解决方案

```csharp
public class ConfigurationMigrator
{
    private readonly ICfgRoot _cfg;
    private readonly ILogger<ConfigurationMigrator> _logger;

    public ConfigurationMigrator(ICfgRoot cfg, ILogger<ConfigurationMigrator> logger)
    {
        _cfg = cfg;
        _logger = logger;
    }

    public async Task<bool> MigrateIfNeededAsync()
    {
        var currentVersion = _cfg.GetValue<string?>("ConfigVersion");
        var targetVersion = "2.0";

        if (currentVersion == targetVersion)
        {
            _logger.LogInformation("配置已是最新版本 {Version}", targetVersion);
            return true;
        }

        _logger.LogInformation("开始配置迁移，从版本 {OldVersion} 到 {NewVersion}", 
                         currentVersion ?? "未知", targetVersion);

        try
        {
            // 执行具体迁移步骤
            await MigrateFrom1To2Async();

            // 更新版本号
            _cfg["ConfigVersion"] = targetVersion;
            await _cfg.SaveAsync();

            _logger.LogInformation("配置迁移成功完成");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "配置迁移失败");
            return false;
        }
    }

    private async Task MigrateFrom1To2Async()
    {
        // 示例：迁移数据库连接字符串格式
        var oldConnectionString = _cfg.GetValue<string>("ConnectionStrings:DefaultConnection");
        if (oldConnectionString.StartsWith("Server="))
        {
            // 解析旧格式
            var builder = new SqlConnectionStringBuilder(oldConnectionString);

            // 构建新格式
            var newFormat = new
            {
                ConnectionStrings = new
                {
                    DefaultConnection = new
                    {
                        ServerName = builder["Server"],
                        DatabaseName = builder["Database"],
                        UserId = builder["User ID"],
                        Password = builder["Password"]
                    }
                }
            };

            // 应用新格式
            _cfg["ConnectionStrings:DefaultConnection"] = JsonSerializer.Serialize(newFormat);
        }

        // 示例：迁移功能开关结构
        if (_cfg.Exists("FeatureFlags"))
        {
            var oldFlags = _cfg.GetSection("FeatureFlags");
            var newFlags = new Dictionary<string, object>();

            foreach (var key in oldFlags.GetChildKeys())
            {
                var value = oldFlags.GetValue<bool>(key);
                newFlags[$"Features:{key}:Enabled"] = value;
            }

            _cfg.Remove("FeatureFlags");
            foreach (var kvp in newFlags)
            {
                _cfg[kvp.Key] = kvp.Value.ToString();
            }
        }
    }
}
```

## 3. 动态配置更新场景

### 场景描述

在微服务架构中，需要在不重启服务的情况下动态更新配置，如功能开关、限流参数等。

### 解决方案

```csharp
public class DynamicConfigurationManager : IDisposable
{
    private readonly ICfgRoot _cfg;
    private readonly ILogger<DynamicConfigurationManager> _logger;
    private readonly ConcurrentDictionary<string, object> _featureCache = new();
    private readonly IDisposable _subscription;

    public DynamicConfigurationManager(ICfgRoot cfg, ILogger<DynamicConfigurationManager> logger)
    {
        _cfg = cfg;
        _logger = logger;

        // 订阅配置变更事件
        _subscription = _cfg.ConfigChanges
            .Subscribe(OnConfigChanged);
    }

    public bool IsFeatureEnabled(string featureName)
    {
        var cacheKey = $"Features:{featureName}:Enabled";

        return (bool)_featureCache.GetOrAdd(cacheKey, _ =>
        {
            var value = _cfg.GetValue<bool>($"Features:{featureName}:Enabled");
            _logger.LogDebug("功能 {FeatureName} 状态: {IsEnabled}", featureName, value);
            return (object)value;
        });
    }

    private void OnConfigChanged(ConfigChangeEvent e)
    {
        // 只处理功能开关相关的变更
        foreach (var (key, change) in e.Changes.Where(c => c.Key.StartsWith("Features:")))
        {
            // 更新缓存
            if (_featureCache.TryGetValue(key, out var oldValue))
            {
                _featureCache[key] = Convert.ToBoolean(change.NewValue);
                _logger.LogInformation("功能状态已更改: {FeatureName} 从 {OldState} 变为 {NewState}",
                                 key.Substring("Features:".Length),
                                 oldValue,
                                 change.NewValue);

                // 触发功能变更事件
                FeatureChanged?.Invoke(key.Substring("Features:".Length),
                                    Convert.ToBoolean(change.NewValue));
            }
        }
    }

    public event Action<string, bool>? FeatureChanged;
    
    public void Dispose() => _subscription?.Dispose();
}
```

### 使用示例

```csharp
// 在服务中注册动态配置管理器
services.AddSingleton<DynamicConfigurationManager>();

// 在控制器中使用
[ApiController]
[Route("api/features")]
public class FeaturesController : ControllerBase
{
    private readonly DynamicConfigurationManager _featureManager;

    public FeaturesController(DynamicConfigurationManager featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet("{featureName}")]
    public IActionResult GetFeatureStatus(string featureName)
    {
        var isEnabled = _featureManager.IsFeatureEnabled(featureName);
        return Ok(new { Feature = featureName, Enabled = isEnabled });
    }

    [HttpPost("{featureName}")]
    public IActionResult SetFeatureStatus(string featureName, [FromBody] bool enabled)
    {
        // 通过配置API更新功能状态
        // 这将触发配置变更事件，所有订阅者将收到通知
        return Ok($"功能 {featureName} 状态已更新为 {(enabled ? "启用" : "禁用")}");
    }
}
```

## 4. 多租户配置场景

### 场景描述

SaaS 应用程序需要为每个租户提供独立的配置，同时允许租户自定义某些设置。

### 解决方案

```csharp
public class TenantConfigurationProvider
{
    private readonly ICfgRoot _baseCfg;
    private readonly ITenantResolver _tenantResolver;
    private readonly ConcurrentDictionary<string, ICfgRoot> _tenantConfigs = new();
    private readonly ILogger<TenantConfigurationProvider> _logger;

    public TenantConfigurationProvider(
        ICfgRoot baseCfg, 
        ITenantResolver tenantResolver,
        ILogger<TenantConfigurationProvider> logger)
    {
        _baseCfg = baseCfg;
        _tenantResolver = tenantResolver;
        _logger = logger;
    }

    public async Task<ICfgRoot> GetTenantConfigurationAsync(string tenantId)
    {
        // 返回缓存的配置（如果存在）
        if (_tenantConfigs.TryGetValue(tenantId, out var cachedConfig))
            return cachedConfig;

        try
        {
            // 加载租户特定配置
            var tenantConfig = await LoadTenantConfigurationAsync(tenantId);

            // 创建合并配置：基础配置 + 租户特定配置
            var mergedConfig = new CfgBuilder()
                .AddSource(new BaseConfigurationSource(_baseCfg))
                .AddSource(new TenantConfigurationSource(tenantConfig))
                .Build();

            // 缓存合并配置
            _tenantConfigs[tenantId] = mergedConfig;

            return mergedConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载租户 {TenantId} 配置失败", tenantId);

            // 返回仅基础配置作为后备
            return _baseCfg;
        }
    }

    private async Task<ICfgRoot> LoadTenantConfigurationAsync(string tenantId)
    {
        // 从数据库或其他存储加载租户配置
        var connectionString = _baseCfg["ConnectionStrings:TenantDb"];
        
        // 这里可以实现自定义的租户配置源
        var builder = new CfgBuilder()
            .AddSource(new TenantDatabaseCfgSource(
                connectionString: connectionString ?? "",
                tenantId: tenantId,
                level: 5,
                writeable: false));

        return builder.Build();
    }

    public void InvalidateTenantCache(string tenantId)
    {
        _tenantConfigs.TryRemove(tenantId, out _);
        _logger.LogDebug("已清除租户 {TenantId} 的配置缓存", tenantId);
    }
}
```

## 5. 配置中心集成场景

### 场景描述

大型分布式系统需要集中管理配置，支持实时更新、版本控制和访问控制。

### 解决方案

```csharp
public class ConfigurationCenterIntegration
{
    private readonly ICfgRoot _cfg;
    private readonly IConfigurationCenterClient _centerClient;
    private readonly ILogger<ConfigurationCenterIntegration> _logger;
    private readonly Timer _syncTimer;

    public ConfigurationCenterIntegration(
        ICfgRoot cfg, 
        IConfigurationCenterClient centerClient,
        ILogger<ConfigurationCenterIntegration> logger)
    {
        _cfg = cfg;
        _centerClient = centerClient;
        _logger = logger;

        // 设置定期同步（每5分钟）
        _syncTimer = new Timer(SyncFromCenter, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        // 订阅配置中心变更
        _centerClient.ConfigurationChanged += OnCenterConfigurationChanged;
    }

    private async void SyncFromCenter(object? state)
    {
        try
        {
            _logger.LogDebug("开始从配置中心同步配置");

            // 获取配置中心中的所有配置项
            var centerConfigs = await _centerClient.GetAllConfigurationsAsync();

            // 应用到本地配置
            var changes = new Dictionary<string, string?>();
            foreach (var config in centerConfigs)
            {
                var localValue = _cfg.GetValue<string?>(config.Key);

                // 如果本地不存在或值不同，则更新
                if (localValue == null || localValue != config.Value)
                {
                    changes[config.Key] = config.Value;
                }
            }

            if (changes.Count > 0)
            {
                await _cfg.ApplyChangesAsync(changes);
                _logger.LogInformation("已同步 {Count} 项配置更改", changes.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "从配置中心同步配置失败");
        }
    }

    private async void OnCenterConfigurationChanged(object? sender, ConfigurationChangeEventArgs e)
    {
        _logger.LogInformation("收到配置中心变更通知: {Key} = {Value}", e.Key, e.Value);

        // 立即应用变更
        await _cfg.ApplyChangesAsync(new Dictionary<string, string?>
        {
            [e.Key] = e.Value
        });
    }

    public void Dispose()
    {
        _syncTimer?.Dispose();
        _centerClient.ConfigurationChanged -= OnCenterConfigurationChanged;
    }
}
```

### 配置中心客户端实现

```csharp
public interface IConfigurationCenterClient
{
    event EventHandler<ConfigurationChangeEventArgs>? ConfigurationChanged;

    Task<IEnumerable<ConfigurationItem>> GetAllConfigurationsAsync();
    Task<ConfigurationItem?> GetConfigurationAsync(string key);
    Task SetConfigurationAsync(string key, string value);
    Task DeleteConfigurationAsync(string key);
}

public class ApolloConfigurationCenterClient : IConfigurationCenterClient
{
    private readonly ApolloClient _apolloClient;
    private readonly string _appId;
    private readonly string _namespace;

    public event EventHandler<ConfigurationChangeEventArgs>? ConfigurationChanged;

    public ApolloConfigurationCenterClient(ApolloClientOptions options)
    {
        _appId = options.AppId;
        _namespace = options.Namespace ?? "application";

        _apolloClient = new ApolloClient(options);
        _apolloClient.ConfigChanged += OnApolloConfigChanged;
    }

    public async Task<IEnumerable<ConfigurationItem>> GetAllConfigurationsAsync()
    {
        var config = await _apolloClient.GetConfigAsync(_namespace);
        return config.Select(kv => new ConfigurationItem { Key = kv.Key, Value = kv.Value });
    }

    public async Task<ConfigurationItem?> GetConfigurationAsync(string key)
    {
        var config = await _apolloClient.GetConfigAsync(_namespace);
        return config.TryGetValue(key, out var value) 
            ? new ConfigurationItem { Key = key, Value = value } 
            : null;
    }

    public async Task SetConfigurationAsync(string key, string value)
    {
        await _apolloClient.SetConfigAsync(_namespace, key, value);
    }

    public async Task DeleteConfigurationAsync(string key)
    {
        await _apolloClient.DeleteConfigAsync(_namespace, key);
    }

    private void OnApolloConfigChanged(object? sender, ApolloConfigChangeEventArgs e)
    {
        ConfigurationChanged?.Invoke(this, new ConfigurationChangeEventArgs
        {
            Key = e.Key,
            Value = e.NewValue,
            Namespace = e.Namespace,
            ChangeTime = e.ChangeTime
        });
    }
}
```

## 6. 高可用配置场景

### 场景描述

关键业务系统需要配置高可用性，防止单点故障导致系统不可用。

### 解决方案

```csharp
public class HighAvailabilityConfigurationProvider
{
    private readonly List<ICfgRoot> _configSources;
    private readonly ILogger<HighAvailabilityConfigurationProvider> _logger;
    private readonly Timer _healthCheckTimer;
    private volatile int _primaryIndex = 0;

    public HighAvailabilityConfigurationProvider(
        IEnumerable<ICfgRoot> configSources,
        ILogger<HighAvailabilityConfigurationProvider> logger)
    {
        _configSources = configSources.ToList();
        _logger = logger;

        // 设置健康检查（每30秒）
        _healthCheckTimer = new Timer(CheckSourceHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    public string? this[string key]
    {
        get
        {
            var lastException = new List<Exception>();

            // 尝试从每个配置源获取值
            for (int i = 0; i < _configSources.Count; i++)
            {
                try
                {
                    var value = _configSources[i][key];
                    if (value != null)
                    {
                        // 更新主配置源索引（用于下次优先使用）
                        if (i != _primaryIndex)
                        {
                            _logger.LogInformation("切换主配置源从 {OldIndex} 到 {NewIndex}",
                                             _primaryIndex, i);
                            _primaryIndex = i;
                        }

                        return value;
                    }
                }
                catch (Exception ex)
                {
                    lastException.Add(ex);
                    _logger.LogWarning(ex, "从配置源 {Index} 获取配置 {Key} 失败", i, key);
                }
            }

            // 所有配置源都失败
            _logger.LogError("无法从任何配置源获取配置 {Key}，最后错误: {Error}",
                           key, string.Join("; ", lastException.Select(e => e.Message)));
            return null;
        }
    }

    private async void CheckSourceHealth(object? state)
    {
        for (int i = 0; i < _configSources.Count; i++)
        {
            try
            {
                // 尝试访问配置源以检查健康状态
                var testKey = $"health_check_{DateTime.UtcNow:yyyyMMddHHmmss}";
                await _configSources[i].Set(testKey, "test");
                var value = _configSources[i].Get(testKey);

                if (value != "test")
                {
                    throw new InvalidOperationException("健康检查失败");
                }

                _logger.LogDebug("配置源 {Index} 健康检查通过", i);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "配置源 {Index} 健康检查失败", i);

                // 如果是主配置源，切换到下一个可用的
                if (i == _primaryIndex)
                {
                    var nextIndex = FindNextHealthySource(i);
                    if (nextIndex != -1)
                    {
                        _logger.LogWarning("切换主配置源从 {OldIndex} 到 {NewIndex} 由于健康检查失败", 
                                         _primaryIndex, nextIndex);
                        _primaryIndex = nextIndex;
                    }
                }
            }
        }
    }

    private int FindNextHealthySource(int currentIndex)
    {
        for (int i = 0; i < _configSources.Count; i++)
        {
            if (i != currentIndex)
            {
                try
                {
                    // 简单检查配置源是否响应
                    _configSources[i].GetChildKeys();
                    return i;
                }
                catch
                {
                    // 继续检查下一个
                }
            }
        }

        return -1; // 没有健康的配置源
    }
}
```

## 7. 配置加密与脱敏场景

### 场景描述

安全敏感型应用程序需要保护配置中的敏感信息，同时在日志和监控中避免泄露这些信息。

### 解决方案

```csharp
public class SecureConfigurationManager
{
    private readonly ICfgRoot _cfg;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<SecureConfigurationManager> _logger;
    private readonly string[] _sensitiveKeys = { 
        "ConnectionStrings:DefaultConnection:Password",
        "ApiKey:Secret",
        "OAuth:ClientSecret"
    };

    public SecureConfigurationManager(
        ICfgRoot cfg, 
        IEncryptionService encryptionService,
        ILogger<SecureConfigurationManager> logger)
    {
        _cfg = cfg;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    public string? this[string key]
    {
        get
        {
            var value = _cfg[key];

            if (value == null)
                return null;

            // 检查是否为敏感键
            if (IsSensitiveKey(key))
            {
                try
                {
                    // 尝试解密
                    var decryptedValue = _encryptionService.Decrypt(value);
                    _logger.LogDebug("已解密敏感配置 {Key}", key);
                    return decryptedValue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解密敏感配置 {Key} 失败", key);
                throw new SecurityException($"无法解密配置: {key}", ex);
            }
        }

        return value;
    }

    public async Task SetAsync(string key, string? value)
    {
        if (value == null)
        {
            await _cfg.Remove(key);
            return;
        }

        // 检查是否为敏感键
        if (IsSensitiveKey(key))
        {
            // 加密敏感值
            var encryptedValue = _encryptionService.Encrypt(value);
            await _cfg.SetValue(key, encryptedValue);
            _logger.LogDebug("已加密并保存敏感配置 {Key}", key);
        }
        else
        {
            await _cfg.SetValue(key, value);
        }
    }

    public string GetSafeForLogging(string key)
    {
        var value = Get(key);

        // 对敏感值进行脱敏处理
        if (value != null && IsSensitiveKey(key))
        {
            return "***REDACTED***";
        }

        return value ?? string.Empty;
    }

    private bool IsSensitiveKey(string key)
    {
        return _sensitiveKeys.Any(sensitiveKey => 
            key.Equals(sensitiveKey, StringComparison.OrdinalIgnoreCase));
    }
}
```

## 8. 配置验证与合规场景

### 场景描述

受监管行业（如金融、医疗）的应用程序需要确保配置符合特定法规要求，并进行验证。

### 解决方案

```csharp
public class ComplianceConfigurationManager
{
    private readonly ICfgRoot _cfg;
    private readonly IComplianceChecker _complianceChecker;
    private readonly ILogger<ComplianceConfigurationManager> _logger;

    public ComplianceConfigurationManager(
        ICfgRoot cfg,
        IComplianceChecker complianceChecker,
        ILogger<ComplianceConfigurationManager> logger)
    {
        _cfg = cfg;
        _complianceChecker = complianceChecker;
        _logger = logger;
    }

    public async Task<bool> ValidateConfigurationAsync()
    {
        _logger.LogInformation("开始配置合规性检查");

        var violations = new List<ComplianceViolation>();

        // 检查密码复杂度要求
        var password = _cfg.GetValue<string>("Security:PasswordPolicy");
        if (!string.IsNullOrEmpty(password))
        {
            var passwordViolation = await _complianceChecker.CheckPasswordComplianceAsync(password);
            if (!passwordViolation.IsCompliant)
            {
                violations.AddRange(passwordViolation.Violations);
            }
        }

        // 检查数据加密要求
        var encryptionEnabled = _cfg.GetValue<bool>("Security:EncryptionEnabled");
        if (!encryptionEnabled)
        {
            violations.Add(new ComplianceViolation
            {
                Code = "ENC001",
                Description = "静态数据加密必须启用",
                Severity = ViolationSeverity.High
            });
        }

        // 检查审计日志要求
        var auditLogEnabled = _cfg.GetValue<bool>("Audit:Enabled");
        var auditLogRetentionDays = _cfg.GetValue<int>("Audit:RetentionDays");
        if (!auditLogEnabled || auditLogRetentionDays < 365)
        {
            violations.Add(new ComplianceViolation
            {
                Code = "AUD001",
                Description = "审计日志必须启用并保留至少365天",
                Severity = ViolationSeverity.Medium
            });
        }

        // 记录违规项
        if (violations.Count > 0)
        {
            foreach (var violation in violations)
            {
                _logger.LogWarning("配置合规性违规: {Code} - {Description}", 
                               violation.Code, violation.Description);
            }

            // 发送到合规监控系统
            await _complianceChecker.ReportViolationsAsync(violations);

            return false;
        }

        _logger.LogInformation("配置合规性检查通过");
        return true;
    }

    public async Task ApplyComplianceDefaultsAsync()
    {
        _logger.LogInformation("应用合规性默认配置");

        var complianceDefaults = await _complianceChecker.GetComplianceDefaultsAsync();

        foreach (var kvp in complianceDefaults)
        {
            // 只设置当前不存在的配置项
            if (!_cfg.Exists(kvp.Key))
            {
                await _cfg.SetValue(kvp.Key, kvp.Value);
                _logger.LogDebug("已应用合规性默认配置: {Key} = {Value}", kvp.Key, kvp.Value);
            }
        }
    }
}
```

## 总结

以上场景展示了 Apq.Cfg 在复杂环境中的应用方式，包括：

| 场景 | 关键特性 |
|------|---------|
| 多环境部署 | 层级配置、环境变量覆盖 |
| 配置迁移 | 版本控制、结构转换 |
| 动态更新 | 变更订阅、缓存刷新 |
| 多租户 | 配置隔离、按需加载 |
| 配置中心 | 远程同步、实时推送 |
| 高可用 | 故障转移、健康检查 |
| 加密脱敏 | 敏感数据保护、日志安全 |
| 合规验证 | 规则检查、默认值应用 |

这些示例可以作为您实际项目中的参考，根据具体需求进行调整和扩展。
