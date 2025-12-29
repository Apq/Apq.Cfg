using System.Security.Cryptography;

namespace Apq.Cfg.Crypto.Rsa;

/// <summary>
/// RSA 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 RSA 加密支持（使用 PEM 格式密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="pemKey">PEM 格式的公钥或私钥</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddRsaEncryption(
        this CfgBuilder builder,
        string pemKey,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new RsaCryptoProvider(pemKey);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 RSA 加密支持（使用 RSA 实例）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="rsa">RSA 实例</param>
    /// <param name="padding">填充模式</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddRsaEncryption(
        this CfgBuilder builder,
        RSA rsa,
        RSAEncryptionPadding? padding = null,
        Action<EncryptionOptions>? configure = null)
    {
        var provider = new RsaCryptoProvider(rsa, padding);
        return builder.AddEncryption(provider, configure);
    }

    /// <summary>
    /// 添加 RSA 加密支持（从环境变量读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="envVarName">环境变量名称</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddRsaEncryptionFromEnv(
        this CfgBuilder builder,
        string envVarName = "APQ_CFG_RSA_KEY",
        Action<EncryptionOptions>? configure = null)
    {
        var key = Environment.GetEnvironmentVariable(envVarName)
            ?? throw new InvalidOperationException($"环境变量 {envVarName} 未设置");
        return builder.AddRsaEncryption(key, configure);
    }

    /// <summary>
    /// 添加 RSA 加密支持（从文件读取密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="keyFilePath">密钥文件路径</param>
    /// <param name="configure">加密选项配置</param>
    /// <returns>配置构建器</returns>
    public static CfgBuilder AddRsaEncryptionFromFile(
        this CfgBuilder builder,
        string keyFilePath,
        Action<EncryptionOptions>? configure = null)
    {
        var key = File.ReadAllText(keyFilePath);
        return builder.AddRsaEncryption(key, configure);
    }
}
