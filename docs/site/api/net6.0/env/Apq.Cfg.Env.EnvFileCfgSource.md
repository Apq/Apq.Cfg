### [Apq\.Cfg\.Env](Apq.Cfg.Env.md 'Apq\.Cfg\.Env')

## EnvFileCfgSource Class

\.env 文件配置源

```csharp
internal sealed class EnvFileCfgSource : Apq.Cfg.Sources.File.FileCfgSourceBase, Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Apq\.Cfg\.Sources\.File\.FileCfgSourceBase](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.file.filecfgsourcebase 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase') &#129106; EnvFileCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource')

| Methods | |
| :--- | :--- |
| [CreatePhysicalFileProviderForEnv\(string\)](Apq.Cfg.Env.EnvFileCfgSource.CreatePhysicalFileProviderForEnv(string).md 'Apq\.Cfg\.Env\.EnvFileCfgSource\.CreatePhysicalFileProviderForEnv\(string\)') | 创建 PhysicalFileProvider，允许访问以点开头的文件（如 \.env） |
| [EscapeValue\(string\)](Apq.Cfg.Env.EnvFileCfgSource.EscapeValue(string).md 'Apq\.Cfg\.Env\.EnvFileCfgSource\.EscapeValue\(string\)') | 转义值中的特殊字符 |
| [NeedsQuoting\(string\)](Apq.Cfg.Env.EnvFileCfgSource.NeedsQuoting(string).md 'Apq\.Cfg\.Env\.EnvFileCfgSource\.NeedsQuoting\(string\)') | 检查值是否需要引号包裹 |
| [ParseEnvLine\(string\)](Apq.Cfg.Env.EnvFileCfgSource.ParseEnvLine(string).md 'Apq\.Cfg\.Env\.EnvFileCfgSource\.ParseEnvLine\(string\)') | 解析 \.env 文件行 |
| [UnquoteValue\(string\)](Apq.Cfg.Env.EnvFileCfgSource.UnquoteValue(string).md 'Apq\.Cfg\.Env\.EnvFileCfgSource\.UnquoteValue\(string\)') | 移除值的引号并处理转义 |
