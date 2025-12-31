# 扩展开发

本指南介绍如何开发自定义配置源和扩展功能。

## 配置源接口

### ICfgSource 接口

所有配置源必须实现的基础接口：

```csharp
public interface ICfgSource
{
    /// <summary>
    /// 配置源层级，数值越大优先级越高
    /// </summary>
    int Level { get; }
    
    /// <summary>
    /// 是否支持写入
    /// </summary>
    bool IsWriteable { get; }
    
    /// <summary>
    /// 是否为主写入源（同层级只能有一个）
    /// </summary>
    bool IsPrimaryWriter { get; }
    
    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 配置源
    /// </summary>
    IConfigurationSource BuildSource();
}
```

### IWritableCfgSource 接口

可写配置源需要额外实现的接口：

```csharp
public interface IWritableCfgSource : ICfgSource
{
    /// <summary>
    /// 设置配置值
    /// </summary>
    void Set(string key, string? value);
    
    /// <summary>
    /// 移除配置键
    /// </summary>
    void Remove(string key);
    
    /// <summary>
    /// 保存配置到持久化存储
    /// </summary>
    Task SaveAsync(CancellationToken cancellationToken = default);
}
```

## 开发自定义配置源

### 示例：自定义 HTTP API 配置源

#### 1. 定义配置选项

```csharp
public class HttpApiCfgOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5000";
    public string Endpoint { get; set; } = "/api/config";
    public string? ApiKey { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(1);
    public bool EnableHotReload { get; set; } = true;
}
```

#### 2. 实现配置源

```csharp
public class HttpApiCfgSource : ICfgSource, IWritableCfgSource, IDisposable
{
    private readonly HttpApiCfgOptions _options;
    private readonly int _level;
    private readonly bool _isPrimaryWriter;
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, string?> _data = new();
    private readonly ConcurrentDictionary<string, string?> _pendingChanges = new();
    private Timer? _pollTimer;
    private Action<Dictionary<string, string?>>? _onReload;
    
    public HttpApiCfgSource(HttpApiCfgOptions options, int level, bool isPrimaryWriter = false)
    {
        _options = options;
        _level = level;
        _isPrimaryWriter = isPrimaryWriter;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.Timeout
        };
        
        if (!string.IsNullOrEmpty(options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        }
    }
    
    public int Level => _level;
    public bool IsWriteable => true;
    public bool IsPrimaryWriter => _isPrimaryWriter;
    
    public IConfigurationSource BuildSource()
    {
        // 初始加载配置
        LoadConfigAsync().GetAwaiter().GetResult();
        
        // 启动热重载
        if (_options.EnableHotReload)
        {
            StartPolling();
        }
        
        return new HttpApiConfigurationSource(this);
    }
    
    private async Task LoadConfigAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_options.Endpoint);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var config = JsonSerializer.Deserialize<Dictionary<string, string?>>(json);
            
            if (config != null)
            {
                _data.Clear();
                foreach (var kvp in config)
                {
                    _data[kvp.Key] = kvp.Value;
                }
            }
        }
        catch (Exception ex)
        {
            // 记录错误，但不抛出异常
            Console.WriteLine($"加载配置失败: {ex.Message}");
        }
    }
    
    private void StartPolling()
    {
        _pollTimer = new Timer(async _ =>
        {
            var oldData = new Dictionary<string, string?>(_data);
            await LoadConfigAsync();
            
            // 检测变更
            var changes = DetectChanges(oldData, _data);
            if (changes.Count > 0)
            {
                _onReload?.Invoke(new Dictionary<string, string?>(_data));
            }
        }, null, _options.PollInterval, _options.PollInterval);
    }
    
    private Dictionary<string, ConfigChange> DetectChanges(
        IDictionary<string, string?> oldData,
        IDictionary<string, string?> newData)
    {
        var changes = new Dictionary<string, ConfigChange>();
        
        // 检测新增和修改
        foreach (var kvp in newData)
        {
            if (!oldData.TryGetValue(kvp.Key, out var oldValue))
            {
                changes[kvp.Key] = new ConfigChange(ChangeType.Added, null, kvp.Value);
            }
            else if (oldValue != kvp.Value)
            {
                changes[kvp.Key] = new ConfigChange(ChangeType.Modified, oldValue, kvp.Value);
            }
        }
        
        // 检测删除
        foreach (var kvp in oldData)
        {
            if (!newData.ContainsKey(kvp.Key))
            {
                changes[kvp.Key] = new ConfigChange(ChangeType.Removed, kvp.Value, null);
            }
        }
        
        return changes;
    }
    
    public void Set(string key, string? value)
    {
        _pendingChanges[key] = value;
        _data[key] = value;
    }
    
    public void Remove(string key)
    {
        _pendingChanges[key] = null;
        _data.TryRemove(key, out _);
    }
    
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_pendingChanges.IsEmpty) return;
        
        var changes = new Dictionary<string, string?>(_pendingChanges);
        _pendingChanges.Clear();
        
        var json = JsonSerializer.Serialize(changes);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(_options.Endpoint, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
    
    internal IReadOnlyDictionary<string, string?> GetData() => _data;
    
    internal void SetOnReload(Action<Dictionary<string, string?>> onReload)
    {
        _onReload = onReload;
    }
    
    public void Dispose()
    {
        _pollTimer?.Dispose();
        _httpClient.Dispose();
    }
}
```

