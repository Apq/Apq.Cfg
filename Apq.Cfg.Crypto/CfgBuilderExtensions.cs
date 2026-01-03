using Apq.Cfg.Crypto.Providers;

namespace Apq.Cfg.Crypto;

/// <summary>
/// CfgBuilder 的加密脱敏扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="provider">加密提供者</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJson("config.json", level: 0)
    ///     .AddEncryption(new AesGcmCryptoProvider(key), options =>
    ///     {
    ///         options.EncryptedPrefix = "[ENCRYPTED]";
    ///     })
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddEncryption(
        this CfgBuilder builder,
        ICryptoProvider provider,
        Action<EncryptionOptions>? configure = null)
    {
        var options = new EncryptionOptions();
        configure?.Invoke(options);

        builder.AddValueTransformer(new EncryptionTransformer(provider, options));
        return builder;
    }

    /// <summary>
    /// 添加敏感值脱敏
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="configure">脱敏选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJson("config.json", level: 0)
    ///     .AddSensitiveMasking(options =>
    ///     {
    ///         options.MaskString = "****";
    ///         options.VisibleChars = 2;
    ///     })
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddSensitiveMasking(
        this CfgBuilder builder,
        Action<MaskingOptions>? configure = null)
    {
        var options = new MaskingOptions();
        configure?.Invoke(options);

        builder.AddValueMasker(new SensitiveMasker(options));
        return builder;
    }

    // ==================== 内置算法扩展方法 ====================

    /// <summary>
    /// 添加 AES-GCM 加密支持
    /// </summary>
    public static CfgBuilder AddAesGcmEncryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new AesGcmCryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 AES-GCM 加密支持（从环境变量读取密钥）
    /// </summary>
    public static CfgBuilder AddAesGcmEncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_ENCRYPTION_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddAesGcmEncryption(key, configure);
    }

    /// <summary>
    /// 添加 AES-CBC 加密支持
    /// </summary>
    public static CfgBuilder AddAesCbcEncryption(
        this CfgBuilder builder,
        string base64EncryptionKey,
        string base64HmacKey,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new AesCbcCryptoProvider(base64EncryptionKey, base64HmacKey);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 ChaCha20-Poly1305 加密支持
    /// </summary>
    public static CfgBuilder AddChaCha20Encryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new ChaCha20CryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 RSA 加密支持（从 PEM 文件）
    /// </summary>
    public static CfgBuilder AddRsaEncryption(
        this CfgBuilder builder,
        string pemFilePath,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = RsaCryptoProvider.FromPemFile(pemFilePath);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 RSA 加密支持（从 PEM 字符串）
    /// </summary>
    public static CfgBuilder AddRsaEncryptionFromPem(
        this CfgBuilder builder,
        string pem,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = RsaCryptoProvider.FromPem(pem);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 SM4 国密加密支持
    /// </summary>
    public static CfgBuilder AddSm4Encryption(
        this CfgBuilder builder,
        string base64Key,
        Sm4Mode mode = Sm4Mode.CBC,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new Sm4CryptoProvider(base64Key, mode);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 Triple DES 加密支持（仅用于遗留系统兼容）
    /// </summary>
    [Obsolete("Triple DES is considered weak. Use AES-GCM for new projects.")]
    public static CfgBuilder AddTripleDesEncryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new TripleDesCryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }
}
