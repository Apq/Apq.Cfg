# Encryption & Masking

Apq.Cfg provides comprehensive configuration encryption and masking features to protect sensitive information.

## Overview

- **Encryption**: Sensitive values (like passwords, API keys) are encrypted at rest and auto-decrypted on read
- **Masking**: Automatically hide sensitive information in logs and debug output
- **Zero-intrusion**: No changes to existing config file format, uses convention-based markers
- **Extensible**: Supports multiple encryption algorithms, user-customizable

## Installation

```bash
dotnet add package Apq.Cfg.Crypto
```

## Quick Start

### Basic Encryption

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;

// Config file config.json:
// {
//     "Database": {
//         "Password": "{ENC}base64encodedciphertext..."
//     }
// }

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv()  // Read key from environment variable
    .Build();

// Auto-decrypt on read
var password = cfg["Database:Password"];
```

### Basic Masking

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddSensitiveMasking()  // Add masking support
    .Build();

// Get masked value for logging
var maskedValue = cfg.GetMasked("Database:Password");
// Output: myS***ord (first and last 3 characters visible)
```

## Encryption Algorithms

Apq.Cfg.Crypto is built on BouncyCastle and supports these algorithms:

| Algorithm | Extension Method | Key Length | Use Case |
|-----------|------------------|------------|----------|
| AES-GCM | `AddAesGcmEncryption` | 128/192/256 bit | **Recommended**, authenticated encryption |
| AES-CBC | `AddAesCbcEncryption` | 128/192/256 bit | Good compatibility, needs HMAC |
| ChaCha20-Poly1305 | `AddChaCha20Encryption` | 256 bit | High performance, mobile-friendly |
| RSA | `AddRsaEncryption` | 2048+ bit | Asymmetric encryption, key distribution |
| SM4 | `AddSm4Encryption` | 128 bit | Chinese national standard |
| Triple DES | `AddTripleDesEncryption` | 128/192 bit | Legacy compatibility (not recommended) |

### AES-GCM Encryption (Recommended)

```csharp
// Method 1: Provide Base64 key directly
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryption("your-base64-encoded-key")
    .Build();

// Method 2: Read key from environment variable (recommended)
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv("APQ_CFG_ENCRYPTION_KEY")
    .Build();
```

### Custom Encryption Options

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryption("key", options =>
    {
        // Custom encryption prefix
        options.EncryptedPrefix = "[ENCRYPTED]";

        // Custom sensitive key patterns
        options.SensitiveKeyPatterns = new List<string>
        {
            "*Password*",
            "*Secret*",
            "*ApiKey*",
            "*ConnectionString*",
            "*Credential*",
            "*Token*",
            "*PrivateKey*"
        };

        // Auto-encrypt on write
        options.AutoEncryptOnWrite = true;
    })
    .Build();
```

## Masking Features

### Custom Masking Options

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddSensitiveMasking(options =>
    {
        // Custom sensitive key patterns
        options.SensitiveKeyPatterns = new List<string>
        {
            "*Password*",
            "*Secret*",
            "*ApiKey*",
            "*Token*"
        };

        // Custom mask string
        options.MaskString = "****";

        // Visible characters at start and end
        options.VisibleChars = 2;

        // Null value placeholder
        options.NullPlaceholder = "[not set]";
    })
    .Build();
```

### Get Masked Snapshot

```csharp
// Get masked snapshot of all configs (for debugging)
var snapshot = cfg.GetMaskedSnapshot();
foreach (var (key, value) in snapshot)
{
    Console.WriteLine($"{key}: {value}");
}
```

## Combined Usage

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddAesGcmEncryptionFromEnv()  // Encryption support
    .AddSensitiveMasking()          // Masking support
    .Build();

// Auto-decrypt on read
var password = cfg["Database:Password"];

// Use masked value for logging
logger.LogInformation("Database password: {Password}", cfg.GetMasked("Database:Password"));
```

## Command Line Tool

Apq.Cfg provides the `apqenc` command-line tool for generating keys, encrypting/decrypting values, and batch encrypting configuration files.

### Install Tool

```bash
dotnet tool install -g Apq.Cfg.Crypto.Tool
```

### Generate Key

```bash
# Generate 256-bit AES-GCM key (default)
apqenc generate-key

