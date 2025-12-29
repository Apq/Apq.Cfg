#### [Apq\.Cfg\.Crypto](index.md 'index')
### [Apq\.Cfg\.Crypto](Apq.Cfg.Crypto.md 'Apq\.Cfg\.Crypto')

## SensitiveMasker Class

敏感值脱敏器

```csharp
public class SensitiveMasker : Apq.Cfg.Security.IValueMasker
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; SensitiveMasker

Implements [Apq\.Cfg\.Security\.IValueMasker](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.security.ivaluemasker 'Apq\.Cfg\.Security\.IValueMasker')

### Remarks
性能优化：
1\. 使用 Lazy 延迟编译正则表达式
2\. 使用 RegexOptions\.Compiled 提升匹配性能
3\. 简单模式使用 string\.Contains 快速路径
4\. 缓存 ShouldMask 结果
5\. 使用 string\.Create 减少字符串分配

| Constructors | |
| :--- | :--- |
| [SensitiveMasker\(MaskingOptions\)](Apq.Cfg.Crypto.SensitiveMasker.SensitiveMasker(Apq.Cfg.Crypto.MaskingOptions).md 'Apq\.Cfg\.Crypto\.SensitiveMasker\.SensitiveMasker\(Apq\.Cfg\.Crypto\.MaskingOptions\)') | 初始化敏感值脱敏器 |

| Methods | |
| :--- | :--- |
| [ClearCache\(\)](Apq.Cfg.Crypto.SensitiveMasker.ClearCache().md 'Apq\.Cfg\.Crypto\.SensitiveMasker\.ClearCache\(\)') | 清除 ShouldMask 缓存 |
| [IsSimpleContainsPattern\(string\)](Apq.Cfg.Crypto.SensitiveMasker.IsSimpleContainsPattern(string).md 'Apq\.Cfg\.Crypto\.SensitiveMasker\.IsSimpleContainsPattern\(string\)') | 判断是否为简单的 \*Keyword\* 模式 |
| [Mask\(string, string\)](Apq.Cfg.Crypto.SensitiveMasker.Mask(string,string).md 'Apq\.Cfg\.Crypto\.SensitiveMasker\.Mask\(string, string\)') | 脱敏处理 |
| [ShouldMask\(string\)](Apq.Cfg.Crypto.SensitiveMasker.ShouldMask(string).md 'Apq\.Cfg\.Crypto\.SensitiveMasker\.ShouldMask\(string\)') | 判断是否应该脱敏该键 |
