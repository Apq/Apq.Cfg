#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection')

## ObjectBinder Class

对象绑定器，支持嵌套对象和集合绑定

```csharp
internal static class ObjectBinder
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ObjectBinder

### Remarks
此类使用反射进行配置绑定，不支持 Native AOT。
对于 AOT 场景，请使用 Apq\.Cfg\.SourceGenerator 包中的 \[CfgSection\] 特性。

| Methods | |
| :--- | :--- |
| [BindObject\(ICfgSection, object, Type\)](Apq.Cfg.DependencyInjection.ObjectBinder.BindObject(Apq.Cfg.ICfgSection,object,System.Type).md 'Apq\.Cfg\.DependencyInjection\.ObjectBinder\.BindObject\(Apq\.Cfg\.ICfgSection, object, System\.Type\)') | 将配置节绑定到对象（非泛型版本） |
| [BindSection&lt;T&gt;\(ICfgSection, T\)](Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).md 'Apq\.Cfg\.DependencyInjection\.ObjectBinder\.BindSection\<T\>\(Apq\.Cfg\.ICfgSection, T\)') | 将配置节绑定到对象 |