# Generate 128-bit key
apqenc generate-key --bits 128

# Generate 192-bit key
apqenc generate-key -b 192
```

Output example:
```
Algorithm: AES-GCM
Key bits: 256
Base64 Key: abc123...xyz789==

Keep this key safe, do not store it in configuration files!
Recommend using environment variable APQ_CFG_ENCRYPTION_KEY to store the key.
```

### Encrypt Single Value

```bash
# Encrypt value
apqenc encrypt --key "base64key..." --value "mySecretPassword"
# Output: {ENC}base64ciphertext...

# Use custom prefix
apqenc encrypt -k "base64key..." -v "mySecret" --prefix "[ENCRYPTED]"
# Output: [ENCRYPTED]base64ciphertext...
```

### Decrypt Value

```bash
# Decrypt value
apqenc decrypt --key "base64key..." --value "{ENC}base64ciphertext..."
# Output: mySecretPassword

# Use custom prefix
apqenc decrypt -k "base64key..." -v "[ENCRYPTED]base64cipher..." -p "[ENCRYPTED]"
```

### Batch Encrypt Configuration File

```bash
# Encrypt sensitive values in config file
apqenc encrypt-file --key "base64key..." --file config.json

# Preview keys to be encrypted (dry run)
apqenc encrypt-file -k "base64key..." -f config.json --dry-run

# Specify output file
apqenc encrypt-file -k "base64key..." -f config.json -o config.encrypted.json

# Custom sensitive key patterns
apqenc encrypt-file -k "base64key..." -f config.json --patterns "*Password*,*Secret*,*ApiKey*"

# Use custom prefix
apqenc encrypt-file -k "base64key..." -f config.json --prefix "[ENC]"
```

### Complete Workflow Example

```bash
# 1. Generate key
apqenc generate-key
# Output: Base64 Key: abc123...xyz789==

# 2. Set environment variable
export APQ_CFG_ENCRYPTION_KEY="abc123...xyz789=="

# 3. Preview keys to be encrypted
apqenc encrypt-file -k "$APQ_CFG_ENCRYPTION_KEY" -f config.json --dry-run

# 4. Execute encryption
apqenc encrypt-file -k "$APQ_CFG_ENCRYPTION_KEY" -f config.json

# 5. Verify encryption result
cat config.json
```

## Key Management Best Practices

### 1. Never Hardcode Keys

```csharp
// ❌ Wrong: Hardcoded key
.AddAesGcmEncryption("hardcoded-key")

// ✅ Correct: Read from environment variable
.AddAesGcmEncryptionFromEnv("APQ_CFG_ENCRYPTION_KEY")
```

### 2. Use Environment Variables

```bash
# Linux/macOS
export APQ_CFG_ENCRYPTION_KEY="your-base64-key"

# Windows PowerShell
$env:APQ_CFG_ENCRYPTION_KEY = "your-base64-key"
```

### 3. Generate Secure Keys

```csharp
using System.Security.Cryptography;

// Generate 256-bit AES key
var key = new byte[32];
RandomNumberGenerator.Fill(key);
var base64Key = Convert.ToBase64String(key);
Console.WriteLine($"Key: {base64Key}");
```

## Algorithm Selection Guide

```
Need asymmetric encryption?
├── Yes → RSA (key distribution scenarios)
└── No
    └── Chinese compliance required?
        ├── Yes → SM4 (national standard)
        └── No
            └── Need high performance/mobile?
                ├── Yes → ChaCha20-Poly1305
                └── No → AES-GCM (recommended default)
```

## Security Notes

1. **Key Security**: Store encryption keys securely, never commit to version control
2. **Algorithm Choice**: Prefer AES-GCM or ChaCha20-Poly1305 (authenticated encryption)
3. **Log Safety**: Always use `GetMasked()` when outputting sensitive configs to logs
4. **Error Handling**: Decryption failures throw exceptions, handle appropriately

## Next Steps

- [Encryption Design](/en/guide/encryption-masking-design) - Architecture and implementation details
- [Dependency Injection](/en/guide/dependency-injection) - DI integration
- [Best Practices](/en/guide/best-practices) - Configuration management best practices
