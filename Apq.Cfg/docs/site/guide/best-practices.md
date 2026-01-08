# 最佳实践

本指南提供了使用 Apq.Cfg 进行配置管理的最佳实践，帮助开发者构建更加健壮、安全和可维护的配置系统。

## 1. 配置层级设计

### 1.1 推荐层级规划

| 层级范围 | 用途 | 示例 |
|----------|------|------|
| 0-2 | 基础配置 | config.json |
| 3-5 | 环境配置 | config.Production.json |
| 6-9 | 本地覆盖 | config.local.json |
| 10-19 | 远程配置 | Consul, Nacos, Apollo |
| 20+ | 环境变量 | 最高优先级覆盖 |

### 1.2 层级原则

配置层级是 Apq.Cfg 的核心概念，数值越大优先级越高。建议遵循以下层级设计原则：

- **0-2层**：系统默认值和基础配置，通常包含在应用程序包中
- **3-5层**：环境特定配置，如开发、测试、预发布环境
- **6-8层**：租户/用户特定配置，允许覆盖环境配置
- **9-10层**：运行时动态配置，具有最高优先级

### 1.2 层级示例

```csharp
var cfg = new CfgBuilder()
    // 0层：系统默认值
    .AddJsonFile("config.json", level: 0, writeable: false)

    // 1层：环境特定默认值
    .AddJsonFile($"config.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", 
              level: 1, writeable: false)

    // 5层：环境变量
    .AddEnvironmentVariables(level: 5, prefix: "APP_")

    // 8层：用户特定配置（可选）
    .AddJsonFile("user.config.json", level: 8, writeable: true, isPrimaryWriter: true)

    .Build();
```

## 2. 安全配置管理

### 2.1 敏感数据处理

避免将敏感信息（如密码、API密钥）直接存储在配置文件中：

```csharp
// 推荐：使用外部配置中心
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddSource(new VaultCfgSource("secret/", level: 5, token: vaultToken)) // 从 HashiCorp Vault 加载
    .Build();

// 避免：直接在配置文件中存储敏感信息
// {
//   "ConnectionStrings": {
//     "DefaultConnection": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;password=myPassword;"
//   }
// }
```

### 2.2 配置加密

对需要本地存储的敏感配置进行加密：

```csharp
// 使用自定义加密配置源
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddSource(new EncryptedJsonCfgSource("config.encrypted.json", level: 2,
        writeable: false, encryptionKey: encryptionKey))
    .Build();
```

### 2.3 访问控制

实施最小权限原则，限制配置源的访问权限：

```csharp
// 生产环境配置文件权限设置
// - config.json: 只读 (应用程序服务账户)
// - config.Production.json: 只读 (配置管理服务账户)
// - config.Development.json: 读写 (开发者账户)
```

## 3. 环境隔离

### 3.1 环境特定配置

为不同环境维护独立的配置文件：

```
/config
    ├── config.json              # 基础配置
    ├── config.Development.json  # 开发环境覆盖
    ├── config.Staging.json      # 预发布环境覆盖
    └── config.Production.json   # 生产环境覆盖
```

### 3.2 环境变量约定

建立清晰的环境变量命名约定：

```csharp
// 推荐：使用层次化命名
var cfg = new CfgBuilder()
    .AddEnvironmentVariables(level: 5, prefix: "APP_")
    // APP_ConnectionStrings__DefaultConnection
    // APP_Logging__LogLevel__Default
    // APP_Caching__Redis__ConnectionString
    .Build();

// 避免：扁平化命名
// APP_CONNECTIONSTRING
// APP_LOGLEVEL
// APP_REDIS_CONNECTION
```

## 4. 配置验证

### 4.1 启动时验证

在应用程序启动时验证关键配置：

```csharp
public class AppSettings
{
    public string ConnectionString { get; set; }
    public int RetryCount { get; set; }
    public TimeSpan Timeout { get; set; }

    [ValidateComplexType]
    public class ValidateComplexType : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is AppSettings settings)
            {
                // 验证连接字符串格式
                if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                    return false;

                // 验证重试次数范围
                if (settings.RetryCount < 0 || settings.RetryCount > 10)
                    return false;

                // 验证超时时间范围
                if (settings.Timeout < TimeSpan.FromSeconds(1) || settings.Timeout > TimeSpan.FromMinutes(5))
                    return false;

                return true;
            }
            return false;
        }
    }
}

// 使用
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .Build();

var settings = ObjectBinder.Bind<AppSettings>(cfg.GetSection("App"));
Validator.ValidateObject(settings, new ValidationContext(settings), validateAllProperties: true);
```

### 4.2 配置健康检查

实施配置健康检查机制：

```csharp
// 定期检查关键配置的有效性
public class ConfigurationHealthCheck : IHealthCheck
{
    private readonly ICfgRoot _cfg;

    public ConfigurationHealthCheck(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 验证数据库连接
            var connectionString = _cfg.GetValue<string>("ConnectionStrings:DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("数据库连接失败", ex);
        }
    }
}
```

## 5. 配置变更管理

### 5.1 配置版本控制

对配置文件实施版本控制：

