# Apq.Cfg.Crypto.Rsa

RSA 非对称加密提供者，为 Apq.Cfg 配置库提供 RSA 加密支持。

## 安装

```bash
dotnet add package Apq.Cfg.Crypto.Rsa
```

## 特性

- RSA 非对称加密
- 支持 PEM 和 XML 格式密钥
- 支持 OAEP 填充（SHA-256）
- 适用于密钥分发场景

## 使用方法

### 基本使用

```csharp
using Apq.Cfg;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.Rsa;

// 使用 PEM 格式私钥（可加密和解密）
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddRsaEncryption(privateKeyPem)
    .AddSensitiveMasking()
    .Build();
```

### 从文件读取密钥

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddRsaEncryptionFromFile("/path/to/private.pem")
    .Build();
```

### 生成密钥对

```csharp
// 生成 PEM 格式密钥对
var (publicKey, privateKey) = RsaCryptoProvider.GenerateKeyPair(2048);

// 生成 XML 格式密钥对
var (publicKeyXml, privateKeyXml) = RsaCryptoProvider.GenerateKeyPairXml(2048);
```

## 适用场景

- 密钥分发：使用公钥加密配置，私钥解密
- 多环境部署：开发环境使用公钥加密，生产环境使用私钥解密
- 安全传输：配置文件可以安全地传输，只有持有私钥的服务器能解密

## 注意事项

- RSA 加密的数据长度有限制，不适合加密大量数据
- 对于大量配置，建议使用混合加密（RSA + AES）
- 密钥大小建议至少 2048 位

## 许可证

MIT License
