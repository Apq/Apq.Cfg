# 配置加密脱敏

Apq.Cfg 提供了完整的配置加密和脱敏功能，用于保护敏感配置信息的安全。

## 功能概述

- **加密**：敏感配置值（如数据库密码、API 密钥）在存储时加密，读取时自动解密
- **脱敏**：日志输出、调试显示时自动隐藏敏感信息
- **零侵入**：不修改现有配置文件格式，通过约定标记敏感配置
- **可扩展**：支持多种加密算法，用户可自定义

## 安装

```bash
dotnet add package Apq.Cfg.Crypto
```

## 快速开始

### 基本加密

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;

// 配置文件 config.json
// {
//     "Database": {
//         "Password": "{ENC}base64encodedciphertext..."
//     }
// }

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesGcmEncryptionFromEnv()  // 从环境变量读取密钥
    .Build();

// 读取时自动解密
var password = cfg.Get("Database:Password");
```

### 基本脱敏

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSensitiveMasking()  // 添加脱敏支持
    .Build();

// 日志输出时使用脱敏值
var maskedValue = cfg.GetMasked("Database:Password");
// 输出: myS***ord（首尾各保留 3 个字符）
```

## 加密功能

### 加密原理

1. 配置值以 `{ENC}` 前缀标记为已加密
2. 读取配置时，自动检测前缀并解密
3. 写入配置时，匹配敏感键模式的值自动加密

### 内置加密算法

Apq.Cfg.Crypto 基于 BouncyCastle 实现，内置支持以下加密算法：

| 算法 | 扩展方法 | 密钥长度 | 适用场景 |
|------|----------|----------|----------|
| AES-GCM | `AddAesGcmEncryption` | 128/192/256 位 | **推荐首选**，认证加密 |
| AES-CBC | `AddAesCbcEncryption` | 128/192/256 位 | 兼容性好，需配合 HMAC |
| ChaCha20-Poly1305 | `AddChaCha20Encryption` | 256 位 | 高性能，移动端友好 |
| RSA | `AddRsaEncryption` | 2048+ 位 | 非对称加密，密钥分发 |
| SM4 | `AddSm4Encryption` | 128 位 | 国密算法，合规要求 |
| Triple DES | `AddTripleDesEncryption` | 128/192 位 | 遗留系统兼容（不推荐） |

### AES-GCM 加密（推荐）

```csharp
// 方式 1：直接提供 Base64 密钥
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesGcmEncryption("your-base64-encoded-key")
    .Build();

// 方式 2：从环境变量读取密钥（推荐）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesGcmEncryptionFromEnv("APQ_CFG_ENCRYPTION_KEY")
    .Build();
```

### AES-CBC 加密

AES-CBC 需要两个密钥：加密密钥和 HMAC 密钥。

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesCbcEncryption(
        base64EncryptionKey: "encryption-key-base64",
        base64HmacKey: "hmac-key-base64")
    .Build();
```

### ChaCha20-Poly1305 加密

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddChaCha20Encryption("your-256-bit-key-base64")
    .Build();
```

### RSA 非对称加密

```csharp
// 从 PEM 文件加载
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddRsaEncryption("path/to/private.pem")
    .Build();

// 从 PEM 字符串加载
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddRsaEncryptionFromPem(pemString)
    .Build();
```

### SM4 国密加密

```csharp
using Apq.Cfg.Crypto.Providers;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSm4Encryption("your-128-bit-key-base64", Sm4Mode.CBC)
    .Build();
```

### 自定义加密选项

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesGcmEncryption("key", options =>
    {
        // 自定义加密前缀
        options.EncryptedPrefix = "[ENCRYPTED]";

        // 自定义敏感键模式
        options.SensitiveKeyPatterns = new List<string>
        {
            "*Password*",
            "*Secret*",
            "*ApiKey*",
            "*ConnectionString*",
            "*Credential*",
            "*Token*",
            "*PrivateKey*"  // 添加自定义模式
        };

        // 是否在写入时自动加密
        options.AutoEncryptOnWrite = true;
    })
    .Build();
```

### 自定义加密提供者

实现 `ICryptoProvider` 接口创建自定义加密提供者：

```csharp
using Apq.Cfg.Crypto;

public class MyCustomCryptoProvider : ICryptoProvider
{
    public string Encrypt(string plainText)
    {
        // 实现加密逻辑
        return encryptedText;
    }

