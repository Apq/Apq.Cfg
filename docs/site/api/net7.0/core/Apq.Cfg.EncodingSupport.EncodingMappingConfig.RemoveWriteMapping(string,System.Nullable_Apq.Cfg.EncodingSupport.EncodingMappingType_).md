#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')

## EncodingMappingConfig\.RemoveWriteMapping\(string, Nullable\<EncodingMappingType\>\) Method

移除写入编码映射规则

```csharp
public Apq.Cfg.EncodingSupport.EncodingMappingConfig RemoveWriteMapping(string pattern, System.Nullable<Apq.Cfg.EncodingSupport.EncodingMappingType> type=null);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.RemoveWriteMapping(string,System.Nullable_Apq.Cfg.EncodingSupport.EncodingMappingType_).pattern'></a>

`pattern` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

匹配模式

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.RemoveWriteMapping(string,System.Nullable_Apq.Cfg.EncodingSupport.EncodingMappingType_).type'></a>

`type` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[EncodingMappingType](Apq.Cfg.EncodingSupport.EncodingMappingType.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingType')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

匹配类型（可选，不指定则移除所有匹配的模式）

#### Returns
[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')