using System.Buffers;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// AES-CBC 加密提供者（使用 BouncyCastle 实现）
/// </summary>
/// <remarks>
/// 性能优化：
/// 1. 使用静态 SecureRandom 实例避免重复创建
/// 2. 使用 ArrayPool 减少内存分配
/// </remarks>
public class AesCbcCryptoProvider : ICryptoProvider, IDisposable
{
    private static readonly SecureRandom SharedRandom = new();
    private readonly byte[] _encryptionKey;
    private readonly byte[] _hmacKey;
    private const int BlockSize = 16;
    private const int HmacSize = 32;

    public AesCbcCryptoProvider(byte[] encryptionKey, byte[] hmacKey)
    {
        if (encryptionKey.Length != 16 && encryptionKey.Length != 24 && encryptionKey.Length != 32)
            throw new ArgumentException("Encryption key must be 128, 192, or 256 bits");
        if (hmacKey.Length != 16 && hmacKey.Length != 24 && hmacKey.Length != 32)
            throw new ArgumentException("HMAC key must be 128, 192, or 256 bits");

        _encryptionKey = (byte[])encryptionKey.Clone();
        _hmacKey = (byte[])hmacKey.Clone();
    }

    public AesCbcCryptoProvider(string base64EncryptionKey, string base64HmacKey)
        : this(Convert.FromBase64String(base64EncryptionKey), Convert.FromBase64String(base64HmacKey))
    {
    }

    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var iv = new byte[BlockSize];
        SharedRandom.NextBytes(iv);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
        cipher.Init(true, new ParametersWithIV(new KeyParameter(_encryptionKey), iv));

        var cipherOutputSize = cipher.GetOutputSize(plainBytes.Length);
        var resultSize = BlockSize + HmacSize + cipherOutputSize;
        var result = ArrayPool<byte>.Shared.Rent(resultSize);
        try
        {
            // 写入 IV
            Buffer.BlockCopy(iv, 0, result, 0, BlockSize);

            // 加密数据写入 IV + HMAC 之后的位置
            var cipherOffset = BlockSize + HmacSize;
            var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, result, cipherOffset);
            cipher.DoFinal(result, cipherOffset + len);

            // 计算 HMAC (IV + ciphertext)
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(_hmacKey));
            hmac.BlockUpdate(iv, 0, BlockSize);
            hmac.BlockUpdate(result, cipherOffset, cipherOutputSize);
            hmac.DoFinal(result, BlockSize); // HMAC 写入 IV 之后

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

        if (data.Length < BlockSize + HmacSize)
            throw new ArgumentException("Invalid cipher text");

        var cipherLength = data.Length - BlockSize - HmacSize;

        // 验证 HMAC
        var hmac = new HMac(new Sha256Digest());
        hmac.Init(new KeyParameter(_hmacKey));
        hmac.BlockUpdate(data, 0, BlockSize); // IV
        hmac.BlockUpdate(data, BlockSize + HmacSize, cipherLength); // ciphertext
        var expectedHmac = new byte[HmacSize];
        hmac.DoFinal(expectedHmac, 0);

        if (!ConstantTimeEquals(data, BlockSize, expectedHmac, 0, HmacSize))
            throw new InvalidOperationException("HMAC verification failed");

        // 解密
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
        cipher.Init(false, new ParametersWithIV(new KeyParameter(_encryptionKey), data.AsSpan(0, BlockSize).ToArray()));

        var outputSize = cipher.GetOutputSize(cipherLength);
        var plainBytes = ArrayPool<byte>.Shared.Rent(outputSize);
        try
        {
            var len = cipher.ProcessBytes(data, BlockSize + HmacSize, cipherLength, plainBytes, 0);
            len += cipher.DoFinal(plainBytes, len);

            return Encoding.UTF8.GetString(plainBytes, 0, len);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(plainBytes, clearArray: true);
        }
    }

    private static bool ConstantTimeEquals(byte[] a, int aOffset, byte[] b, int bOffset, int length)
    {
        uint result = 0;
        for (int i = 0; i < length; i++)
            result |= (uint)(a[aOffset + i] ^ b[bOffset + i]);

        return result == 0;
    }

    public void Dispose()
    {
        Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
        Array.Clear(_hmacKey, 0, _hmacKey.Length);
    }
}
