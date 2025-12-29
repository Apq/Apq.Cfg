# Apq.Cfg.Crypto.AesGcm

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

AES-GCM 加密实现包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg.Crypto

## 功能特性

- 使用 AES-GCM 认证加密算法
- 提供数据机密性和完整性保护
- 支持 128、192、256 位密钥
- 自动生成随机 nonce

## 用法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.AesGcm;

var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddAesGcmEncryption("base64key...")
    .AddSensitiveMasking()
    .Build();

// 读取时自动解密
var connectionString = cfg.Get("Database:ConnectionString");
```

### 从环境变量读取密钥

```csharp
// 设置环境变量 APQ_CFG_ENCRYPTION_KEY=base64key...
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddAesGcmEncryptionFromEnv()
    .Build();

// 或使用自定义环境变量名
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddAesGcmEncryptionFromEnv("MY_ENCRYPTION_KEY")
    .Build();
```

### 写入时自动加密

```csharp
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: true, isPrimaryWriter: true)
    .AddAesGcmEncryption("base64key...")
    .Build();

// 写入时自动加密（如果键匹配敏感模式）
cfg.Set("Database:Password", "mySecretPassword");
await cfg.SaveAsync();

// 文件中保存的是: "Database:Password": "{ENC}base64ciphertext..."
```

### 自定义加密选项

```csharp
var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddAesGcmEncryption("base64key...", options =>
    {
        options.EncryptedPrefix = "[ENCRYPTED]";  // 自定义前缀
        options.SensitiveKeyPatterns.Add("*ApiSecret*");  // 添加敏感键模式
    })
    .Build();
```

## 生成密钥

### 使用 .NET 代码生成

```csharp
using System.Security.Cryptography;

// 生成 256 位密钥
var key = new byte[32];
RandomNumberGenerator.Fill(key);
var base64Key = Convert.ToBase64String(key);
Console.WriteLine($"密钥: {base64Key}");
```

### 使用 PowerShell 生成

```powershell
$key = [byte[]]::new(32)
[System.Security.Cryptography.RandomNumberGenerator]::Fill($key)
[Convert]::ToBase64String($key)
```

## 密文格式

密文格式：`nonce(12字节) + tag(16字节) + cipher`

- **nonce**：12 字节随机数，每次加密自动生成
- **tag**：16 字节认证标签，用于验证数据完整性
- **cipher**：加密后的数据

## 安全最佳实践

1. **不要**将加密密钥存储在配置文件中
2. 使用环境变量或密钥管理服务存储密钥
3. 定期轮换密钥
4. 使用 256 位密钥获得最高安全性

## 方法签名

```csharp
public static CfgBuilder AddAesGcmEncryption(
    this CfgBuilder builder,
    string base64Key,
    Action<EncryptionOptions>? configure = null)

public static CfgBuilder AddAesGcmEncryption(
    this CfgBuilder builder,
    byte[] key,
    Action<EncryptionOptions>? configure = null)

public static CfgBuilder AddAesGcmEncryptionFromEnv(
    this CfgBuilder builder,
    string envVarName = "APQ_CFG_ENCRYPTION_KEY",
    Action<EncryptionOptions>? configure = null)
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
