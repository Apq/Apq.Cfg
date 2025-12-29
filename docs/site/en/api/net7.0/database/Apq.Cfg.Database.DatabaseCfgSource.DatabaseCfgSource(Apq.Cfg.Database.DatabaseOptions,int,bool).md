### [Apq\.Cfg\.Database](Apq.Cfg.Database.md 'Apq\.Cfg\.Database').[DatabaseCfgSource](Apq.Cfg.Database.DatabaseCfgSource.md 'Apq\.Cfg\.Database\.DatabaseCfgSource')

## DatabaseCfgSource\(DatabaseOptions, int, bool\) Constructor

初始化 DatabaseCfgSource 实例

```csharp
public DatabaseCfgSource(Apq.Cfg.Database.DatabaseOptions options, int level, bool isPrimaryWriter);
```
#### Parameters

<a name='Apq.Cfg.Database.DatabaseCfgSource.DatabaseCfgSource(Apq.Cfg.Database.DatabaseOptions,int,bool).options'></a>

`options` [DatabaseOptions](Apq.Cfg.Database.DatabaseOptions.md 'Apq\.Cfg\.Database\.DatabaseOptions')

数据库连接选项

<a name='Apq.Cfg.Database.DatabaseCfgSource.DatabaseCfgSource(Apq.Cfg.Database.DatabaseOptions,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Database.DatabaseCfgSource.DatabaseCfgSource(Apq.Cfg.Database.DatabaseOptions,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主要写入源