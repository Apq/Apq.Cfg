#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgSection](Apq.Cfg.ICfgSection.md 'Apq\.Cfg\.ICfgSection')

## ICfgSection\.Get Method

| Overloads | |
| :--- | :--- |
| [Get\(string\)](Apq.Cfg.ICfgSection.Get.md#Apq.Cfg.ICfgSection.Get(string) 'Apq\.Cfg\.ICfgSection\.Get\(string\)') | 获取配置值（相对于此节的键） |
| [Get&lt;T&gt;\(string\)](Apq.Cfg.ICfgSection.Get.md#Apq.Cfg.ICfgSection.Get_T_(string) 'Apq\.Cfg\.ICfgSection\.Get\<T\>\(string\)') | 获取配置值并转换为指定类型 |

<a name='Apq.Cfg.ICfgSection.Get(string)'></a>

## ICfgSection\.Get\(string\) Method

获取配置值（相对于此节的键）

```csharp
string? Get(string key);
```
#### Parameters

<a name='Apq.Cfg.ICfgSection.Get(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

相对于此节的键名

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
配置值，不存在时返回null

### Example

```csharp
var dbSection = cfg.GetSection("Database");
var host = dbSection.Get("Host"); // 等同于 cfg.Get("Database:Host")
```

<a name='Apq.Cfg.ICfgSection.Get_T_(string)'></a>

## ICfgSection\.Get\<T\>\(string\) Method

获取配置值并转换为指定类型

```csharp
T? Get<T>(string key);
```
#### Type parameters

<a name='Apq.Cfg.ICfgSection.Get_T_(string).T'></a>

`T`

目标类型
#### Parameters

<a name='Apq.Cfg.ICfgSection.Get_T_(string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

相对于此节的键名

#### Returns
[T](Apq.Cfg.ICfgSection.md#Apq.Cfg.ICfgSection.Get_T_(string).T 'Apq\.Cfg\.ICfgSection\.Get\<T\>\(string\)\.T')  
转换后的配置值，不存在或转换失败时返回默认值

### Example

```csharp
var dbSection = cfg.GetSection("Database");
var port = dbSection.Get<int>("Port"); // 等同于 cfg.Get<int>("Database:Port")
```