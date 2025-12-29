#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg\.DependencyInjection](Apq.Cfg.DependencyInjection.md 'Apq\.Cfg\.DependencyInjection').[ObjectBinder](Apq.Cfg.DependencyInjection.ObjectBinder.md 'Apq\.Cfg\.DependencyInjection\.ObjectBinder')

## ObjectBinder\.BindSection\<T\>\(ICfgSection, T\) Method

将配置节绑定到对象

```csharp
public static void BindSection<T>(Apq.Cfg.ICfgSection section, T target)
    where T : class;
```
#### Type parameters

<a name='Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).section'></a>

`section` [ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

配置节

<a name='Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).target'></a>

`target` [T](Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).md#Apq.Cfg.DependencyInjection.ObjectBinder.BindSection_T_(Apq.Cfg.ICfgSection,T).T 'Apq\.Cfg\.DependencyInjection\.ObjectBinder\.BindSection\<T\>\(Apq\.Cfg\.ICfgSection, T\)\.T')

目标对象