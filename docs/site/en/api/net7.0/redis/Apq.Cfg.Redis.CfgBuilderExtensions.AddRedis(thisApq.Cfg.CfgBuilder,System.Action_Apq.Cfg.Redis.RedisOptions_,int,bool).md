### [Apq\.Cfg\.Redis](Apq.Cfg.Redis.md 'Apq\.Cfg\.Redis').[CfgBuilderExtensions](Apq.Cfg.Redis.CfgBuilderExtensions.md 'Apq\.Cfg\.Redis\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddRedis\(this CfgBuilder, Action\<RedisOptions\>, int, bool\) Method

添加 Redis 配置源

```csharp
public static Apq.Cfg.CfgBuilder AddRedis(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Redis.RedisOptions> configure, int level, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Redis.CfgBuilderExtensions.AddRedis(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Redis.RedisOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

<a name='Apq.Cfg.Redis.CfgBuilderExtensions.AddRedis(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Redis.RedisOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[RedisOptions](Apq.Cfg.Redis.RedisOptions.md 'Apq\.Cfg\.Redis\.RedisOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

<a name='Apq.Cfg.Redis.CfgBuilderExtensions.AddRedis(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Redis.RedisOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='Apq.Cfg.Redis.CfgBuilderExtensions.AddRedis(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Redis.RedisOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')