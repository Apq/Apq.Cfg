### [Apq\.Cfg\.Env](Apq.Cfg.Env.md 'Apq\.Cfg\.Env')

## EnvConfigurationProvider Class

\.env 文件配置提供程序

```csharp
internal sealed class EnvConfigurationProvider : Microsoft.Extensions.Configuration.FileConfigurationProvider
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Microsoft\.Extensions\.Configuration\.ConfigurationProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.configurationprovider 'Microsoft\.Extensions\.Configuration\.ConfigurationProvider') &#129106; [Microsoft\.Extensions\.Configuration\.FileConfigurationProvider](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.fileconfigurationprovider 'Microsoft\.Extensions\.Configuration\.FileConfigurationProvider') &#129106; EnvConfigurationProvider

| Methods | |
| :--- | :--- |
| [ParseEnvLine\(string\)](Apq.Cfg.Env.EnvConfigurationProvider.ParseEnvLine(string).md 'Apq\.Cfg\.Env\.EnvConfigurationProvider\.ParseEnvLine\(string\)') | 解析 \.env 文件行 |
| [UnquoteValue\(string\)](Apq.Cfg.Env.EnvConfigurationProvider.UnquoteValue(string).md 'Apq\.Cfg\.Env\.EnvConfigurationProvider\.UnquoteValue\(string\)') | 移除值的引号并处理转义 |
