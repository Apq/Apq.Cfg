# 最佳实践

本指南总结了使用 Apq.Cfg 的最佳实践。

## 配置组织

### 分层配置

```
config/
├── appsettings.json           # 基础配置
├── appsettings.Development.json  # 开发环境
├── appsettings.Staging.json      # 预发布环境
├── appsettings.Production.json   # 生产环境
└── appsettings.local.json        # 本地覆盖（gitignore）
```

### 配置加载顺序

```csharp
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
```

### 配置分组

```json
{
  "App": {
    "Name": "MyApp",
    "Version": "1.0.0"
  },
  "Database": {
    "Primary": { ... },
    "Readonly": { ... }
  },
  "Cache": { ... },
  "Logging": { ... },
  "Features": { ... }
}
```

## 类型安全

### 使用强类型配置

```csharp
// ✅ 推荐：强类型
public class DatabaseOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
}

var dbOptions = cfg.GetSection("Database").Get<DatabaseOptions>();

// ❌ 避免：魔法字符串
var host = cfg["Database:Host"];
var port = int.Parse(cfg["Database:Port"]);
```

### 配置验证

```csharp
public class DatabaseOptions
{
    [Required]
    [MinLength(1)]
    public string Host { get; set; } = "";
    
    [Range(1, 65535)]
    public int Port { get; set; } = 5432;
    
    [Required]
    public string ConnectionString { get; set; } = "";
}

// 启动时验证
services.AddOptions<DatabaseOptions>()
    .Bind(configuration.GetSection("Database"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### 使用源生成器

```csharp
[CfgSection("Database")]
public partial class DatabaseConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
}

// 自动生成绑定代码
var config = DatabaseConfig.FromConfiguration(cfg);
```

## 安全实践

### 敏感信息处理

```csharp
// ❌ 避免：配置文件中存储密码
{
  "Database": {
    "Password": "secret123"
  }
}

// ✅ 推荐：使用环境变量或密钥管理
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables("MYAPP_")  // MYAPP_Database__Password
    .AddVault("https://vault.example.com", "secret/myapp")
    .Build();
```

### 配置加密

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .ConfigureEncryption(options =>
    {
        options.EncryptedKeys = new[] { "Database:Password", "Api:Secret" };
        options.DecryptionKey = Environment.GetEnvironmentVariable("CONFIG_KEY");
    })
    .Build();
```

### 日志脱敏

```csharp
// 配置日志时排除敏感字段
services.AddOptions<DatabaseOptions>()
    .Bind(configuration.GetSection("Database"))
    .Configure(options =>
    {
        // 日志中不显示密码
        options.LoggingExcludes = new[] { "Password", "ConnectionString" };
    });
```

## 错误处理

### 必需配置检查

```csharp
var connectionString = cfg["Database:ConnectionString"];
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("数据库连接字符串未配置");
}

// 或使用 GetRequiredSection
var dbSection = cfg.GetRequiredSection("Database");
```

### 默认值处理

```csharp
// 提供合理的默认值
public class CacheOptions
{
    public bool Enabled { get; set; } = true;
    public int MaxSize { get; set; } = 1000;
    public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(5);
}
```

### 配置回退

```csharp
var cfg = new CfgBuilder()
    .AddConsul("http://consul:8500", "myapp/config")
    .AddJsonFile("config.fallback.json", fallback: true)
    .Build();
```

## 测试实践

### 配置模拟

```csharp
[Fact]
public void TestWithMockConfig()
{
    var cfg = new CfgBuilder()
        .AddInMemory(new Dictionary<string, string>
        {
            ["Database:Host"] = "localhost",
            ["Database:Port"] = "5432"
        })
        .Build();
    
    var service = new MyService(cfg);
    // 测试...
}
```

### 配置快照

```csharp
[Fact]
public void TestWithOptionsSnapshot()
{
    var options = Options.Create(new DatabaseOptions
    {
        Host = "localhost",
        Port = 5432
    });
    
    var service = new MyService(options);
    // 测试...
}
```

## 运维实践

### 配置版本控制

```json
{
  "_version": "1.2.0",
  "_lastModified": "2024-01-15T10:30:00Z",
  "Database": { ... }
}
```

### 配置审计

```csharp
cfg.ConfigChanged += (sender, e) =>
{
    foreach (var change in e.Changes)
    {
        _auditLogger.Log(
            $"配置变更: {change.Key} [{change.Type}] " +
            $"由 {e.Source} 在 {e.Timestamp} 触发");
    }
};
```

### 健康检查

```csharp
services.AddHealthChecks()
    .AddCheck("config", () =>
    {
        try
        {
            var required = cfg["Database:ConnectionString"];
            return string.IsNullOrEmpty(required)
                ? HealthCheckResult.Unhealthy("缺少必需配置")
                : HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    });
```

## 下一步

- [扩展开发](/guide/extension) - 自定义配置源开发
- [配置源](/config-sources/) - 了解所有配置源
