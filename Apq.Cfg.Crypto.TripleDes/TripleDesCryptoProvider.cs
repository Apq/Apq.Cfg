using System.Security.Cryptography;
using System.Text;

namespace Apq.Cfg.Crypto.TripleDes;

/// <summary>
/// Triple DES 加密提供者
/// 用于遗留系统兼容，新项目建议使用 AES-GCM
/// </summary>
[Obsolete("Triple DES is considered legacy. Use AES-GCM for new projects.")]
public sealed class TripleDesCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    /// <summary>
    /// 使用字节数组密钥创建 Triple DES 加密提供者
    /// </summary>
    /// <param name="key">192 位密钥（24 字节）</param>
    public TripleDesCryptoProvider(byte[] key)
    {
        if (key.Length != 24)
            throw new ArgumentException("Key must be 192 bits (24 bytes)", nameof(key));
        _key = (byte[])key.Clone();
    }

    /// <summary>
    /// 使用 Base64 编码的密钥创建 Triple DES 加密提供者
    /// </summary>
    /// <param name="base64Key">Base64 编码的密钥</param>
    public TripleDesCryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    /// <inheritdoc />
    public string Encrypt(string plainText)
    {
        ThrowIfDisposed();
        if (plainText == null) throw new ArgumentNullException(nameof(plainText));

        var plainBytes = Encoding.UTF8.GetBytes(plainText);

#pragma warning disable SYSLIB0022 // TripleDES is obsolete
        using var tdes = TripleDES.Create();
#pragma warning restore SYSLIB0022
        tdes.Key = _key;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.PKCS7;
        tdes.GenerateIV();

        using var encryptor = tdes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // 格式: IV(8) + cipher
        var result = new byte[tdes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(tdes.IV, 0, result, 0, tdes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, tdes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        ThrowIfDisposed();
        if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));

        var data = Convert.FromBase64String(cipherText);

        if (data.Length < 16) // 8 (IV) + 8 (min cipher)
            throw new CryptographicException("Invalid cipher text: data too short");

        var iv = new byte[8];
        var cipherBytes = new byte[data.Length - 8];
        Buffer.BlockCopy(data, 0, iv, 0, 8);
        Buffer.BlockCopy(data, 8, cipherBytes, 0, cipherBytes.Length);

#pragma warning disable SYSLIB0022 // TripleDES is obsolete
        using var tdes = TripleDES.Create();
#pragma warning restore SYSLIB0022
        tdes.Key = _key;
        tdes.IV = iv;
        tdes.Mode = CipherMode.CBC;
        tdes.Padding = PaddingMode.PKCS7;

        using var decryptor = tdes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// 生成随机密钥
    /// </summary>
    /// <returns>Base64 编码的密钥</returns>
    public static string GenerateKey()
    {
        var key = new byte[24];
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
