#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.GetSection\(string\) Method

获取子节

```csharp
Apq.Cfg.ICfgSection GetSection(string key);
```
#### Parameters

<a name='Apq.Cfg.ICfgSection.GetSection(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

子节键名

#### Returns
[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')  
子节对象

### Example

```csharp
var dbSection = cfg.GetSection("Database");
var connSection = dbSection.GetSection("Connection"); // 等同于 cfg.GetSection("Database:Connection")
```