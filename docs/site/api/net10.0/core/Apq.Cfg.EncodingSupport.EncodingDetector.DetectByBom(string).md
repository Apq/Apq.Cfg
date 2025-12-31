#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingDetector](Apq.Cfg.EncodingSupport.EncodingDetector.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector')

## EncodingDetector\.DetectByBom\(string\) Method

通过 BOM 检测编码

```csharp
private static System.Nullable<(System.Text.Encoding Encoding,int BomLength)> DetectByBom(string filePath);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingDetector.DetectByBom(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')[,](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.valuetuple 'System\.ValueTuple')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')