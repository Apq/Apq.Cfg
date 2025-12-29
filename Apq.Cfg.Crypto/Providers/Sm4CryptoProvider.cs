using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// SM4 国密算法加密提供者（使用 BouncyCastle 实现）
/// </summary>
public class Sm4CryptoProvider : ICryptoProvider, IDisposable
{
    private readonly byte[] _key;
    private readonly Sm4Mode _mode;
    private const int BlockSize = 16;

    public Sm4CryptoProvider(byte[] key, Sm4Mode mode = Sm4Mode.CBC)
    {
        if (key.Length != 16)
            throw new ArgumentException("SM4 key must be 128 bits");
        _key = (byte[])key.Clone();
        _mode = mode;
    }

    public Sm4CryptoProvider(string base64Key, Sm4Mode mode = Sm4Mode.CBC)
        : this(Convert.FromBase64String(base64Key), mode)
    {
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var iv = new byte[BlockSize];
        var random = new SecureRandom();
        random.NextBytes(iv);

        var cipher = CreateCipher(true, iv);
        var cipherBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
        var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherBytes, 0);
        cipher.DoFinal(cipherBytes, len);

        // 格式: IV(16) + cipher
        var result = new byte[iv.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        var iv = new byte[BlockSize];
        var cipherBytes = new byte[data.Length - BlockSize];
        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);
        Buffer.BlockCopy(data, BlockSize, cipherBytes, 0, cipherBytes.Length);

        var cipher = CreateCipher(false, iv);
        var plainBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
        var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainBytes, 0);
        len += cipher.DoFinal(plainBytes, len);

        return Encoding.UTF8.GetString(plainBytes, 0, len);
    }

    private IBufferedCipher CreateCipher(bool forEncryption, byte[] iv)
    {
        var engine = new SM4Engine();
        IBufferedCipher cipher;

        switch (_mode)
        {
            case Sm4Mode.CBC:
                cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                cipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(_key), iv));
                break;
            case Sm4Mode.ECB:
                cipher = new PaddedBufferedBlockCipher(engine, new Pkcs7Padding());
                cipher.Init(forEncryption, new KeyParameter(_key));
                break;
            default:
                throw new NotSupportedException($"SM4 mode {_mode} is not supported");
        }

        return cipher;
    }

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}

public enum Sm4Mode
{
    CBC,
    ECB
}
