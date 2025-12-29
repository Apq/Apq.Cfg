namespace Apq.Cfg.Crypto.TripleDes;

/// <summary>
/// Triple DES 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Triple DES 加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="base64Key">Base64 编码的密钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    [Obsolete("Triple DES is considered legacy. Use AddAesGcmEncryption for new projects.")]
    public static CfgBuilder AddTripleDesEncryption(
        this CfgBuilder builder,
        string base64Key,
        Action<EncryptionOptions>? configure = null)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var provider = new TripleDesCryptoProvider(base64Key);
#pragma warning restore CS0618
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 Triple DES 加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    [Obsolete("Triple DES is considered legacy. Use AddAesGcmEncryptionFromEnv for new projects.")]
    public static CfgBuilder AddTripleDesEncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_3DES_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
#pragma warning disable CS0618 // Type or member is obsolete
        return builder.AddTripleDesEncryption(key, configure);
#pragma warning restore CS0618
    }
}
