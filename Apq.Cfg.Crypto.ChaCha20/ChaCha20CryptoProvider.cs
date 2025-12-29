using System.Security.Cryptography;
using System.Text;

namespace Apq.Cfg.Crypto.ChaCha20;

/// <summary>
/// ChaCha20-Poly1305 加密提供者
/// 高性能认证加密，特别适合移动端和嵌入式设备
/// </summary>
public sealed class ChaCha20CryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    /// <summary>
    /// 使用字节数组密钥创建 ChaCha20-Poly1305 加密提供者
    /// </summary>
    /// <param name="key">256 位密钥</param>
    public ChaCha20CryptoProvider(byte[] key)
    {
        if (key.Length != 32)
            throw new ArgumentException("Key must be 256 bits (32 bytes)", nameof(key));
        _key = (byte[])key.Clone();
    }

    /// <summary>
    /// 使用 Base64 编码的密钥创建 ChaCha20-Poly1305 加密提供者
    /// </summary>
    /// <param name="base64Key">Base64 编码的密钥</param>
    public ChaCha20CryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    /// <inheritdoc />
    public string Encrypt(string plainText)
    {
        ThrowIfDisposed();
        if (plainText == null) throw new ArgumentNullException(nameof(plainText));

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[16];

        using var chacha = new ChaCha20Poly1305(_key);
        chacha.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // 格式: nonce(12) + tag(16) + cipher
        var result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        ThrowIfDisposed();
        if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));

        var data = Convert.FromBase64String(cipherText);

        if (data.Length < 28) // 12 (nonce) + 16 (tag)
            throw new CryptographicException("Invalid cipher text: data too short");

        var nonce = new byte[12];
        var tag = new byte[16];
        var cipherBytes = new byte[data.Length - 28];

        Buffer.BlockCopy(data, 0, nonce, 0, 12);
        Buffer.BlockCopy(data, 12, tag, 0, 16);
        Buffer.BlockCopy(data, 28, cipherBytes, 0, cipherBytes.Length);

        var plainBytes = new byte[cipherBytes.Length];

        using var chacha = new ChaCha20Poly1305(_key);
        chacha.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// 生成随机密钥
    /// </summary>
    /// <returns>Base64 编码的密钥</returns>
    public static string GenerateKey()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            CryptographicOperations.ZeroMemory(_key);
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }
}
