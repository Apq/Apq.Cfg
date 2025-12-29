#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## ICfgRoot Interface

配置根接口，提供统一的配置访问和管理功能

```csharp
public interface ICfgRoot : System.IDisposable, System.IAsyncDisposable
```

Derived  
&#8627; [MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

Implements [System\.IDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable 'System\.IDisposable'), [System\.IAsyncDisposable](https://learn.microsoft.com/en-us/dotnet/api/system.iasyncdisposable 'System\.IAsyncDisposable')

| Properties | |
| :--- | :--- |
| [ConfigChanges](Apq.Cfg.ICfgRoot.ConfigChanges.md 'Apq\.Cfg\.ICfgRoot\.ConfigChanges') | 获取配置变更的可观察序列 |

| Methods | |
| :--- | :--- |
| [Exists\(string\)](Apq.Cfg.ICfgRoot.Exists(string).md 'Apq\.Cfg\.ICfgRoot\.Exists\(string\)') | 检查配置键是否存在 |
| [Get\(string\)](Apq.Cfg.ICfgRoot.Get.md#Apq.Cfg.ICfgRoot.Get(string) 'Apq\.Cfg\.ICfgRoot\.Get\(string\)') | 获取配置值 |
| [Get&lt;T&gt;\(string\)](Apq.Cfg.ICfgRoot.Get.md#Apq.Cfg.ICfgRoot.Get_T_(string) 'Apq\.Cfg\.ICfgRoot\.Get\<T\>\(string\)') | 获取配置值并转换为指定类型 |
| [GetChildKeys\(\)](Apq.Cfg.ICfgRoot.GetChildKeys().md 'Apq\.Cfg\.ICfgRoot\.GetChildKeys\(\)') | 获取所有顶级配置键 |
| [GetMany\(IEnumerable&lt;string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 批量获取多个配置值，减少锁竞争 |
| [GetMany\(IEnumerable&lt;string&gt;, Action&lt;string,string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany(System.Collections.Generic.IEnumerable_string_,System.Action_string,string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Action\<string,string\>\)') | 高性能批量获取：通过回调方式返回结果，零堆分配 |
| [GetMany&lt;T&gt;\(IEnumerable&lt;string&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_) 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>\)') | 批量获取多个配置值并转换为指定类型 |
| [GetMany&lt;T&gt;\(IEnumerable&lt;string&gt;, Action&lt;string,T&gt;\)](Apq.Cfg.ICfgRoot.GetMany.md#Apq.Cfg.ICfgRoot.GetMany_T_(System.Collections.Generic.IEnumerable_string_,System.Action_string,T_) 'Apq\.Cfg\.ICfgRoot\.GetMany\<T\>\(System\.Collections\.Generic\.IEnumerable\<string\>, System\.Action\<string,T\>\)') | 高性能批量获取：通过回调方式返回结果并转换类型，零堆分配 |
| [GetSection\(string\)](Apq.Cfg.ICfgRoot.GetSection(string).md 'Apq\.Cfg\.ICfgRoot\.GetSection\(string\)') | 获取配置节 |
| [Remove\(string, Nullable&lt;int&gt;\)](Apq.Cfg.ICfgRoot.Remove(string,System.Nullable_int_).md 'Apq\.Cfg\.ICfgRoot\.Remove\(string, System\.Nullable\<int\>\)') | 移除配置键 |
| [SaveAsync\(Nullable&lt;int&gt;, CancellationToken\)](Apq.Cfg.ICfgRoot.SaveAsync(System.Nullable_int_,System.Threading.CancellationToken).md 'Apq\.Cfg\.ICfgRoot\.SaveAsync\(System\.Nullable\<int\>, System\.Threading\.CancellationToken\)') | 保存配置更改到持久化存储 |
| [Set\(string, string, Nullable&lt;int&gt;\)](Apq.Cfg.ICfgRoot.Set(string,string,System.Nullable_int_).md 'Apq\.Cfg\.ICfgRoot\.Set\(string, string, System\.Nullable\<int\>\)') | 设置配置值 |
| [SetMany\(IEnumerable&lt;KeyValuePair&lt;string,string&gt;&gt;, Nullable&lt;int&gt;\)](Apq.Cfg.ICfgRoot.SetMany(System.Collections.Generic.IEnumerable_System.Collections.Generic.KeyValuePair_string,string__,System.Nullable_int_).md 'Apq\.Cfg\.ICfgRoot\.SetMany\(System\.Collections\.Generic\.IEnumerable\<System\.Collections\.Generic\.KeyValuePair\<string,string\>\>, System\.Nullable\<int\>\)') | 批量设置多个配置值，减少锁竞争 |
| [ToMicrosoftConfiguration\(\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration() 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(\)') | 转换为 Microsoft Configuration（静态快照） |
| [ToMicrosoftConfiguration\(DynamicReloadOptions\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions) 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(Apq\.Cfg\.Changes\.DynamicReloadOptions\)') | 转换为支持动态重载的 Microsoft Configuration |
