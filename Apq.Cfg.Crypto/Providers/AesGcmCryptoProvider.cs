using System.Buffers;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// AES-GCM 加密提供者（使用 BouncyCastle 实现）
/// </summary>
/// <remarks>
/// 性能优化：
/// 1. 使用静态 SecureRandom 实例避免重复创建
/// 2. 使用 ArrayPool 减少内存分配
/// </remarks>
public class AesGcmCryptoProvider : ICryptoProvider, IDisposable
{
    private static readonly SecureRandom SharedRandom = new();
    private readonly byte[] _key;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public AesGcmCryptoProvider(byte[] key)
    {
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("Key must be 128, 192, or 256 bits");
        _key = (byte[])key.Clone();
    }

    public AesGcmCryptoProvider(string base64Key)
        : this(Convert.FromBase64String(base64Key))
    {
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[NonceSize];
        SharedRandom.NextBytes(nonce);

        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(_key), TagSize * 8, nonce);
        cipher.Init(true, parameters);

        var outputSize = cipher.GetOutputSize(plainBytes.Length);
        var resultSize = NonceSize + outputSize;
        var result = ArrayPool<byte>.Shared.Rent(resultSize);
        try
        {
            // 直接写入 nonce 到结果数组
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);

            // 加密数据直接写入结果数组的 nonce 之后
            var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, result, NonceSize);
            cipher.DoFinal(result, NonceSize + len);

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

        var nonce = new byte[NonceSize];
        Buffer.BlockCopy(data, 0, nonce, 0, NonceSize);

        var cipherLength = data.Length - NonceSize;
        var cipher = new GcmBlockCipher(new AesEngine());
        var parameters = new AeadParameters(new KeyParameter(_key), TagSize * 8, nonce);
        cipher.Init(false, parameters);

        var outputSize = cipher.GetOutputSize(cipherLength);
        var plainBytes = ArrayPool<byte>.Shared.Rent(outputSize);
        try
        {
            var len = cipher.ProcessBytes(data, NonceSize, cipherLength, plainBytes, 0);
            cipher.DoFinal(plainBytes, len);

            return Encoding.UTF8.GetString(plainBytes, 0, outputSize);
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
