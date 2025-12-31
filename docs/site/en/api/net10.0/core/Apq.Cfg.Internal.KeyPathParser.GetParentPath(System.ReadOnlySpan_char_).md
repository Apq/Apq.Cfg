#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.GetParentPath\(ReadOnlySpan\<char\>\) Method

获取键的父路径（最后一个分隔符之前）

```csharp
public static System.ReadOnlySpan<char> GetParentPath(System.ReadOnlySpan<char> key);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.GetParentPath(System.ReadOnlySpan_char_).key'></a>

`key` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

完整键

#### Returns
[System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')  
父路径，如果没有分隔符则返回空