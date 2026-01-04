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
        Assert.True(options.EnableCors);
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
        options.EnableCors = false;
        options.CorsOrigins = ["http://localhost:3000", "http://localhost:5000"];

        // Assert
        Assert.False(options.Enabled);
        Assert.Equal("/custom/api", options.RoutePrefix);
        Assert.Equal(AuthenticationType.ApiKey, options.Authentication);
        Assert.Equal("test-key", options.ApiKey);
        Assert.False(options.EnableCors);
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
    public void OpenApiRoutePrefix_HasCorrectDefaultForTargetFramework()
    {
        // Arrange & Act
        var options = new WebApiOptions();

        // Assert - 根据目标框架不同，默认值不同
#if NET8_0
        Assert.Equal("swagger", options.OpenApiRoutePrefix);
#else
        Assert.Equal("scalar/v1", options.OpenApiRoutePrefix);
#endif
    }

    [Fact]
    public void CorsOrigins_CanBeCustomized()
    {
        // Arrange
        var options = new WebApiOptions
        {
            CorsOrigins = ["https://example.com", "https://app.example.com"]
        };

        // Assert
        Assert.Equal(2, options.CorsOrigins.Length);
        Assert.Contains("https://example.com", options.CorsOrigins);
        Assert.Contains("https://app.example.com", options.CorsOrigins);
    }
}
