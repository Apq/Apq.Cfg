# Extension Methods

Extension methods for various configuration sources and features.

## Configuration Sources

### JSON

```csharp
builder.AddJsonFile(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
```

### YAML (Apq.Cfg.Yaml)

```csharp
builder.AddYamlFile(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
```

### XML (Apq.Cfg.Xml)

```csharp
builder.AddXmlFile(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
```

### INI (Apq.Cfg.Ini)

```csharp
builder.AddIniFile(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
```

### TOML (Apq.Cfg.Toml)

```csharp
builder.AddTomlFile(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
```

### Environment Variables

```csharp
builder.AddEnvironmentVariables(level, prefix)
```

## Remote Sources

### Consul (Apq.Cfg.Consul)

```csharp
builder.AddConsul(options => { }, level, writeable, reloadOnChange)
```

### Nacos (Apq.Cfg.Nacos)

```csharp
builder.AddNacos(options => { }, level, reloadOnChange)
```

### Vault (Apq.Cfg.Vault)

```csharp
builder.AddVault(options => { }, level)
```

## Encryption (Apq.Cfg.Crypto)

### AES-GCM

```csharp
builder.AddAesGcmEncryption(base64Key)
builder.AddAesGcmEncryptionFromEnv(envVarName)
```

### Masking

```csharp
builder.AddSensitiveMasking()
builder.AddSensitiveMasking(options => { })
```

## DI Integration

### Service Registration

```csharp
services.AddApqCfg(builder => { })
```

### Options Configuration

```csharp
services.ConfigureApqCfg<TOptions>(sectionPath)
```

## Next Steps

- [CfgBuilder](/en/api/cfg-builder) - Builder API
- [Config Sources](/en/config-sources/) - All sources
