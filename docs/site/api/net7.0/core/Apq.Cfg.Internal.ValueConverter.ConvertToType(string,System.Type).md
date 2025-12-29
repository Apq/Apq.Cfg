#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[ValueConverter](Apq.Cfg.Internal.ValueConverter.md 'Apq\.Cfg\.Internal\.ValueConverter')

## ValueConverter\.ConvertToType\(string, Type\) Method

将字符串值转换为指定类型（非泛型版本，用于反射场景）

```csharp
public static object? ConvertToType(string value, System.Type targetType);
```
#### Parameters

<a name='Apq.Cfg.Internal.ValueConverter.ConvertToType(string,System.Type).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

字符串值

<a name='Apq.Cfg.Internal.ValueConverter.ConvertToType(string,System.Type).targetType'></a>

`targetType` [System\.Type](https://learn.microsoft.com/en-us/dotnet/api/system.type 'System\.Type')

目标类型

#### Returns
[System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')  
转换后的值，转换失败返回 null

### Remarks
此方法使用 Enum\.Parse 和 Convert\.ChangeType，在 AOT 场景下可能不兼容。
对于 AOT 场景，请使用 Apq\.Cfg\.SourceGenerator 包中的 \[CfgSection\] 特性。