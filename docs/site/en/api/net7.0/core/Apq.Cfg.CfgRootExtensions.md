#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## CfgRootExtensions Class

ICfgRoot 扩展方法

```csharp
public static class CfgRootExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CfgRootExtensions

| Methods | |
| :--- | :--- |
| [GetMasked\(this ICfgRoot, string\)](Apq.Cfg.CfgRootExtensions.GetMasked(thisApq.Cfg.ICfgRoot,string).md 'Apq\.Cfg\.CfgRootExtensions\.GetMasked\(this Apq\.Cfg\.ICfgRoot, string\)') | 获取脱敏后的配置值（用于日志输出） |
| [GetMaskedSnapshot\(this ICfgRoot\)](Apq.Cfg.CfgRootExtensions.GetMaskedSnapshot(thisApq.Cfg.ICfgRoot).md 'Apq\.Cfg\.CfgRootExtensions\.GetMaskedSnapshot\(this Apq\.Cfg\.ICfgRoot\)') | 获取所有配置的脱敏快照（用于调试） |
| [GetOrDefault&lt;T&gt;\(this ICfgRoot, string, T\)](Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).md 'Apq\.Cfg\.CfgRootExtensions\.GetOrDefault\<T\>\(this Apq\.Cfg\.ICfgRoot, string, T\)') | 获取配置值，如果不存在则返回默认值 |
| [GetRequired&lt;T&gt;\(this ICfgRoot, string\)](Apq.Cfg.CfgRootExtensions.GetRequired_T_(thisApq.Cfg.ICfgRoot,string).md 'Apq\.Cfg\.CfgRootExtensions\.GetRequired\<T\>\(this Apq\.Cfg\.ICfgRoot, string\)') | 获取必需的配置值，如果不存在则抛出异常 |
| [TryGet&lt;T&gt;\(this ICfgRoot, string, T\)](Apq.Cfg.CfgRootExtensions.TryGet_T_(thisApq.Cfg.ICfgRoot,string,T).md 'Apq\.Cfg\.CfgRootExtensions\.TryGet\<T\>\(this Apq\.Cfg\.ICfgRoot, string, T\)') | 尝试获取配置值 |
