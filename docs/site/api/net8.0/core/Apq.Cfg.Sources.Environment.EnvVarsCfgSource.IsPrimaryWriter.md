#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources\.Environment](Apq.Cfg.Sources.Environment.md 'Apq\.Cfg\.Sources\.Environment').[EnvVarsCfgSource](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource')

## EnvVarsCfgSource\.IsPrimaryWriter Property

获取是否为主要写入源，环境变量配置源不支持写入，因此始终为 false

```csharp
public bool IsPrimaryWriter { get; }
```

Implements [IsPrimaryWriter](Apq.Cfg.Sources.ICfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Sources\.ICfgSource\.IsPrimaryWriter')

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')