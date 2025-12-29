using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// Triple DES 加密提供者（使用 BouncyCastle 实现）
/// </summary>
[Obsolete("Triple DES is considered weak. Use AES-GCM for new projects.")]
public class TripleDesCryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private const int BlockSize = 8;

    public TripleDesCryptoProvider(byte[] key)
    {
        if (key.Length != 16 && key.Length != 24)
            throw new ArgumentException("Triple DES key must be 128 or 192 bits (16 or 24 bytes)");
        _key = (byte[])key.Clone();
    }

    public TripleDesCryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var iv = new byte[BlockSize];
        var random = new SecureRandom();
        random.NextBytes(iv);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEdeEngine()), new Pkcs7Padding());
        cipher.Init(true, new ParametersWithIV(new DesEdeParameters(_key), iv));

        var cipherBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
        var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherBytes, 0);
        cipher.DoFinal(cipherBytes, len);

        // 格式: IV(8) + cipher
        var result = new byte[iv.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        if (data.Length < BlockSize)
            throw new ArgumentException("Invalid cipher text");

        var iv = new byte[BlockSize];
        var cipherBytes = new byte[data.Length - BlockSize];

        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);
        Buffer.BlockCopy(data, BlockSize, cipherBytes, 0, cipherBytes.Length);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEdeEngine()), new Pkcs7Padding());
        cipher.Init(false, new ParametersWithIV(new DesEdeParameters(_key), iv));

        var plainBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
        var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainBytes, 0);
        len += cipher.DoFinal(plainBytes, len);

        return Encoding.UTF8.GetString(plainBytes, 0, len);
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}