    public string Decrypt(string cipherText)
    {
        // 实现解密逻辑
        return decryptedText;
    }
}

// 使用自定义提供者
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddEncryption(new MyCustomCryptoProvider())
    .Build();
```

## 脱敏功能

### 脱敏原理

1. 根据敏感键模式匹配配置键
2. 匹配的值在输出时自动脱敏
3. 保留首尾指定数量的字符，中间用掩码替换

### 基本用法

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSensitiveMasking()
    .Build();

// 获取脱敏值
var masked = cfg.GetMasked("Database:Password");
// 原值: "mySecretPassword123"
// 脱敏: "myS***123"
```

### 获取脱敏快照

```csharp
// 获取所有配置的脱敏快照（用于调试）
var snapshot = cfg.GetMaskedSnapshot();
foreach (var (key, value) in snapshot)
{
    Console.WriteLine($"{key}: {value}");
}
```

### 自定义脱敏选项

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSensitiveMasking(options =>
    {
        // 自定义敏感键模式
        options.SensitiveKeyPatterns = new List<string>
        {
            "*Password*",
            "*Secret*",
            "*ApiKey*",
            "*Token*"
        };

        // 自定义掩码字符串
        options.MaskString = "****";

        // 首尾保留的字符数
        options.VisibleChars = 2;

        // null 值的占位符
        options.NullPlaceholder = "[未设置]";
    })
    .Build();
```

### 默认敏感键模式

默认情况下，以下模式的键会被识别为敏感键：

- `*Password*`
- `*Secret*`
- `*ApiKey*`
- `*ConnectionString*`
- `*Credential*`
- `*Token*`

模式支持通配符：

- `*` 匹配任意字符（零个或多个）
- `?` 匹配单个字符

## 加密与脱敏组合使用

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesGcmEncryptionFromEnv()  // 加密支持
    .AddSensitiveMasking()          // 脱敏支持
    .Build();

// 读取时自动解密
var password = cfg.Get("Database:Password");

// 日志输出时使用脱敏值
logger.LogInformation("数据库密码: {Password}", cfg.GetMasked("Database:Password"));
```

## 配置文件示例

```json
{
    "Database": {
        "Host": "localhost",
        "Port": 5432,
        "Name": "mydb",
        "Password": "{ENC}base64encodedciphertext..."
    },
    "Api": {
        "Key": "{ENC}base64encodedciphertext...",
        "Url": "https://api.example.com"
    }
}
```

## 密钥管理最佳实践

### 1. 不要硬编码密钥

```csharp
// ❌ 错误：硬编码密钥
.AddAesGcmEncryption("hardcoded-key")

// ✅ 正确：从环境变量读取
.AddAesGcmEncryptionFromEnv("APQ_CFG_ENCRYPTION_KEY")
```

### 2. 使用环境变量

```bash
# Linux/macOS
export APQ_CFG_ENCRYPTION_KEY="your-base64-key"

# Windows PowerShell
$env:APQ_CFG_ENCRYPTION_KEY = "your-base64-key"
```

### 3. 生成安全密钥

```csharp
using System.Security.Cryptography;

// 生成 256 位 AES 密钥
var key = new byte[32];
RandomNumberGenerator.Fill(key);
var base64Key = Convert.ToBase64String(key);
Console.WriteLine($"密钥: {base64Key}");
```

### 4. 定期轮换密钥

建议定期更换加密密钥，并重新加密所有敏感配置。

## 算法选择指南

```
是否需要非对称加密？
├── 是 → RSA（密钥分发场景）
└── 否
    └── 是否有国密合规要求？
        ├── 是 → SM4（国密算法）
        └── 否
            └── 是否需要高性能/移动端？
                ├── 是 → ChaCha20-Poly1305
                └── 否 → AES-GCM（推荐默认选择）
```

## 安全注意事项

1. **密钥安全**：加密密钥应妥善保管，不要提交到版本控制系统
2. **算法选择**：优先使用 AES-GCM 或 ChaCha20-Poly1305 等认证加密算法
3. **日志安全**：始终使用 `GetMasked()` 输出敏感配置到日志
4. **异常处理**：解密失败时会抛出异常，应妥善处理

## 下一步

- [加密脱敏设计](/guide/encryption-masking-design) - 了解架构设计和实现原理
- [依赖注入](/guide/dependency-injection) - DI 集成
- [最佳实践](/guide/best-practices) - 配置管理最佳实践
