# ICfgRoot

The main configuration root interface.

## Properties

### ConfigChanges

Observable stream of configuration changes.

```csharp
IObservable<ConfigChangeEvent> ConfigChanges { get; }
```

## Methods

### Get

Get a configuration value.

```csharp
string? Get(string key)
T? Get<T>(string key)
```

### Exists

Check if a key exists.

```csharp
bool Exists(string key)
```

### GetSection

Get a configuration section.

```csharp
ICfgSection GetSection(string key)
```

### Set

Set a configuration value.

```csharp
void Set(string key, string? value, int? targetLevel = null)
```

### Remove

Remove a configuration key.

```csharp
void Remove(string key, int? targetLevel = null)
```

### SaveAsync

Save pending changes.

```csharp
Task SaveAsync(int? targetLevel = null, CancellationToken ct = default)
```

### GetMany

Get multiple values.

```csharp
IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys)
void GetMany(IEnumerable<string> keys, Action<string, string?> onValue)
```

### SetMany

Set multiple values.

```csharp
void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null)
```

### ToMicrosoftConfiguration

Convert to IConfigurationRoot.

```csharp
IConfigurationRoot ToMicrosoftConfiguration()
```

## Example

```csharp
// Read
var name = cfg.Get("App:Name");
var port = cfg.Get<int>("App:Port");

// Write
cfg.Set("App:Name", "NewName");
await cfg.SaveAsync();

// Subscribe to changes
cfg.ConfigChanges.Subscribe(e => Console.WriteLine("Changed!"));
```

## Next Steps

- [ICfgSection](/en/api/icfg-section) - Section interface
- [CfgBuilder](/en/api/cfg-builder) - Builder API
