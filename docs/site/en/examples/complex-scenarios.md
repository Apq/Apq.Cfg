# Complex Scenarios

Enterprise application configuration patterns.

## Microservices Configuration

```csharp
var cfg = new CfgBuilder()
    // Base configuration
    .AddJson("config.json", level: 0, writeable: false)

    // Service discovery
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = $"services/{serviceName}/config/";
    }, level: 10, reloadOnChange: true)

    // Shared configuration
    .AddConsul(options =>
    {
        options.Address = "http://consul:8500";
        options.KeyPrefix = "shared/config/";
    }, level: 5, reloadOnChange: true)

    // Secrets
    .AddVault(options =>
    {
        options.Address = "http://vault:8200";
        options.SecretPath = $"secret/services/{serviceName}";
    }, level: 15)

    // Environment overrides
    .AddEnvironmentVariables(level: 20, prefix: "SVC_")
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
        return _cfg.Get<bool>($"Features:{feature}:Enabled");
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
        var tenantValue = _cfg.Get($"Tenants:{tenantId}:{key}");
        if (tenantValue != null)
            return tenantValue;

        // Fall back to default
        return _cfg.Get($"Defaults:{key}") ?? "";
    }
}
```

## Next Steps

- [Best Practices](/en/guide/best-practices) - Configuration best practices
- [Architecture](/en/guide/architecture) - Internal architecture
