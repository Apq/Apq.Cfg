#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgRootExtensions](Apq.Cfg.CfgRootExtensions.md 'Apq\.Cfg\.CfgRootExtensions')

## CfgRootExtensions\.GetOrDefault\<T\>\(this ICfgRoot, string, T\) Method

获取配置值，如果不存在则返回默认值

```csharp
public static T? GetOrDefault<T>(this Apq.Cfg.ICfgRoot root, string key, T? defaultValue=default(T?));
```
#### Type parameters

<a name='Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).T'></a>

`T`
#### Parameters

<a name='Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).root'></a>

`root` [ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

<a name='Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).defaultValue'></a>

`defaultValue` [T](Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).md#Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).T 'Apq\.Cfg\.CfgRootExtensions\.GetOrDefault\<T\>\(this Apq\.Cfg\.ICfgRoot, string, T\)\.T')

#### Returns
[T](Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).md#Apq.Cfg.CfgRootExtensions.GetOrDefault_T_(thisApq.Cfg.ICfgRoot,string,T).T 'Apq\.Cfg\.CfgRootExtensions\.GetOrDefault\<T\>\(this Apq\.Cfg\.ICfgRoot, string, T\)\.T')