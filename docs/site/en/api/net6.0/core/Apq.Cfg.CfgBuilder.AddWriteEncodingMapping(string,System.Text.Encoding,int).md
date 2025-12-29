#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddWriteEncodingMapping\(string, Encoding, int\) Method

添加写入编码映射（完整路径）

```csharp
public Apq.Cfg.CfgBuilder AddWriteEncodingMapping(string filePath, System.Text.Encoding encoding, int priority=100);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddWriteEncodingMapping(string,System.Text.Encoding,int).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

文件完整路径

<a name='Apq.Cfg.CfgBuilder.AddWriteEncodingMapping(string,System.Text.Encoding,int).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

编码

<a name='Apq.Cfg.CfgBuilder.AddWriteEncodingMapping(string,System.Text.Encoding,int).priority'></a>

`priority` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

优先级（数值越大优先级越高，默认 100）

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')