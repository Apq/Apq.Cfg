#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Sources\.File](Apq.Cfg.Sources.File.md 'Apq\.Cfg\.Sources\.File')

## FileCfgSourceBase Class

文件配置源基类，提供编码检测和写入编码功能

```csharp
public abstract class FileCfgSourceBase : Apq.Cfg.Sources.ICfgSource, System.IDisposable
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; FileCfgSourceBase

Derived  
&#8627; [JsonFileCfgSource](Apq.Cfg.Sources.JsonFileCfgSource.md 'Apq\.Cfg\.Sources\.JsonFileCfgSource')

Implements [ICfgSource](Apq.Cfg.Sources.ICfgSource.md 'Apq\.Cfg\.Sources\.ICfgSource'), [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable')

| Properties | |
| :--- | :--- |
| [EncodingConfidenceThreshold](Apq.Cfg.Sources.File.FileCfgSourceBase.EncodingConfidenceThreshold.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.EncodingConfidenceThreshold') | 编码检测置信度阈值（0\.0\-1\.0），可在运行时修改。 可通过环境变量 APQ\_CFG\_ENCODING\_CONFIDENCE 设置默认值。 |
| [EncodingDetector](Apq.Cfg.Sources.File.FileCfgSourceBase.EncodingDetector.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.EncodingDetector') | 全局编码检测器实例 |
| [EncodingOptionsValue](Apq.Cfg.Sources.File.FileCfgSourceBase.EncodingOptionsValue.md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.EncodingOptionsValue') | 编码选项 |

| Methods | |
| :--- | :--- |
| [CreatePhysicalFileProvider\(string\)](Apq.Cfg.Sources.File.FileCfgSourceBase.CreatePhysicalFileProvider(string).md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.CreatePhysicalFileProvider\(string\)') | 创建 PhysicalFileProvider 并跟踪以便后续释放 |
| [DetectEncodingEnhanced\(string\)](Apq.Cfg.Sources.File.FileCfgSourceBase.DetectEncodingEnhanced(string).md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.DetectEncodingEnhanced\(string\)') | 检测文件编码（用于读取）\- 使用增强的编码检测器 |
| [GetWriteEncoding\(\)](Apq.Cfg.Sources.File.FileCfgSourceBase.GetWriteEncoding().md 'Apq\.Cfg\.Sources\.File\.FileCfgSourceBase\.GetWriteEncoding\(\)') | 获取写入编码 |