#### 3. 实现 IConfigurationSource

```csharp
internal class HttpApiConfigurationSource : IConfigurationSource
{
    private readonly HttpApiCfgSource _cfgSource;
    
    public HttpApiConfigurationSource(HttpApiCfgSource cfgSource)
    {
        _cfgSource = cfgSource;
    }
    
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new HttpApiConfigurationProvider(_cfgSource);
    }
}
```

#### 4. 实现 IConfigurationProvider

```csharp
internal class HttpApiConfigurationProvider : ConfigurationProvider
{
    private readonly HttpApiCfgSource _cfgSource;
    
    public HttpApiConfigurationProvider(HttpApiCfgSource cfgSource)
    {
        _cfgSource = cfgSource;
        
        // 订阅重载事件
        _cfgSource.SetOnReload(newData =>
        {
            Data = new Dictionary<string, string?>(newData, StringComparer.OrdinalIgnoreCase);
            OnReload();
        });
    }
    
    public override void Load()
    {
        Data = new Dictionary<string, string?>(_cfgSource.GetData(), StringComparer.OrdinalIgnoreCase);
    }
}
```

#### 5. 添加扩展方法

```csharp
public static class HttpApiCfgBuilderExtensions
{
    public static CfgBuilder AddHttpApi(
        this CfgBuilder builder,
        Action<HttpApiCfgOptions> configure,
        int level,
        bool isPrimaryWriter = false)
    {
        var options = new HttpApiCfgOptions();
        configure(options);
        
        var source = new HttpApiCfgSource(options, level, isPrimaryWriter);
        return builder.AddSource(source);
    }
    
    public static CfgBuilder AddHttpApi(
        this CfgBuilder builder,
        string baseUrl,
        string endpoint,
        int level,
        bool enableHotReload = true)
    {
        return builder.AddHttpApi(options =>
        {
            options.BaseUrl = baseUrl;
            options.Endpoint = endpoint;
            options.EnableHotReload = enableHotReload;
        }, level);
    }
}
```

#### 6. 使用自定义配置源

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false)
    .AddHttpApi(options =>
    {
        options.BaseUrl = "http://config-server:5000";
        options.Endpoint = "/api/config/myapp";
        options.ApiKey = "secret-key";
        options.PollInterval = TimeSpan.FromMinutes(5);
        options.EnableHotReload = true;
    }, level: 10, isPrimaryWriter: true)
    .Build();

// 读取配置
var value = cfg.Get("SomeKey");

// 写入配置
cfg.Set("NewKey", "NewValue");
await cfg.SaveAsync();
```

## 开发文件配置源

### 继承 FileCfgSourceBase

对于文件类型的配置源，可以继承基类简化开发：

```csharp
public class CustomFileCfgSource : FileCfgSourceBase
{
    public CustomFileCfgSource(
        string filePath,
        int level,
        bool writeable = false,
        bool isPrimaryWriter = false,
        bool reloadOnChange = false,
        EncodingOptions? encoding = null)
        : base(filePath, level, writeable, isPrimaryWriter, reloadOnChange, encoding)
    {
    }
    
