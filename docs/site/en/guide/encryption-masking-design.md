# Encryption & Masking Design

This document describes the architecture design and implementation of Apq.Cfg's encryption and masking features.

::: tip Usage Guide
For usage instructions, see [Encryption & Masking](/en/guide/encryption-masking).
:::

## Architecture Design

The encryption and masking features use interface abstraction and dependency injection for decoupling. The core library has no encryption dependencies:

```
┌─────────────────────────────────────────────────────────────────┐
│                        User Application                          │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │ Apq.Cfg.Crypto  │  │ Apq.Cfg.Crypto  │  │   User Custom   │  │
│  │  .DataProtection│  │  (Built-in)     │  │   Extension     │  │
│  │  (Platform)     │  │  BouncyCastle   │  │                 │  │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘  │
│           │                    │                    │           │
│           └────────────────────┼────────────────────┘           │
│                                │                                │
│                    ┌───────────▼───────────┐                    │
│                    │      Apq.Cfg          │                    │
│                    │   (Core Library)      │                    │
│                    │   Interface Only      │                    │
│                    └───────────────────────┘                    │
└─────────────────────────────────────────────────────────────────┘
```

### Design Principles

1. **Zero Dependencies in Core**: Apq.Cfg core only defines interfaces, no encryption implementation
2. **Pluggable Architecture**: Interface abstraction supports multiple encryption algorithms
3. **Extension Package Model**: Encryption via independent `Apq.Cfg.Crypto` package
4. **User Extensible**: Users can implement custom crypto providers

## Core Interfaces

### IValueTransformer

For encryption/decryption scenarios, automatically transforms values on read/write:

```csharp
public interface IValueTransformer
{
    string Name { get; }
    int Priority { get; }

    bool ShouldTransform(string key, string? value);
    string? TransformOnRead(string key, string? value);   // Decrypt on read
    string? TransformOnWrite(string key, string? value);  // Encrypt on write
}
```

### IValueMasker

For log output scenarios, hides sensitive information:

```csharp
public interface IValueMasker
{
    bool ShouldMask(string key);
    string Mask(string key, string? value);
}
```

### ICryptoProvider

Encryption algorithm abstraction:

```csharp
public interface ICryptoProvider
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
```

## Processing Flow

### Decrypt on Read

```
┌─────────────────────────────────────────────────────────────┐
│                      Read Flow                               │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ Config      │ -> │ Transformer │ -> │ Return          │  │
│  │ Source      │    │ Chain       │    │ Plaintext       │  │
│  │ (Cipher)    │    │ (Decrypt)   │    │                 │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
│                                                             │
│  Example:                                                   │
│  "{ENC}base64..." -> EncryptionTransformer -> "myPassword"  │
└─────────────────────────────────────────────────────────────┘
```

### Encrypt on Write

```
┌─────────────────────────────────────────────────────────────┐
│                      Write Flow                              │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ Plaintext   │ -> │ Transformer │ -> │ Write to        │  │
│  │ Value       │    │ Chain       │    │ Source          │  │
│  │             │    │ (Encrypt)   │    │ (Cipher)        │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
│                                                             │
│  Example:                                                   │
│  "myPassword" -> EncryptionTransformer -> "{ENC}base64..."  │
└─────────────────────────────────────────────────────────────┘
```

## Sensitive Key Matching

Uses wildcard pattern matching with performance optimizations:

### Matching Algorithm

1. **Simple Pattern Fast Path**: `*Keyword*` patterns use `string.Contains`
2. **Complex Pattern Regex**: Other patterns compiled to regex
3. **Result Caching**: Cache match results to avoid recomputation

```csharp
public bool ShouldMask(string key)
{
    return _shouldMaskCache.GetOrAdd(key, k =>
    {
        // Fast path: simple Contains check
        foreach (var keyword in _simpleContainsPatterns.Value)
        {
            if (k.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // Slow path: regex matching
        foreach (var regex in _compiledPatterns.Value)
        {
            if (regex.IsMatch(k))
                return true;
        }

        return false;
    });
}
```

## Built-in Encryption Algorithms

| Algorithm | Class | Security | Use Case |
|-----------|-------|----------|----------|
| AES-GCM | `AesGcmCryptoProvider` | ⭐⭐⭐⭐⭐ | Recommended, authenticated |
| AES-CBC | `AesCbcCryptoProvider` | ⭐⭐⭐⭐ | Good compatibility |
| ChaCha20-Poly1305 | `ChaCha20CryptoProvider` | ⭐⭐⭐⭐⭐ | High performance |
| RSA | `RsaCryptoProvider` | ⭐⭐⭐⭐ | Asymmetric, key distribution |
| SM4 | `Sm4CryptoProvider` | ⭐⭐⭐⭐ | Chinese national standard |
| Triple DES | `TripleDesCryptoProvider` | ⭐⭐⭐ | Legacy compatibility |

## Package Structure

```
Apq.Cfg                          # Core (interfaces only)
├── Security/
│   ├── IValueTransformer.cs
│   └── IValueMasker.cs
└── Internal/
    ├── ValueTransformerChain.cs
    └── ValueMaskerChain.cs

Apq.Cfg.Crypto                   # Crypto package (BouncyCastle)
├── ICryptoProvider.cs
├── EncryptionTransformer.cs
├── EncryptionOptions.cs
├── SensitiveMasker.cs
├── MaskingOptions.cs
├── CfgBuilderExtensions.cs
└── Providers/
    ├── AesGcmCryptoProvider.cs
    ├── AesCbcCryptoProvider.cs
    ├── ChaCha20CryptoProvider.cs
    ├── RsaCryptoProvider.cs
    ├── Sm4CryptoProvider.cs
    └── TripleDesCryptoProvider.cs
```

## Dependencies

| Package | Dependencies |
|---------|-------------|
| Apq.Cfg | No crypto dependencies |
| Apq.Cfg.Crypto | Apq.Cfg, BouncyCastle.Cryptography |
| Apq.Cfg.Crypto.DataProtection | Apq.Cfg.Crypto, Microsoft.AspNetCore.DataProtection |

## Next Steps

- [Encryption & Masking](/en/guide/encryption-masking) - Usage guide
- [Architecture](/en/guide/architecture) - Overall architecture
- [Extension Development](/en/guide/extension) - Custom extensions
