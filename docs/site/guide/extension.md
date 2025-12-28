# 扩展开发

本指南介绍如何开发自定义配置源。

## 配置源接口

### ICfgSource 接口

```csharp
public interface ICfgSource
{
    /// <summary>
    /// 加载配置数据
    /// </summary>
    Task<IDictionary<string, string>> LoadAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 配置源名称
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// 是否支持重载
    /// </summary>
    bool SupportsReload { get; }
    
    /// <summary>
    /// 获取重载令牌
    /// </summary>
    IChangeToken? GetReloadToken();
}
```

## 实现自定义配置源

### 基本实现

```csharp
public class MyCustomCfgSource : ICfgSource
{
    private readonly string _connectionString;
    
    public MyCustomCfgSource(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public string Name => "MyCustomSource";
    
    public bool SupportsReload => false;
    
    public IChangeToken? GetReloadToken() => null;
    
    public async Task<IDictionary<string, string>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, string>();
        
        // 从自定义数据源加载配置
        // 例如：数据库、API、文件等
        
        return data;
    }
}
```

### 支持重载的实现

```csharp
public class MyReloadableCfgSource : ICfgSource, IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private ConfigurationReloadToken _reloadToken = new();
    
    public string Name => "MyReloadableSource";
    
    public bool SupportsReload => true;
    
    public IChangeToken GetReloadToken() => _reloadToken;
    
    public async Task<IDictionary<string, string>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        // 加载配置...
        return new Dictionary<string, string>();
    }
    
    /// <summary>
    /// 触发重载
    /// </summary>
    public void TriggerReload()
    {
        var previousToken = Interlocked.Exchange(
            ref _reloadToken, 
            new ConfigurationReloadToken());
        previousToken.OnReload();
    }
    
    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
```

## 扩展方法

### 创建扩展方法

```csharp
public static class CfgBuilderExtensions
{
    public static CfgBuilder AddMyCustomSource(
        this CfgBuilder builder,
        string connectionString,
        bool optional = false)
    {
        var source = new MyCustomCfgSource(connectionString);
        return builder.AddSource(source, optional);
    }
    
    public static CfgBuilder AddMyCustomSource(
        this CfgBuilder builder,
        Action<MyCustomOptions> configure)
    {
        var options = new MyCustomOptions();
        configure(options);
        
        var source = new MyCustomCfgSource(options);
        return builder.AddSource(source, options.Optional);
    }
}
```

### 使用扩展方法

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("appsettings.json")
    .AddMyCustomSource("connection-string")
    .AddMyCustomSource(options =>
    {
        options.ConnectionString = "...";
        options.Optional = true;
        options.ReloadOnChange = true;
    })
    .Build();
```

## 文件配置源基类

### 继承 FileCfgSourceBase

```csharp
public class MyFileCfgSource : FileCfgSourceBase
{
    public MyFileCfgSource(string path, bool optional = false, bool reloadOnChange = false)
        : base(path, optional, reloadOnChange)
    {
    }
    
    public override string Name => $"MyFile:{Path}";
    
    protected override IDictionary<string, string> ParseContent(string content)
    {
        var data = new Dictionary<string, string>();
        
        // 解析文件内容
        // 将内容转换为扁平的键值对
        
        return data;
    }
}
```

## 远程配置源示例

### HTTP API 配置源

```csharp
public class HttpApiCfgSource : ICfgSource, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private readonly TimeSpan _pollInterval;
    private readonly Timer _timer;
    private ConfigurationReloadToken _reloadToken = new();
    
    public HttpApiCfgSource(string url, TimeSpan pollInterval)
    {
        _url = url;
        _pollInterval = pollInterval;
        _httpClient = new HttpClient();
        
        // 定时轮询
        _timer = new Timer(async _ => await PollForChanges(), 
            null, pollInterval, pollInterval);
    }
    
    public string Name => $"HttpApi:{_url}";
    
    public bool SupportsReload => true;
    
    public IChangeToken GetReloadToken() => _reloadToken;
    
    public async Task<IDictionary<string, string>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync(_url, cancellationToken);
        var json = JsonDocument.Parse(response);
        
        var data = new Dictionary<string, string>();
        FlattenJson(json.RootElement, "", data);
        
        return data;
    }
    
    private void FlattenJson(JsonElement element, string prefix, 
        Dictionary<string, string> data)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var key = string.IsNullOrEmpty(prefix) 
                        ? property.Name 
                        : $"{prefix}:{property.Name}";
                    FlattenJson(property.Value, key, data);
                }
                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    FlattenJson(item, $"{prefix}:{index++}", data);
                }
                break;
            default:
                data[prefix] = element.ToString();
                break;
        }
    }
    
    private async Task PollForChanges()
    {
        // 检测变更并触发重载
        var previousToken = Interlocked.Exchange(
            ref _reloadToken, 
            new ConfigurationReloadToken());
        previousToken.OnReload();
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
        _httpClient?.Dispose();
    }
}
```

## 测试配置源

```csharp
[Fact]
public async Task LoadAsync_ReturnsExpectedData()
{
    // Arrange
    var source = new MyCustomCfgSource("test-connection");
    
    // Act
    var data = await source.LoadAsync();
    
    // Assert
    Assert.NotEmpty(data);
    Assert.Equal("expected-value", data["Expected:Key"]);
}

[Fact]
public void GetReloadToken_ReturnsValidToken()
{
    // Arrange
    var source = new MyReloadableCfgSource();
    
    // Act
    var token = source.GetReloadToken();
    
    // Assert
    Assert.NotNull(token);
    Assert.False(token.HasChanged);
}
```

## 发布 NuGet 包

### 项目文件配置

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net6.0;net7.0;net8.0</TargetFrameworks>
    <PackageId>Apq.Cfg.MyCustomSource</PackageId>
    <Version>1.0.0</Version>
    <Authors>Your Name</Authors>
    <Description>Apq.Cfg 自定义配置源扩展</Description>
    <PackageTags>apq.cfg;configuration;custom</PackageTags>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Apq.Cfg" Version="1.0.*" />
  </ItemGroup>
</Project>
```

## 下一步

- [API 参考](/api/) - 完整 API 文档
- [示例](/examples/) - 更多示例代码
