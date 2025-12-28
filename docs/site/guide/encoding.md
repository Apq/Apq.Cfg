# 编码处理

Apq.Cfg 提供强大的文件编码检测和处理能力。

## 自动编码检测

默认情况下，Apq.Cfg 会自动检测文件编码：

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json") // 自动检测编码
    .Build();
```

### 支持的编码

| 编码 | BOM 检测 | 内容检测 |
|------|----------|----------|
| UTF-8 | ✅ | ✅ |
| UTF-8 (无 BOM) | - | ✅ |
| UTF-16 LE | ✅ | ✅ |
| UTF-16 BE | ✅ | ✅ |
| UTF-32 LE | ✅ | - |
| UTF-32 BE | ✅ | - |
| GB2312/GBK | - | ✅ |
| Big5 | - | ✅ |
| ISO-8859-1 | - | ✅ |

## 指定编码

### 显式指定编码

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", encoding: Encoding.UTF8)
    .AddYamlFile("config.yaml", encoding: Encoding.GetEncoding("GBK"))
    .Build();
```

### 使用编码选项

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", encodingOptions: new EncodingOptions
    {
        DefaultEncoding = Encoding.UTF8,
        DetectEncodingFromByteOrderMarks = true,
        FallbackEncoding = Encoding.GetEncoding("GBK")
    })
    .Build();
```

## 编码映射

为不同文件配置不同的编码：

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncoding(options =>
    {
        // 按文件扩展名映射
        options.AddMapping(".json", Encoding.UTF8);
        options.AddMapping(".yaml", Encoding.UTF8);
        options.AddMapping(".ini", Encoding.GetEncoding("GBK"));
        
        // 按文件名映射
        options.AddMapping("legacy-config.xml", Encoding.GetEncoding("GB2312"));
    })
    .AddJsonFile("config.json")
    .AddIniFile("settings.ini")
    .Build();
```

## 编码检测结果

获取检测到的编码信息：

```csharp
var source = new JsonFileCfgSource("config.json");
var result = source.DetectEncoding();

Console.WriteLine($"检测到的编码: {result.Encoding.EncodingName}");
Console.WriteLine($"检测方法: {result.DetectionMethod}");
Console.WriteLine($"置信度: {result.Confidence}");
```

### 检测方法

| 方法 | 说明 |
|------|------|
| `BOM` | 通过字节顺序标记检测 |
| `Content` | 通过内容分析检测 |
| `Default` | 使用默认编码 |
| `Specified` | 用户指定编码 |

## 处理编码错误

### 错误处理策略

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncoding(options =>
    {
        options.OnDecodingError = DecodingErrorAction.Replace;
        options.ReplacementChar = '?';
    })
    .AddJsonFile("config.json")
    .Build();
```

### 错误处理选项

| 选项 | 说明 |
|------|------|
| `Throw` | 抛出异常（默认） |
| `Replace` | 用替换字符替代 |
| `Skip` | 跳过无效字符 |

## 最佳实践

### 1. 统一使用 UTF-8

推荐所有配置文件使用 UTF-8 编码：

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncoding(options =>
    {
        options.DefaultEncoding = Encoding.UTF8;
    })
    .AddJsonFile("config.json")
    .Build();
```

### 2. 处理遗留文件

对于遗留系统的配置文件：

```csharp
var cfg = new CfgBuilder()
    .ConfigureEncoding(options =>
    {
        // 遗留 INI 文件使用 GBK
        options.AddMapping("*.ini", Encoding.GetEncoding("GBK"));
        // 新文件使用 UTF-8
        options.DefaultEncoding = Encoding.UTF8;
    })
    .Build();
```

### 3. 保存时保持编码

写入配置时保持原有编码：

```csharp
// 读取时记录编码
var detectedEncoding = source.DetectEncoding().Encoding;

// 写入时使用相同编码
File.WriteAllText("config.json", content, detectedEncoding);
```

## 下一步

- [编码处理流程](/guide/encoding-workflow) - 深入了解编码检测和写入的完整流程
- [性能优化](/guide/performance) - 性能调优指南
- [最佳实践](/guide/best-practices) - 最佳实践指南
