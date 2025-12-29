### [Apq\.Cfg\.Redis](Apq.Cfg.Redis.md 'Apq\.Cfg\.Redis')

## RedisCfgSource Class

Redis 配置源

```csharp
internal sealed class RedisCfgSource : Apq.Cfg.Sources.IWritableCfgSource, Apq.Cfg.Sources.ICfgSource, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; RedisCfgSource

Implements [Apq\.Cfg\.Sources\.IWritableCfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.iwritablecfgsource 'Apq\.Cfg\.Sources\.IWritableCfgSource'), [Apq\.Cfg\.Sources\.ICfgSource](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource 'Apq\.Cfg\.Sources\.ICfgSource'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Constructors | |
| :--- | :--- |
| [RedisCfgSource\(RedisOptions, int, bool\)](Apq.Cfg.Redis.RedisCfgSource.RedisCfgSource(Apq.Cfg.Redis.RedisOptions,int,bool).md 'Apq\.Cfg\.Redis\.RedisCfgSource\.RedisCfgSource\(Apq\.Cfg\.Redis\.RedisOptions, int, bool\)') | 初始化 RedisCfgSource 实例 |

| Properties | |
| :--- | :--- |
| [IsPrimaryWriter](Apq.Cfg.Redis.RedisCfgSource.IsPrimaryWriter.md 'Apq\.Cfg\.Redis\.RedisCfgSource\.IsPrimaryWriter') | 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标 |
| [IsWriteable](Apq.Cfg.Redis.RedisCfgSource.IsWriteable.md 'Apq\.Cfg\.Redis\.RedisCfgSource\.IsWriteable') | 获取是否可写，Redis 支持通过 API 写入配置，因此始终为 true |
| [Level](Apq.Cfg.Redis.RedisCfgSource.Level.md 'Apq\.Cfg\.Redis\.RedisCfgSource\.Level') | 获取配置层级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ApplyChangesAsync\(IReadOnlyDictionary&lt;string,string&gt;, CancellationToken\)](Apq.Cfg.Redis.RedisCfgSource.ApplyChangesAsync(System.Collections.Generic.IReadOnlyDictionary_string,string_,System.Threading.CancellationToken).md 'Apq\.Cfg\.Redis\.RedisCfgSource\.ApplyChangesAsync\(System\.Collections\.Generic\.IReadOnlyDictionary\<string,string\>, System\.Threading\.CancellationToken\)') | 应用配置更改到 Redis |
| [BuildSource\(\)](Apq.Cfg.Redis.RedisCfgSource.BuildSource().md 'Apq\.Cfg\.Redis\.RedisCfgSource\.BuildSource\(\)') | 构建 Microsoft\.Extensions\.Configuration 的内存配置源，从 Redis 加载数据 |
| [Dispose\(\)](Apq.Cfg.Redis.RedisCfgSource.Dispose().md 'Apq\.Cfg\.Redis\.RedisCfgSource\.Dispose\(\)') | 释放资源，关闭 Redis 连接 |
| [EnsureAllowAdmin\(string\)](Apq.Cfg.Redis.RedisCfgSource.EnsureAllowAdmin(string).md 'Apq\.Cfg\.Redis\.RedisCfgSource\.EnsureAllowAdmin\(string\)') | 确保连接字符串包含 allowAdmin 选项 |
| [ThrowIfDisposed\(\)](Apq.Cfg.Redis.RedisCfgSource.ThrowIfDisposed().md 'Apq\.Cfg\.Redis\.RedisCfgSource\.ThrowIfDisposed\(\)') | 检查对象是否已释放，如果已释放则抛出异常 |
