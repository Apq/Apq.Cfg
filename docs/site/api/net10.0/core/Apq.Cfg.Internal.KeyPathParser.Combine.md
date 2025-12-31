#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Internal](Apq.Cfg.Internal.md 'Apq\.Cfg\.Internal').[KeyPathParser](Apq.Cfg.Internal.KeyPathParser.md 'Apq\.Cfg\.Internal\.KeyPathParser')

## KeyPathParser\.Combine Method

| Overloads | |
| :--- | :--- |
| [Combine\(string, string\)](Apq.Cfg.Internal.KeyPathParser.Combine.md#Apq.Cfg.Internal.KeyPathParser.Combine(string,string) 'Apq\.Cfg\.Internal\.KeyPathParser\.Combine\(string, string\)') | 组合两个键路径（字符串版本，用于常见场景） |
| [Combine\(ReadOnlySpan&lt;char&gt;, ReadOnlySpan&lt;char&gt;\)](Apq.Cfg.Internal.KeyPathParser.Combine.md#Apq.Cfg.Internal.KeyPathParser.Combine(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_) 'Apq\.Cfg\.Internal\.KeyPathParser\.Combine\(System\.ReadOnlySpan\<char\>, System\.ReadOnlySpan\<char\>\)') | 组合两个键路径（避免不必要的分配） |

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(string,string)'></a>

## KeyPathParser\.Combine\(string, string\) Method

组合两个键路径（字符串版本，用于常见场景）

```csharp
public static string Combine(string? parent, string child);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(string,string).parent'></a>

`parent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

父路径

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(string,string).child'></a>

`child` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

子键

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
组合后的完整键

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_)'></a>

## KeyPathParser\.Combine\(ReadOnlySpan\<char\>, ReadOnlySpan\<char\>\) Method

组合两个键路径（避免不必要的分配）

```csharp
public static string Combine(System.ReadOnlySpan<char> parent, System.ReadOnlySpan<char> child);
```
#### Parameters

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_).parent'></a>

`parent` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

父路径

<a name='Apq.Cfg.Internal.KeyPathParser.Combine(System.ReadOnlySpan_char_,System.ReadOnlySpan_char_).child'></a>

`child` [System\.ReadOnlySpan&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')[System\.Char](https://learn.microsoft.com/en-us/dotnet/api/system.char 'System\.Char')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.readonlyspan-1 'System\.ReadOnlySpan\`1')

子键

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
组合后的完整键