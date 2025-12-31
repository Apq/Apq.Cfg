#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingDetector](Apq.Cfg.EncodingSupport.EncodingDetector.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector')

## EncodingDetector\.ValidateEncodingForFile\(string, Encoding\) Method

验证指定编码是否能正确读取文件

```csharp
private static bool ValidateEncodingForFile(string filePath, System.Text.Encoding encoding);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingDetector.ValidateEncodingForFile(string,System.Text.Encoding).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

文件路径

<a name='Apq.Cfg.EncodingSupport.EncodingDetector.ValidateEncodingForFile(string,System.Text.Encoding).encoding'></a>

`encoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

要验证的编码

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
如果能正确读取返回 true，否则返回 false