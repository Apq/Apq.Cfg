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
}
