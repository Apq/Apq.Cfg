using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.WebApi.Authentication;

/// <summary>
/// API Key 认证处理器
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrEmpty(Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API Key not configured"));
        }

        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Missing {Options.HeaderName} header"));
        }

        var providedApiKey = apiKeyHeader.ToString();

        if (!string.Equals(providedApiKey, Options.ApiKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "ApiKeyUser"),
            new Claim(ClaimTypes.AuthenticationMethod, ApiKeyDefaults.AuthenticationScheme)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
