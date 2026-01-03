using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// RSA 加密提供者（使用 BouncyCastle 实现）
/// </summary>
public class RsaCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly IAsymmetricBlockCipher _cipher;
    private readonly RsaKeyParameters _keyParameters;

    public RsaCryptoProvider(RsaKeyParameters keyParameters)
    {
        _keyParameters = keyParameters;
        _cipher = new Pkcs1Encoding(new RsaEngine());
    }

    public static RsaCryptoProvider FromPem(string pem)
    {
        using var reader = new StringReader(pem);
        var pemObject = new PemReader(reader).ReadObject();

        return pemObject switch
        {
            RsaPrivateCrtKeyParameters privateKey => new RsaCryptoProvider(privateKey),
            RsaKeyParameters publicKey => new RsaCryptoProvider(publicKey),
            _ => throw new ArgumentException("Invalid PEM format: not an RSA key")
        };
    }

    public static RsaCryptoProvider FromPemFile(string pemFilePath)
    {
        var pem = File.ReadAllText(pemFilePath);
        return FromPem(pem);
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);

        if (_keyParameters.IsPrivate)
            throw new InvalidOperationException("Cannot encrypt with private key");

        _cipher.Init(true, _keyParameters);
        var cipherBytes = _cipher.ProcessBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(cipherBytes);
    }

    public string Decrypt(string cipherText)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);

        if (!_keyParameters.IsPrivate)
            throw new InvalidOperationException("Cannot decrypt with public key");

        _cipher.Init(false, _keyParameters);
        var plainBytes = _cipher.ProcessBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public void Dispose()
    {
        // RSA key parameters are immutable, nothing to clear
    }
}
