# Complex Scenarios

Enterprise application configuration patterns.

## Microservices Configuration

```csharp
var cfg = new CfgBuilder()
    // Base configuration (uses default level 0)
    .AddJsonFile("config.json")

    // Service discovery (uses default level 200)
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = $"services/{serviceName}/config/";
    }, reloadOnChange: true)

    // Shared configuration (custom level 150)
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "shared/config/";
    }, level: 150, reloadOnChange: true)

    // Secrets (uses default level 300)
    .AddVault(options =>
    {
        options.Address = "http://vault:8200";
        options.SecretPath = $"secret/services/{serviceName}";
    })

    // Environment overrides (uses default level 400)
    .AddEnvironmentVariables(prefix: "SVC_")
    .Build();
```

## Feature Flags

```csharp
public class FeatureService
{
    private readonly ICfgRoot _cfg;

    public FeatureService(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    public bool IsEnabled(string feature)
    {
        return _cfg.GetValue<bool>($"Features:{feature}:Enabled");
    }

    public T GetFeatureConfig<T>(string feature) where T : new()
    {
        var section = _cfg.GetSection($"Features:{feature}");
        // Bind to T
        return new T();
    }
}
```

## Multi-Tenant Configuration

```csharp
public class TenantConfigService
{
    private readonly ICfgRoot _cfg;

    public TenantConfigService(ICfgRoot cfg)
    {
        _cfg = cfg;
    }

    public string GetTenantSetting(string tenantId, string key)
    {
        // Try tenant-specific first
        var tenantValue = _cfg[$"Tenants:{tenantId}:{key}"];
        if (tenantValue != null)
            return tenantValue;

        // Fall back to default
        return _cfg[$"Defaults:{key}"] ?? "";
    }
}
```

## Next Steps

- [Best Practices](/en/guide/best-practices) - Configuration best practices
- [Architecture](/en/guide/architecture) - Internal architecture
