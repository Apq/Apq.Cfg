# 加密脱敏设计

本文档详细说明 Apq.Cfg 加密脱敏功能的架构设计和实现原理。

::: tip 使用指南
如果您只是想了解如何使用加密脱敏功能，请参阅 [加密脱敏](/guide/encryption-masking)。
:::

## 架构设计

加密脱敏功能采用接口抽象和依赖注入实现解耦，核心库不依赖任何加密扩展包：

```
┌─────────────────────────────────────────────────────────────────┐
│                        用户应用程序                              │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │ Apq.Cfg.Crypto  │  │ Apq.Cfg.Crypto  │  │   用户自定义     │  │
│  │  .DataProtection│  │    (内置算法)    │  │   扩展项目       │  │
│  │  (平台特定)      │  │  BouncyCastle   │  │                 │  │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘  │
│           │                    │                    │           │
│           └────────────────────┼────────────────────┘           │
│                                │                                │
│                    ┌───────────▼───────────┐                    │
│                    │      Apq.Cfg          │                    │
│                    │   (核心配置库)         │                    │
│                    │   定义接口抽象         │                    │
│                    └───────────────────────┘                    │
└─────────────────────────────────────────────────────────────────┘
```

### 设计原则

1. **核心库零依赖**：Apq.Cfg 核心库只定义接口，不包含任何加密实现
2. **可插拔架构**：通过接口抽象支持多种加密算法
3. **扩展包模式**：加密功能通过独立的 `Apq.Cfg.Crypto` 包提供
4. **用户可扩展**：用户可以实现自定义加密提供者

## 核心接口

### IValueTransformer（值转换器）

用于加密/解密场景，在配置读写时自动转换值：

```csharp
public interface IValueTransformer
{
    string Name { get; }           // 转换器名称
    int Priority { get; }          // 优先级（数值越大越先执行）

    bool ShouldTransform(string key, string? value);
    string? TransformOnRead(string key, string? value);   // 读取时转换（解密）
    string? TransformOnWrite(string key, string? value);  // 写入时转换（加密）
}
```

### IValueMasker（值脱敏器）

用于日志输出等场景，隐藏敏感信息：

```csharp
public interface IValueMasker
{
    bool ShouldMask(string key);
    string Mask(string key, string? value);
}
```

### ICryptoProvider（加密提供者）

加密算法的抽象接口：

```csharp
public interface ICryptoProvider
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
```

## 处理流程

### 读取时解密

```
┌─────────────────────────────────────────────────────────────┐
│                      配置读取流程                             │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ 配置源      │ -> │ 值转换器链  │ -> │ 返回明文        │  │
│  │ (密文)      │    │ (解密)      │    │                 │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
│                                                             │
│  示例：                                                      │
│  "{ENC}base64..." -> EncryptionTransformer -> "myPassword"  │
└─────────────────────────────────────────────────────────────┘
```

### 写入时加密

```
┌─────────────────────────────────────────────────────────────┐
│                      配置写入流程                             │
│                                                             │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────┐  │
│  │ 明文值      │ -> │ 值转换器链  │ -> │ 写入配置源      │  │
│  │             │    │ (加密)      │    │ (密文)          │  │
│  └─────────────┘    └─────────────┘    └─────────────────┘  │
│                                                             │
│  示例：                                                      │
│  "myPassword" -> EncryptionTransformer -> "{ENC}base64..."  │
└─────────────────────────────────────────────────────────────┘
```

## 敏感键匹配算法

使用通配符模式匹配敏感配置键，并进行了性能优化：

### 匹配算法优化

1. **简单模式快速路径**：`*Keyword*` 形式使用 `string.Contains` 快速匹配
2. **复杂模式正则匹配**：其他模式编译为正则表达式
3. **结果缓存**：缓存匹配结果避免重复计算

```csharp
public bool ShouldMask(string key)
{
    return _shouldMaskCache.GetOrAdd(key, k =>
    {
        // 快速路径：简单的 Contains 检查
        foreach (var keyword in _simpleContainsPatterns.Value)
        {
            if (k.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // 慢路径：正则表达式匹配
        foreach (var regex in _compiledPatterns.Value)
        {
            if (regex.IsMatch(k))
                return true;
        }

        return false;
    });
}
```

### 模式分类

```csharp
// 分离简单模式和复杂模式
_simpleContainsPatterns = new Lazy<string[]>(() =>
    _options.SensitiveKeyPatterns
        .Where(IsSimpleContainsPattern)
        .Select(p => p.Substring(1, p.Length - 2)) // 去掉首尾的 *
        .ToArray());

// 延迟编译复杂正则表达式
_compiledPatterns = new Lazy<Regex[]>(() =>
    _options.SensitiveKeyPatterns
        .Where(p => !IsSimpleContainsPattern(p))
        .Select(pattern => new Regex(
            "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled))
        .ToArray());
```

