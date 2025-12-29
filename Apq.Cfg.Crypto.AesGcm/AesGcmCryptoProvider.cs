using System.Security.Cryptography;
using System.Text;

namespace Apq.Cfg.Crypto.AesGcm;

/// <summary>
/// AES-GCM 加密提供者
/// </summary>
/// <remarks>
/// AES-GCM 是一种认证加密算法，提供数据机密性和完整性保护。
/// 密文格式：nonce(12字节) + tag(16字节) + cipher
/// </remarks>
public class AesGcmCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    private const int NonceSize = 12;
    private const int TagSize = 16;

    /// <summary>
    /// 使用字节数组密钥初始化 AES-GCM 加密提供者
    /// </summary>
    /// <param name="key">加密密钥，必须是 128、192 或 256 位（16、24 或 32 字节）</param>
    /// <exception cref="ArgumentException">密钥长度不正确时抛出</exception>
    public AesGcmCryptoProvider(byte[] key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("密钥必须是 128、192 或 256 位（16、24 或 32 字节）", nameof(key));
        _key = (byte[])key.Clone();
    }

    /// <summary>
    /// 使用 Base64 编码的密钥初始化 AES-GCM 加密提供者
    /// </summary>
    /// <param name="base64Key">Base64 编码的加密密钥</param>
    /// <exception cref="ArgumentException">密钥长度不正确时抛出</exception>
    public AesGcmCryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    /// <summary>
    /// 加密明文
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <returns>Base64 编码的密文</returns>
    public string Encrypt(string plainText)
    {
        ThrowIfDisposed();

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[NonceSize];
        RandomNumberGenerator.Fill(nonce);

        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

#if NET8_0_OR_GREATER
        using var aes = new System.Security.Cryptography.AesGcm(_key, TagSize);
#else
        using var aes = new System.Security.Cryptography.AesGcm(_key);
#endif
        aes.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // 格式: nonce(12) + tag(16) + cipher
        var result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// 解密密文
    /// </summary>
    /// <param name="cipherText">Base64 编码的密文</param>
    /// <returns>明文</returns>
    /// <exception cref="ArgumentException">密文格式不正确时抛出</exception>
    public string Decrypt(string cipherText)
    {
        ThrowIfDisposed();

        var data = Convert.FromBase64String(cipherText);

        if (data.Length < NonceSize + TagSize)
            throw new ArgumentException("密文格式不正确：长度不足", nameof(cipherText));

        var nonce = new byte[NonceSize];
        var tag = new byte[TagSize];
        var cipherBytes = new byte[data.Length - NonceSize - TagSize];

        Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(data, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

        var plainBytes = new byte[cipherBytes.Length];

#if NET8_0_OR_GREATER
        using var aes = new System.Security.Cryptography.AesGcm(_key, TagSize);
#else
        using var aes = new System.Security.Cryptography.AesGcm(_key);
#endif
        aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AesGcmCryptoProvider));
    }

    /// <summary>
    /// 释放资源，清除内存中的密钥
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        Array.Clear(_key, 0, _key.Length);
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
