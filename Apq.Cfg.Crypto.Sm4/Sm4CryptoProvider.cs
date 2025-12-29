using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Apq.Cfg.Crypto.Sm4;

/// <summary>
/// SM4 国密算法加密提供者
/// 使用 GCM 模式提供认证加密
/// </summary>
public sealed class Sm4CryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    /// <summary>
    /// 使用字节数组密钥创建 SM4 加密提供者
    /// </summary>
    /// <param name="key">128 位密钥（16 字节）</param>
    public Sm4CryptoProvider(byte[] key)
    {
        if (key.Length != 16)
            throw new ArgumentException("Key must be 128 bits (16 bytes)", nameof(key));
        _key = (byte[])key.Clone();
    }

    /// <summary>
    /// 使用 Base64 编码的密钥创建 SM4 加密提供者
    /// </summary>
    /// <param name="base64Key">Base64 编码的密钥</param>
    public Sm4CryptoProvider(string base64Key)
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

        var cipher = new GcmBlockCipher(new SM4Engine());
        var parameters = new AeadParameters(new KeyParameter(_key), 128, nonce);
        cipher.Init(true, parameters);

        var cipherBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
        var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherBytes, 0);
        cipher.DoFinal(cipherBytes, len);

        // 格式: nonce(12) + cipher+tag
        var result = new byte[nonce.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length, cipherBytes.Length);

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
        var cipherBytes = new byte[data.Length - 12];
        Buffer.BlockCopy(data, 0, nonce, 0, 12);
        Buffer.BlockCopy(data, 12, cipherBytes, 0, cipherBytes.Length);

        var cipher = new GcmBlockCipher(new SM4Engine());
        var parameters = new AeadParameters(new KeyParameter(_key), 128, nonce);
        cipher.Init(false, parameters);

        var plainBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
        var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainBytes, 0);
        
        try
        {
            cipher.DoFinal(plainBytes, len);
        }
        catch (InvalidCipherTextException ex)
        {
            throw new CryptographicException("Decryption failed: authentication tag mismatch", ex);
        }

        // 移除填充
        var actualLength = plainBytes.Length;
        while (actualLength > 0 && plainBytes[actualLength - 1] == 0)
            actualLength--;

        return Encoding.UTF8.GetString(plainBytes, 0, actualLength);
    }

    /// <summary>
    /// 生成随机密钥
    /// </summary>
    /// <returns>Base64 编码的密钥</returns>
    public static string GenerateKey()
    {
        var key = new byte[16];
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
