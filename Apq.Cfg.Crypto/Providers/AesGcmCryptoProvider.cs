using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// AES-GCM 加密提供者（使用 BouncyCastle 实现）
/// </summary>
public class AesGcmCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public AesGcmCryptoProvider(byte[] key)
    {
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("Key must be 128, 192, or 256 bits");
        _key = key;
    }

    public AesGcmCryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[NonceSize];
        var random = new SecureRandom();
        random.NextBytes(nonce);

        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(_key), TagSize * 8, nonce);
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

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        var nonce = new byte[NonceSize];
        var cipherBytes = new byte[data.Length - NonceSize];

        Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(data, NonceSize, cipherBytes, 0, cipherBytes.Length);

        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(_key), TagSize * 8, nonce);
        cipher.Init(false, parameters);

        var plainBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
        var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainBytes, 0);
        cipher.DoFinal(plainBytes, len);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}
