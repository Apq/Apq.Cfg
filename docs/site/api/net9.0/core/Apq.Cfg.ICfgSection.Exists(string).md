#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.Exists\(string\) Method

检查配置键是否存在

```csharp
bool Exists(string key);
```
#### Parameters

<a name='Apq.Cfg.ICfgSection.Exists(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

相对于此节的键名

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
存在返回true，否则返回false

### Example

```csharp
var dbSection = cfg.GetSection("Database");
if (dbSection.Exists("ConnectionString"))
{
    // 处理连接字符串
}
```