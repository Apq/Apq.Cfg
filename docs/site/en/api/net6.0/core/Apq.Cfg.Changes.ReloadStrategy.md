#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.Changes](Apq.Cfg.Changes.md 'Apq\.Cfg\.Changes')

## ReloadStrategy Enum

配置重载策略

```csharp
public enum ReloadStrategy
```
### Fields

<a name='Apq.Cfg.Changes.ReloadStrategy.Eager'></a>

`Eager` 0

立即重载：文件变更后自动重载（默认行为）

<a name='Apq.Cfg.Changes.ReloadStrategy.Lazy'></a>

`Lazy` 1

延迟重载：访问配置时才检查并重载

<a name='Apq.Cfg.Changes.ReloadStrategy.Manual'></a>

`Manual` 2

手动重载：只有调用 Reload\(\) 方法时才重载