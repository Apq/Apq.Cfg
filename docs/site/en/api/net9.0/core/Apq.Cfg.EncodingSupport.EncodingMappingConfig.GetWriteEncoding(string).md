#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingMappingConfig](Apq.Cfg.EncodingSupport.EncodingMappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingMappingConfig')

## EncodingMappingConfig\.GetWriteEncoding\(string\) Method

获取文件的写入编码

```csharp
public System.Text.Encoding? GetWriteEncoding(string filePath);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingMappingConfig.GetWriteEncoding(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

文件路径

#### Returns
[System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')  
匹配的编码，如果没有匹配则返回 null