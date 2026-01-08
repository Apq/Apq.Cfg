using System.Security.Cryptography;
using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.Providers;
using BenchmarkDotNet.Attributes;

namespace Apq.Cfg.Benchmarks;

/// <summary>
/// 加密脱敏性能基准测试
/// 测试各种加密算法、脱敏操作、缓存效果等场景
/// </summary>
[Config(typeof(BenchmarkConfig))]
public class CryptoBenchmarks : IDisposable
{
    private readonly string _testDir;
    private readonly string _aesKey;
    private readonly string _aesKey128;
    private readonly string _chacha20Key;
    private readonly string _sm4Key;
    private readonly string _tripleDesKey;
    private readonly string _encKey;
    private readonly string _hmacKey;

    private AesGcmCryptoProvider _aesGcmProvider = null!;
    private AesCbcCryptoProvider _aesCbcProvider = null!;
    private ChaCha20CryptoProvider _chacha20Provider = null!;
    private Sm4CryptoProvider _sm4Provider = null!;
#pragma warning disable CS0618 // Type or member is obsolete
    private TripleDesCryptoProvider _tripleDesProvider = null!;
#pragma warning restore CS0618

    private EncryptionTransformer _encryptionTransformer = null!;
    private SensitiveMasker _sensitiveMasker = null!;

    private ICfgRoot _cfgWithEncryption = null!;
    private ICfgRoot _cfgWithMasking = null!;
    private ICfgRoot _cfgPlain = null!;

    // 测试数据
    private readonly string _shortText = "Password123";
    private readonly string _mediumText = "This is a medium length password for testing encryption performance";
    private readonly string _longText;
    private string _encryptedShort = null!;
    private string _encryptedMedium = null!;

    public CryptoBenchmarks()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgCryptoBench_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        // 生成测试密钥
        var keyBytes32 = new byte[32];
        var keyBytes16 = new byte[16];
        var keyBytes24 = new byte[24];
        RandomNumberGenerator.Fill(keyBytes32);
        RandomNumberGenerator.Fill(keyBytes16);
        RandomNumberGenerator.Fill(keyBytes24);

        _aesKey = Convert.ToBase64String(keyBytes32);
        _aesKey128 = Convert.ToBase64String(keyBytes16);
        _chacha20Key = Convert.ToBase64String(keyBytes32);
        _sm4Key = Convert.ToBase64String(keyBytes16);
        _tripleDesKey = Convert.ToBase64String(keyBytes24);
        _encKey = Convert.ToBase64String(keyBytes32);
        _hmacKey = Convert.ToBase64String(keyBytes32);

