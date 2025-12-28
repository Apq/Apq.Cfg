# 最佳实践

本节汇总了使用 Apq.Cfg 的最佳实践和建议。

## 配置管理原则

### 1. 分层配置

将配置按环境和用途分层：

```
config/
├── appsettings.json          # 基础配置
├── appsettings.Development.json  # 开发环境
├── appsettings.Staging.json      # 预发布环境
└── appsettings.Production.json   # 生产环境
```

```csharp
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var cfg = new CfgBuilder()
    .AddJsonFile("config/appsettings.json")
    .AddJsonFile($"config/appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();
```

### 2. 敏感信息管理

- **不要**将敏感信息提交到代码仓库
- 使用环境变量或密钥管理服务
- 生产环境使用 Vault 等专业工具

```csharp
// 开发环境：使用本地文件
// 生产环境：使用 Vault
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddVault(options => {
        options.Address = "https://vault.example.com";
        options.Token = Environment.GetEnvironmentVariable("VAULT_TOKEN");
    })
    .Build();
```

### 3. 配置验证

在应用启动时验证配置：

```csharp
var settings = cfg.Bind<DatabaseSettings>();

if (string.IsNullOrEmpty(settings.ConnectionString))
{
    throw new InvalidOperationException("数据库连接字符串未配置");
}

if (settings.Timeout <= 0)
{
    throw new InvalidOperationException("数据库超时时间必须大于 0");
}
```

### 4. 使用强类型配置

避免使用魔法字符串：

```csharp
// ❌ 不推荐
var connStr = cfg.Get<string>("Database:ConnectionString");

// ✅ 推荐
var dbSettings = cfg.GetSection("Database").Bind<DatabaseSettings>();
var connStr = dbSettings.ConnectionString;
```

## 性能优化

### 1. 缓存配置对象

```csharp
// ❌ 每次都绑定
public void DoSomething()
{
    var settings = _cfg.Bind<AppSettings>(); // 每次调用都会创建新对象
}

// ✅ 缓存绑定结果
private readonly AppSettings _settings;

public MyService(IOptions<AppSettings> options)
{
    _settings = options.Value; // 只绑定一次
}
```

### 2. 使用源代码生成器

```csharp
// 使用 CfgSectionAttribute 启用编译时绑定
[CfgSection("App")]
public partial class AppSettings
{
    public string Name { get; set; }
    public int MaxRetries { get; set; }
}
```

## 更多内容

- [配置源选择指南](/best-practices/source-selection)
- [安全性最佳实践](/best-practices/security)
- [生产环境部署](/best-practices/production)
