#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.GetMasked\(string\) Method

获取脱敏后的配置值（用于日志输出）

```csharp
public string GetMasked(string key);
```
#### Parameters

<a name='Apq.Cfg.MergedCfgRoot.GetMasked(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
脱敏后的值

### Example

```csharp
// 日志输出时使用脱敏值
logger.LogInformation("连接字符串: {ConnectionString}", cfg.GetMasked("Database:ConnectionString"));
// 输出: 连接字符串: Ser***ion
```