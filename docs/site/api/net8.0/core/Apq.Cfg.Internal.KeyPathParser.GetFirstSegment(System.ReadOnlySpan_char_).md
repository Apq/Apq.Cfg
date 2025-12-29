#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.GetFirstSegment\(ReadOnlySpan\<char\>\) Method

获取键的第一个段（不分配新字符串）

```csharp
public static System.ReadOnlySpan<char> GetFirstSegment(System.ReadOnlySpan<char> key);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.GetFirstSegment(System.ReadOnlySpan_char_).key'></a>

`key` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

完整键

#### Returns
[System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')  
第一个段