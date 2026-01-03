using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Apq.Cfg.WebUI.Models;

namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 配置 API 代理服务
/// </summary>
public class ConfigProxyService
{
    private readonly HttpClient _httpClient;
    private readonly IAppService _appService;

    public ConfigProxyService(HttpClient httpClient, IAppService appService)
    {
        _httpClient = httpClient;
        _appService = appService;
    }

    public async Task<string> GetAsync(string appId, string? path)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Get, path);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string appId, string? path, object? body)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Put, path, body);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string appId, string? path, object? body)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Post, path, body);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string appId, string? path)
    {
        var request = await CreateRequestAsync(appId, HttpMethod.Delete, path);
        var response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(
        string appId, HttpMethod method, string? path, object? body = null)
    {
        var app = await _appService.GetByIdAsync(appId)
            ?? throw new InvalidOperationException($"App {appId} not found");

        var url = string.IsNullOrEmpty(path)
            ? app.Url
            : $"{app.Url.TrimEnd('/')}/{path}";

        var request = new HttpRequestMessage(method, url);

        // 添加认证头
        switch (app.AuthType)
        {
            case AuthType.ApiKey:
                request.Headers.Add("X-Api-Key", app.ApiKey);
                break;
            case AuthType.JwtBearer:
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", app.Token);
                break;
        }

        // 添加请求体
        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }
}
