### [Apq\.Cfg\.Database](Apq.Cfg.Database.md 'Apq\.Cfg\.Database').[CfgBuilderExtensions](Apq.Cfg.Database.CfgBuilderExtensions.md 'Apq\.Cfg\.Database\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddDatabase\(this CfgBuilder, Action\<DatabaseOptions\>, int, bool\) Method

添加数据库配置源

```csharp
public static Apq.Cfg.CfgBuilder AddDatabase(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Database.DatabaseOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Database.CfgBuilderExtensions.AddDatabase(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Database.DatabaseOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Database.CfgBuilderExtensions.AddDatabase(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Database.DatabaseOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[DatabaseOptions](Apq.Cfg.Database.DatabaseOptions.md 'Apq\.Cfg\.Database\.DatabaseOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

<a name='Apq.Cfg.Database.CfgBuilderExtensions.AddDatabase(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Database.DatabaseOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='Apq.Cfg.Database.CfgBuilderExtensions.AddDatabase(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Database.DatabaseOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')