    protected override Dictionary<string, string?> ParseContent(string content)
    {
        // 实现自定义解析逻辑
        var result = new Dictionary<string, string?>();
        
        foreach (var line in content.Split('\n'))
        {
            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                result[parts[0].Trim()] = parts[1].Trim();
            }
        }
        
        return result;
    }
    
    protected override string SerializeContent(Dictionary<string, string?> data)
    {
        // 实现自定义序列化逻辑
        var sb = new StringBuilder();
        
        foreach (var kvp in data)
        {
            sb.AppendLine($"{kvp.Key}={kvp.Value}");
        }
        
        return sb.ToString();
    }
}
```

## 开发远程配置源

### 继承 RemoteCfgSourceBase

对于远程配置中心，可以继承基类：

```csharp
public abstract class RemoteCfgSourceBase : ICfgSource, IWritableCfgSource, IDisposable
{
    protected readonly ConcurrentDictionary<string, string?> Data = new();
    protected readonly ConcurrentDictionary<string, string?> PendingChanges = new();
    protected Action<Dictionary<string, string?>>? OnReload;
    
    public abstract int Level { get; }
    public abstract bool IsWriteable { get; }
    public abstract bool IsPrimaryWriter { get; }
    
    public abstract IConfigurationSource BuildSource();
    
    protected abstract Task ConnectAsync();
    protected abstract Task DisconnectAsync();
    protected abstract Task LoadDataAsync();
    protected abstract Task SaveDataAsync(Dictionary<string, string?> changes);
    protected abstract void SetupWatcher();
    
    public void Set(string key, string? value)
    {
        PendingChanges[key] = value;
        Data[key] = value;
    }
    
    public void Remove(string key)
    {
        PendingChanges[key] = null;
        Data.TryRemove(key, out _);
    }
    
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (PendingChanges.IsEmpty) return;
        
        var changes = new Dictionary<string, string?>(PendingChanges);
        PendingChanges.Clear();
        
        await SaveDataAsync(changes);
    }
    
    protected void NotifyReload()
    {
        OnReload?.Invoke(new Dictionary<string, string?>(Data));
    }
    
    public virtual void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}
```

## 自定义编码映射

### 添加自定义编码规则

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        // 完整路径映射
        config.AddReadMapping(
            @"C:\legacy\old-config.ini",
            EncodingMappingType.ExactPath,
            Encoding.GetEncoding("GB2312"),
            priority: 100);
        
        // 通配符映射
        config.AddWriteMapping(
            "*.ps1",
            EncodingMappingType.Wildcard,
            new UTF8Encoding(true),  // UTF-8 with BOM
            priority: 50);
        
        // 正则表达式映射
        config.AddReadMapping(
            @"logs[/\\].*\.log$",
            EncodingMappingType.Regex,
            Encoding.Unicode,
            priority: 30);
        
        // 清除默认规则
        config.ClearReadMappings();
        config.ClearWriteMappings();
    })
    .AddJson("config.json", level: 0, writeable: false)
    .Build();
```

### 自定义编码检测

```csharp
var cfg = new CfgBuilder()
    // 设置编码检测置信度阈值
    .WithEncodingConfidenceThreshold(0.8f)
    
    // 启用编码检测日志
    .WithEncodingDetectionLogging(result =>
    {
        Console.WriteLine($"文件: {result.FilePath}");
        Console.WriteLine($"检测编码: {result.DetectedEncoding?.EncodingName}");
        Console.WriteLine($"置信度: {result.Confidence:P0}");
        Console.WriteLine($"检测方法: {result.DetectionMethod}");
    })
    
    .AddJson("config.json", level: 0)
    .Build();
```

## 自定义类型转换

### 注册自定义类型转换器

```csharp
// 自定义类型
public class ConnectionString
{
    public string Server { get; set; } = "";
    public string Database { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Password { get; set; } = "";
    
    public static ConnectionString Parse(string value)
    {
        var result = new ConnectionString();
        var parts = value.Split(';');
        
        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2)
            {
                switch (kv[0].Trim().ToLower())
                {
                    case "server": result.Server = kv[1].Trim(); break;
                    case "database": result.Database = kv[1].Trim(); break;
                    case "user id": result.UserId = kv[1].Trim(); break;
                    case "password": result.Password = kv[1].Trim(); break;
                }
            }
        }
        
        return result;
    }
    
    public override string ToString()
    {
        return $"Server={Server};Database={Database};User Id={UserId};Password={Password}";
    }
}

// 使用
var connStr = cfg.Get<ConnectionString>("Database:ConnectionString");
```

