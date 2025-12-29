#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources\.Environment](Apq.Cfg.Sources.Environment.md 'Apq\.Cfg\.Sources\.Environment')

## EnvVarsCfgSource Class

环境变量配置源

```csharp
internal sealed class EnvVarsCfgSource : Apq.Cfg.Sources.ICfgSource
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EnvVarsCfgSource

Implements [ICfgSource](Apq.Cfg.Sources.ICfgSource.md 'Apq\.Cfg\.Sources\.ICfgSource')

| Constructors | |
| :--- | :--- |
| [EnvVarsCfgSource\(string, int\)](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.EnvVarsCfgSource(string,int).md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource\.EnvVarsCfgSource\(string, int\)') | 初始化 EnvVarsCfgSource 实例 |

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，环境变量配置源不支持写入，因此始终为 false |
| [IsWriteable](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.IsWriteable.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource\.IsWriteable') | 获取是否可写，环境变量配置源不支持写入，因此始终为 false |
| [Level](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.Level.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [BuildSource\(\)](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.BuildSource().md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的环境变量配置源 |
