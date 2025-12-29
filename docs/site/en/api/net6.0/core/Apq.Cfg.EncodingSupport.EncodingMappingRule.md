#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingMappingRule Class

编码映射规则

```csharp
public sealed class EncodingMappingRule
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EncodingMappingRule

| Constructors | |
| :--- | :--- |
| [EncodingMappingRule\(string, EncodingMappingType, Encoding, int\)](Apq.Cfg.EncodingSupport.EncodingMappingRule.EncodingMappingRule(string,Apq.Cfg.EncodingSupport.EncodingMappingType,System.Text.Encoding,int).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.EncodingMappingRule\(string, Apq\.Cfg\.EncodingSupport\.EncodingMappingType, System\.Text\.Encoding, int\)') | 创建编码映射规则 |

| Properties | |
| :--- | :--- |
| [Encoding](Apq.Cfg.EncodingSupport.EncodingMappingRule.Encoding.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.Encoding') | 目标编码 |
| [Pattern](Apq.Cfg.EncodingSupport.EncodingMappingRule.Pattern.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.Pattern') | 匹配模式 |
| [Priority](Apq.Cfg.EncodingSupport.EncodingMappingRule.Priority.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.Priority') | 优先级（数值越大优先级越高，默认 0） |
| [Type](Apq.Cfg.EncodingSupport.EncodingMappingRule.Type.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.Type') | 匹配类型 |

| Methods | |
| :--- | :--- |
| [IsMatch\(string\)](Apq.Cfg.EncodingSupport.EncodingMappingRule.IsMatch(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingRule\.IsMatch\(string\)') | 检查文件路径是否匹配此规则 |