        // 生成长文本
        _longText = new string('A', 10000);
    }

    [GlobalSetup]
    public void Setup()
    {
        // 初始化加密提供者
        _aesGcmProvider = new AesGcmCryptoProvider(_aesKey);
        _aesCbcProvider = new AesCbcCryptoProvider(_encKey, _hmacKey);
        _chacha20Provider = new ChaCha20CryptoProvider(_chacha20Key);
        _sm4Provider = new Sm4CryptoProvider(_sm4Key);
#pragma warning disable CS0618
        _tripleDesProvider = new TripleDesCryptoProvider(_tripleDesKey);
#pragma warning restore CS0618

        // 初始化转换器和脱敏器
        _encryptionTransformer = new EncryptionTransformer(_aesGcmProvider);
        _sensitiveMasker = new SensitiveMasker();

        // 预加密测试数据
        _encryptedShort = "{ENC}" + _aesGcmProvider.Encrypt(_shortText);
        _encryptedMedium = "{ENC}" + _aesGcmProvider.Encrypt(_mediumText);

        // 创建测试配置文件
        SetupConfigFiles();
    }

    private void SetupConfigFiles()
    {
        // 带加密的配置
        var encryptedJsonPath = Path.Combine(_testDir, "encrypted_config.json");
        File.WriteAllText(encryptedJsonPath, $$"""
            {
                "Database": {
                    "Password": "{{_encryptedShort}}",
                    "ConnectionString": "{{"{ENC}" + _aesGcmProvider.Encrypt("Server=localhost;Database=test;User=admin;Password=secret")}}"
                },
                "ApiKey": "{{"{ENC}" + _aesGcmProvider.Encrypt("sk-1234567890abcdef")}}",
                "PlainValue": "NotEncrypted"
            }
            """);

        _cfgWithEncryption = new CfgBuilder()
            .AddJsonFile(encryptedJsonPath, level: 0, writeable: false)
            .AddAesGcmEncryption(_aesKey)
            .Build();

        // 带脱敏的配置
        var maskingJsonPath = Path.Combine(_testDir, "masking_config.json");
        File.WriteAllText(maskingJsonPath, """
            {
                "Database": {
                    "Password": "MySecretPassword123",
                    "Host": "localhost"
                },
                "ApiKey": "sk-1234567890abcdef",
                "Token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
            }
            """);

        _cfgWithMasking = new CfgBuilder()
            .AddJsonFile(maskingJsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // 普通配置（对照组）
        var plainJsonPath = Path.Combine(_testDir, "plain_config.json");
        File.WriteAllText(plainJsonPath, """
            {
                "Database": {
                    "Password": "MySecretPassword123",
                    "Host": "localhost"
                },
                "ApiKey": "sk-1234567890abcdef"
            }
            """);

        _cfgPlain = new CfgBuilder()
            .AddJsonFile(plainJsonPath, level: 0, writeable: false)
            .Build();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        _cfgWithEncryption?.Dispose();
        _cfgWithMasking?.Dispose();
        _cfgPlain?.Dispose();
        _aesGcmProvider?.Dispose();
        _aesCbcProvider?.Dispose();
        _chacha20Provider?.Dispose();
        _sm4Provider?.Dispose();
        _tripleDesProvider?.Dispose();

        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region 加密算法对比

    /// <summary>
    /// AES-GCM 加密短文本
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Encryption", "ShortText")]
    public string AesGcm_Encrypt_Short()
    {
        return _aesGcmProvider.Encrypt(_shortText);
    }

    /// <summary>
    /// AES-CBC 加密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "ShortText")]
    public string AesCbc_Encrypt_Short()
    {
        return _aesCbcProvider.Encrypt(_shortText);
    }

    /// <summary>
    /// ChaCha20-Poly1305 加密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "ShortText")]
    public string ChaCha20_Encrypt_Short()
    {
        return _chacha20Provider.Encrypt(_shortText);
    }

    /// <summary>
    /// SM4 加密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "ShortText")]
    public string Sm4_Encrypt_Short()
    {
        return _sm4Provider.Encrypt(_shortText);
    }

    /// <summary>
    /// Triple DES 加密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "ShortText")]
    public string TripleDes_Encrypt_Short()
    {
        return _tripleDesProvider.Encrypt(_shortText);
    }

    #endregion

    #region 解密算法对比

    /// <summary>
    /// AES-GCM 解密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Decryption", "ShortText")]
    public string AesGcm_Decrypt_Short()
    {
        return _aesGcmProvider.Decrypt(_encryptedShort.Substring(5)); // 去掉 {ENC} 前缀
    }

    /// <summary>
    /// ChaCha20-Poly1305 解密短文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Decryption", "ShortText")]
    public string ChaCha20_Decrypt_Short()
    {
        var encrypted = _chacha20Provider.Encrypt(_shortText);
        return _chacha20Provider.Decrypt(encrypted);
    }

    #endregion

    #region 不同文本长度对比

    /// <summary>
    /// AES-GCM 加密中等长度文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "MediumText")]
    public string AesGcm_Encrypt_Medium()
    {
        return _aesGcmProvider.Encrypt(_mediumText);
    }

    /// <summary>
    /// AES-GCM 加密长文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "LongText")]
    public string AesGcm_Encrypt_Long()
    {
        return _aesGcmProvider.Encrypt(_longText);
    }

    /// <summary>
    /// ChaCha20 加密长文本
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Encryption", "LongText")]
    public string ChaCha20_Encrypt_Long()
    {
        return _chacha20Provider.Encrypt(_longText);
    }

    #endregion

    #region 批量加密

    /// <summary>
    /// AES-GCM 批量加密 100 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BatchEncryption")]
    public void AesGcm_Encrypt_Batch100()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _aesGcmProvider.Encrypt(_shortText);
        }
    }

    /// <summary>
    /// ChaCha20 批量加密 100 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("BatchEncryption")]
    public void ChaCha20_Encrypt_Batch100()
    {
        for (int i = 0; i < 100; i++)
        {
            _ = _chacha20Provider.Encrypt(_shortText);
        }
    }

    #endregion

    #region EncryptionTransformer 性能

    /// <summary>
    /// EncryptionTransformer ShouldTransform 检查（敏感键）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Transformer")]
    public bool Transformer_ShouldTransform_SensitiveKey()
    {
        return _encryptionTransformer.ShouldTransform("Database:Password", "value");
    }

    /// <summary>
    /// EncryptionTransformer ShouldTransform 检查（非敏感键）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Transformer")]
    public bool Transformer_ShouldTransform_NonSensitiveKey()
    {
        return _encryptionTransformer.ShouldTransform("AppName", "value");
    }

    /// <summary>
    /// EncryptionTransformer ShouldTransform 检查（加密值）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Transformer")]
    public bool Transformer_ShouldTransform_EncryptedValue()
    {
        return _encryptionTransformer.ShouldTransform("AnyKey", _encryptedShort);
    }

    /// <summary>
    /// EncryptionTransformer 解密转换
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Transformer")]
    public string? Transformer_TransformOnRead()
    {
        return _encryptionTransformer.TransformOnRead("Password", _encryptedShort);
    }

    /// <summary>
    /// EncryptionTransformer 批量 ShouldTransform（缓存效果）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Transformer", "Cache")]
    public void Transformer_ShouldTransform_Batch1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _encryptionTransformer.ShouldTransform("Database:Password", "value");
        }
    }

    #endregion

    #region SensitiveMasker 性能

    /// <summary>
    /// SensitiveMasker ShouldMask 检查（敏感键）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Masker")]
    public bool Masker_ShouldMask_SensitiveKey()
    {
        return _sensitiveMasker.ShouldMask("Database:Password");
    }

    /// <summary>
    /// SensitiveMasker ShouldMask 检查（非敏感键）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Masker")]
    public bool Masker_ShouldMask_NonSensitiveKey()
    {
        return _sensitiveMasker.ShouldMask("AppName");
    }

    /// <summary>
    /// SensitiveMasker 脱敏操作
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Masker")]
    public string Masker_Mask()
    {
        return _sensitiveMasker.Mask("Password", "MySecretPassword123");
    }

    /// <summary>
    /// SensitiveMasker 批量 ShouldMask（缓存效果）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Masker", "Cache")]
    public void Masker_ShouldMask_Batch1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _sensitiveMasker.ShouldMask("Database:Password");
        }
    }

    #endregion

    #region CfgRoot 集成性能

    /// <summary>
    /// 读取加密配置值（首次，需要解密）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration")]
    public string? CfgRoot_GetEncrypted_FirstAccess()
    {
        return _cfgWithEncryption["Database:Password"];
    }

    /// <summary>
    /// 读取加密配置值（缓存命中）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration", "Cache")]
    public void CfgRoot_GetEncrypted_Cached1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfgWithEncryption["Database:Password"];
        }
    }

    /// <summary>
    /// 读取普通配置值（对照组）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration")]
    public string? CfgRoot_GetPlain()
    {
        return _cfgPlain["Database:Password"];
    }

    /// <summary>
    /// 读取普通配置值 1000 次（对照组）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration", "Cache")]
    public void CfgRoot_GetPlain_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfgPlain["Database:Password"];
        }
    }

    /// <summary>
    /// 获取脱敏值
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration")]
    public string? CfgRoot_GetMasked()
    {
        return _cfgWithMasking.GetMasked("Database:Password");
    }

    /// <summary>
    /// 获取脱敏值 1000 次
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration", "Cache")]
    public void CfgRoot_GetMasked_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _cfgWithMasking.GetMasked("Database:Password");
        }
    }

    /// <summary>
    /// 获取脱敏快照
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Integration")]
    public IReadOnlyDictionary<string, string> CfgRoot_GetMaskedSnapshot()
    {
        return _cfgWithMasking.GetMaskedSnapshot();
    }

    #endregion

    #region 模式匹配性能

    /// <summary>
    /// 简单模式匹配（*Password*）
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("PatternMatching")]
    public void PatternMatch_Simple_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _sensitiveMasker.ShouldMask("Database:Password");
            _ = _sensitiveMasker.ShouldMask("User:Password");
            _ = _sensitiveMasker.ShouldMask("Admin:Password");
        }
    }

    /// <summary>
    /// 多种模式匹配
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("PatternMatching")]
    public void PatternMatch_Various_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _sensitiveMasker.ShouldMask("Password");
            _ = _sensitiveMasker.ShouldMask("ApiKey");
            _ = _sensitiveMasker.ShouldMask("Secret");
            _ = _sensitiveMasker.ShouldMask("Token");
            _ = _sensitiveMasker.ShouldMask("ConnectionString");
        }
    }

    /// <summary>
    /// 非匹配键检查
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("PatternMatching")]
    public void PatternMatch_NoMatch_1000()
    {
        for (int i = 0; i < 1000; i++)
        {
            _ = _sensitiveMasker.ShouldMask("AppName");
            _ = _sensitiveMasker.ShouldMask("Version");
            _ = _sensitiveMasker.ShouldMask("Environment");
        }
    }

    #endregion
}
