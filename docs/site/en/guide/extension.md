# Extension Development

Learn how to create custom configuration sources for Apq.Cfg.

## ICfgSource Interface

All configuration sources must implement the base interface:

```csharp
public interface ICfgSource
{
    /// <summary>
    /// Configuration source name (unique within the same level)
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Configuration level, higher values have higher priority
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Configuration source type name
    /// </summary>
    string Type { get; }

    /// <summary>
    /// Whether the source supports writing
    /// </summary>
    bool IsWriteable { get; }

    /// <summary>
    /// Whether this is the primary writer (only one per level)
    /// </summary>
    bool IsPrimaryWriter { get; }

    /// <summary>
    /// Total number of configuration keys (all leaf nodes)
    /// </summary>
    int KeyCount { get; }

    /// <summary>
    /// Number of top-level configuration keys
    /// </summary>
    int TopLevelKeyCount { get; }

    /// <summary>
    /// Build Microsoft.Extensions.Configuration source
    /// </summary>
    IConfigurationSource BuildSource();

    /// <summary>
    /// Get all configuration values from this source
    /// </summary>
    IEnumerable<KeyValuePair<string, string?>> GetAllValues();
}
```

## IWritableCfgSource Interface

For writable sources, implement `IWritableCfgSource`:

```csharp
public interface IWritableCfgSource : ICfgSource
{
    /// <summary>
    /// Apply configuration changes
    /// </summary>
    /// <param name="changes">Changes to apply</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken);
}
```

## Implement a Custom Source

```csharp
public class MyCustomSource : ICfgSource
{
    private readonly Dictionary<string, string?> _data = new();

    public string Name { get; set; } = "MyCustom";
    public int Level { get; }
    public string Type => "MyCustom";
    public bool IsWriteable => false;
    public bool IsPrimaryWriter => false;
    public int KeyCount => _data.Count;
    public int TopLevelKeyCount => _data.Keys
        .Select(k => k.Split(':')[0])
        .Distinct()
        .Count();

    public MyCustomSource(int level)
    {
        Level = level;
    }

    public IConfigurationSource BuildSource()
    {
        return new MyCustomConfigurationSource(this);
    }

    public IEnumerable<KeyValuePair<string, string?>> GetAllValues() => _data;
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

## Writable Source Example

```csharp
public class MyWritableSource : ICfgSource, IWritableCfgSource
{
    private readonly ConcurrentDictionary<string, string?> _data = new();

    public string Name { get; set; } = "MyWritable";
    public int Level { get; }
    public string Type => "MyWritable";
    public bool IsWriteable => true;
    public bool IsPrimaryWriter { get; }
    public int KeyCount => _data.Count;
    public int TopLevelKeyCount => _data.Keys
        .Select(k => k.Split(':')[0])
        .Distinct()
        .Count();

    public MyWritableSource(int level, bool isPrimaryWriter = false)
    {
        Level = level;
        IsPrimaryWriter = isPrimaryWriter;
    }

    public IConfigurationSource BuildSource() => new MyWritableConfigurationSource(this);
    public IEnumerable<KeyValuePair<string, string?>> GetAllValues() => _data;

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken ct)
    {
        foreach (var kvp in changes)
        {
            if (kvp.Value == null)
                _data.TryRemove(kvp.Key, out _);
            else
                _data[kvp.Key] = kvp.Value;
        }

        // Persist changes to storage
        await PersistAsync(ct);
    }

    private Task PersistAsync(CancellationToken ct)
    {
        // Implement persistence logic
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
