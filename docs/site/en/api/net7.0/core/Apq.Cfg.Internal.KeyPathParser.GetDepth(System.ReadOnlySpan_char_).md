#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.GetDepth\(ReadOnlySpan\<char\>\) Method

计算键的深度（分隔符数量 \+ 1）

```csharp
public static int GetDepth(System.ReadOnlySpan<char> key);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.GetDepth(System.ReadOnlySpan_char_).key'></a>

`key` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

完整键

#### Returns
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
深度