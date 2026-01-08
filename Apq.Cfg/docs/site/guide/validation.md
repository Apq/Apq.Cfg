# 配置验证

Apq.Cfg 提供了内置的配置验证框架，支持在构建时或运行时验证配置值的有效性。

## 基本用法

### 构建时验证

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)
        .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
    .Build();

// 验证配置
var result = cfg.Validate(v => v
    .Required("Database:ConnectionString")
    .Range("Database:Port", 1, 65535));

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"配置错误: {error}");
    }
}
```

### 构建并验证

```csharp
// 构建并验证，失败时抛出异常
var (cfg, result) = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v.Required("Database:ConnectionString"))
    .BuildAndValidate();

// 构建并验证，不抛出异常
var (cfg2, result2) = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v.Required("Database:ConnectionString"))
    .BuildAndValidate(throwOnError: false);

if (!result2.IsValid)
{
    // 处理验证错误
}
```

## 内置验证规则

### Required - 必填验证

验证配置键是否存在且不为空。

```csharp
cfg.Validate(v => v
    .Required("Database:ConnectionString")
    .Required("App:Name", "App:Version")); // 多个键
```

### Range - 范围验证

验证数值是否在指定范围内。支持 int、long、double、decimal、DateTime 类型。

```csharp
cfg.Validate(v => v
    .Range("Server:Port", 1, 65535)           // int
    .Range("App:Timeout", 0.1, 60.0)          // double
    .Range("Order:Amount", 0m, 1000000m));    // decimal
```

### Regex - 正则表达式验证

验证字符串是否匹配指定的正则表达式。

```csharp
cfg.Validate(v => v
    .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
    .Regex("App:Phone", @"^\d{11}$"));
```

### OneOf - 枚举值验证

验证值是否在允许的值列表中。

```csharp
cfg.Validate(v => v
    .OneOf("App:Environment", "Development", "Staging", "Production")
    .OneOf("App:LogLevel", new[] { "Debug", "Info", "Warning", "Error" }, ignoreCase: true));
```

### Length - 长度验证

验证字符串长度是否在指定范围内。

```csharp
cfg.Validate(v => v
    .Length("App:Name", minLength: 3, maxLength: 50)
    .MinLength("App:Description", 10)
    .MaxLength("App:Code", 20));
```

### DependsOn - 依赖验证

验证当依赖键存在时，当前键也必须存在。

```csharp
cfg.Validate(v => v
    .DependsOn("Database:Password", "Database:Username")); // 如果有用户名，密码也必须存在
```

### Custom - 自定义验证

使用自定义函数进行验证。

```csharp
cfg.Validate(v => v
    .Custom("App:Name",
        value => value?.StartsWith("App") == true,
        "应用名称必须以 'App' 开头")
    .Custom("Database:Port",
        value => int.TryParse(value, out var port) && port % 2 == 0,
        "端口号必须是偶数",
        ruleName: "EvenPort"));
```

## 扩展方法

### Validate

验证配置并返回结果。

```csharp
var result = cfg.Validate(v => v.Required("App:Name"));
```

### ValidateAndThrow

验证配置，失败时抛出 `ConfigValidationException`。

```csharp
try
{
    cfg.ValidateAndThrow(v => v.Required("App:Name"));
}
catch (ConfigValidationException ex)
{
    Console.WriteLine($"验证失败: {ex.Message}");
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

### TryValidate

尝试验证配置，返回是否成功。

```csharp
if (cfg.TryValidate(v => v.Required("App:Name"), out var result))
{
    Console.WriteLine("配置验证通过");
}
else
{
    Console.WriteLine($"配置验证失败，共 {result.ErrorCount} 个错误");
}
```

## 依赖注入集成

### 启动时验证

```csharp
services.AddApqCfgWithValidation(cfg => cfg
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)));
```

### 注册验证器

```csharp
services.AddApqCfg(cfg => cfg.AddJsonFile("config.json", level: 0));
services.AddConfigValidator(v => v
    .Required("Database:ConnectionString")
    .Range("Database:Port", 1, 65535));

// 在服务中使用
public class MyService
{
    public MyService(ICfgRoot cfg, IConfigValidator validator)
    {
        var result = cfg.Validate(validator);
        if (!result.IsValid)
        {
            throw new ConfigValidationException(result);
        }
    }
}
```

## ValidationResult API

```csharp
var result = cfg.Validate(v => v.Required("App:Name"));

// 检查是否验证通过
if (result.IsValid) { }

// 获取错误数量
var count = result.ErrorCount;

// 获取所有错误
foreach (var error in result.Errors)
{
    Console.WriteLine($"[{error.RuleName}] {error.Key}: {error.Message}");
}

// 获取指定键的错误
var nameErrors = result.GetErrorsForKey("App:Name");

// 检查指定键是否有错误
if (result.HasErrorsForKey("App:Name")) { }
```

## 自定义验证规则

实现 `IValidationRule` 接口创建自定义规则：

```csharp
public class UrlRule : IValidationRule
{
    public string Name => "Url";
    public string Key { get; }

    public UrlRule(string key)
    {
        Key = key;
    }

    public ValidationError? Validate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            return new ValidationError(Key, $"'{value}' 不是有效的 URL", Name, value);
        }

        return null;
    }
}

// 使用自定义规则
cfg.Validate(v => v.AddRule(new UrlRule("App:WebsiteUrl")));
```

## 最佳实践

1. **在应用启动时验证关键配置**：使用 `BuildAndValidate` 或 `AddApqCfgWithValidation` 确保配置有效。

2. **组合使用多个规则**：对同一个键可以应用多个规则。

```csharp
cfg.Validate(v => v
    .Required("Database:Port")
    .Range("Database:Port", 1, 65535));
```

3. **使用自定义错误消息**：提供清晰的错误消息帮助排查问题。

```csharp
cfg.Validate(v => v
    .Required("Database:ConnectionString", "数据库连接字符串是必需的")
    .Range("Database:Port", 1, 65535, "端口号必须在 1-65535 之间"));
```

4. **在 CI/CD 中验证配置**：在部署前验证配置文件的有效性。
