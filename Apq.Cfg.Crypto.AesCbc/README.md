# Apq.Cfg.Crypto.AesCbc

AES-CBC 加密提供者，为 Apq.Cfg 配置库提供 AES-CBC 加密支持，带 HMAC-SHA256 认证。

## 安装

```bash
dotnet add package Apq.Cfg.Crypto.AesCbc
```

## 特性

- AES-CBC 加密（128/192/256 位密钥）
- HMAC-SHA256 认证防止篡改
- 支持主密钥自动派生
- 线程安全

## 使用方法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.AesCbc;

// 使用主密钥（自动派生加密密钥和 HMAC 密钥）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesCbcEncryption("base64MasterKey...")
    .AddSensitiveMasking()
    .Build();
```

### 使用独立密钥

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesCbcEncryption(
        "base64EncryptionKey...",
        "base64HmacKey...")
    .Build();
```

### 从环境变量读取密钥

```csharp
// 设置环境变量 APQ_CFG_AES_CBC_KEY=base64key...
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddAesCbcEncryptionFromEnv()
    .Build();
```

### 生成密钥

```csharp
// 生成主密钥
var masterKey = AesCbcCryptoProvider.GenerateMasterKey();

// 生成独立密钥对
var (encryptionKey, hmacKey) = AesCbcCryptoProvider.GenerateKeys(256);
```

## 安全说明

- AES-CBC 需要配合 HMAC 使用以防止填充预言攻击
- 本实现使用 Encrypt-then-MAC 模式
- 如果不需要兼容旧系统，推荐使用 AES-GCM

## 许可证

MIT License
