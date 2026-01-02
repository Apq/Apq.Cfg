# Extension Development

Learn how to create custom configuration sources for Apq.Cfg.

## Implement ICfgSource

```csharp
public class MyCustomSource : ICfgSource
{
    public int Level { get; }
    public bool IsWriteable => false;
    public bool IsPrimaryWriter => false;

    public MyCustomSource(int level)
    {
        Level = level;
    }

    public IConfigurationSource BuildSource()
    {
        return new MyCustomConfigurationSource(this);
    }
}
```

## Implement IConfigurationSource

```csharp
public class MyCustomConfigurationSource : IConfigurationSource
{
    private readonly MyCustomSource _source;

    public MyCustomConfigurationSource(MyCustomSource source)
    {
        _source = source;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MyCustomConfigurationProvider(_source);
    }
}
```

## Implement IConfigurationProvider

```csharp
public class MyCustomConfigurationProvider : ConfigurationProvider
{
    private readonly MyCustomSource _source;

    public MyCustomConfigurationProvider(MyCustomSource source)
    {
        _source = source;
    }

    public override void Load()
    {
        // Load configuration data
        Data = new Dictionary<string, string?>
        {
            ["MyKey"] = "MyValue"
        };
    }
}
```

## Writable Source

For writable sources, implement `IWritableCfgSource`:

```csharp
public class MyWritableSource : ICfgSource, IWritableCfgSource
{
    public int Level { get; }
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }

    private readonly Dictionary<string, string?> _pending = new();

    public void SetValue(string key, string? value)
    {
        _pending[key] = value;
    }

    public void Remove(string key)
    {
        _pending[key] = null;
    }

    public Task SaveAsync(CancellationToken ct = default)
    {
        // Persist changes
        return Task.CompletedTask;
    }
}
```

## Extension Method

Create a fluent extension method:

```csharp
public static class CfgBuilderExtensions
{
    public static CfgBuilder AddMyCustomSource(
        this CfgBuilder builder,
        int level,
        bool writeable = false)
    {
        var source = new MyCustomSource(level);
        return builder.AddSource(source);
    }
}
```

## Usage

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddMyCustomSource(level: 5)
    .Build();
```

## Next Steps

- [Architecture](/en/guide/architecture) - Internal architecture details
- [API Reference](/en/api/) - Complete API documentation
