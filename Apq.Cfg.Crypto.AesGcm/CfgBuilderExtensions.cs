namespace Apq.Cfg.Crypto.AesGcm;

/// <summary>
/// CfgBuilder 的 AES-GCM 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 AES-GCM 加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64Key">Base64 编码的加密密钥</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJson("config.json", level: 0)
    ///     .AddAesGcmEncryption("base64key...")
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddAesGcmEncryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new AesGcmCryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 AES-GCM 加密支持（使用字节数组密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="key">加密密钥字节数组</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    public static CfgBuilder AddAesGcmEncryption(
        this CfgBuilder builder,
        byte[] key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new AesGcmCryptoProvider(key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 AES-GCM 加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称，默认为 APQ_CFG_ENCRYPTION_KEY</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <exception cref="InvalidOperationException">环境变量未设置时抛出</exception>
    /// <example>
    /// <code>
    /// // 设置环境变量 APQ_CFG_ENCRYPTION_KEY=base64key...
    /// var cfg = new CfgBuilder()
    ///     .AddJson("config.json", level: 0)
    ///     .AddAesGcmEncryptionFromEnv()
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddAesGcmEncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_ENCRYPTION_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddAesGcmEncryption(key, configure);
    }
}
