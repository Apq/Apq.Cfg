using Microsoft.AspNetCore.DataProtection;

namespace Apq.Cfg.Crypto.DataProtection;

/// <summary>
/// CfgBuilder 的 Data Protection 加密扩展方法
/// </summary>
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加 Data Protection 加密支持
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="provider">Data Protection 提供者</param>
    /// <param name="purpose">保护目的，用于隔离不同用途的加密数据</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var dataProtectionProvider = DataProtectionProvider.Create("MyApp");
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddDataProtectionEncryption(dataProtectionProvider)
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddDataProtectionEncryption(
        this CfgBuilder builder,
        IDataProtectionProvider provider,
        string purpose = "Apq.Cfg",
        Action<EncryptionOptions>? configure = null)
    {
        var cryptoProvider = new DataProtectionCryptoProvider(provider, purpose);
        return builder.AddEncryption(cryptoProvider, configure);
    }

    /// <summary>
    /// 添加 Data Protection 加密支持（使用默认提供者）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="applicationName">应用程序名称，用于密钥隔离</param>
    /// <param name="purpose">保护目的</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddDataProtectionEncryption("MyApp")
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddDataProtectionEncryption(
        this CfgBuilder builder,
        string applicationName,
        string purpose = "Apq.Cfg",
        Action<EncryptionOptions>? configure = null)
    {
        var provider = DataProtectionProvider.Create(applicationName);
        return builder.AddDataProtectionEncryption(provider, purpose, configure);
    }

    /// <summary>
    /// 添加 Data Protection 加密支持（使用指定目录存储密钥）
    /// </summary>
    /// <param name="builder">配置构建器</param>
    /// <param name="keyDirectory">密钥存储目录</param>
    /// <param name="applicationName">应用程序名称</param>
    /// <param name="purpose">保护目的</param>
    /// <param name="configure">加密选项配置委托</param>
    /// <returns>配置构建器实例，支持链式调用</returns>
    /// <example>
    /// <code>
    /// var cfg = new CfgBuilder()
    ///     .AddJsonFile("config.json", level: 0)
    ///     .AddDataProtectionEncryption(new DirectoryInfo("/keys"), "MyApp")
    ///     .Build();
    /// </code>
    /// </example>
    public static CfgBuilder AddDataProtectionEncryption(
        this CfgBuilder builder,
        DirectoryInfo keyDirectory,
        string applicationName,
        string purpose = "Apq.Cfg",
        Action<EncryptionOptions>? configure = null)
    {
        var provider = DataProtectionProvider.Create(keyDirectory, options =>
        {
            options.SetApplicationName(applicationName);
        });
        return builder.AddDataProtectionEncryption(provider, purpose, configure);
    }
}
