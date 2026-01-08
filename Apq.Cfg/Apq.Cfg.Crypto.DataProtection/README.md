# Apq.Cfg.Crypto.DataProtection

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

ASP.NET Core Data Protection 加密实现包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg.Crypto
- Microsoft.AspNetCore.DataProtection

## 功能特性

- 使用 ASP.NET Core Data Protection API
- 支持跨机器、跨应用的密钥管理
- 支持密钥轮换和撤销
- 适用于 ASP.NET Core 应用程序

## 用法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.DataProtection;

var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddDataProtectionEncryption("MyApp")
    .AddSensitiveMasking()
    .Build();

// 使用索引器访问（自动解密）
var connStr = cfg["Database:ConnectionString"];
```

### 使用自定义 Data Protection 提供者

```csharp
using Microsoft.AspNetCore.DataProtection;

// 在 ASP.NET Core 应用中使用 DI 注入的提供者
public class MyService
{
    private readonly ICfgRoot _cfg;

    public MyService(IDataProtectionProvider dataProtectionProvider)
    {
        _cfg = new CfgBuilder()
            .AddJsonFile("config.json", level: 0, writeable: false)
            .AddDataProtectionEncryption(dataProtectionProvider)
            .Build();
    }
}
```

### 使用指定目录存储密钥

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddDataProtectionEncryption(
        new DirectoryInfo("/var/keys"),
        "MyApp")
    .Build();
```

### 自定义保护目的

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0, writeable: false)
    .AddDataProtectionEncryption(
        "MyApp",
        purpose: "ConfigEncryption")
    .Build();
```

## 方法签名

```csharp
public static CfgBuilder AddDataProtectionEncryption(
    this CfgBuilder builder,
    IDataProtectionProvider provider,
    string purpose = "Apq.Cfg",
    Action<EncryptionOptions>? configure = null)

public static CfgBuilder AddDataProtectionEncryption(
    this CfgBuilder builder,
    string applicationName,
    string purpose = "Apq.Cfg",
    Action<EncryptionOptions>? configure = null)

public static CfgBuilder AddDataProtectionEncryption(
    this CfgBuilder builder,
    DirectoryInfo keyDirectory,
    string applicationName,
    string purpose = "Apq.Cfg",
    Action<EncryptionOptions>? configure = null)
```

## 与 AES-GCM 的对比

| 特性 | Data Protection | AES-GCM |
|------|-----------------|---------|
| 密钥管理 | 自动管理 | 手动管理 |
| 密钥轮换 | 支持 | 需手动实现 |
| 跨机器 | 需配置共享存储 | 需共享密钥 |
| 依赖 | ASP.NET Core | 无 |
| 适用场景 | ASP.NET Core 应用 | 通用场景 |

## 安全最佳实践

1. 在生产环境中配置持久化密钥存储
2. 使用 Azure Key Vault 或其他密钥管理服务
3. 定期轮换密钥
4. 为不同用途使用不同的 purpose

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