## 开发 CfgBuilder 扩展

### 添加便捷方法

```csharp
public static class CfgBuilderExtensions
{
    /// <summary>
    /// 添加标准的多环境配置
    /// </summary>
    public static CfgBuilder AddStandardConfig(
        this CfgBuilder builder,
        string environment,
        string basePath = "")
    {
        var path = string.IsNullOrEmpty(basePath) ? "" : basePath + "/";
        
        return builder
            .AddJson($"{path}config.json", level: 0)
            .AddJson($"{path}config.{environment}.json", level: 1, optional: true)
            .AddJson($"{path}config.local.json", level: 2, optional: true, writeable: true)
            .AddEnvironmentVariables(prefix: "APP_", level: 10);
    }
    
    /// <summary>
    /// 添加开发环境配置
    /// </summary>
    public static CfgBuilder AddDevelopmentConfig(this CfgBuilder builder)
    {
        return builder
            .AddJson("config.json", level: 0)
            .AddJson("config.Development.json", level: 1, optional: true)
            .AddEnv(".env", level: 2, optional: true)
            .AddEnv(".env.local", level: 3, optional: true)
            .AddEnvironmentVariables(prefix: "APP_", level: 10);
    }
    
    /// <summary>
    /// 添加生产环境配置（带远程配置中心）
    /// </summary>
    public static CfgBuilder AddProductionConfig(
        this CfgBuilder builder,
        string consulAddress,
        string serviceName)
    {
        return builder
            .AddJson("config.json", level: 0)
            .AddJson("config.Production.json", level: 1, optional: true)
            .AddConsul(options =>
            {
                options.Address = consulAddress;
                options.KeyPrefix = $"services/{serviceName}/";
                options.EnableHotReload = true;
            }, level: 10)
            .AddEnvironmentVariables(prefix: "APP_", level: 20);
    }
}
```

## 测试自定义配置源

### 单元测试示例

```csharp
public class HttpApiCfgSourceTests
{
    [Fact]
    public async Task Should_Load_Config_From_Api()
    {
        // Arrange
        using var server = new MockHttpServer();
        server.SetupGet("/api/config", new Dictionary<string, string?>
        {
            ["Key1"] = "Value1",
            ["Key2"] = "Value2"
        });
        
        var cfg = new CfgBuilder()
            .AddHttpApi(options =>
            {
                options.BaseUrl = server.BaseUrl;
                options.Endpoint = "/api/config";
            }, level: 0)
            .Build();
        
        // Act
        var value1 = cfg.Get("Key1");
        var value2 = cfg.Get("Key2");
        
        // Assert
        Assert.Equal("Value1", value1);
        Assert.Equal("Value2", value2);
    }
    
    [Fact]
    public async Task Should_Save_Config_To_Api()
    {
        // Arrange
        using var server = new MockHttpServer();
        server.SetupGet("/api/config", new Dictionary<string, string?>());
        server.SetupPost("/api/config");
        
        var cfg = new CfgBuilder()
            .AddHttpApi(options =>
            {
                options.BaseUrl = server.BaseUrl;
                options.Endpoint = "/api/config";
            }, level: 0, isPrimaryWriter: true)
            .Build();
        
        // Act
        cfg.Set("NewKey", "NewValue");
        await cfg.SaveAsync();
        
        // Assert
        var savedData = server.GetLastPostData<Dictionary<string, string?>>();
        Assert.Equal("NewValue", savedData["NewKey"]);
    }
    
    [Fact]
    public async Task Should_Detect_Config_Changes()
    {
        // Arrange
        using var server = new MockHttpServer();
        var initialData = new Dictionary<string, string?> { ["Key1"] = "Value1" };
        server.SetupGet("/api/config", initialData);
        
        var changeDetected = false;
        var cfg = new CfgBuilder()
            .AddHttpApi(options =>
            {
                options.BaseUrl = server.BaseUrl;
                options.Endpoint = "/api/config";
                options.PollInterval = TimeSpan.FromMilliseconds(100);
                options.EnableHotReload = true;
            }, level: 0)
            .Build();
        
        cfg.OnChange += (key, oldValue, newValue) =>
        {
            changeDetected = true;
        };
        
        // Act - 更新服务器数据
        server.SetupGet("/api/config", new Dictionary<string, string?> 
        { 
            ["Key1"] = "UpdatedValue" 
        });
        
        // 等待轮询
        await Task.Delay(200);
        
        // Assert
        Assert.True(changeDetected);
        Assert.Equal("UpdatedValue", cfg.Get("Key1"));
    }
}
```

