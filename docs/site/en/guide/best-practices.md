# Best Practices

This guide covers best practices for using Apq.Cfg effectively.

## Configuration Organization

### Recommended Level Planning

| Level Range | Purpose | Example |
|-------------|---------|---------|
| 0-2 | Base configuration | config.json |
| 3-5 | Environment config | config.Production.json |
| 6-9 | Local overrides | config.local.json |
| 10-19 | Remote config | Consul, Nacos, Apollo |
| 20+ | Environment variables | Highest priority |

### Use Meaningful Levels

```csharp
// Level 0: Base defaults
// Level 1-9: Environment-specific
// Level 10-19: Remote configuration
// Level 20+: Overrides

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddJson($"config.{env}.json", level: 1, optional: true)
    .AddConsul(options => { }, level: 10)
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
    .Build();
```

### Separate Concerns

```
config/
├── config.json           # Application settings
├── config.Development.json
├── config.Production.json
├── logging.json          # Logging configuration
└── features.json         # Feature flags
```

## Security

### Never Commit Secrets

```csharp
// ❌ Wrong
.AddJson("secrets.json", level: 5)  // Don't commit this file

// ✅ Correct
.AddVault(options => { }, level: 15)
.AddEnvironmentVariables(level: 20, prefix: "SECRET_")
```

### Use Encryption

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv()
    .Build();
```

### Mask Sensitive Values in Logs

```csharp
// ❌ Wrong
logger.LogInfo($"Password: {cfg["Database:Password"]}");

// ✅ Correct
logger.LogInfo($"Password: {cfg.GetMasked("Database:Password")}");
```

## Error Handling

### Handle Missing Configuration

```csharp
var timeout = cfg.GetValue<int?>("Service:Timeout") ?? 30;
```

### Validate Required Configuration

```csharp
var connectionString = cfg["Database:ConnectionString"]
    ?? throw new InvalidOperationException("Database connection string is required");
```

## Hot Reload

### Keep Handlers Fast

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    // ✅ Fast: Just invalidate cache
    _cache.Clear();

    // ❌ Slow: Don't do heavy work here
    // RebuildEntireApplication();
});
```

### Handle Exceptions

```csharp
cfg.ConfigChanges.Subscribe(e =>
{
    try
    {
        RefreshConfiguration();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to refresh configuration");
    }
});
```

## Testing

### Use In-Memory Configuration

```csharp
var testConfig = new Dictionary<string, string?>
{
    ["App:Name"] = "TestApp",
    ["App:Port"] = "8080"
};

var cfg = new CfgBuilder()
    .AddInMemory(testConfig, level: 0)
    .Build();
```

### Mock ICfgRoot

```csharp
var mockCfg = new Mock<ICfgRoot>();
mockCfg.Setup(c => c["App:Name"]).Returns("TestApp");
```

## Next Steps

- [Architecture](/en/guide/architecture) - Internal architecture
- [Extension Development](/en/guide/extension) - Create custom sources