```json
{
  "Version": "1.2.0",
  "Info": {
    "Description": "应用程序主配置文件",
    "LastModified": "2023-12-01T10:30:00Z",
    "ModifiedBy": "admin@example.com"
  },
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }
}
```

### 5.2 配置变更审计

记录配置变更历史：

```csharp
public class ConfigurationAuditService
{
    private readonly ICfgRoot _cfg;
    private readonly IAuditLogger _auditLogger;

    public ConfigurationAuditService(ICfgRoot cfg, IAuditLogger auditLogger)
    {
        _cfg = cfg;
        _auditLogger = auditLogger;

        // 订阅配置变更事件
        _cfg.ConfigChanges.Subscribe(OnConfigurationChanged);
    }

    private void OnConfigurationChanged(ConfigChangeEvent change)
    {
        _auditLogger.LogInformation("配置已更改: {Key}={Key}, OldValue={OldValue}, NewValue={NewValue}, ChangedAt={ChangedAt}",
            change.Key, 
            change.OldValue, 
            change.NewValue, 
            DateTime.UtcNow);
    }
}
```

## 6. 性能优化

### 6.1 配置缓存

缓存频繁访问的配置值：

```csharp
public class CachedConfigurationService
{
    private readonly ICfgRoot _cfg;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public CachedConfigurationService(ICfgRoot cfg, IMemoryCache cache)
    {
        _cfg = cfg;
        _cache = cache;
    }

    public T Get<T>(string key)
    {
        var cacheKey = $"cfg_{key}";

        return _cache.GetOrCreate(cacheKey, () => _cfg.GetValue<T>(key), _cacheDuration);
    }
}
```

### 6.2 批量操作

使用批量操作减少配置源访问：

```csharp
// 推荐：使用批量获取
var keys = new[] { "FeatureA:Enabled", "FeatureB:Enabled", "FeatureC:Enabled" };
var features = cfg.GetMany(keys);

// 避免：多次单独获取
var featureA = cfg.GetValue<bool>("FeatureA:Enabled");
var featureB = cfg.GetValue<bool>("FeatureB:Enabled");
var featureC = cfg.GetValue<bool>("FeatureC:Enabled");
```

## 7. 监控和诊断

### 7.1 配置使用监控

监控配置访问模式：

```csharp
public class ConfigurationMetrics
{
    private readonly IMetrics _metrics;
    private readonly ConcurrentDictionary<string, int> _accessCounts = new();

    public void RecordAccess(string key)
    {
        _accessCounts.AddOrUpdate(key, 1, (_, count) => count + 1);
        _metrics.Counter("configuration.access.count").Inc();
        _metrics.Histogram("configuration.access.frequency").Observe(_accessCounts.Count);
    }

    public void ReportTopAccessedKeys()
    {
        var topKeys = _accessCounts.OrderByDescending(kvp => kvp.Value)
                              .Take(10)
                              .Select(kvp => kvp.Key);

        _metrics.Gauge("configuration.top_keys", topKeys);
    }
}
```

### 7.2 配置加载诊断

诊断配置加载问题：

```csharp
public class ConfigurationDiagnostics
{
    public static void LogConfigurationSources(ICfgRoot cfg)
    {
        var sb = new StringBuilder();
        sb.AppendLine("配置源加载情况:");

        // 这里需要访问内部实现，实际项目中可能需要添加诊断API
        foreach (var source in cfg.GetSources())
        {
            sb.AppendLine($"- {source.GetType().Name}: Level={source.Level}, Writeable={source.IsWriteable}");
        }

        // 记录到日志或诊断系统
        Console.WriteLine(sb.ToString());
    }
}
```

## 8. 故障排除

### 8.1 常见问题

1. **配置未生效**
   - 检查配置层级是否正确
   - 确认配置键名拼写正确
   - 验证配置源是否成功加载

2. **配置值类型转换错误**
   - 检查目标类型是否匹配
   - 确认字符串格式正确
   - 使用 TryGet 方法避免异常

3. **配置热重载不工作**
   - 确认配置源支持热重载
   - 检查文件权限
   - 验证文件监控机制

### 8.2 调试技巧

1. **启用详细日志**
   ```csharp
   var cfg = new CfgBuilder()
       .AddJsonFile("config.json", level: 0, writeable: false)
       .AddEnvironmentVariables(level: 5, prefix: "APP_")
       .WithEncodingDetectionLogging(result => Console.WriteLine($"编码检测: {result}"))
       .Build();
   ```

2. **导出配置快照**
   ```csharp
   // 导出当前配置快照用于调试
   var snapshot = cfg.ExportSnapshot();
   File.WriteAllText("debug-config.json", snapshot);
   ```

3. **配置诊断工具**
   ```csharp
   // 创建诊断工具
   var diagnostics = new ConfigurationDiagnostics(cfg);
   diagnostics.ValidateRequiredKeys();
   diagnostics.ReportConflictingValues();
   diagnostics.CheckForDeprecatedSettings();
   ```

## 下一步

- [扩展开发](/guide/extension) - 自定义配置源开发
- [配置源](/config-sources/) - 了解所有配置源
