using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Apq.Cfg.Crypto.Providers;

/// <summary>
/// AES-CBC 加密提供者（使用 BouncyCastle 实现）
/// </summary>
public class AesCbcCryptoProvider : ICryptoProvider, IDisposable
{
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
        var random = new SecureRandom();
        random.NextBytes(iv);

        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
        cipher.Init(true, new ParametersWithIV(new KeyParameter(_encryptionKey), iv));

        var cipherBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
        var len = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, cipherBytes, 0);
        cipher.DoFinal(cipherBytes, len);

        // 计算HMAC
        var hmac = new Org.BouncyCastle.Crypto.Macs.HMac(new Org.BouncyCastle.Crypto.Digests.Sha256Digest());
        hmac.Init(new KeyParameter(_hmacKey));
        hmac.BlockUpdate(iv, 0, iv.Length);
        hmac.BlockUpdate(cipherBytes, 0, cipherBytes.Length);
        var hmacBytes = new byte[hmac.GetMacSize()];
        hmac.DoFinal(hmacBytes, 0);

        // 格式: iv(16) + hmac(32) + cipher
        var result = new byte[iv.Length + hmacBytes.Length + cipherBytes.Length];
        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(hmacBytes, 0, result, iv.Length, hmacBytes.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, iv.Length + hmacBytes.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);

        if (data.Length < BlockSize + HmacSize)
            throw new ArgumentException("Invalid cipher text");

        var iv = new byte[BlockSize];
        var hmacBytes = new byte[HmacSize];
        var cipherBytes = new byte[data.Length - BlockSize - HmacSize];

        Buffer.BlockCopy(data, 0, iv, 0, BlockSize);
        Buffer.BlockCopy(data, BlockSize, hmacBytes, 0, HmacSize);
        Buffer.BlockCopy(data, BlockSize + HmacSize, cipherBytes, 0, cipherBytes.Length);

        // 验证HMAC
        var hmac = new Org.BouncyCastle.Crypto.Macs.HMac(new Org.BouncyCastle.Crypto.Digests.Sha256Digest());
        hmac.Init(new KeyParameter(_hmacKey));
        hmac.BlockUpdate(iv, 0, iv.Length);
        hmac.BlockUpdate(cipherBytes, 0, cipherBytes.Length);
        var expectedHmac = new byte[hmac.GetMacSize()];
        hmac.DoFinal(expectedHmac, 0);

        if (!ConstantTimeEquals(hmacBytes, expectedHmac))
            throw new InvalidOperationException("HMAC verification failed");

        // 解密
        var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
        cipher.Init(false, new ParametersWithIV(new KeyParameter(_encryptionKey), iv));

        var plainBytes = new byte[cipher.GetOutputSize(cipherBytes.Length)];
        var len = cipher.ProcessBytes(cipherBytes, 0, cipherBytes.Length, plainBytes, 0);
        len += cipher.DoFinal(plainBytes, len);

        return Encoding.UTF8.GetString(plainBytes, 0, len);
    }

    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        uint result = 0;
        for (int i = 0; i < a.Length; i++)
            result |= (uint)(a[i] ^ b[i]);

        return result == 0;
    }

    public void Dispose()
    {
        Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
        Array.Clear(_hmacKey, 0, _hmacKey.Length);
    }
}