## CfgBuilder 扩展点

```csharp
public sealed class CfgBuilder
{
    private readonly List<IValueTransformer> _transformers = new();
    private readonly List<IValueMasker> _maskers = new();

    // 添加值转换器（供扩展包使用）
    public CfgBuilder AddValueTransformer(IValueTransformer transformer);

    // 添加值脱敏器（供扩展包使用）
    public CfgBuilder AddValueMasker(IValueMasker masker);

    public ICfgRoot Build()
    {
        var transformerChain = _transformers.Count > 0
            ? new ValueTransformerChain(_transformers)
            : null;
        var maskerChain = _maskers.Count > 0
            ? new ValueMaskerChain(_maskers)
            : null;

        return new MergedCfgRoot(_sources, transformerChain, maskerChain);
    }
}
```

## 值转换器链

多个转换器按优先级顺序执行：

```csharp
internal sealed class ValueTransformerChain
{
    private readonly IValueTransformer[] _transformers;

    public ValueTransformerChain(IEnumerable<IValueTransformer> transformers)
    {
        _transformers = transformers.OrderByDescending(t => t.Priority).ToArray();
    }

    public string? TransformOnRead(string key, string? value)
    {
        foreach (var transformer in _transformers)
        {
            if (transformer.ShouldTransform(key, value))
            {
                value = transformer.TransformOnRead(key, value);
            }
        }
        return value;
    }

    public string? TransformOnWrite(string key, string? value)
    {
        foreach (var transformer in _transformers)
        {
            if (transformer.ShouldTransform(key, value))
            {
                value = transformer.TransformOnWrite(key, value);
            }
        }
        return value;
    }
}
```

## 脱敏算法

```csharp
public string Mask(string key, string? value)
{
    if (value == null)
        return _options.NullPlaceholder;

    var visibleChars = _options.VisibleChars;
    if (value.Length <= visibleChars * 2)
        return _options.MaskString;

    // 使用 string.Create 减少字符串分配
    return string.Create(totalLength, (value, visibleChars, maskString), static (span, state) =>
    {
        var (val, visible, mask) = state;
        val.AsSpan(0, visible).CopyTo(span);
        mask.AsSpan().CopyTo(span.Slice(visible));
        val.AsSpan(val.Length - visible).CopyTo(span.Slice(visible + mask.Length));
    });
}
```

## 内置加密算法

| 算法 | 类名 | 安全级别 | 适用场景 |
|------|------|----------|----------|
| AES-GCM | `AesGcmCryptoProvider` | ⭐⭐⭐⭐⭐ | 推荐首选，认证加密 |
| AES-CBC | `AesCbcCryptoProvider` | ⭐⭐⭐⭐ | 兼容性好，需配合 HMAC |
| ChaCha20-Poly1305 | `ChaCha20CryptoProvider` | ⭐⭐⭐⭐⭐ | 高性能，移动端友好 |
| RSA | `RsaCryptoProvider` | ⭐⭐⭐⭐ | 非对称加密，密钥分发 |
| SM4 | `Sm4CryptoProvider` | ⭐⭐⭐⭐ | 国密算法，合规要求 |
| Triple DES | `TripleDesCryptoProvider` | ⭐⭐⭐ | 遗留系统兼容 |

## 包结构

```
Apq.Cfg                          # 核心库（定义接口）
├── Security/
│   ├── IValueTransformer.cs
│   └── IValueMasker.cs
└── Internal/
    ├── ValueTransformerChain.cs
    └── ValueMaskerChain.cs

Apq.Cfg.Crypto                   # 加密核心包（依赖 BouncyCastle）
├── ICryptoProvider.cs
├── EncryptionTransformer.cs
├── EncryptionOptions.cs
├── SensitiveMasker.cs
├── MaskingOptions.cs
├── CfgBuilderExtensions.cs
└── Providers/
    ├── AesGcmCryptoProvider.cs
    ├── AesCbcCryptoProvider.cs
    ├── ChaCha20CryptoProvider.cs
    ├── RsaCryptoProvider.cs
    ├── Sm4CryptoProvider.cs
    └── TripleDesCryptoProvider.cs
```

## 依赖关系

| 包 | 依赖 |
|---|---|
| Apq.Cfg | 无加密依赖 |
| Apq.Cfg.Crypto | Apq.Cfg, BouncyCastle.Cryptography |
| Apq.Cfg.Crypto.DataProtection | Apq.Cfg.Crypto, Microsoft.AspNetCore.DataProtection |

## 下一步

- [加密脱敏](/guide/encryption-masking) - 了解如何使用加密脱敏功能
- [架构设计](/guide/architecture) - 了解整体架构设计
- [扩展开发](/guide/extension) - 开发自定义扩展
