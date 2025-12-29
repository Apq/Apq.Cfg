#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal')

## KeyPathParser Class

键路径解析工具，使用 Span 优化避免字符串分配

```csharp
internal static class KeyPathParser
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; KeyPathParser

| Fields | |
| :--- | :--- |
| [Separator](Apq.Cfg.Internal.KeyPathParser.Separator.md 'Apq\.Cfg\.Internal\.KeyPathParser\.Separator') | 配置键分隔符 |

| Methods | |
| :--- | :--- |
| [Combine\(string, string\)](Apq.Cfg.Internal.KeyPathParser.Combine.md#Apq.Cfg.Internal.KeyPathParser.Combine(string,string) 'Apq\.Cfg\.Internal\.KeyPathParser\.Combine\(string, string\)') | 组合两个键路径（字符串版本，用于常见场景） |
| [Combine\(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.Combine.md#Apq.Cfg.Internal.KeyPathParser.Combine(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_) 'Apq\.Cfg\.Internal\.KeyPathParser\.Combine\(System\.ReadOnlySpan\<char\>, System\.ReadOnlySpan\<char\>\)') | 组合两个键路径（避免不必要的分配） |
| [EnumerateSegments\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.EnumerateSegments(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.EnumerateSegments\(System\.ReadOnlySpan\<char\>\)') | 枚举键的所有段（零分配迭代器） |
| [GetDepth\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.GetDepth(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.GetDepth\(System\.ReadOnlySpan\<char\>\)') | 计算键的深度（分隔符数量 \+ 1） |
| [GetFirstSegment\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.GetFirstSegment(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.GetFirstSegment\(System\.ReadOnlySpan\<char\>\)') | 获取键的第一个段（不分配新字符串） |
| [GetLastSegment\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.GetLastSegment(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.GetLastSegment\(System\.ReadOnlySpan\<char\>\)') | 获取键的最后一个段 |
| [GetParentPath\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.GetParentPath(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.GetParentPath\(System\.ReadOnlySpan\<char\>\)') | 获取键的父路径（最后一个分隔符之前） |
| [GetRemainder\(ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.GetRemainder(System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.GetRemainder\(System\.ReadOnlySpan\<char\>\)') | 获取键的剩余部分（第一个分隔符之后） |
| [StartsWithSegment\(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.StartsWithSegment(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_).md 'Apq\.Cfg\.Internal\.KeyPathParser\.StartsWithSegment\(System\.ReadOnlySpan\<char\>, System\.ReadOnlySpan\<char\>\)') | 检查键是否以指定前缀开头（支持精确段匹配） |
