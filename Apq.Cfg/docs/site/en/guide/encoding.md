# Encoding

Apq.Cfg provides intelligent encoding detection and handling for configuration files.

## Automatic Detection

By default, Apq.Cfg automatically detects file encoding:

1. BOM (Byte Order Mark) detection
2. UTF.Unknown library detection
3. Fallback to UTF-8

## Specify Encoding

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, encoding: Encoding.UTF8)
    .Build();
```

## Encoding Mapping

Configure encoding rules for specific files or patterns:

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        // Exact path
        config.AddReadMapping("/path/to/config.json", EncodingMappingType.ExactPath, Encoding.UTF8);

        // Wildcard pattern
        config.AddReadMapping("*.ps1", EncodingMappingType.Wildcard, Encoding.UTF8);

        // Regex pattern
        config.AddReadMapping(@"logs[/\\].*\.log$", EncodingMappingType.Regex, Encoding.Unicode);
    })
    .AddJsonFile("config.json", level: 0)
    .Build();
```

## Priority Order

| Match Type | Default Priority | Example |
|------------|------------------|---------|
| ExactPath | 100 | `/path/to/config.json` |
| Wildcard | 0 | `*.ps1` |
| Regex | 0 | `logs[/\\].*\.log$` |
| Built-in PowerShell | -100 | `*.ps1`, `*.psm1` |

## Common Encodings

| Encoding | Use Case |
|----------|----------|
| UTF-8 | Default, most common |
| UTF-8 with BOM | Windows PowerShell scripts |
| UTF-16 LE | Windows native |
| GB2312/GBK | Chinese legacy systems |

## Next Steps

- [Encoding Workflow](/en/guide/encoding-workflow) - Detailed encoding process
- [Best Practices](/en/guide/best-practices) - Configuration best practices
