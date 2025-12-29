using System.Security.Cryptography;
using System.Text;

namespace Apq.Cfg.Crypto.AesCbc;

/// <summary>
/// AES-CBC 加密提供者（带 HMAC 认证）
/// </summary>
public sealed class AesCbcCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _hmacKey;
    private bool _disposed;

    /// <summary>
    /// 使用字节数组密钥创建 AES-CBC 加密提供者
    /// </summary>
    /// <param name="encryptionKey">加密密钥（128、192 或 256 位）</param>
    /// <param name="hmacKey">HMAC 密钥（至少 256 位）</param>
    public AesCbcCryptoProvider(byte[] encryptionKey, byte[] hmacKey)
    {
        if (encryptionKey.Length != 16 && encryptionKey.Length != 24 && encryptionKey.Length != 32)
            throw new ArgumentException("Encryption key must be 128, 192, or 256 bits (16, 24, or 32 bytes)", nameof(encryptionKey));
        if (hmacKey.Length < 32)
            throw new ArgumentException("HMAC key must be at least 256 bits (32 bytes)", nameof(hmacKey));

        _encryptionKey = (byte[])encryptionKey.Clone();
        _hmacKey = (byte[])hmacKey.Clone();
    }

    /// <summary>
    /// 使用 Base64 编码的密钥创建 AES-CBC 加密提供者
    /// </summary>
    /// <param name="base64EncryptionKey">Base64 编码的加密密钥</param>
    /// <param name="base64HmacKey">Base64 编码的 HMAC 密钥</param>
    public AesCbcCryptoProvider(string base64EncryptionKey, string base64HmacKey)
        : this(Convert.FromBase64String(base64EncryptionKey), Convert.FromBase64String(base64HmacKey))
    {
    }

    /// <summary>
    /// 使用单个密钥创建 AES-CBC 加密提供者（自动派生 HMAC 密钥）
    /// </summary>
    /// <param name="masterKey">主密钥（至少 256 位）</param>
    public AesCbcCryptoProvider(byte[] masterKey)
    {
        if (masterKey.Length < 32)
            throw new ArgumentException("Master key must be at least 256 bits (32 bytes)", nameof(masterKey));

        // 使用 HKDF 派生加密密钥和 HMAC 密钥
        _encryptionKey = DeriveKey(masterKey, "encryption", 32);
        _hmacKey = DeriveKey(masterKey, "hmac", 32);
    }

    /// <summary>
    /// 使用 Base64 编码的主密钥创建 AES-CBC 加密提供者
    /// </summary>
    /// <param name="base64MasterKey">Base64 编码的主密钥</param>
    public AesCbcCryptoProvider(string base64MasterKey)
        : this(Convert.FromBase64String(base64MasterKey))
    {
    }

    /// <inheritdoc />
    public string Encrypt(string plainText)
    {
        ThrowIfDisposed();
        if (plainText == null) throw new ArgumentNullException(nameof(plainText));

        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // 格式: IV(16) + cipher + HMAC(32)
        var dataToSign = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, dataToSign, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, dataToSign, aes.IV.Length, cipherBytes.Length);

        using var hmac = new HMACSHA256(_hmacKey);
        var signature = hmac.ComputeHash(dataToSign);

        var result = new byte[dataToSign.Length + signature.Length];
        Buffer.BlockCopy(dataToSign, 0, result, 0, dataToSign.Length);
        Buffer.BlockCopy(signature, 0, result, dataToSign.Length, signature.Length);

        return Convert.ToBase64String(result);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        ThrowIfDisposed();
        if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));

        var data = Convert.FromBase64String(cipherText);

        if (data.Length < 48) // 16 (IV) + 16 (min cipher) + 32 (HMAC)
            throw new CryptographicException("Invalid cipher text: data too short");

        // 验证 HMAC
        var dataToVerify = new byte[data.Length - 32];
        var providedHmac = new byte[32];
        Buffer.BlockCopy(data, 0, dataToVerify, 0, dataToVerify.Length);
        Buffer.BlockCopy(data, dataToVerify.Length, providedHmac, 0, 32);

        using var hmac = new HMACSHA256(_hmacKey);
        var computedHmac = hmac.ComputeHash(dataToVerify);

        if (!CryptographicOperations.FixedTimeEquals(providedHmac, computedHmac))
            throw new CryptographicException("HMAC verification failed: data may have been tampered with");

        // 解密
        var iv = new byte[16];
        var cipherBytes = new byte[dataToVerify.Length - 16];
        Buffer.BlockCopy(dataToVerify, 0, iv, 0, 16);
        Buffer.BlockCopy(dataToVerify, 16, cipherBytes, 0, cipherBytes.Length);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// 生成随机密钥对
    /// </summary>
    /// <param name="keySize">密钥大小（128、192 或 256）</param>
    /// <returns>Base64 编码的加密密钥和 HMAC 密钥</returns>
    public static (string EncryptionKey, string HmacKey) GenerateKeys(int keySize = 256)
    {
        if (keySize != 128 && keySize != 192 && keySize != 256)
            throw new ArgumentException("Key size must be 128, 192, or 256 bits", nameof(keySize));

        var encryptionKey = new byte[keySize / 8];
        var hmacKey = new byte[32];

        RandomNumberGenerator.Fill(encryptionKey);
        RandomNumberGenerator.Fill(hmacKey);

        return (Convert.ToBase64String(encryptionKey), Convert.ToBase64String(hmacKey));
    }

    /// <summary>
    /// 生成随机主密钥
    /// </summary>
    /// <returns>Base64 编码的主密钥</returns>
    public static string GenerateMasterKey()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }

    private static byte[] DeriveKey(byte[] masterKey, string purpose, int length)
    {
        using var hmac = new HMACSHA256(masterKey);
        var purposeBytes = Encoding.UTF8.GetBytes(purpose);
        var hash = hmac.ComputeHash(purposeBytes);
        var result = new byte[length];
        Buffer.BlockCopy(hash, 0, result, 0, Math.Min(hash.Length, length));
        return result;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            CryptographicOperations.ZeroMemory(_encryptionKey);
            CryptographicOperations.ZeroMemory(_hmacKey);
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }
}
