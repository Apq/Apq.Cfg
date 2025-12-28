# 性能优化

本节介绍 Apq.Cfg 的性能特性和优化建议。

## 性能特性

### 零反射绑定

使用源代码生成器可以实现编译时绑定，避免运行时反射开销：

```csharp
// 添加源代码生成器包
// dotnet add package Apq.Cfg.SourceGenerator

[CfgSection("App")]
public partial class AppSettings
{
    public string Name { get; set; }
    public int MaxRetries { get; set; }
}

// 生成的代码在编译时完成绑定
var settings = cfg.GetSection("App").Bind<AppSettings>();
```

### 内置缓存

Apq.Cfg 内置多级缓存机制：

- **解析缓存**：配置文件解析结果缓存
- **值缓存**：类型转换结果缓存
- **绑定缓存**：对象绑定结果缓存

### 延迟加载

配置值在首次访问时才进行解析和转换。

## 基准测试结果

| 操作 | Apq.Cfg | Microsoft.Extensions.Configuration |
|-----|---------|-------------------------------------|
| 读取字符串 | 15 ns | 18 ns |
| 读取整数 | 22 ns | 45 ns |
| 绑定对象 | 120 ns | 850 ns |
| 绑定对象（源生成器） | 45 ns | - |

*测试环境：.NET 8, Intel i7-12700, 32GB RAM*

## 优化建议

### 1. 使用源代码生成器

对于频繁访问的配置，使用源代码生成器可以显著提升性能：

```csharp
[CfgSection("Database")]
public partial class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }
    public int MaxPoolSize { get; set; }
}
```

### 2. 缓存绑定结果

```csharp
// 在启动时绑定并缓存
public class ConfigurationService
{
    public AppSettings App { get; }
    public DatabaseSettings Database { get; }

    public ConfigurationService(ICfgRoot cfg)
    {
        App = cfg.GetSection("App").Bind<AppSettings>();
        Database = cfg.GetSection("Database").Bind<DatabaseSettings>();
    }
}
```

### 3. 避免频繁重载

```csharp
// 设置合理的重载间隔
builder.AddJsonFile("config.json", reloadOnChange: true, reloadDelay: TimeSpan.FromSeconds(5));
```

## 更多内容

- [缓存策略详解](/performance/caching)
- [基准测试报告](/performance/benchmarks)
