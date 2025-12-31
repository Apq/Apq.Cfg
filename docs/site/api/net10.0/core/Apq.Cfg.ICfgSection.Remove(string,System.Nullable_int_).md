#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.Remove\(string, Nullable\<int\>\) Method

移除配置键

```csharp
void Remove(string key, System.Nullable<int> targetLevel=null);
```
#### Parameters

<a name='Apq.Cfg.ICfgSection.Remove(string,System.Nullable_int_).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

相对于此节的键名

<a name='Apq.Cfg.ICfgSection.Remove(string,System.Nullable_int_).targetLevel'></a>

`targetLevel` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

目标层级，为null时从所有层级移除

### Example

```csharp
var dbSection = cfg.GetSection("Database");
dbSection.Remove("OldSetting"); // 等同于 cfg.Remove("Database:OldSetting")
```