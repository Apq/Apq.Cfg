#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources\.Environment](Apq.Cfg.Sources.Environment.md 'Apq\.Cfg\.Sources\.Environment').[EnvVarsCfgSource](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource')

## EnvVarsCfgSource\(string, int\) Constructor

初始化 EnvVarsCfgSource 实例

```csharp
public EnvVarsCfgSource(string? prefix, int level);
```
#### Parameters

<a name='Apq.Cfg.Sources.Environment.EnvVarsCfgSource.EnvVarsCfgSource(string,int).prefix'></a>

`prefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

环境变量前缀，为 null 时加载所有环境变量

<a name='Apq.Cfg.Sources.Environment.EnvVarsCfgSource.EnvVarsCfgSource(string,int).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高