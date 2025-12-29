#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Security](Apq.Cfg.Security.md 'Apq\.Cfg\.Security')

## IValueTransformer Interface

配置值转换器接口，用于加密/解密、脱敏等场景

```csharp
public interface IValueTransformer
```

| Properties | |
| :--- | :--- |
| [Name](Apq.Cfg.Security.IValueTransformer.Name.md 'Apq\.Cfg\.Security\.IValueTransformer\.Name') | 转换器名称，用于标识 |
| [Priority](Apq.Cfg.Security.IValueTransformer.Priority.md 'Apq\.Cfg\.Security\.IValueTransformer\.Priority') | 优先级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ShouldTransform\(string, string\)](Apq.Cfg.Security.IValueTransformer.ShouldTransform(string,string).md 'Apq\.Cfg\.Security\.IValueTransformer\.ShouldTransform\(string, string\)') | 判断是否应该处理该键 |
| [TransformOnRead\(string, string\)](Apq.Cfg.Security.IValueTransformer.TransformOnRead(string,string).md 'Apq\.Cfg\.Security\.IValueTransformer\.TransformOnRead\(string, string\)') | 读取时转换（如解密） |
| [TransformOnWrite\(string, string\)](Apq.Cfg.Security.IValueTransformer.TransformOnWrite(string,string).md 'Apq\.Cfg\.Security\.IValueTransformer\.TransformOnWrite\(string, string\)') | 写入时转换（如加密） |
