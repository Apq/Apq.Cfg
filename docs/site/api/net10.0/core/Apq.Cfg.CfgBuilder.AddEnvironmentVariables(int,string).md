#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddEnvironmentVariables\(int, string\) Method

添加环境变量配置源

```csharp
public Apq.Cfg.CfgBuilder AddEnvironmentVariables(int level, string? prefix=null);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddEnvironmentVariables(int,string).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.CfgBuilder.AddEnvironmentVariables(int,string).prefix'></a>

`prefix` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

环境变量前缀，为null时加载所有环境变量

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddEnvironmentVariables(level: 1, prefix: "APP_")
    .Build();
```