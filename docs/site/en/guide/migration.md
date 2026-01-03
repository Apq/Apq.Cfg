# Migrating from Microsoft.Extensions.Configuration

This guide helps you migrate from `Microsoft.Extensions.Configuration` to Apq.Cfg.

## Why Migrate?

| Feature | Microsoft.Extensions.Configuration | Apq.Cfg |
|---------|-----------------------------------|---------|
| Config Templates/Variable Substitution | ❌ Not supported | ✅ Supported |
| Config Encryption & Masking | ❌ Requires third-party | ✅ Built-in |
| Config Validation | ⚠️ Requires extra setup | ✅ Built-in |
| Config Snapshot Export | ❌ Not supported | ✅ Supported |
| Batch Operations | ❌ Not supported | ✅ Supported (zero allocation) |
| Remote Config Centers | ⚠️ Requires third-party | ✅ Built-in 6+ centers |
| Writable Config | ❌ Read-only | ✅ Write and persist |

## 1. Replace ConfigurationBuilder

### Basic Configuration

```csharp
// Before (Microsoft.Extensions.Configuration)
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// After (Apq.Cfg)
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson($"config.{env}.json", level: 1, optional: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();
```

### Configuration with Hot Reload

```csharp
// Before
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", reloadOnChange: true)
    .Build();

// After
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, reloadOnChange: true)
    .Build();
```

## 2. Replace Configuration Reading

### Basic Reading

```csharp
// Before
var value = config["Section:Key"];
var typedValue = config.GetValue<int>("Section:Key");
var section = config.GetSection("Section");

// After
var value = cfg["Section:Key"];
var typedValue = cfg.GetValue<int>("Section:Key");
var section = cfg.GetSection("Section");
```

### Reading with Default Value

```csharp
// Before
var port = config.GetValue<int>("Server:Port", 8080);

// After
var port = cfg.GetValue("Server:Port", 8080);
```

### Safe Reading (TryGetValue)

```csharp
// Before
var value = config["Key"];
if (value != null)
{
    // use value
}

// After
if (cfg.TryGetValue<int>("Key", out var value))
{
    // use value
}
```

## 3. Replace Dependency Injection

### Register Configuration

```csharp
// Before
services.AddSingleton<IConfiguration>(config);
services.Configure<DatabaseOptions>(config.GetSection("Database"));

// After
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_"));

services.ConfigureApqCfg<DatabaseOptions>("Database");
```

### Using IOptions

```csharp
// Same for both
public class MyService
{
    private readonly DatabaseOptions _options;

    public MyService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
}
```

## 4. Replace Configuration Binding

### Binding to Objects

```csharp
// Before
var options = new DatabaseOptions();
config.GetSection("Database").Bind(options);

// After (using source generator, zero reflection)
[CfgSection("Database")]
public partial class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
}

var options = DatabaseOptions.BindFrom(cfg.GetSection("Database"));
```

## 5. New Features

After migration, you can use Apq.Cfg's unique features:

### Config Templates

```csharp
// config.json: { "App:Name": "MyApp", "App:LogPath": "${App:Name}/logs" }
var logPath = cfg.GetResolved("App:LogPath");
// Returns: "MyApp/logs"
```

### Config Encryption

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true)
    .AddAesGcmEncryptionFromEnv()
    .Build();

// Auto-decrypt {ENC} prefixed values
var password = cfg["Database:Password"];
```

### Config Validation

```csharp
var (cfg, result) = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Server:Port", 1, 65535))
    .BuildAndValidate();
```

### Writable Configuration

```csharp
cfg["App:LastRun"] = DateTime.Now.ToString();
await cfg.SaveAsync();
```

### Batch Operations

```csharp
// High-performance batch reading (zero allocation)
cfg.GetMany(new[] { "Key1", "Key2", "Key3" }, (key, value) =>
{
    Console.WriteLine($"{key}: {value}");
});
```

## 6. API Comparison Table

| Microsoft.Extensions.Configuration | Apq.Cfg | Notes |
|-----------------------------------|---------|-------|
| `config["Key"]` | `cfg["Key"]` | Same |
| `config.GetValue<T>("Key")` | `cfg.GetValue<T>("Key")` | Shorter method name |
| `config.GetValue<T>("Key", default)` | `cfg.GetValue("Key", default)` | Same |
| `config.GetSection("Path")` | `cfg.GetSection("Path")` | Same |
| `config.GetChildren()` | `cfg.GetChildKeys()` | Returns key names |
| `config.GetReloadToken()` | `cfg.ConfigChanges` | Rx subscription |
| N/A | `cfg.GetResolved("Key")` | Variable substitution |
| N/A | `cfg.SetValue("Key", "Value")` | Writable config |
| N/A | `cfg.SaveAsync()` | Persistence |
| N/A | `cfg.GetMasked("Key")` | Masked output |
| N/A | `cfg.ExportSnapshot()` | Snapshot export |

## 7. Important Notes

### Config File Naming

Apq.Cfg recommends using `config.json` instead of `appsettings.json`:

```csharp
// Recommended
.AddJson("config.json", level: 0)
.AddJson("config.local.json", level: 1)

// Not recommended
.AddJson("appsettings.json", level: 0)
```

### Level Design

Apq.Cfg uses the `level` parameter to control configuration priority. Higher values have higher priority:

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)           // Base config
    .AddJson("config.local.json", level: 1)     // Local override
    .AddEnvironmentVariables(level: 2)          // Env vars highest
    .Build();
```

### Resource Disposal

Apq.Cfg's `ICfgRoot` implements `IDisposable` and `IAsyncDisposable`:

```csharp
// Recommended: use using
using var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .Build();

// Or let DI manage the lifecycle
services.AddApqCfg(cfg => cfg.AddJson("config.json", level: 0));
```

## Next Steps

- [Basic Usage](/en/guide/basic-usage) - Learn more basic features
- [Config Templates](/en/guide/template) - Use variable substitution
- [Encryption & Masking](/en/guide/encryption-masking) - Protect sensitive configs
- [Best Practices](/en/guide/best-practices) - Configuration design tips
