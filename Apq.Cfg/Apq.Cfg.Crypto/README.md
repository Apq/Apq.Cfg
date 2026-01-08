# Apq.Cfg.Crypto

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

配置加密脱敏核心抽象包，提供 `ICryptoProvider` 接口和通用加密转换器。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg

## 功能特性

- **加密**：敏感配置值（如数据库密码、API密钥）在存储时加密，读取时自动解密
- **脱敏**：日志输出、调试显示时自动隐藏敏感信息
- **零侵入**：不修改现有配置文件格式，通过约定标记敏感配置
- **可扩展**：支持多种加密算法，用户可自定义

## 用法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddEncryption(new MyCustomCryptoProvider())
    .AddSensitiveMasking()
    .Build();

// 使用索引器访问（自动解密）
var connStr = cfg["Database:ConnectionString"];

// 日志输出时自动脱敏
var maskedValue = cfg.GetMasked("Database:ConnectionString");
// 输出: Ser***ion
```

### 配置文件格式

加密值使用 `{ENC}` 前缀标记：

```json
{
    "Database": {
        "ConnectionString": "{ENC}base64encodedciphertext...",
        "Password": "{ENC}base64encodedciphertext..."
    },
    "Api": {
        "Key": "{ENC}base64encodedciphertext..."
    }
}
```

### 自定义加密选项

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddEncryption(provider, options =>
    {
        // 自定义前缀
        options.EncryptedPrefix = "[ENCRYPTED]";
        
        // 添加自定义敏感键模式
        options.SensitiveKeyPatterns.Add("*ApiSecret*");
        
        // 禁用自动加密
        options.AutoEncryptOnWrite = false;
    })
    .Build();
```

### 自定义脱敏选项

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddSensitiveMasking(options =>
    {
        options.MaskString = "****";
        options.VisibleChars = 2;
        options.NullPlaceholder = "<空>";
    })
    .Build();
```

### 实现自定义加密提供者

```csharp
public class MyCustomCryptoProvider : ICryptoProvider
{
    public string Encrypt(string plainText)
    {
        // 实现加密逻辑
        return Convert.ToBase64String(/* 加密后的字节 */);
    }

    public string Decrypt(string cipherText)
    {
        // 实现解密逻辑
        var bytes = Convert.FromBase64String(cipherText);
        return /* 解密后的字符串 */;
    }
}
```

## 敏感键模式

默认的敏感键模式（支持通配符 `*` 和 `?`）：

| 模式 | 说明 |
|------|------|
| `*Password*` | 匹配包含 Password 的键 |
| `*Secret*` | 匹配包含 Secret 的键 |
| `*ApiKey*` | 匹配包含 ApiKey 的键 |
| `*ConnectionString*` | 匹配包含 ConnectionString 的键 |
| `*Credential*` | 匹配包含 Credential 的键 |
| `*Token*` | 匹配包含 Token 的键 |

## 前缀格式说明

默认使用 `{ENC}` 前缀，使用花括号是为了避免与配置节分隔符 `:` 混淆：

```json
// 容易混淆的格式
"Password": "ENC:base64cipher..."  // ENC 看起来像是配置节名

// 推荐的格式
"Password": "{ENC}base64cipher..."  // 花括号明确标识这是前缀
```

## 相关包

- **Apq.Cfg.Crypto.AesGcm**：AES-GCM 加密实现
- **Apq.Cfg.Crypto.DataProtection**：ASP.NET Core Data Protection 实现

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
