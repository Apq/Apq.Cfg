#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ServiceCollectionExtensions](Apq.Cfg.ServiceCollectionExtensions.md 'Apq\.Cfg\.ServiceCollectionExtensions')

## ServiceCollectionExtensions\.ConfigureApqCfg Method

| Overloads | |
| :--- | :--- |
| [ConfigureApqCfg&lt;TOptions&gt;\(this IServiceCollection, string\)](Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string) 'Apq\.Cfg\.ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, string\)') | 配置强类型选项（从 ICfgRoot 绑定），支持嵌套对象和集合 |
| [ConfigureApqCfg&lt;TOptions&gt;\(this IServiceCollection, string, Action&lt;TOptions&gt;\)](Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_) 'Apq\.Cfg\.ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, string, System\.Action\<TOptions\>\)') | 配置强类型选项并启用配置变更监听 |

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string)'></a>

## ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this IServiceCollection, string\) Method

配置强类型选项（从 ICfgRoot 绑定），支持嵌套对象和集合

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection ConfigureApqCfg<TOptions>(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, string sectionKey)
    where TOptions : class, new();
```
#### Type parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string).TOptions'></a>

`TOptions`

配置选项类型
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string).sectionKey'></a>

`sectionKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置节键名，用于定位 TOptions 的配置数据

#### Returns
[Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')  
服务集合，支持链式调用

### Example

```csharp
// 定义配置选项类
public class DatabaseOptions
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
    public RetryOptions Retry { get; set; } = new();
}

public class RetryOptions
{
    public int Count { get; set; } = 3;
    public int Delay { get; set; } = 1000;
}

// 注册配置服务
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddEnvironmentVariables(prefix: "APP_", level: 2));

// 绑定强类型配置
services.ConfigureApqCfg<DatabaseOptions>("Database");

// 使用配置
var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
var retryCount = dbOptions.Retry.Count;
```

### Remarks
此方法会注册 IOptions\<TOptions\>、IOptionsMonitor\<TOptions\> 和 IOptionsSnapshot\<TOptions\> 服务。
配置选项类必须有无参构造函数。
支持嵌套对象和集合类型的自动绑定。

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_)'></a>

## ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this IServiceCollection, string, Action\<TOptions\>\) Method

配置强类型选项并启用配置变更监听

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection ConfigureApqCfg<TOptions>(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, string sectionKey, System.Action<TOptions> onChange)
    where TOptions : class, new();
```
#### Type parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_).TOptions'></a>

`TOptions`

配置选项类型
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_).sectionKey'></a>

`sectionKey` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置节键名，用于定位 TOptions 的配置数据

<a name='Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_).onChange'></a>

`onChange` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[TOptions](Apq.Cfg.ServiceCollectionExtensions.md#Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_).TOptions 'Apq\.Cfg\.ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, string, System\.Action\<TOptions\>\)\.TOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置变更回调函数，当配置发生变化时调用

#### Returns
[Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')  
服务集合，支持链式调用

### Example

```csharp
// 定义配置选项类
public class DatabaseOptions
{
    public string? ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
}

// 注册配置服务并监听变更
services.ConfigureApqCfg<DatabaseOptions>("Database", options => {
    Console.WriteLine($"数据库连接字符串已更新: {options.ConnectionString}");
    
    // 执行必要的重新连接逻辑
    ReconnectDatabase(options.ConnectionString);
});

void ReconnectDatabase(string connectionString)
{
    // 实现数据库重新连接逻辑
}
```

### Remarks
此方法会注册 IOptions\<TOptions\>、IOptionsMonitor\<TOptions\> 和 IOptionsSnapshot\<TOptions\> 服务，
同时添加一个配置变更监听器。
变更回调会在配置源发生变化且导致 TOptions 实例更新时触发。
返回的 IDisposable 对象可用于取消监听，但已自动注册到服务容器，会在应用关闭时释放。