using Apq.Cfg.Validation;
using Apq.Cfg.Validation.Rules;

namespace Apq.Cfg.Tests;

/// <summary>
/// 配置验证测试
/// </summary>
public class ValidationTests : IDisposable
{
    private readonly string _testDir;

    public ValidationTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"ApqCfgValidationTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    #region RequiredRule Tests

    [Fact]
    public void Required_WithValue_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "ConnectionString": "Server=localhost;Database=test"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Required("Database:ConnectionString"));

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Required_WithoutValue_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Host": "localhost"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Required("Database:ConnectionString"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Database:ConnectionString", result.Errors[0].Key);
        Assert.Equal("Required", result.Errors[0].RuleName);
    }

    [Fact]
    public void Required_MultipleKeys_ValidatesAll()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Host": "localhost"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Required("Database:Host", "Database:Port", "Database:Name"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.ErrorCount); // Port and Name are missing
    }

    #endregion

    #region RangeRule Tests

    [Fact]
    public void Range_WithinRange_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Server": {
                    "Port": 8080
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Range("Server:Port", 1, 65535));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Range_BelowMin_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Server": {
                    "Port": 0
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Range("Server:Port", 1, 65535));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Range", result.Errors[0].RuleName);
    }

    [Fact]
    public void Range_AboveMax_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Server": {
                    "Port": 70000
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Range("Server:Port", 1, 65535));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Range_InvalidNumber_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Server": {
                    "Port": "not-a-number"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Range("Server:Port", 1, 65535));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    [Fact]
    public void Range_EmptyValue_Passes()
    {
        // Arrange - Range rule should not validate empty values (Required rule should)
        var jsonPath = CreateConfigFile("""
            {
                "Server": {
                    "Host": "localhost"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Range("Server:Port", 1, 65535));

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region RegexRule Tests

    [Fact]
    public void Regex_MatchingPattern_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Email": "test@example.com"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Regex_NotMatchingPattern_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Email": "invalid-email"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Regex", result.Errors[0].RuleName);
    }

    #endregion

    #region CustomRule Tests

    [Fact]
    public void Custom_ValidatorReturnsTrue_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "ValidName"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Custom(
            "App:Name",
            value => value?.StartsWith("Valid") == true,
            "Name must start with 'Valid'"));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Custom_ValidatorReturnsFalse_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "InvalidName"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Custom(
            "App:Name",
            value => value?.StartsWith("Valid") == true,
            "Name must start with 'Valid'"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Name must start with 'Valid'", result.Errors[0].Message);
    }

    #endregion

    #region OneOf (EnumValues) Tests

    [Fact]
    public void OneOf_ValidValue_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Environment": "Production"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.OneOf("App:Environment", "Development", "Staging", "Production"));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void OneOf_InvalidValue_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Environment": "Unknown"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.OneOf("App:Environment", "Development", "Staging", "Production"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("EnumValues", result.Errors[0].RuleName);
    }

    [Fact]
    public void OneOf_CaseInsensitive_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Environment": "production"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.OneOf(
            "App:Environment",
            new[] { "Development", "Staging", "Production" },
            ignoreCase: true));

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region LengthRule Tests

    [Fact]
    public void Length_WithinRange_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "MyApp"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.Length("App:Name", minLength: 3, maxLength: 50));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Length_TooShort_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "AB"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.MinLength("App:Name", 3));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("Length", result.Errors[0].RuleName);
    }

    [Fact]
    public void Length_TooLong_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "ThisIsAVeryLongApplicationName"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.MaxLength("App:Name", 10));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    #endregion

    #region DependsOn Tests

    [Fact]
    public void DependsOn_BothExist_Passes()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Host": "localhost",
                    "Port": 5432
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.DependsOn("Database:Port", "Database:Host"));

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void DependsOn_DependencyExistsButKeyMissing_Fails()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Host": "localhost"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.DependsOn("Database:Port", "Database:Host"));

        // Assert
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal("DependsOn", result.Errors[0].RuleName);
    }

    [Fact]
    public void DependsOn_DependencyMissing_Passes()
    {
        // Arrange - If dependency doesn't exist, the rule passes
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Name": "testdb"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v.DependsOn("Database:Port", "Database:Host"));

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region CfgBuilder Integration Tests

    [Fact]
    public void BuildAndValidate_ValidConfig_ReturnsConfig()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "ConnectionString": "Server=localhost;Database=test",
                    "Port": 5432
                }
            }
            """);

        // Act
        var (cfg, result) = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddValidation(v => v
                .Required("Database:ConnectionString")
                .Range("Database:Port", 1, 65535))
            .BuildAndValidate(throwOnError: false);

        // Assert
        Assert.True(result.IsValid);
        Assert.NotNull(cfg);
        Assert.Equal("Server=localhost;Database=test", cfg["Database:ConnectionString"]);

        cfg.Dispose();
    }

    [Fact]
    public void BuildAndValidate_InvalidConfig_ThrowsException()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Port": 70000
                }
            }
            """);

        // Act & Assert
        var ex = Assert.Throws<ConfigValidationException>(() =>
        {
            new CfgBuilder()
                .AddJson(jsonPath, level: 0, writeable: false)
                .AddValidation(v => v
                    .Required("Database:ConnectionString")
                    .Range("Database:Port", 1, 65535))
                .BuildAndValidate(throwOnError: true);
        });

        Assert.Equal(2, ex.Errors.Count);
    }

    [Fact]
    public void BuildAndValidate_InvalidConfig_NoThrow_ReturnsErrors()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "Database": {
                    "Port": 70000
                }
            }
            """);

        // Act
        var (cfg, result) = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .AddValidation(v => v
                .Required("Database:ConnectionString")
                .Range("Database:Port", 1, 65535))
            .BuildAndValidate(throwOnError: false);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.ErrorCount);
        Assert.NotNull(cfg);

        cfg.Dispose();
    }

    #endregion

    #region Extension Methods Tests

    [Fact]
    public void ValidateAndThrow_ValidConfig_DoesNotThrow()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert - Should not throw
        cfg.ValidateAndThrow(v => v.Required("App:Name"));
    }

    [Fact]
    public void ValidateAndThrow_InvalidConfig_Throws()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Version": "1.0.0"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act & Assert
        Assert.Throws<ConfigValidationException>(() =>
            cfg.ValidateAndThrow(v => v.Required("App:Name")));
    }

    [Fact]
    public void TryValidate_ValidConfig_ReturnsTrue()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var isValid = cfg.TryValidate(v => v.Required("App:Name"), out var result);

        // Assert
        Assert.True(isValid);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void TryValidate_InvalidConfig_ReturnsFalse()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Version": "1.0.0"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var isValid = cfg.TryValidate(v => v.Required("App:Name"), out var result);

        // Assert
        Assert.False(isValid);
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
    }

    #endregion

    #region ValidationResult Tests

    [Fact]
    public void ValidationResult_GetErrorsForKey_ReturnsCorrectErrors()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Port": "invalid"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v
            .Required("App:Name")
            .Range("App:Port", 1, 65535));

        // Assert
        var nameErrors = result.GetErrorsForKey("App:Name").ToList();
        var portErrors = result.GetErrorsForKey("App:Port").ToList();

        Assert.Single(nameErrors);
        Assert.Single(portErrors);
        Assert.Equal("Required", nameErrors[0].RuleName);
        Assert.Equal("Range", portErrors[0].RuleName);
    }

    [Fact]
    public void ValidationResult_HasErrorsForKey_ReturnsCorrectResult()
    {
        // Arrange
        var jsonPath = CreateConfigFile("""
            {
                "App": {
                    "Name": "TestApp"
                }
            }
            """);

        using var cfg = new CfgBuilder()
            .AddJson(jsonPath, level: 0, writeable: false)
            .Build();

        // Act
        var result = cfg.Validate(v => v
            .Required("App:Name")
            .Required("App:Version"));

        // Assert
        Assert.False(result.HasErrorsForKey("App:Name"));
        Assert.True(result.HasErrorsForKey("App:Version"));
    }

    #endregion

    #region Helper Methods

    private string CreateConfigFile(string content)
    {
        var path = Path.Combine(_testDir, $"config_{Guid.NewGuid():N}.json");
        File.WriteAllText(path, content);
        return path;
    }

    #endregion
}
