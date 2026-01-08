using Apq.Cfg.Crypto;
using Apq.Cfg.Crypto.Providers;
using System.Security.Cryptography;

namespace Apq.Cfg.Tests;

/// <summary>
/// 配置加密脱敏测试
/// </summary>
public class CryptoTests : IDisposable
{
    private readonly string _testDir;
    private readonly string _testKey;

    public CryptoTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgCryptoTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);

        // 生成测试用的 256 位密钥
        var keyBytes = new byte[32];
        RandomNumberGenerator.Fill(keyBytes);
        _testKey = Convert.ToBase64String(keyBytes);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region AesGcmCryptoProvider 测试

    [Fact]
    public void AesGcmCryptoProvider_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "Hello, World!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_EncryptDecrypt_ChineseText()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "你好，世界！这是一个测试。";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_EncryptDecrypt_EmptyString()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_EncryptDecrypt_LongText()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = new string('A', 10000);

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_DifferentEncryptions_ProduceDifferentCiphertext()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "Same text";

        // Act
        var encrypted1 = provider.Encrypt(plainText);
        var encrypted2 = provider.Encrypt(plainText);

        // Assert - 由于随机 nonce，每次加密结果应该不同
        Assert.NotEqual(encrypted1, encrypted2);
    }

    [Fact]
    public void AesGcmCryptoProvider_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(new byte[15]); // 不是 16, 24, 或 32 字节

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AesGcmCryptoProvider(invalidKey));
    }

    [Fact]
    public void AesGcmCryptoProvider_128BitKey_Works()
    {
        // Arrange
        var keyBytes = new byte[16]; // 128 位
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

        using var provider = new AesGcmCryptoProvider(key);
        var plainText = "Test with 128-bit key";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_192BitKey_Works()
    {
        // Arrange
        var keyBytes = new byte[24]; // 192 位
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

        using var provider = new AesGcmCryptoProvider(key);
        var plainText = "Test with 192-bit key";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_WrongKey_ThrowsException()
    {
        // Arrange
        var keyBytes1 = new byte[32];
        var keyBytes2 = new byte[32];
        RandomNumberGenerator.Fill(keyBytes1);
        RandomNumberGenerator.Fill(keyBytes2);

        using var provider1 = new AesGcmCryptoProvider(Convert.ToBase64String(keyBytes1));
        using var provider2 = new AesGcmCryptoProvider(Convert.ToBase64String(keyBytes2));

        var plainText = "Secret message";
        var encrypted = provider1.Encrypt(plainText);

        // Act & Assert - 使用错误的密钥解密应该抛出异常
        Assert.ThrowsAny<Exception>(() => provider2.Decrypt(encrypted));
    }

    #endregion

    #region EncryptionTransformer 测试

    [Fact]
    public void EncryptionTransformer_ShouldTransform_EncryptedValue_ReturnsTrue()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);

        // Act & Assert
        Assert.True(transformer.ShouldTransform("AnyKey", "{ENC}SomeEncryptedValue"));
    }

    [Fact]
    public void EncryptionTransformer_ShouldTransform_SensitiveKey_ReturnsTrue()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);

        // Act & Assert
        Assert.True(transformer.ShouldTransform("Database:Password", "plaintext"));
        Assert.True(transformer.ShouldTransform("ApiKey", "plaintext"));
        Assert.True(transformer.ShouldTransform("ConnectionString", "plaintext"));
        Assert.True(transformer.ShouldTransform("MySecret", "plaintext"));
        Assert.True(transformer.ShouldTransform("UserCredential", "plaintext"));
        Assert.True(transformer.ShouldTransform("AccessToken", "plaintext"));
    }

    [Fact]
    public void EncryptionTransformer_ShouldTransform_NonSensitiveKey_ReturnsFalse()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var options = new EncryptionOptions { AutoEncryptOnWrite = false };
        var transformer = new EncryptionTransformer(provider, options);

        // Act & Assert
        Assert.False(transformer.ShouldTransform("AppName", "plaintext"));
        Assert.False(transformer.ShouldTransform("Version", "1.0.0"));
    }

    [Fact]
    public void EncryptionTransformer_TransformOnRead_DecryptsValue()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);
        var plainText = "MySecretPassword";
        var encrypted = "{ENC}" + provider.Encrypt(plainText);

        // Act
        var decrypted = transformer.TransformOnRead("Password", encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void EncryptionTransformer_TransformOnRead_NonEncryptedValue_ReturnsAsIs()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);
        var plainText = "NotEncrypted";

        // Act
        var result = transformer.TransformOnRead("SomeKey", plainText);

        // Assert
        Assert.Equal(plainText, result);
    }

    [Fact]
    public void EncryptionTransformer_TransformOnWrite_EncryptsSensitiveKey()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);
        var plainText = "MySecretPassword";

        // Act
        var encrypted = transformer.TransformOnWrite("Database:Password", plainText);

        // Assert
        Assert.StartsWith("{ENC}", encrypted);
        Assert.NotEqual(plainText, encrypted);

        // 验证可以解密
        var decrypted = transformer.TransformOnRead("Database:Password", encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void EncryptionTransformer_TransformOnWrite_AlreadyEncrypted_ReturnsAsIs()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);
        var alreadyEncrypted = "{ENC}SomeEncryptedValue";

        // Act
        var result = transformer.TransformOnWrite("Password", alreadyEncrypted);

        // Assert
        Assert.Equal(alreadyEncrypted, result);
    }

    [Fact]
    public void EncryptionTransformer_CustomPrefix_Works()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var options = new EncryptionOptions { EncryptedPrefix = "[ENCRYPTED]" };
        var transformer = new EncryptionTransformer(provider, options);
        var plainText = "MySecret";

        // Act
        var encrypted = transformer.TransformOnWrite("Password", plainText);
        var decrypted = transformer.TransformOnRead("Password", encrypted);

        // Assert
        Assert.StartsWith("[ENCRYPTED]", encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void EncryptionTransformer_CustomPatterns_Works()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var options = new EncryptionOptions
        {
            SensitiveKeyPatterns = new List<string> { "*MyCustomSecret*" }
        };
        var transformer = new EncryptionTransformer(provider, options);

        // Act & Assert
        Assert.True(transformer.ShouldTransform("App:MyCustomSecret", "value"));
        Assert.False(transformer.ShouldTransform("Password", "value")); // 默认模式被替换
    }

    #endregion

    #region SensitiveMasker 测试

    [Fact]
    public void SensitiveMasker_ShouldMask_SensitiveKey_ReturnsTrue()
    {
        // Arrange
        var masker = new SensitiveMasker();

        // Act & Assert
        Assert.True(masker.ShouldMask("Database:Password"));
        Assert.True(masker.ShouldMask("ApiKey"));
        Assert.True(masker.ShouldMask("ConnectionString"));
    }

    [Fact]
    public void SensitiveMasker_ShouldMask_NonSensitiveKey_ReturnsFalse()
    {
        // Arrange
        var masker = new SensitiveMasker();

        // Act & Assert
        Assert.False(masker.ShouldMask("AppName"));
        Assert.False(masker.ShouldMask("Version"));
    }

    [Fact]
    public void SensitiveMasker_Mask_LongValue_ShowsPartial()
    {
        // Arrange
        var masker = new SensitiveMasker();
        var value = "MySecretPassword123";

        // Act
        var masked = masker.Mask("Password", value);

        // Assert
        Assert.Equal("MyS***123", masked);
    }

    [Fact]
    public void SensitiveMasker_Mask_ShortValue_ShowsOnlyMask()
    {
        // Arrange
        var masker = new SensitiveMasker();
        var value = "abc"; // 长度 <= VisibleChars * 2 (默认 3 * 2 = 6)

        // Act
        var masked = masker.Mask("Password", value);

        // Assert
        Assert.Equal("***", masked);
    }

    [Fact]
    public void SensitiveMasker_Mask_NullValue_ReturnsPlaceholder()
    {
        // Arrange
        var masker = new SensitiveMasker();

        // Act
        var masked = masker.Mask("Password", null);

        // Assert
        Assert.Equal("[null]", masked);
    }

    [Fact]
    public void SensitiveMasker_CustomOptions_Works()
    {
        // Arrange
        var options = new MaskingOptions
        {
            MaskString = "****",
            VisibleChars = 2,
            NullPlaceholder = "[空]"
        };
        var masker = new SensitiveMasker(options);
        var value = "MySecretPassword";

        // Act
        var masked = masker.Mask("Password", value);
        var nullMasked = masker.Mask("Password", null);

        // Assert
        Assert.Equal("My****rd", masked);
        Assert.Equal("[空]", nullMasked);
    }

    #endregion

    #region CfgBuilder 集成测试

    [Fact]
    public void CfgBuilder_AddEncryption_IntegratesCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        using var provider = new AesGcmCryptoProvider(_testKey);
        var encryptedPassword = "{ENC}" + provider.Encrypt("MySecretPassword");

        File.WriteAllText(jsonPath, $$"""
            {
                "Database": {
                    "Host": "localhost",
                    "Password": "{{encryptedPassword}}"
                }
            }
            """);

        // Act
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddAesGcmEncryption(_testKey)
            .Build();

        // Assert
        Assert.Equal("localhost", cfg["Database:Host"]);
        Assert.Equal("MySecretPassword", cfg["Database:Password"]);
    }

    [Fact]
    public async Task CfgBuilder_AddEncryption_EncryptsOnWrite()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """{"AppName": "Test"}""");

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: true, isPrimaryWriter: true)
            .AddAesGcmEncryption(_testKey)
            .Build();

        // Act
        cfg.SetValue("Database:Password", "MyNewPassword");
        await cfg.SaveAsync();

        // Assert - 读取原始文件验证已加密
        var rawContent = File.ReadAllText(jsonPath);
        Assert.Contains("{ENC}", rawContent);
        Assert.DoesNotContain("MyNewPassword", rawContent);

        // 通过配置读取验证可以解密
        Assert.Equal("MyNewPassword", cfg["Database:Password"]);
    }

    [Fact]
    public void CfgBuilder_AddSensitiveMasking_IntegratesCorrectly()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "Database": {
                    "Host": "localhost",
                    "Password": "MySecretPassword123"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // Act
        var maskedPassword = cfg.GetMasked("Database:Password");
        var maskedHost = cfg.GetMasked("Database:Host");

        // Assert
        Assert.Equal("MyS***123", maskedPassword);
        Assert.Equal("localhost", maskedHost); // 非敏感键不脱敏
    }

    [Fact]
    public void CfgBuilder_GetMaskedSnapshot_ReturnsAllMaskedValues()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        File.WriteAllText(jsonPath, """
            {
                "AppName": "TestApp",
                "Database": {
                    "Password": "SecretPassword123"
                },
                "ApiKey": "my-api-key-12345"
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddSensitiveMasking()
            .Build();

        // Act
        var snapshot = cfg.GetMaskedSnapshot();

        // Assert
        Assert.Equal("TestApp", snapshot["AppName"]);
        Assert.Equal("Sec***123", snapshot["Database:Password"]);
        Assert.Equal("my-***345", snapshot["ApiKey"]);
    }

    [Fact]
    public void CfgBuilder_EncryptionAndMasking_WorkTogether()
    {
        // Arrange
        var jsonPath = Path.Combine(_testDir, "config.json");
        using var provider = new AesGcmCryptoProvider(_testKey);
        var encryptedPassword = "{ENC}" + provider.Encrypt("MySecretPassword123");

        File.WriteAllText(jsonPath, $$"""
            {
                "Database": {
                    "Password": "{{encryptedPassword}}"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddAesGcmEncryption(_testKey)
            .AddSensitiveMasking()
            .Build();

        // Act
        var decrypted = cfg["Database:Password"];
        var masked = cfg.GetMasked("Database:Password");

        // Assert
        Assert.Equal("MySecretPassword123", decrypted);
        Assert.Equal("MyS***123", masked);
    }

    [Fact]
    public void CfgBuilder_AddAesGcmEncryptionFromEnv_Works()
    {
        // Arrange
        var envVarName = $"TEST_KEY_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(envVarName, _testKey);

        try
        {
            var jsonPath = Path.Combine(_testDir, "config.json");
            using var provider = new AesGcmCryptoProvider(_testKey);
            var encryptedPassword = "{ENC}" + provider.Encrypt("EnvKeyPassword");

            File.WriteAllText(jsonPath, $$"""
                {
                    "Password": "{{encryptedPassword}}"
                }
                """);

            using var cfg = new CfgBuilder()
                .AddJsonFile(jsonPath, level: 0, writeable: false)
                .AddAesGcmEncryptionFromEnv(envVarName)
                .Build();

            // Act & Assert
            Assert.Equal("EnvKeyPassword", cfg["Password"]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVarName, null);
        }
    }

    [Fact]
    public void CfgBuilder_AddAesGcmEncryptionFromEnv_MissingEnvVar_ThrowsException()
    {
        // Arrange
        var envVarName = $"NONEXISTENT_KEY_{Guid.NewGuid():N}";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            new CfgBuilder()
                .AddAesGcmEncryptionFromEnv(envVarName);
        });
    }

    #endregion

    #region 边界条件测试

    [Fact]
    public void EncryptionTransformer_NullValue_ReturnsNull()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);

        // Act
        var readResult = transformer.TransformOnRead("Key", null);
        var writeResult = transformer.TransformOnWrite("Key", null);

        // Assert
        Assert.Null(readResult);
        Assert.Null(writeResult);
    }

    [Fact]
    public void AesGcmCryptoProvider_SpecialCharacters_Works()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?`~\n\t\r";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesGcmCryptoProvider_UnicodeEmoji_Works()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var plainText = "Emoji test: 😀🎉🔐💻🌍";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void SensitiveMasker_PatternMatching_CaseInsensitive()
    {
        // Arrange
        var masker = new SensitiveMasker();

        // Act & Assert
        Assert.True(masker.ShouldMask("password"));
        Assert.True(masker.ShouldMask("PASSWORD"));
        Assert.True(masker.ShouldMask("Password"));
        Assert.True(masker.ShouldMask("PaSsWoRd"));
    }

    [Fact]
    public void EncryptionTransformer_PatternMatching_CaseInsensitive()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);

        // Act & Assert
        Assert.True(transformer.ShouldTransform("password", "value"));
        Assert.True(transformer.ShouldTransform("PASSWORD", "value"));
        Assert.True(transformer.ShouldTransform("Password", "value"));
    }

    #endregion

    #region AesCbcCryptoProvider 测试

    [Fact]
    public void AesCbcCryptoProvider_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        var encKeyBytes = new byte[32];
        var hmacKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(encKeyBytes);
        RandomNumberGenerator.Fill(hmacKeyBytes);
        var encKey = Convert.ToBase64String(encKeyBytes);
        var hmacKey = Convert.ToBase64String(hmacKeyBytes);

        using var provider = new AesCbcCryptoProvider(encKey, hmacKey);
        var plainText = "Hello, AES-CBC!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesCbcCryptoProvider_EncryptDecrypt_ChineseText()
    {
        // Arrange
        var encKeyBytes = new byte[32];
        var hmacKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(encKeyBytes);
        RandomNumberGenerator.Fill(hmacKeyBytes);

        using var provider = new AesCbcCryptoProvider(encKeyBytes, hmacKeyBytes);
        var plainText = "你好，AES-CBC 加密测试！";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void AesCbcCryptoProvider_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(new byte[15]);
        var validKey = Convert.ToBase64String(new byte[32]);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new AesCbcCryptoProvider(invalidKey, validKey));
        Assert.Throws<ArgumentException>(() => new AesCbcCryptoProvider(validKey, invalidKey));
    }

    [Fact]
    public void AesCbcCryptoProvider_WrongKey_ThrowsException()
    {
        // Arrange
        var encKeyBytes1 = new byte[32];
        var encKeyBytes2 = new byte[32];
        var hmacKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(encKeyBytes1);
        RandomNumberGenerator.Fill(encKeyBytes2);
        RandomNumberGenerator.Fill(hmacKeyBytes);

        using var provider1 = new AesCbcCryptoProvider(encKeyBytes1, hmacKeyBytes);
        using var provider2 = new AesCbcCryptoProvider(encKeyBytes2, hmacKeyBytes);

        var plainText = "Secret message";
        var encrypted = provider1.Encrypt(plainText);

        // Act & Assert - HMAC 验证失败
        Assert.ThrowsAny<Exception>(() => provider2.Decrypt(encrypted));
    }

    [Fact]
    public void AesCbcCryptoProvider_TamperedData_ThrowsException()
    {
        // Arrange
        var encKeyBytes = new byte[32];
        var hmacKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(encKeyBytes);
        RandomNumberGenerator.Fill(hmacKeyBytes);

        using var provider = new AesCbcCryptoProvider(encKeyBytes, hmacKeyBytes);
        var plainText = "Secret message";
        var encrypted = provider.Encrypt(plainText);

        // 篡改密文
        var data = Convert.FromBase64String(encrypted);
        data[data.Length - 1] ^= 0xFF;
        var tampered = Convert.ToBase64String(data);

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => provider.Decrypt(tampered));
    }

    #endregion

    #region ChaCha20CryptoProvider 测试

    [Fact]
    public void ChaCha20CryptoProvider_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        using var provider = new ChaCha20CryptoProvider(_testKey);
        var plainText = "Hello, ChaCha20!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void ChaCha20CryptoProvider_EncryptDecrypt_ChineseText()
    {
        // Arrange
        var keyBytes = new byte[32];
        RandomNumberGenerator.Fill(keyBytes);

        using var provider = new ChaCha20CryptoProvider(keyBytes);
        var plainText = "你好，ChaCha20 加密测试！";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void ChaCha20CryptoProvider_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(new byte[16]); // 必须是 32 字节

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ChaCha20CryptoProvider(invalidKey));
    }

    [Fact]
    public void ChaCha20CryptoProvider_DifferentEncryptions_ProduceDifferentCiphertext()
    {
        // Arrange
        using var provider = new ChaCha20CryptoProvider(_testKey);
        var plainText = "Same text";

        // Act
        var encrypted1 = provider.Encrypt(plainText);
        var encrypted2 = provider.Encrypt(plainText);

        // Assert - 由于随机 nonce，每次加密结果应该不同
        Assert.NotEqual(encrypted1, encrypted2);
    }

    #endregion

    #region Sm4CryptoProvider 测试

    [Fact]
    public void Sm4CryptoProvider_CBC_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        var keyBytes = new byte[16]; // SM4 密钥必须是 128 位
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

        using var provider = new Sm4CryptoProvider(key, Sm4Mode.CBC);
        var plainText = "Hello, SM4-CBC!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void Sm4CryptoProvider_ECB_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        var keyBytes = new byte[16];
        RandomNumberGenerator.Fill(keyBytes);

        using var provider = new Sm4CryptoProvider(keyBytes, Sm4Mode.ECB);
        var plainText = "Hello, SM4-ECB!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void Sm4CryptoProvider_EncryptDecrypt_ChineseText()
    {
        // Arrange
        var keyBytes = new byte[16];
        RandomNumberGenerator.Fill(keyBytes);

        using var provider = new Sm4CryptoProvider(keyBytes);
        var plainText = "你好，国密 SM4 加密测试！";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void Sm4CryptoProvider_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(new byte[32]); // SM4 必须是 16 字节

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Sm4CryptoProvider(invalidKey));
    }

    #endregion

    #region TripleDesCryptoProvider 测试

    [Fact]
    public void TripleDesCryptoProvider_EncryptDecrypt_RoundTrip()
    {
        // Arrange
        var keyBytes = new byte[24]; // Triple DES 密钥 192 位
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

#pragma warning disable CS0618 // Type or member is obsolete
        using var provider = new TripleDesCryptoProvider(key);
#pragma warning restore CS0618

        var plainText = "Hello, Triple DES!";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plainText, encrypted);
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void TripleDesCryptoProvider_128BitKey_Works()
    {
        // Arrange
        var keyBytes = new byte[16]; // 128 位密钥
        RandomNumberGenerator.Fill(keyBytes);

#pragma warning disable CS0618
        using var provider = new TripleDesCryptoProvider(keyBytes);
#pragma warning restore CS0618

        var plainText = "Test with 128-bit key";

        // Act
        var encrypted = provider.Encrypt(plainText);
        var decrypted = provider.Decrypt(encrypted);

        // Assert
        Assert.Equal(plainText, decrypted);
    }

    [Fact]
    public void TripleDesCryptoProvider_InvalidKeyLength_ThrowsException()
    {
        // Arrange
        var invalidKey = Convert.ToBase64String(new byte[32]); // 必须是 16 或 24 字节

        // Act & Assert
#pragma warning disable CS0618
        Assert.Throws<ArgumentException>(() => new TripleDesCryptoProvider(invalidKey));
#pragma warning restore CS0618
    }

    #endregion

    #region CfgBuilder 扩展方法测试

    [Fact]
    public void CfgBuilder_AddAesCbcEncryption_Works()
    {
        // Arrange
        var encKeyBytes = new byte[32];
        var hmacKeyBytes = new byte[32];
        RandomNumberGenerator.Fill(encKeyBytes);
        RandomNumberGenerator.Fill(hmacKeyBytes);
        var encKey = Convert.ToBase64String(encKeyBytes);
        var hmacKey = Convert.ToBase64String(hmacKeyBytes);

        using var provider = new AesCbcCryptoProvider(encKey, hmacKey);
        var encryptedPassword = "{ENC}" + provider.Encrypt("AesCbcPassword");

        var jsonPath = Path.Combine(_testDir, "aescbc_config.json");
        File.WriteAllText(jsonPath, $$"""
            {
                "Password": "{{encryptedPassword}}"
            }
            """);

        // Act
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddAesCbcEncryption(encKey, hmacKey)
            .Build();

        // Assert
        Assert.Equal("AesCbcPassword", cfg["Password"]);
    }

    [Fact]
    public void CfgBuilder_AddChaCha20Encryption_Works()
    {
        // Arrange
        using var provider = new ChaCha20CryptoProvider(_testKey);
        var encryptedPassword = "{ENC}" + provider.Encrypt("ChaCha20Password");

        var jsonPath = Path.Combine(_testDir, "chacha20_config.json");
        File.WriteAllText(jsonPath, $$"""
            {
                "Password": "{{encryptedPassword}}"
            }
            """);

        // Act
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddChaCha20Encryption(_testKey)
            .Build();

        // Assert
        Assert.Equal("ChaCha20Password", cfg["Password"]);
    }

    [Fact]
    public void CfgBuilder_AddSm4Encryption_Works()
    {
        // Arrange
        var keyBytes = new byte[16];
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

        using var provider = new Sm4CryptoProvider(key);
        var encryptedPassword = "{ENC}" + provider.Encrypt("Sm4Password");

        var jsonPath = Path.Combine(_testDir, "sm4_config.json");
        File.WriteAllText(jsonPath, $$"""
            {
                "Password": "{{encryptedPassword}}"
            }
            """);

        // Act
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddSm4Encryption(key)
            .Build();

        // Assert
        Assert.Equal("Sm4Password", cfg["Password"]);
    }

    [Fact]
    public void CfgBuilder_AddTripleDesEncryption_Works()
    {
        // Arrange
        var keyBytes = new byte[24];
        RandomNumberGenerator.Fill(keyBytes);
        var key = Convert.ToBase64String(keyBytes);

#pragma warning disable CS0618
        using var provider = new TripleDesCryptoProvider(key);
#pragma warning restore CS0618
        var encryptedPassword = "{ENC}" + provider.Encrypt("TripleDesPassword");

        var jsonPath = Path.Combine(_testDir, "tripledes_config.json");
        File.WriteAllText(jsonPath, $$"""
            {
                "Password": "{{encryptedPassword}}"
            }
            """);

        // Act
#pragma warning disable CS0618
        using var cfg = new CfgBuilder()
            .AddJsonFile(jsonPath, level: 0, writeable: false)
            .AddTripleDesEncryption(key)
            .Build();
#pragma warning restore CS0618

        // Assert
        Assert.Equal("TripleDesPassword", cfg["Password"]);
    }

    #endregion

    #region EncryptionTransformer 缓存测试

    [Fact]
    public void EncryptionTransformer_ClearCache_Works()
    {
        // Arrange
        using var provider = new AesGcmCryptoProvider(_testKey);
        var transformer = new EncryptionTransformer(provider);

        // 触发缓存
        transformer.ShouldTransform("Password", "value");
        transformer.ShouldTransform("NonSensitive", "value");

        // Act - 清除缓存不应抛出异常
        transformer.ClearCache();

        // Assert - 清除后仍能正常工作
        Assert.True(transformer.ShouldTransform("Password", "value"));
    }

    #endregion

    #region SensitiveMasker 缓存测试

    [Fact]
    public void SensitiveMasker_ClearCache_Works()
    {
        // Arrange
        var masker = new SensitiveMasker();

        // 触发缓存
        masker.ShouldMask("Password");
        masker.ShouldMask("NonSensitive");

        // Act - 清除缓存不应抛出异常
        masker.ClearCache();

        // Assert - 清除后仍能正常工作
        Assert.True(masker.ShouldMask("Password"));
    }

    #endregion
}
