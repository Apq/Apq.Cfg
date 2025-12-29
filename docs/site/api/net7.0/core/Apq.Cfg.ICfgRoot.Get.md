#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.Get Method

| Overloads | |
| :--- | :--- |
| [Get\(string\)](Apq.Cfg.ICfgRoot.Get.md#Apq.Cfg.ICfgRoot.Get(string) 'Apq\.Cfg\.ICfgRoot\.Get\(string\)') | 获取配置值 |
| [Get&lt;T&gt;\(string\)](Apq.Cfg.ICfgRoot.Get.md#Apq.Cfg.ICfgRoot.Get_T_(string) 'Apq\.Cfg\.ICfgRoot\.Get\<T\>\(string\)') | 获取配置值并转换为指定类型 |

<a name='Apq.Cfg.ICfgRoot.Get(string)'></a>

## ICfgRoot\.Get\(string\) Method

获取配置值

```csharp
string? Get(string key);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.Get(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
配置值，不存在时返回null

<a name='Apq.Cfg.ICfgRoot.Get_T_(string)'></a>

## ICfgRoot\.Get\<T\>\(string\) Method

获取配置值并转换为指定类型

```csharp
T? Get<T>(string key);
```
#### Type parameters

<a name='Apq.Cfg.ICfgRoot.Get_T_(string).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.ICfgRoot.Get_T_(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置键

#### Returns
[T](Apq.Cfg.ICfgRoot.md#Apq.Cfg.ICfgRoot.Get_T_(string).T 'Apq\.Cfg\.ICfgRoot\.Get\<T\>\(string\)\.T')  
转换后的配置值，不存在或转换失败时返回默认值

### Example

```csharp
var port = cfg.Get<int>("Server:Port");
var enabled = cfg.Get<bool>("Features:NewUI");
```