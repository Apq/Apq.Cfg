namespace Apq.Cfg.Crypto.ChaCha20;

/// <summary>
/// ChaCha20-Poly1305 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 ChaCha20-Poly1305 加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64Key">Base64 编码的密钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddChaCha20Encryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new ChaCha20CryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 ChaCha20-Poly1305 加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddChaCha20EncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_CHACHA20_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddChaCha20Encryption(key, configure);
    }
}
