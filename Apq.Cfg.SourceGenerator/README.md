# Apq.Cfg.SourceGenerator

基于 Roslyn 的源生成器，为 Apq.Cfg 配置类自动生成零反射的绑定代码，支持 Native AOT。

## 特性

- **零反射绑定** - 编译时生成强类型绑定代码，无运行时反射开销
- **Native AOT 兼容** - 完全支持 .NET Native AOT 发布
- **增量生成** - 使用 Roslyn 增量源生成器，仅在代码变更时重新生成
- **丰富类型支持** - 支持简单类型、嵌套对象、数组、List、Dictionary、HashSet 等

## 安装

```bash
dotnet add package Apq.Cfg.SourceGenerator
```

## 使用方法

### 1. 定义配置类

使用 `[CfgSection]` 特性标记配置类，类必须是 `partial` 的：

```csharp
using Apq.Cfg;

[CfgSection("AppSettings")]
public partial class AppConfig
{
    public string? Name { get; set; }
    public int Port { get; set; }
    public DatabaseConfig? Database { get; set; }
}

[CfgSection]
public partial class DatabaseConfig
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
}
```

### 2. 使用生成的绑定方法

源生成器会自动为每个配置类生成 `BindFrom` 和 `BindTo` 静态方法：

```csharp
// 从配置节创建新实例
var config = AppConfig.BindFrom(cfgRoot.GetSection("AppSettings"));

// 或绑定到已有实例
var existingConfig = new AppConfig();
AppConfig.BindTo(cfgRoot.GetSection("AppSettings"), existingConfig);
```

### 3. 使用扩展方法（可选）

如果在 `[CfgSection]` 中指定了 `SectionPath`，还会生成 `ICfgRoot` 扩展方法：

```csharp
// 直接从 ICfgRoot 获取配置
var config = cfgRoot.GetAppConfig();
```

## 支持的类型

### 简单类型
- `string`, `int`, `long`, `short`, `byte`, `sbyte`
- `uint`, `ulong`, `ushort`
- `float`, `double`, `decimal`
- `bool`, `char`
- `DateTime`, `DateTimeOffset`, `TimeSpan`
- `Guid`, `Uri`
- `DateOnly`, `TimeOnly` (.NET 6+)
- 枚举类型

### 集合类型
- `T[]` (数组)
- `List<T>`
- `HashSet<T>`
- `Dictionary<TKey, TValue>`

### 复杂类型
- 嵌套的配置类（需要同样标记 `[CfgSection]`）

## 生成的代码示例

对于上面的 `AppConfig` 类，源生成器会生成类似以下的代码：

```csharp
partial class AppConfig
{
    public static AppConfig BindFrom(ICfgSection section)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        var result = new AppConfig();
        BindTo(section, result);
        return result;
    }

    public static void BindTo(ICfgSection section, AppConfig target)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));
        if (target == null) throw new ArgumentNullException(nameof(target));

        // Name: string?
        {
            var __value = section.Get("Name");
            if (__value != null)
            {
                target.Name = __value;
            }
        }

        // Port: int
        {
            var __value = section.Get("Port");
            if (__value != null)
            {
                var __converted = int.TryParse(__value, out var __intVal) ? __intVal : (int?)null;
                if (__converted != null) target.Port = __converted.Value;
            }
        }

        // Database: DatabaseConfig? (复杂对象)
        {
            var __childSection = section.GetSection("Database");
            var __childKeys = __childSection.GetChildKeys().ToList();
            if (__childKeys.Count > 0)
            {
                target.Database = DatabaseConfig.BindFrom(__childSection);
            }
        }
    }
}
```

## 查看生成的代码

在项目文件中添加以下配置可以保留生成的源代码：

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)\GeneratedFiles</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

生成的文件将位于 `obj/GeneratedFiles/` 目录下。

## 与 Apq.Cfg 配合使用

```csharp
using Apq.Cfg;

// 使用 CfgBuilder 创建配置根
var cfgRoot = new CfgBuilder()
    .AddJson("config.json")
    .AddIni("config.ini")
    .AddEnvironmentVariables("APP_")
    .Build();

// 使用源生成器绑定配置
var appConfig = AppConfig.BindFrom(cfgRoot.GetSection("App"));

Console.WriteLine($"App: {appConfig.Name}");
Console.WriteLine($"Port: {appConfig.Port}");
Console.WriteLine($"Database: {appConfig.Database?.ConnectionString}");
```

## 要求

- .NET 6.0 或更高版本
- C# 9.0 或更高版本（支持 `partial` 类）

## 许可证

MIT License
