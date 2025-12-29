#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.EnumerateSegments\(ReadOnlySpan\<char\>\) Method

枚举键的所有段（零分配迭代器）

```csharp
public static Apq.Cfg.Internal.KeyPathParser.SegmentEnumerator EnumerateSegments(System.ReadOnlySpan<char> key);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.EnumerateSegments(System.ReadOnlySpan_char_).key'></a>

`key` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

完整键

#### Returns
[SegmentEnumerator](Apq.Cfg.Internal.KeyPathParser.SegmentEnumerator.md 'Apq\.Cfg\.Internal\.KeyPathParser\.SegmentEnumerator')  
段枚举器