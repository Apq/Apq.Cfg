using System.Text.Json;
using Apq.Cfg.WebUI.Models;

namespace Apq.Cfg.WebUI.Services;

/// <summary>
/// 应用管理服务实现（JSON 文件存储）
/// </summary>
public class AppService : IAppService
{
    private readonly string _dataFile;
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private List<AppEndpoint>? _cache;

    public AppService(IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
    {
        _dataFile = Path.Combine(env.ContentRootPath, "Data", "apps.json");
        _httpClient = httpClientFactory.CreateClient();

        // 确保目录存在
        var dir = Path.GetDirectoryName(_dataFile);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public async Task<List<AppEndpoint>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (_cache != null) return _cache;

            if (!File.Exists(_dataFile))
                return _cache = new List<AppEndpoint>();

            var json = await File.ReadAllTextAsync(_dataFile);
            _cache = JsonSerializer.Deserialize<List<AppEndpoint>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
            return _cache;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<AppEndpoint?> GetByIdAsync(string id)
    {
        var apps = await GetAllAsync();
        return apps.FirstOrDefault(a => a.Id == id);
    }

    public async Task<AppEndpoint> AddAsync(AppEndpoint app)
    {
        var apps = await GetAllAsync();
        app.Id = Guid.NewGuid().ToString("N");
        app.CreatedAt = DateTime.UtcNow;
        apps.Add(app);
        await SaveAsync(apps);
        return app;
    }

    public async Task<bool> UpdateAsync(AppEndpoint app)
    {
        var apps = await GetAllAsync();
        var index = apps.FindIndex(a => a.Id == app.Id);
        if (index < 0) return false;

        apps[index] = app;
        await SaveAsync(apps);
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var apps = await GetAllAsync();
        var removed = apps.RemoveAll(a => a.Id == id);
        if (removed == 0) return false;

        await SaveAsync(apps);
        return true;
    }

    public async Task<bool> TestConnectionAsync(string id)
    {
        var app = await GetByIdAsync(id);
        if (app == null) return false;

        try
        {
            // 测试 /merged 端点
            var url = $"{app.Url.TrimEnd('/')}/merged";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // 添加认证头
            switch (app.AuthType)
            {
                case AuthType.ApiKey:
                    request.Headers.Add("X-Api-Key", app.ApiKey);
                    break;
                case AuthType.JwtBearer:
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", app.Token);
                    break;
            }

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task SaveAsync(List<AppEndpoint> apps)
    {
        _cache = apps;
        var json = JsonSerializer.Serialize(apps, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_dataFile, json);
    }
}
