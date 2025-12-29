#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## ICfgSection Interface

配置节接口，表示配置的一个子节

```csharp
public interface ICfgSection
```

Derived  
&#8627; [CfgSection](Apq.Cfg.CfgSection.md 'Apq\.Cfg\.CfgSection')

| Properties | |
| :--- | :--- |
| [Path](Apq.Cfg.ICfgSection.Path.md 'Apq\.Cfg\.ICfgSection\.Path') | 获取此节的路径前缀 |

| Methods | |
| :--- | :--- |
| [Exists\(string\)](Apq.Cfg.ICfgSection.Exists(string).md 'Apq\.Cfg\.ICfgSection\.Exists\(string\)') | 检查配置键是否存在 |
| [Get\(string\)](Apq.Cfg.ICfgSection.Get.md#Apq.Cfg.ICfgSection.Get(string) 'Apq\.Cfg\.ICfgSection\.Get\(string\)') | 获取配置值（相对于此节的键） |
| [Get&lt;T&gt;\(string\)](Apq.Cfg.ICfgSection.Get.md#Apq.Cfg.ICfgSection.Get_T_(string) 'Apq\.Cfg\.ICfgSection\.Get\<T\>\(string\)') | 获取配置值并转换为指定类型 |
| [GetChildKeys\(\)](Apq.Cfg.ICfgSection.GetChildKeys().md 'Apq\.Cfg\.ICfgSection\.GetChildKeys\(\)') | 获取所有子节的键名 |
| [GetSection\(string\)](Apq.Cfg.ICfgSection.GetSection(string).md 'Apq\.Cfg\.ICfgSection\.GetSection\(string\)') | 获取子节 |
| [Remove\(string, Nullable&lt;int&gt;\)](Apq.Cfg.ICfgSection.Remove(string,System.Nullable_int_).md 'Apq\.Cfg\.ICfgSection\.Remove\(string, System\.Nullable\<int\>\)') | 移除配置键 |
| [Set\(string, string, Nullable&lt;int&gt;\)](Apq.Cfg.ICfgSection.Set(string,string,System.Nullable_int_).md 'Apq\.Cfg\.ICfgSection\.Set\(string, string, System\.Nullable\<int\>\)') | 设置配置值 |
