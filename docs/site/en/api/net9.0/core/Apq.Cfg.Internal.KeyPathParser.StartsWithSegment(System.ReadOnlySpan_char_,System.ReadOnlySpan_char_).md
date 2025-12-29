#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.StartsWithSegment\(ReadOnlySpan\<char\>, ReadOnlySpan\<char\>\) Method

检查键是否以指定前缀开头（支持精确段匹配）

```csharp
public static bool StartsWithSegment(System.ReadOnlySpan<char> key, System.ReadOnlySpan<char> prefix);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.StartsWithSegment(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_).key'></a>

`key` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

完整键

<a name='Apq.Cfg.Internal.KeyPathParser.StartsWithSegment(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_).prefix'></a>

`prefix` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

前缀

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
是否匹配