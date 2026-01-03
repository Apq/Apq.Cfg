using Microsoft.AspNetCore.DataProtection;

namespace Apq.Cfg.Crypto.DataProtection;

/// <summary>
/// 使用 ASP.NET Core Data Protection 的加密提供者
/// </summary>
/// <remarks>
/// Data Protection 提供了跨机器、跨应用的密钥管理和加密功能。
/// 适用于 ASP.NET Core 应用程序，支持密钥轮换和撤销。
/// </remarks>
public class DataProtectionCryptoProvider : ICryptoProvider
{
    private readonly IDataProtector _protector;

    /// <summary>
    /// 初始化 Data Protection 加密提供者
    /// </summary>
    /// <param name="provider">Data Protection 提供者</param>
    /// <param name="purpose">保护目的，用于隔离不同用途的加密数据</param>
    public DataProtectionCryptoProvider(IDataProtectionProvider provider, string purpose = "Apq.Cfg")
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));
        _protector = provider.CreateProtector(purpose);
    }

    /// <summary>
    /// 加密明文
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <returns>加密后的字符串</returns>
    public string Encrypt(string plainText)
    {
        return _protector.Protect(plainText);
    }

    /// <summary>
    /// 解密密文
    /// </summary>
    /// <param name="cipherText">密文</param>
    /// <returns>明文</returns>
    public string Decrypt(string cipherText)
    {
        return _protector.Unprotect(cipherText);
    }
}
