namespace Apq.Cfg.Tests;

using Apq.Cfg.WebApi;

/// <summary>
/// WebApiOptions 配置选项测试
/// </summary>
public class WebApiOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var options = new WebApiOptions();

        // Assert
        Assert.True(options.Enabled);
        Assert.Equal("/api/apqcfg", options.RoutePrefix);
        Assert.Equal(AuthenticationType.None, options.Authentication);
        Assert.Null(options.ApiKey);
        Assert.Equal("X-Api-Key", options.ApiKeyHeaderName);
        Assert.Null(options.Jwt);
        Assert.True(options.AllowRead);
        Assert.False(options.AllowWrite);
        Assert.False(options.AllowDelete);
        Assert.True(options.MaskSensitiveValues);
        Assert.False(options.EnableCors);
        Assert.Single(options.CorsOrigins);
        Assert.Equal("*", options.CorsOrigins[0]);
    }

    [Fact]
    public void SectionName_IsCorrect()
    {
        // Assert
        Assert.Equal("ApqCfg:WebApi", WebApiOptions.SectionName);
    }

    [Fact]
    public void SensitiveKeyPatterns_HasDefaultPatterns()
    {
        // Arrange & Act
        var options = new WebApiOptions();

        // Assert
        Assert.Contains("*Password*", options.SensitiveKeyPatterns);
        Assert.Contains("*Secret*", options.SensitiveKeyPatterns);
        Assert.Contains("*Key*", options.SensitiveKeyPatterns);
        Assert.Contains("*Token*", options.SensitiveKeyPatterns);
        Assert.Contains("*ConnectionString*", options.SensitiveKeyPatterns);
    }

    [Fact]
    public void OpenApiOptions_HasDefaultValues()
    {
        // Arrange & Act
        var options = new WebApiOptions();

        // Assert
        Assert.True(options.OpenApiEnabled);
        Assert.Equal("Apq.Cfg Web API", options.OpenApiTitle);
        Assert.Equal("Apq.Cfg 配置管理 RESTful API", options.OpenApiDescription);
        Assert.Equal("v1", options.OpenApiVersion);
        Assert.True(options.OpenApiShowAuthorizationButton);
    }

    [Fact]
    public void Properties_CanBeModified()
    {
        // Arrange
        var options = new WebApiOptions();

        // Act
        options.Enabled = false;
        options.RoutePrefix = "/custom/api";
        options.Authentication = AuthenticationType.ApiKey;
        options.ApiKey = "test-key";
        options.AllowWrite = true;
        options.AllowDelete = true;
        options.MaskSensitiveValues = false;
        options.EnableCors = true;
        options.CorsOrigins = ["http://localhost:3000", "http://localhost:5000"];

        // Assert
        Assert.False(options.Enabled);
        Assert.Equal("/custom/api", options.RoutePrefix);
        Assert.Equal(AuthenticationType.ApiKey, options.Authentication);
        Assert.Equal("test-key", options.ApiKey);
        Assert.True(options.AllowWrite);
        Assert.True(options.AllowDelete);
        Assert.False(options.MaskSensitiveValues);
        Assert.True(options.EnableCors);
        Assert.Equal(2, options.CorsOrigins.Length);
    }

    [Fact]
    public void JwtOptions_CanBeConfigured()
    {
        // Arrange
        var options = new WebApiOptions
        {
            Authentication = AuthenticationType.JwtBearer,
            Jwt = new JwtOptions
            {
                Authority = "https://auth.example.com",
                Audience = "my-api",
                RequireHttpsMetadata = true,
                ValidateIssuer = true,
                ValidateAudience = true
            }
        };

        // Assert
        Assert.NotNull(options.Jwt);
        Assert.Equal("https://auth.example.com", options.Jwt.Authority);
        Assert.Equal("my-api", options.Jwt.Audience);
        Assert.True(options.Jwt.RequireHttpsMetadata);
        Assert.True(options.Jwt.ValidateIssuer);
        Assert.True(options.Jwt.ValidateAudience);
    }

    [Fact]
    public void SensitiveKeyPatterns_CanBeCustomized()
    {
        // Arrange
        var options = new WebApiOptions
        {
            SensitiveKeyPatterns = ["*Credential*", "*Private*"]
        };

        // Assert
        Assert.Equal(2, options.SensitiveKeyPatterns.Length);
        Assert.Contains("*Credential*", options.SensitiveKeyPatterns);
        Assert.Contains("*Private*", options.SensitiveKeyPatterns);
    }
}
