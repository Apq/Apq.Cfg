#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.EncodingSupport](Apq.Cfg.EncodingSupport.md 'Apq\.Cfg\.EncodingSupport')

## EncodingReadStrategy Enum

编码读取策略

```csharp
public enum EncodingReadStrategy
```
### Fields

<a name='Apq.Cfg.EncodingSupport.EncodingReadStrategy.AutoDetect'></a>

`AutoDetect` 0

自动检测编码（BOM 优先，然后使用 UTF\.Unknown）

<a name='Apq.Cfg.EncodingSupport.EncodingReadStrategy.Specified'></a>

`Specified` 1

使用指定的编码

<a name='Apq.Cfg.EncodingSupport.EncodingReadStrategy.Preserve'></a>

`Preserve` 2

保持原编码（读取时检测，写入时保持）