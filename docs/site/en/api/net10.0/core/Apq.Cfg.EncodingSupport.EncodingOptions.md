#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingOptions Class

编码选项配置

```csharp
public sealed class EncodingOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EncodingOptions

| Fields | |
| :--- | :--- |
| [Default](Apq.Cfg.EncodingSupport.EncodingOptions.Default.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.Default') | 默认编码选项（自动检测读取，UTF\-8 无 BOM 写入） |
| [PowerShell](Apq.Cfg.EncodingSupport.EncodingOptions.PowerShell.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.PowerShell') | PowerShell 脚本编码选项（UTF\-8 带 BOM） |

| Properties | |
| :--- | :--- |
| [ConfidenceThreshold](Apq.Cfg.EncodingSupport.EncodingOptions.ConfidenceThreshold.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.ConfidenceThreshold') | 编码检测置信度阈值（0\.0\-1\.0），默认 0\.6 |
| [EnableCache](Apq.Cfg.EncodingSupport.EncodingOptions.EnableCache.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.EnableCache') | 是否启用编码检测缓存，默认 true |
| [EnableLogging](Apq.Cfg.EncodingSupport.EncodingOptions.EnableLogging.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.EnableLogging') | 是否启用编码检测日志，默认 false |
| [FallbackEncoding](Apq.Cfg.EncodingSupport.EncodingOptions.FallbackEncoding.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.FallbackEncoding') | 回退编码（自动检测失败时使用），默认 UTF\-8 |
| [ReadEncoding](Apq.Cfg.EncodingSupport.EncodingOptions.ReadEncoding.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.ReadEncoding') | 指定的读取编码（当 ReadStrategy 为 Specified 时使用） |
| [ReadStrategy](Apq.Cfg.EncodingSupport.EncodingOptions.ReadStrategy.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.ReadStrategy') | 读取策略，默认自动检测 |
| [WriteEncoding](Apq.Cfg.EncodingSupport.EncodingOptions.WriteEncoding.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.WriteEncoding') | 指定的写入编码（当 WriteStrategy 为 Specified 时使用） |
| [WriteStrategy](Apq.Cfg.EncodingSupport.EncodingOptions.WriteStrategy.md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.WriteStrategy') | 写入策略，默认 UTF\-8 无 BOM |

| Methods | |
| :--- | :--- |
| [GetReadEncoding\(Encoding\)](Apq.Cfg.EncodingSupport.EncodingOptions.GetReadEncoding(System.Text.Encoding).md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.GetReadEncoding\(System\.Text\.Encoding\)') | 获取读取时使用的编码 |
| [GetWriteEncoding\(Encoding\)](Apq.Cfg.EncodingSupport.EncodingOptions.GetWriteEncoding(System.Text.Encoding).md 'Apq\.Cfg\.EncodingSupport\.EncodingOptions\.GetWriteEncoding\(System\.Text\.Encoding\)') | 获取写入时使用的编码 |
