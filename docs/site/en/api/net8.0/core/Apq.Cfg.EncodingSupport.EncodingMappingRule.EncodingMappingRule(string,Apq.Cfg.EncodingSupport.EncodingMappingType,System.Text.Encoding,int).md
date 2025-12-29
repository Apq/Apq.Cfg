#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingMappingRule](Apq.Cfg.EncodingSupport.EncodingMappingRule.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule')

## EncodingMappingRule\(string, EncodingMappingType, Encoding, int\) Constructor

创建编码映射规则

```csharp
public EncodingMappingRule(string pattern, Apq.Cfg.EncodingSupport.EncodingMappingType type, System.Text.Encoding encoding, int priority=0);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingMappingRule.EncodingMappingRule(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).pattern'></a>

`pattern` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

匹配模式

<a name='Apq.Cfg.EncodingSupport.EncodingMappingRule.EncodingMappingRule(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).type'></a>

`type` [EncodingMappingType](Apq.Cfg.EncodingSupport.EncodingMappingType.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingType')

匹配类型

<a name='Apq.Cfg.EncodingSupport.EncodingMappingRule.EncodingMappingRule(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

目标编码

<a name='Apq.Cfg.EncodingSupport.EncodingMappingRule.EncodingMappingRule(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).priority'></a>

`priority` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

优先级（数值越大优先级越高）