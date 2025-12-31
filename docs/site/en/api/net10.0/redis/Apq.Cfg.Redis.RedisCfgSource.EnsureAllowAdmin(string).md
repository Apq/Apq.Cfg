### [Apq\.Cfg\.Redis](Apq.Cfg.Redis.md 'Apq\.Cfg\.Redis').[RedisCfgSource](Apq.Cfg.Redis.RedisCfgSource.md 'Apq\.Cfg\.Redis\.RedisCfgSource')

## RedisCfgSource\.EnsureAllowAdmin\(string\) Method

确保连接字符串包含 allowAdmin 选项

```csharp
private static string EnsureAllowAdmin(string connectionString);
```
#### Parameters

<a name='Apq.Cfg.Redis.RedisCfgSource.EnsureAllowAdmin(string).connectionString'></a>

`connectionString` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

原始连接字符串

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
包含 allowAdmin 选项的连接字符串