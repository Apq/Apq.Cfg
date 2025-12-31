### [Apq\.Cfg\.Redis](Apq.Cfg.Redis.md 'Apq\.Cfg\.Redis').[RedisCfgSource](Apq.Cfg.Redis.RedisCfgSource.md 'Apq\.Cfg\.Redis\.RedisCfgSource')

## RedisCfgSource\(RedisOptions, int, bool\) Constructor

初始化 RedisCfgSource 实例

```csharp
public RedisCfgSource(Apq.Cfg.Redis.RedisOptions options, int level, bool isPrimaryWriter);
```
#### Parameters

<a name='Apq.Cfg.Redis.RedisCfgSource.RedisCfgSource(Apq.Cfg.Redis.RedisOptions,int,bool).options'></a>

`options` [RedisOptions](Apq.Cfg.Redis.RedisOptions.md 'Apq\.Cfg\.Redis\.RedisOptions')

Redis 连接选项

<a name='Apq.Cfg.Redis.RedisCfgSource.RedisCfgSource(Apq.Cfg.Redis.RedisOptions,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Redis.RedisCfgSource.RedisCfgSource(Apq.Cfg.Redis.RedisOptions,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主要写入源