# 编码处理

Apq.Cfg 提供强大的文件编码检测和处理能力。

## 自动编码检测

默认情况下，Apq.Cfg 会自动检测文件编码：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: false) // 自动检测编码
    .Build();
```

### 检测优先级

1. **BOM 检测**：首先检查文件的字节顺序标记
2. **映射规则**：检查是否有匹配的编码映射规则
3. **UTF.Unknown 库**：使用 UTF.Unknown 库进行内容分析
4. **回退编码**：使用配置的回退编码（默认 UTF-8）

### 支持的 BOM 编码

| BOM 字节 | 编码 |
|---------|------|
| `EF BB BF` | UTF-8 |
| `FF FE` | UTF-16 LE |
| `FE FF` | UTF-16 BE |
| `FF FE 00 00` | UTF-32 LE |
| `00 00 FE FF` | UTF-32 BE |

## 编码选项

### EncodingOptions 配置

```csharp
var options = new EncodingOptions
{
    ReadStrategy = EncodingReadStrategy.AutoDetect,  // 读取策略
    WriteStrategy = EncodingWriteStrategy.Utf8NoBom, // 写入策略
    FallbackEncoding = Encoding.UTF8,                // 回退编码
    ConfidenceThreshold = 0.6f,                      // 置信度阈值
    EnableCache = true,                              // 启用缓存
    EnableLogging = false                            // 启用日志
};

var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, encoding: options)
    .Build();
```

### 读取策略

| 策略 | 说明 |
|------|------|
| `AutoDetect` | 自动检测编码（默认） |
| `Specified` | 使用指定的编码 |

### 写入策略

| 策略 | 说明 |
|------|------|
| `Utf8NoBom` | UTF-8 无 BOM（默认） |
| `Utf8WithBom` | UTF-8 带 BOM |
| `Preserve` | 保持原文件编码 |
| `Specified` | 使用指定的编码 |

## 预定义编码选项

```csharp
// 默认配置：自动检测读取，UTF-8 无 BOM 写入
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, encoding: EncodingOptions.Default)
    .Build();

// PowerShell 脚本配置：UTF-8 带 BOM
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true, encoding: EncodingOptions.PowerShell)
    .Build();
```

## 编码映射

为不同文件配置不同的编码：

### 完整路径映射

```csharp
var cfg = new CfgBuilder()
    .AddReadEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))
    .AddWriteEncodingMapping(@"C:\legacy\old.ini", Encoding.GetEncoding("GB2312"))
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 通配符映射

```csharp
var cfg = new CfgBuilder()
    // 所有 PS1 文件写入时使用 UTF-8 BOM
    .AddWriteEncodingMappingWildcard("*.ps1", new UTF8Encoding(true))
    // 所有 INI 文件使用 GBK
    .AddReadEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GBK"))
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 正则表达式映射

```csharp
var cfg = new CfgBuilder()
    // 日志文件使用 Unicode
    .AddWriteEncodingMappingRegex(@"logs[/\\].*\.log$", Encoding.Unicode)
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 通配符语法

| 符号 | 含义 | 示例 |
|------|------|------|
| `*` | 匹配任意字符（不含路径分隔符） | `*.json` 匹配 `config.json` |
| `**` | 匹配任意字符（含路径分隔符） | `**/*.txt` 匹配 `a/b/c.txt` |
| `?` | 匹配单个字符 | `config?.json` 匹配 `config1.json` |

## 高级配置

### 配置编码映射

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncodingMapping(config =>
    {
        // 添加多条规则
        config.AddReadMapping("*.xml", EncodingMappingType.Wildcard,
            Encoding.UTF8, priority: 50);
        config.AddWriteMapping("**/*.txt", EncodingMappingType.Wildcard,
            new UTF8Encoding(true), priority: 10);

        // 清除默认规则
        config.ClearWriteMappings();
    })
    .WithEncodingConfidenceThreshold(0.8f)  // 提高检测置信度阈值
    .WithEncodingDetectionLogging(result =>  // 启用日志
    {
        Console.WriteLine($"检测到编码: {result}");
    })
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 映射规则优先级

| 匹配类型 | 默认优先级 | 说明 |
|---------|-----------|------|
| ExactPath | 100 | 完整路径精确匹配 |
| Wildcard | 0 | 通配符匹配 |
| Regex | 0 | 正则表达式匹配 |
| 内置 PowerShell | -100 | `*.ps1`, `*.psm1`, `*.psd1` |

## 缓存机制

编码检测结果会被缓存以提高性能：

```csharp
// 禁用缓存
var options = new EncodingOptions { EnableCache = false };

// 手动清除缓存
FileCfgSourceBase.EncodingDetector.ClearCache();

// 清除特定文件的缓存
FileCfgSourceBase.EncodingDetector.InvalidateCache("config.json");
```

## 内置默认映射

`EncodingDetector` 构造时自动添加以下写入映射：

| 模式 | 编码 | 优先级 |
|------|------|--------|
| `*.ps1` | UTF-8 BOM | -100 |
| `*.psm1` | UTF-8 BOM | -100 |
| `*.psd1` | UTF-8 BOM | -100 |

这些默认映射优先级最低，可被用户配置覆盖。

## 环境变量

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `APQ_CFG_ENCODING_CONFIDENCE` | 编码检测置信度阈值 | 0.6 |

## 最佳实践

### 1. 统一使用 UTF-8

推荐所有配置文件使用 UTF-8 编码：

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
// 默认写入策略就是 UTF-8 无 BOM
```

### 2. 处理遗留文件

对于遗留系统的配置文件：

```csharp
var cfg = new CfgBuilder()
    // 遗留 INI 文件使用 GBK
    .AddReadEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GBK"))
    .AddWriteEncodingMappingWildcard("*.ini", Encoding.GetEncoding("GBK"))
    .AddJson("config.json", level: 0, writeable: true)
    .Build();
```

### 3. 保存时保持编码

使用 `Preserve` 策略保持原有编码：

```csharp
var options = new EncodingOptions
{
    WriteStrategy = EncodingWriteStrategy.Preserve
};

var cfg = new CfgBuilder()
    .AddJson("legacy.json", level: 0, writeable: true, encoding: options)
    .Build();
```

## 下一步

- [编码处理流程](/guide/encoding-workflow) - 深入了解编码检测和写入的完整流程
- [性能优化](/guide/performance) - 性能调优指南
- [最佳实践](/guide/best-practices) - 最佳实践指南
