# Examples Overview

This section provides various usage examples for Apq.Cfg.

## Default Levels

Each configuration source has a default level. If not specified, the default level is used:

| Source Type | Default Level |
|-------------|---------------|
| Json, Ini, Xml, Yaml, Toml | 0 |
| Redis, Database | 100 |
| Consul, Etcd, Nacos, Apollo, Zookeeper | 200 |
| Vault | 300 |
| .env, EnvironmentVariables | 400 |

## Example Categories

### Basic Examples

- [Basic Examples](/en/examples/basic) - Basic configuration reading and type conversion
- [Multi-Source](/en/examples/multi-source) - Combining multiple configuration sources

### Integration Examples

- [DI Integration](/en/examples/di-integration) - ASP.NET Core integration
- [Dynamic Reload](/en/examples/dynamic-reload) - Configuration hot reload

### Advanced Examples

- [Complex Scenarios](/en/examples/complex-scenarios) - Enterprise application configuration
- [Encryption & Masking](/en/guide/encryption-masking) - Configuration encryption and masking

## Quick Examples

### Simplest Usage

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("config.json")  // Uses default level 0
    .Build();

var appName = cfg.Get("App:Name");
Console.WriteLine($"App Name: {appName}");
```

### Multi-Source

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")                        // Uses default level 0
    .AddYaml("config.yaml", level: 10, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")       // Uses default level 400
    .Build();
```

### Strongly-Typed Binding

```csharp
public class DatabaseConfig
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
}

var dbSection = cfg.GetSection("Database");
var dbConfig = new DatabaseConfig
{
    Host = dbSection.Get("Host") ?? "localhost",
    Port = dbSection.Get<int>("Port")
};
Console.WriteLine($"Database: {dbConfig.Host}:{dbConfig.Port}");
```

### Dependency Injection

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApqCfg(cfg => cfg
    .AddJson("config.json")                        // Uses default level 0
    .AddEnvironmentVariables(prefix: "APP_"));    // Uses default level 400

builder.Services.ConfigureApqCfg<DatabaseConfig>("Database");
```

### Dynamic Reload

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", reloadOnChange: true)  // Uses default level 0
    .Build();

cfg.ConfigChanges.Subscribe(e =>
{
    Console.WriteLine("Configuration updated!");
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"  [{change.Type}] {key}");
    }
});
```

### Encryption & Masking

```csharp
using Apq.Cfg.Crypto;

var cfg = new CfgBuilder()
    .AddJson("config.json")            // Uses default level 0
    .AddAesGcmEncryptionFromEnv()      // Read key from environment variable
    .AddSensitiveMasking()             // Add masking support
    .Build();

// Encrypted value in config: { "Database": { "Password": "{ENC}base64..." } }
// Auto-decrypt on read
var password = cfg.Get("Database:Password");

// Use masked value for logging
Console.WriteLine($"Password: {cfg.GetMasked("Database:Password")}");
// Output: Password: myS***ord
```

### Writable Configuration

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", writeable: true, isPrimaryWriter: true)  // Uses default level 0
    .Build();

// Modify configuration
cfg.Set("App:Name", "NewName");
cfg.Set("Database:Port", "5433");

// Save to file
await cfg.SaveAsync();
```

## Run Sample Project

The repository includes complete sample code:

```bash
cd Samples/Apq.Cfg.Samples
dotnet run
```

## Next Steps

Choose an example to learn more:

- [Basic Examples](/en/examples/basic)
- [Multi-Source](/en/examples/multi-source)
- [DI Integration](/en/examples/di-integration)
