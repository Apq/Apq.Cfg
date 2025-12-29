# Performance

Apq.Cfg is designed for high performance with minimal memory allocation.

## Performance Features

### Zero-Allocation Batch Operations

```csharp
// Efficient batch read
cfg.GetMany(keys, (key, value) =>
{
    // Process each value without allocating intermediate collections
});
```

### Lazy Loading

Configuration sources are loaded on-demand, reducing startup time.

### Caching

- Parsed values are cached
- Encoding detection results are cached
- Sensitive key pattern matching is cached

## Benchmarks

| Operation | Time | Allocations |
|-----------|------|-------------|
| Get (string) | ~50ns | 0 bytes |
| Get<int> | ~80ns | 0 bytes |
| GetSection | ~100ns | 24 bytes |
| GetMany (100 keys) | ~2μs | 0 bytes |

## Optimization Tips

### 1. Use Batch Operations

```csharp
// ❌ Slow: Multiple individual reads
var name = cfg.Get("App:Name");
var port = cfg.Get("App:Port");
var debug = cfg.Get("App:Debug");

// ✅ Fast: Batch read
var keys = new[] { "App:Name", "App:Port", "App:Debug" };
cfg.GetMany(keys, (key, value) => { /* process */ });
```

### 2. Cache Sections

```csharp
// ❌ Slow: Repeated section lookups
var host = cfg.GetSection("Database").Get("Host");
var port = cfg.GetSection("Database").Get("Port");

// ✅ Fast: Cache the section
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host");
var port = dbSection.Get("Port");
```

### 3. Minimize Hot Reload Scope

Only enable `reloadOnChange` for sources that actually need it.

### 4. Use Appropriate Levels

Keep frequently-changing configuration at higher levels to minimize merge overhead.

## Memory Considerations

- Configuration values are stored as strings
- Large configuration files increase memory usage
- Consider splitting large configs into multiple files

## Next Steps

- [Best Practices](/en/guide/best-practices) - Configuration best practices
- [Architecture](/en/guide/architecture) - Internal architecture
