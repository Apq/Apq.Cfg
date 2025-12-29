using System.Security.Cryptography;
using System.Text;

namespace Apq.Cfg.Crypto.Rsa;

/// <summary>
/// RSA 非对称加密提供者
/// 适用于密钥分发场景，公钥加密、私钥解密
/// </summary>
public sealed class RsaCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly RSA _rsa;
    private readonly RSAEncryptionPadding _padding;
    private bool _disposed;

    /// <summary>
    /// 使用 RSA 实例创建加密提供者
    /// </summary>
    /// <param name="rsa">RSA 实例</param>
    /// <param name="padding">填充模式</param>
    public RsaCryptoProvider(RSA rsa, RSAEncryptionPadding? padding = null)
    {
        _rsa = rsa ?? throw new ArgumentNullException(nameof(rsa));
        _padding = padding ?? RSAEncryptionPadding.OaepSHA256;
    }

    /// <summary>
    /// 使用 PEM 格式的密钥创建加密提供者
    /// </summary>
    /// <param name="pemKey">PEM 格式的公钥或私钥</param>
    /// <param name="padding">填充模式</param>
    public RsaCryptoProvider(string pemKey, RSAEncryptionPadding? padding = null)
    {
        _rsa = RSA.Create();
        _padding = padding ?? RSAEncryptionPadding.OaepSHA256;

        if (pemKey.Contains("PRIVATE KEY"))
        {
            _rsa.ImportFromPem(pemKey);
        }
        else if (pemKey.Contains("PUBLIC KEY"))
        {
            _rsa.ImportFromPem(pemKey);
        }
        else
        {
            throw new ArgumentException("Invalid PEM key format", nameof(pemKey));
        }
    }

    /// <summary>
    /// 使用 XML 格式的密钥创建加密提供者
    /// </summary>
    /// <param name="xmlKey">XML 格式的密钥</param>
    /// <param name="padding">填充模式</param>
    /// <param name="isXml">标记为 XML 格式</param>
    public RsaCryptoProvider(string xmlKey, RSAEncryptionPadding? padding, bool isXml)
    {
        _rsa = RSA.Create();
        _padding = padding ?? RSAEncryptionPadding.OaepSHA256;
        _rsa.FromXmlString(xmlKey);
    }

    /// <inheritdoc />
    public string Encrypt(string plainText)
    {
        ThrowIfDisposed();
        if (plainText == null) throw new ArgumentNullException(nameof(plainText));

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = _rsa.Encrypt(plainBytes, _padding);
        return Convert.ToBase64String(cipherBytes);
    }

    /// <inheritdoc />
    public string Decrypt(string cipherText)
    {
        ThrowIfDisposed();
        if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));

        var cipherBytes = Convert.FromBase64String(cipherText);
        var plainBytes = _rsa.Decrypt(cipherBytes, _padding);
        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// 生成 RSA 密钥对
    /// </summary>
    /// <param name="keySize">密钥大小（2048、3072 或 4096）</param>
    /// <returns>PEM 格式的公钥和私钥</returns>
    public static (string PublicKey, string PrivateKey) GenerateKeyPair(int keySize = 2048)
    {
        if (keySize < 2048)
            throw new ArgumentException("Key size must be at least 2048 bits for security", nameof(keySize));

        using var rsa = RSA.Create(keySize);
#if NET7_0_OR_GREATER
        var publicKey = rsa.ExportRSAPublicKeyPem();
        var privateKey = rsa.ExportRSAPrivateKeyPem();
#else
        var publicKey = ExportToPem(rsa.ExportRSAPublicKey(), "RSA PUBLIC KEY");
        var privateKey = ExportToPem(rsa.ExportRSAPrivateKey(), "RSA PRIVATE KEY");
#endif
        return (publicKey, privateKey);
    }

    /// <summary>
    /// 生成 RSA 密钥对（XML 格式）
    /// </summary>
    /// <param name="keySize">密钥大小</param>
    /// <returns>XML 格式的公钥和私钥</returns>
    public static (string PublicKey, string PrivateKey) GenerateKeyPairXml(int keySize = 2048)
    {
        if (keySize < 2048)
            throw new ArgumentException("Key size must be at least 2048 bits for security", nameof(keySize));

        using var rsa = RSA.Create(keySize);
        var publicKey = rsa.ToXmlString(false);
        var privateKey = rsa.ToXmlString(true);
        return (publicKey, privateKey);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _rsa.Dispose();
            _disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().FullName);
    }

#if !NET7_0_OR_GREATER
    private static string ExportToPem(byte[] data, string label)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"-----BEGIN {label}-----");
        sb.AppendLine(Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks));
        sb.AppendLine($"-----END {label}-----");
        return sb.ToString();
    }
#endif
}
