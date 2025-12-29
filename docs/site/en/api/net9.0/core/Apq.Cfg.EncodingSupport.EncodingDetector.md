#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingDetector Class

增强的编码检测器，支持 BOM 优先检测、缓存、日志和自定义映射

```csharp
public sealed class EncodingDetector
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; EncodingDetector

| Fields | |
| :--- | :--- |
| [DefaultWriteEncoding](Apq.Cfg.EncodingSupport.EncodingDetector.DefaultWriteEncoding.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.DefaultWriteEncoding') | 默认写入编码（UTF\-8 无 BOM） |

| Properties | |
| :--- | :--- |
| [Default](Apq.Cfg.EncodingSupport.EncodingDetector.Default.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.Default') | 全局默认实例 |
| [DefaultOptions](Apq.Cfg.EncodingSupport.EncodingDetector.DefaultOptions.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.DefaultOptions') | 默认编码选项 |
| [MappingConfig](Apq.Cfg.EncodingSupport.EncodingDetector.MappingConfig.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.MappingConfig') | 编码映射配置 |

| Methods | |
| :--- | :--- |
| [ClearCache\(\)](Apq.Cfg.EncodingSupport.EncodingDetector.ClearCache().md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.ClearCache\(\)') | 清除所有缓存 |
| [Detect\(string, EncodingOptions\)](Apq.Cfg.EncodingSupport.EncodingDetector.Detect(string,Apq.Cfg.EncodingSupport.EncodingOptions).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.Detect\(string, Apq\.Cfg\.EncodingSupport\.EncodingOptions\)') | 检测文件编码（用于读取） |
| [DetectByBom\(string\)](Apq.Cfg.EncodingSupport.EncodingDetector.DetectByBom(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.DetectByBom\(string\)') | 通过 BOM 检测编码 |
| [DetectInternal\(string, EncodingOptions\)](Apq.Cfg.EncodingSupport.EncodingDetector.DetectInternal(string,Apq.Cfg.EncodingSupport.EncodingOptions).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.DetectInternal\(string, Apq\.Cfg\.EncodingSupport\.EncodingOptions\)') | 内部检测逻辑 |
| [GetStats\(\)](Apq.Cfg.EncodingSupport.EncodingDetector.GetStats().md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.GetStats\(\)') | 获取统计信息 |
| [GetWriteEncoding\(string\)](Apq.Cfg.EncodingSupport.EncodingDetector.GetWriteEncoding(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.GetWriteEncoding\(string\)') | 获取文件的写入编码 |
| [InvalidateCache\(string\)](Apq.Cfg.EncodingSupport.EncodingDetector.InvalidateCache(string).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.InvalidateCache\(string\)') | 清除指定文件的缓存 |
| [LogDetection\(EncodingDetectionResult, EncodingOptions\)](Apq.Cfg.EncodingSupport.EncodingDetector.LogDetection(Apq.Cfg.EncodingSupport.EncodingDetectionResult,Apq.Cfg.EncodingSupport.EncodingOptions).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.LogDetection\(Apq\.Cfg\.EncodingSupport\.EncodingDetectionResult, Apq\.Cfg\.EncodingSupport\.EncodingOptions\)') | 记录检测日志 |
| [TryGetFromCache\(string, EncodingDetectionResult\)](Apq.Cfg.EncodingSupport.EncodingDetector.TryGetFromCache(string,Apq.Cfg.EncodingSupport.EncodingDetectionResult).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.TryGetFromCache\(string, Apq\.Cfg\.EncodingSupport\.EncodingDetectionResult\)') | 尝试从缓存获取 |
| [ValidateEncodingForFile\(string, Encoding\)](Apq.Cfg.EncodingSupport.EncodingDetector.ValidateEncodingForFile(string,System.Text.Encoding).md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.ValidateEncodingForFile\(string, System\.Text\.Encoding\)') | 验证指定编码是否能正确读取文件 |

| Events | |
| :--- | :--- |
| [OnEncodingDetected](Apq.Cfg.EncodingSupport.EncodingDetector.OnEncodingDetected.md 'Apq\.Cfg\.EncodingSupport\.EncodingDetector\.OnEncodingDetected') | 编码检测日志事件 |
