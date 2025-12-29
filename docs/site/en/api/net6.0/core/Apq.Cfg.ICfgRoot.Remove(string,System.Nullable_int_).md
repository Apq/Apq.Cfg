#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.Remove\(string, Nullable\<int\>\) Method

移除配置键

```csharp
void Remove(string key, System.Nullable<int> targetLevel=null);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.Remove(string,System.Nullable_int_).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

<a name='Apq.Cfg.ICfgRoot.Remove(string,System.Nullable_int_).targetLevel'></a>

`targetLevel` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

目标层级，为null时从所有层级移除