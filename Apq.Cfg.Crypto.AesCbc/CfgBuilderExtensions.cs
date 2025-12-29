namespace Apq.Cfg.Crypto.AesCbc;

/// <summary>
/// AES-CBC 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 AES-CBC 加密支持（使用独立的加密密钥和 HMAC 密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64EncryptionKey">Base64 编码的加密密钥</param>
    /// <param name="base64HmacKey">Base64 编码的 HMAC 密钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
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
    /// 添加 AES-CBC 加密支持（使用主密钥自动派生）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64MasterKey">Base64 编码的主密钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddAesCbcEncryption(
        this CfgBuilder builder,
        string base64MasterKey,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new AesCbcCryptoProvider(base64MasterKey);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 AES-CBC 加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddAesCbcEncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_AES_CBC_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddAesCbcEncryption(key, configure);
    }
}
