using System.Buffers;
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
/// <remarks>
/// 性能优化：
/// 1. 使用静态 SecureRandom 实例避免重复创建
/// 2. 使用 ArrayPool 减少内存分配
/// </remarks>
[Obsolete("Triple DES is considered weak. Use AES-GCM for new projects.")]
public class TripleDesCryptoProvider : ICryptoProvider, IDisposable
{
    private static readonly SecureRandom SharedRandom = new();
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
        SharedRandom.NextBytes(iv);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEdeEngine()), new Pkcs7Padding());
        cipher.Init(true, new ParametersWithIV(new DesEdeParameters(_key), iv));

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

        if (data.Length < BlockSize)
            throw new ArgumentException("Invalid cipher text");

        var iv = new byte[BlockSize];
        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);

        var cipherLength = data.Length - BlockSize;
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEdeEngine()), new Pkcs7Padding());
        cipher.Init(false, new ParametersWithIV(new DesEdeParameters(_key), iv));

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

    public void Dispose()
    {
        Array.Clear(_key, 0, _key.Length);
    }
}
