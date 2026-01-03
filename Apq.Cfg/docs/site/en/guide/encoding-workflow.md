# Encoding Workflow

This document describes the encoding detection and handling workflow in Apq.Cfg.

## Detection Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     Encoding Detection                       │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ User        │ -> │ Encoding    │ -> │ Cache Result    │  │
│  │ Specified   │    │ Mapping     │    │                 │  │
│  └──────┬──────┘    └──────┬──────┘    └───────┬─────────┘  │
│         │                  │                   │            │
│         ▼                  ▼                   ▼            │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ BOM         │ -> │ UTF.Unknown │ -> │ Fallback        │  │
│  │ Detection   │    │ Library     │    │ (UTF-8)         │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Priority Order

1. **User Specified**: Explicit encoding parameter
2. **Encoding Mapping**: Pattern-based rules
3. **BOM Detection**: Byte Order Mark
4. **UTF.Unknown**: Statistical detection
5. **Fallback**: UTF-8 default

## Encoding Mapping

### Configuration

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        // Exact path (highest priority)
        config.AddReadMapping(
            "/path/to/config.json",
            EncodingMappingType.ExactPath,
            Encoding.UTF8,
            priority: 100);

        // Wildcard pattern
        config.AddReadMapping(
            "*.ps1",
            EncodingMappingType.Wildcard,
            new UTF8Encoding(true),  // UTF-8 with BOM
            priority: 50);

        // Regex pattern
        config.AddReadMapping(
            @"logs[/\\].*\.log$",
            EncodingMappingType.Regex,
            Encoding.Unicode,
            priority: 0);
    })
    .Build();
```

### Priority Table

| Match Type | Default Priority | Example |
|------------|------------------|---------|
| ExactPath | 100 | `/path/to/config.json` |
| Wildcard | 0 | `*.ps1` |
| Regex | 0 | `logs[/\\].*\.log$` |
| Built-in PowerShell | -100 | `*.ps1`, `*.psm1` |

## BOM Detection

| BOM Bytes | Encoding |
|-----------|----------|
| `EF BB BF` | UTF-8 |
| `FF FE` | UTF-16 LE |
| `FE FF` | UTF-16 BE |
| `FF FE 00 00` | UTF-32 LE |
| `00 00 FE FF` | UTF-32 BE |

## Write Encoding

When writing files, encoding is determined by:

1. **Existing file**: Preserve original encoding
2. **New file**: Use configured write mapping or UTF-8

```csharp
config.AddWriteMapping(
    "*.json",
    EncodingMappingType.Wildcard,
    Encoding.UTF8);
```

## Caching

Encoding detection results are cached per file path to avoid repeated detection.

## Next Steps

- [Encoding](/en/guide/encoding) - Basic encoding usage
- [Best Practices](/en/guide/best-practices) - Configuration best practices
