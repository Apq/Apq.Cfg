namespace Apq.Cfg.Crypto.Sm4;

/// <summary>
/// SM4 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 SM4 国密加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64Key">Base64 编码的密钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddSm4Encryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new Sm4CryptoProvider(base64Key);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 SM4 国密加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddSm4EncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_SM4_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddSm4Encryption(key, configure);
    }
}
