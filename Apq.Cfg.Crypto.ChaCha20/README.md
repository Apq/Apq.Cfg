# Apq.Cfg.Crypto.ChaCha20

ChaCha20-Poly1305 加密提供者，为 Apq.Cfg 配置库提供高性能认证加密支持。

## 安装

```bash
dotnet add package Apq.Cfg.Crypto.ChaCha20
```

## 特性

- ChaCha20-Poly1305 认证加密
- 高性能，特别适合移动端和嵌入式设备
- 无需硬件加速也能保持高性能
- 线程安全

## 使用方法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.ChaCha20;

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddChaCha20Encryption("base64key...")
    .AddSensitiveMasking()
    .Build();
```

### 从环境变量读取密钥

```csharp
// 设置环境变量 APQ_CFG_CHACHA20_KEY=base64key...
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddChaCha20EncryptionFromEnv()
    .Build();
```

### 生成密钥

```csharp
var key = ChaCha20CryptoProvider.GenerateKey();
Console.WriteLine($"Generated key: {key}");
```

## 适用场景

- 移动端应用
- 嵌入式设备
- 不支持 AES 硬件加速的平台
- 需要高性能加密的场景

## 许可证

MIT License
