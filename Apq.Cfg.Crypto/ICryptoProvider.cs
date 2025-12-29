namespace Apq.Cfg.Crypto;

/// <summary>
/// 加密提供者接口
/// </summary>
public interface ICryptoProvider
{
    /// <summary>
    /// 加密明文
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <returns>密文（Base64 编码）</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// 解密密文
    /// </summary>
    /// <param name="cipherText">密文（Base64 编码）</param>
    /// <returns>明文</returns>
    string Decrypt(string cipherText);
}
