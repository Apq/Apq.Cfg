# ICfgSection

Configuration section interface for accessing sub-trees.

## Properties

### Path

The full path of this section.

```csharp
string Path { get; }
```

## Methods

### Indexer

Get or set a value relative to this section.

```csharp
string? this[string key] { get; set; }
```

### GetValue

Get a typed value relative to this section.

```csharp
T? GetValue<T>(string key)
```

### Set

Set a value relative to this section.

```csharp
void SetValue(string key, string? value, int? targetLevel = null)
```

### GetSection

Get a child section.

```csharp
ICfgSection GetSection(string key)
```

### GetChildKeys

Get all child keys.

```csharp
IEnumerable<string> GetChildKeys()
```

## Example

```csharp
var dbSection = cfg.GetSection("Database");

// Read values
var host = dbSection["Host"];
var port = dbSection.GetValue<int>("Port");

// Nested section
var connSection = dbSection.GetSection("Connection");
var timeout = connSection.GetValue<int>("Timeout");

// Enumerate children
foreach (var key in dbSection.GetChildKeys())
{
    Console.WriteLine($"{key}: {dbSection[key]}");
}
```

## Next Steps

- [ICfgRoot](/en/api/icfg-root) - Root interface
- [Extensions](/en/api/extensions) - Extension methods
