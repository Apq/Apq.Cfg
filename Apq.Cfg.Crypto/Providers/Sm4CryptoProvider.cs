using System.Buffers;
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
/// <remarks>
/// 性能优化：
/// 1. 使用静态 SecureRandom 实例避免重复创建
/// 2. 使用 ArrayPool 减少内存分配
/// </remarks>
public class Sm4CryptoProvider : ICryptoProvider, IDisposable
{
    private static readonly SecureRandom SharedRandom = new();
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
        SharedRandom.NextBytes(iv);

        var cipher = CreateCipher(true, iv);
        var outputSize = cipher.GetOutputSize(plainBytes.Length);
        var resultSize = BlockSize + outputSize;
        var result = ArrayPool<byte>.Shared.Rent(resultSize);
        try
        {
            // 写入 IV
            Buffer.BlockCopy(iv, 0, result, 0, BlockSize);

            // 加密数据写入 IV 之后
            var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, result, BlockSize);
            cipher.DoFinal(result, BlockSize + len);

            return Convert.ToBase64String(result, 0, resultSize);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(result, clearArray: true);
        }
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        var iv = new byte[BlockSize];
        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);

        var cipherLength = data.Length - BlockSize;
        var cipher = CreateCipher(false, iv);
        var outputSize = cipher.GetOutputSize(cipherLength);
        var plainBytes = ArrayPool<byte>.Shared.Rent(outputSize);
        try
        {
            var len = cipher.ProcessBytes(data, BlockSize, cipherLength, plainBytes, 0);
            len += cipher.DoFinal(plainBytes, len);

            return Encoding.UTF8.GetString(plainBytes, 0, len);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(plainBytes, clearArray: true);
        }
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
