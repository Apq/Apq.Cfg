### [Apq\.Cfg\.Redis](Apq.Cfg.Redis.md 'Apq\.Cfg\.Redis').[RedisCfgSource](Apq.Cfg.Redis.RedisCfgSource.md 'Apq\.Cfg\.Redis\.RedisCfgSource')

## RedisCfgSource\.BuildSource\(\) Method

构建 Microsoft\.Extensions\.Configuration 的内存配置源，从 Redis 加载数据

```csharp
public Microsoft.Extensions.Configuration.IConfigurationSource BuildSource();
```

Implements [BuildSource\(\)](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.sources.icfgsource.buildsource 'Apq\.Cfg\.Sources\.ICfgSource\.BuildSource')

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationSource](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationsource 'Microsoft\.Extensions\.Configuration\.IConfigurationSource')  
Microsoft\.Extensions\.Configuration\.Memory\.MemoryConfigurationSource 实例

#### Exceptions

[System\.ObjectDisposedException](https://learn.microsoft.com/en-us/dotnet/api/system.objectdisposedexception 'System\.ObjectDisposedException')  
当对象已释放时抛出