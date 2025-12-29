#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddReadEncodingMappingRegex\(string, Encoding, int\) Method

添加读取编码映射（正则表达式）

```csharp
public Apq.Cfg.CfgBuilder AddReadEncodingMappingRegex(string regexPattern, System.Text.Encoding encoding, int priority=0);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddReadEncodingMappingRegex(string,System.Text.Encoding,int).regexPattern'></a>

`regexPattern` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

正则表达式模式

<a name='Apq.Cfg.CfgBuilder.AddReadEncodingMappingRegex(string,System.Text.Encoding,int).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

编码

<a name='Apq.Cfg.CfgBuilder.AddReadEncodingMappingRegex(string,System.Text.Encoding,int).priority'></a>

`priority` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

优先级（数值越大优先级越高，默认 0）

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')