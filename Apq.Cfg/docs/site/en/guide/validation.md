# Configuration Validation

Apq.Cfg provides a built-in configuration validation framework that supports validating configuration values at build time or runtime.

## Basic Usage

### Build-time Validation

```csharp
var cfg = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)
        .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
    .Build();

// Validate configuration
var result = cfg.Validate(v => v
    .Required("Database:ConnectionString")
    .Range("Database:Port", 1, 65535));

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"Configuration error: {error}");
    }
}
```

### Build and Validate

```csharp
// Build and validate, throw exception on failure
var (cfg, result) = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v.Required("Database:ConnectionString"))
    .BuildAndValidate();

// Build and validate, don't throw exception
var (cfg2, result2) = new CfgBuilder()
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v.Required("Database:ConnectionString"))
    .BuildAndValidate(throwOnError: false);

if (!result2.IsValid)
{
    // Handle validation errors
}
```

## Built-in Validation Rules

### Required - Required Validation

Validates that a configuration key exists and is not empty.

```csharp
cfg.Validate(v => v
    .Required("Database:ConnectionString")
    .Required("App:Name", "App:Version")); // Multiple keys
```

### Range - Range Validation

Validates that a numeric value is within a specified range. Supports int, long, double, decimal, and DateTime types.

```csharp
cfg.Validate(v => v
    .Range("Server:Port", 1, 65535)           // int
    .Range("App:Timeout", 0.1, 60.0)          // double
    .Range("Order:Amount", 0m, 1000000m));    // decimal
```

### Regex - Regular Expression Validation

Validates that a string matches a specified regular expression.

```csharp
cfg.Validate(v => v
    .Regex("App:Email", @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
    .Regex("App:Phone", @"^\d{11}$"));
```

### OneOf - Enum Values Validation

Validates that a value is in the allowed values list.

```csharp
cfg.Validate(v => v
    .OneOf("App:Environment", "Development", "Staging", "Production")
    .OneOf("App:LogLevel", new[] { "Debug", "Info", "Warning", "Error" }, ignoreCase: true));
```

### Length - Length Validation

Validates that a string length is within a specified range.

```csharp
cfg.Validate(v => v
    .Length("App:Name", minLength: 3, maxLength: 50)
    .MinLength("App:Description", 10)
    .MaxLength("App:Code", 20));
```

### DependsOn - Dependency Validation

Validates that when a dependency key exists, the current key must also exist.

```csharp
cfg.Validate(v => v
    .DependsOn("Database:Password", "Database:Username")); // If username exists, password must also exist
```

### Custom - Custom Validation

Use a custom function for validation.

```csharp
cfg.Validate(v => v
    .Custom("App:Name",
        value => value?.StartsWith("App") == true,
        "Application name must start with 'App'")
    .Custom("Database:Port",
        value => int.TryParse(value, out var port) && port % 2 == 0,
        "Port number must be even",
        ruleName: "EvenPort"));
```

## Extension Methods

### Validate

Validates configuration and returns the result.

```csharp
var result = cfg.Validate(v => v.Required("App:Name"));
```

### ValidateAndThrow

Validates configuration and throws `ConfigValidationException` on failure.

```csharp
try
{
    cfg.ValidateAndThrow(v => v.Required("App:Name"));
}
catch (ConfigValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Message}");
    foreach (var error in ex.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}
```

### TryValidate

Attempts to validate configuration and returns whether it succeeded.

```csharp
if (cfg.TryValidate(v => v.Required("App:Name"), out var result))
{
    Console.WriteLine("Configuration validation passed");
}
else
{
    Console.WriteLine($"Configuration validation failed with {result.ErrorCount} errors");
}
```

## Dependency Injection Integration

### Validate on Startup

```csharp
services.AddApqCfgWithValidation(cfg => cfg
    .AddJsonFile("config.json", level: 0)
    .AddValidation(v => v
        .Required("Database:ConnectionString")
        .Range("Database:Port", 1, 65535)));
```

### Register Validator

```csharp
services.AddApqCfg(cfg => cfg.AddJsonFile("config.json", level: 0));
services.AddConfigValidator(v => v
    .Required("Database:ConnectionString")
    .Range("Database:Port", 1, 65535));

// Use in service
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

// Check if validation passed
if (result.IsValid) { }

// Get error count
var count = result.ErrorCount;

// Get all errors
foreach (var error in result.Errors)
{
    Console.WriteLine($"[{error.RuleName}] {error.Key}: {error.Message}");
}

// Get errors for a specific key
var nameErrors = result.GetErrorsForKey("App:Name");

// Check if a specific key has errors
if (result.HasErrorsForKey("App:Name")) { }
```

## Custom Validation Rules

Implement the `IValidationRule` interface to create custom rules:

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
            return new ValidationError(Key, $"'{value}' is not a valid URL", Name, value);
        }

        return null;
    }
}

// Use custom rule
cfg.Validate(v => v.AddRule(new UrlRule("App:WebsiteUrl")));
```

## Best Practices

1. **Validate critical configuration at application startup**: Use `BuildAndValidate` or `AddApqCfgWithValidation` to ensure configuration is valid.

2. **Combine multiple rules**: Apply multiple rules to the same key.

```csharp
cfg.Validate(v => v
    .Required("Database:Port")
    .Range("Database:Port", 1, 65535));
```

3. **Use custom error messages**: Provide clear error messages to help troubleshoot issues.

```csharp
cfg.Validate(v => v
    .Required("Database:ConnectionString", "Database connection string is required")
    .Range("Database:Port", 1, 65535, "Port must be between 1 and 65535"));
```

4. **Validate configuration in CI/CD**: Validate configuration files before deployment.
