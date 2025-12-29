# Apq.Cfg.Crypto.Sm4

SM4 国密算法加密提供者，为 Apq.Cfg 配置库提供符合中国国家密码标准的加密支持。

## 安装

```bash
dotnet add package Apq.Cfg.Crypto.Sm4
```

## 特性

- SM4 国密算法（GB/T 32907-2016）
- GCM 模式认证加密
- 符合国家密码管理局标准
- 适用于有合规要求的场景

## 使用方法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.Sm4;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSm4Encryption("base64key...")
    .AddSensitiveMasking()
    .Build();
```

### 从环境变量读取密钥

```csharp
// 设置环境变量 APQ_CFG_SM4_KEY=base64key...
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddSm4EncryptionFromEnv()
    .Build();
```

### 生成密钥

```csharp
var key = Sm4CryptoProvider.GenerateKey();
Console.WriteLine($"Generated SM4 key: {key}");
```

## 适用场景

- 政府机关信息系统
- 金融行业应用
- 有国密合规要求的企业
- 需要通过等保测评的系统

## 依赖

本包依赖 BouncyCastle.Cryptography 提供 SM4 算法实现。

## 许可证

MIT License