### 集成测试示例

```csharp
public class HttpApiCfgSourceIntegrationTests : IClassFixture<ConfigServerFixture>
{
    private readonly ConfigServerFixture _fixture;
    
    public HttpApiCfgSourceIntegrationTests(ConfigServerFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Should_Work_With_Real_Server()
    {
        // Arrange
        var cfg = new CfgBuilder()
            .AddHttpApi(options =>
            {
                options.BaseUrl = _fixture.ServerUrl;
                options.Endpoint = "/api/config";
                options.ApiKey = _fixture.ApiKey;
            }, level: 0, isPrimaryWriter: true)
            .Build();
        
        // Act
        cfg.Set("IntegrationTest:Key", "IntegrationTest:Value");
        await cfg.SaveAsync();
        
        // 重新加载验证
        var newCfg = new CfgBuilder()
            .AddHttpApi(options =>
            {
                options.BaseUrl = _fixture.ServerUrl;
                options.Endpoint = "/api/config";
                options.ApiKey = _fixture.ApiKey;
            }, level: 0)
            .Build();
        
        // Assert
        Assert.Equal("IntegrationTest:Value", newCfg.Get("IntegrationTest:Key"));
    }
}
```

## 发布自定义配置源

### 创建 NuGet 包

1. 创建项目文件：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net6.0;net8.0;net10.0</TargetFrameworks>
    <PackageId>Apq.Cfg.HttpApi</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Description>HTTP API configuration source for Apq.Cfg</Description>
    <PackageTags>configuration;apq;http;api</PackageTags>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <RepositoryUrl>https://github.com/yourname/Apq.Cfg.HttpApi</RepositoryUrl>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Apq.Cfg" Version="1.0.*" />
  </ItemGroup>
  
  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
  </ItemGroup>
</Project>
```

2. 创建 README.md：

```markdown
# Apq.Cfg.HttpApi

HTTP API configuration source for Apq.Cfg.

## Installation

```bash
dotnet add package Apq.Cfg.HttpApi
```

## Usage

```csharp
var cfg = new CfgBuilder()
    .AddHttpApi(options =>
    {
        options.BaseUrl = "http://config-server:5000";
        options.Endpoint = "/api/config";
        options.EnableHotReload = true;
    }, level: 10)
    .Build();
```
```

3. 打包发布：

```bash
dotnet pack -c Release
dotnet nuget push bin/Release/Apq.Cfg.HttpApi.1.0.0.nupkg -s https://api.nuget.org/v3/index.json -k YOUR_API_KEY
```

## 最佳实践

### 配置源开发建议

| 建议 | 说明 |
|------|------|
| 实现 IDisposable | 正确释放资源（HTTP 客户端、定时器等） |
| 线程安全 | 使用 ConcurrentDictionary 存储数据 |
| 错误处理 | 捕获异常，记录日志，不影响应用启动 |
| 可选配置 | 支持 optional 参数，允许配置源不存在 |
| 热重载 | 实现变更检测和通知机制 |
| 单元测试 | 编写完整的测试用例 |

### 性能优化建议

1. **缓存数据**：避免频繁读取远程配置
2. **批量保存**：合并多次写入操作
3. **异步操作**：使用异步方法避免阻塞
4. **连接池**：复用 HTTP 客户端等资源

## 下一步

- [API 参考](/api/) - 完整 API 文档
- [示例](/examples/) - 更多示例代码
- [复杂场景](/examples/complex-scenarios) - 企业级应用示例
