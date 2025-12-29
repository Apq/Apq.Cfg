#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport').[EncodingOptions](Apq.Cfg.EncodingSupport.EncodingOptions.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions')

## EncodingOptions\.GetWriteEncoding\(Encoding\) Method

获取写入时使用的编码

```csharp
public System.Text.Encoding GetWriteEncoding(System.Text.Encoding? detectedEncoding=null);
```
#### Parameters

<a name='Apq.Cfg.EncodingSupport.EncodingOptions.GetWriteEncoding(System.Text.Encoding).detectedEncoding'></a>

`detectedEncoding` [System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')

检测到的原文件编码（用于 Preserve 策略）

#### Returns
[System\.Text\.Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding 'System\.Text\.Encoding')  
写入编码