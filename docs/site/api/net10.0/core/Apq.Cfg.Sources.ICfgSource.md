#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources](Apq.Cfg.Sources.md 'Apq\.Cfg\.Sources')

## ICfgSource Interface

配置源接口，定义了配置源的基本行为

```csharp
public interface ICfgSource
```

Derived  
&#8627; [EnvVarsCfgSource](Apq.Cfg.Sources.Environment.EnvVarsCfgSource.md 'Apq\.Cfg\.Sources\.Environment\.EnvVarsCfgSource')  
&#8627; [FileCfgSourceBase](Apq.Cfg.Sources.File.FileCfgSourceBase.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase')  
&#8627; [IWritableCfgSource](Apq.Cfg.Sources.IWritableCfgSource.md 'Apq\.Cfg\.Sources\.IWritableCfgSource')  
&#8627; [JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Sources.ICfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Sources\.ICfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标 |
| [IsWriteable](Apq.Cfg.Sources.ICfgSource.IsWriteable.md 'Apq\.Cfg\.Sources\.ICfgSource\.IsWriteable') | 获取是否可写，指示该配置源是否支持写入操作 |
| [Level](Apq.Cfg.Sources.ICfgSource.Level.md 'Apq\.Cfg\.Sources\.ICfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [BuildSource\(\)](Apq.Cfg.Sources.ICfgSource.BuildSource().md 'Apq\.Cfg\.Sources\.ICfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的配置源 |
