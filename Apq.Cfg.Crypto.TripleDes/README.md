# Apq.Cfg.Crypto.TripleDes

Triple DES 加密提供者，为 Apq.Cfg 配置库提供遗留系统兼容的加密支持。

## ⚠️ 安全警告

**Triple DES 已被视为遗留算法，仅用于与旧系统兼容。新项目请使用 AES-GCM。**

## 安装

```bash
dotnet add package Apq.Cfg.Crypto.TripleDes
```

## 使用方法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.TripleDes;

#pragma warning disable CS0618 // 忽略过时警告
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddTripleDesEncryption("base64key...")
    .Build();
#pragma warning restore CS0618
```

### 生成密钥

```csharp
#pragma warning disable CS0618
var key = TripleDesCryptoProvider.GenerateKey();
#pragma warning restore CS0618
```

## 适用场景

- 与遗留系统集成
- 迁移旧系统配置
- 合规要求必须使用 3DES 的场景

## 迁移建议

如果可能，建议迁移到 AES-GCM：

```csharp
// 旧代码
.AddTripleDesEncryption(key)

// 新代码
.AddAesGcmEncryption(newKey)
```

## 许可证

MIT License
