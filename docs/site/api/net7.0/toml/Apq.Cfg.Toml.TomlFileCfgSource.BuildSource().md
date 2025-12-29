### [Apq\.Cfg\.Toml](Apq.Cfg.Toml.md 'Apq\.Cfg\.Toml').[TomlFileCfgSource](Apq.Cfg.Toml.TomlFileCfgSource.md 'Apq\.Cfg\.Toml\.TomlFileCfgSource')

## TomlFileCfgSource\.BuildSource\(\) Method

构建 Microsoft\.Extensions\.Configuration 的 TOML 配置源

```csharp
public override Microsoft.Extensions.Configuration.IConfigurationSource BuildSource();
```

Implements [BuildSource\(\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource.buildsource 'Apq\.Cfg\.Sources\.ICfgSource\.BuildSource')

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationSource](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationsource 'Microsoft\.Extensions\.Configuration\.IConfigurationSource')  
TomlSource 实例，内部实现了 IConfigurationSource