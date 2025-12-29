#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto')

## EncryptionTransformer Class

加密值转换器

```csharp
public class EncryptionTransformer : Apq.Cfg.Security.IValueTransformer
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EncryptionTransformer

Implements [Apq\.Cfg\.Security\.IValueTransformer](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.security.ivaluetransformer 'Apq\.Cfg\.Security\.IValueTransformer')

### Remarks
性能优化：
1\. 使用 Lazy 延迟编译正则表达式
2\. 使用 RegexOptions\.Compiled 提升匹配性能
3\. 使用 StringComparison\.Ordinal 进行前缀检查
4\. 简单模式使用 string\.Contains 快速路径
5\. 缓存敏感键匹配结果

| Constructors | |
| :--- | :--- |
| [EncryptionTransformer\(ICryptoProvider, EncryptionOptions\)](Apq.Cfg.Crypto.EncryptionTransformer.EncryptionTransformer(Apq.Cfg.Crypto.ICryptoProvider,Apq.Cfg.Crypto.EncryptionOptions).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.EncryptionTransformer\(Apq\.Cfg\.Crypto\.ICryptoProvider, Apq\.Cfg\.Crypto\.EncryptionOptions\)') | 初始化加密值转换器 |

| Properties | |
| :--- | :--- |
| [Name](Apq.Cfg.Crypto.EncryptionTransformer.Name.md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.Name') | 转换器名称 |
| [Priority](Apq.Cfg.Crypto.EncryptionTransformer.Priority.md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.Priority') | 优先级，数值越大优先级越高 |

| Methods | |
| :--- | :--- |
| [ClearCache\(\)](Apq.Cfg.Crypto.EncryptionTransformer.ClearCache().md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.ClearCache\(\)') | 清除敏感键匹配缓存 |
| [IsSimpleContainsPattern\(string\)](Apq.Cfg.Crypto.EncryptionTransformer.IsSimpleContainsPattern(string).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.IsSimpleContainsPattern\(string\)') | 判断是否为简单的 \*Keyword\* 模式 |
| [MatchSensitiveKey\(string\)](Apq.Cfg.Crypto.EncryptionTransformer.MatchSensitiveKey(string).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.MatchSensitiveKey\(string\)') | 使用缓存匹配敏感键 |
| [ShouldTransform\(string, string\)](Apq.Cfg.Crypto.EncryptionTransformer.ShouldTransform(string,string).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.ShouldTransform\(string, string\)') | 判断是否应该处理该键 |
| [TransformOnRead\(string, string\)](Apq.Cfg.Crypto.EncryptionTransformer.TransformOnRead(string,string).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.TransformOnRead\(string, string\)') | 读取时转换（解密） |
| [TransformOnWrite\(string, string\)](Apq.Cfg.Crypto.EncryptionTransformer.TransformOnWrite(string,string).md 'Apq\.Cfg\.Crypto\.EncryptionTransformer\.TransformOnWrite\(string, string\)') | 写入时转换（加密） |
