#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ServiceCollectionExtensions](Apq.Cfg.ServiceCollectionExtensions.md 'Apq\.Cfg\.ServiceCollectionExtensions')

## ServiceCollectionExtensions\.AddApqCfg Method

| Overloads | |
| :--- | :--- |
| [AddApqCfg\(this IServiceCollection, Action&lt;CfgBuilder,IServiceProvider&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder,System.IServiceProvider_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder,System\.IServiceProvider\>\)') | 添加 Apq\.Cfg 配置服务（支持访问 IServiceProvider） |
| [AddApqCfg\(this IServiceCollection, Action&lt;CfgBuilder&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder\>\)') | 添加 Apq\.Cfg 配置服务 |
| [AddApqCfg\(this IServiceCollection, Func&lt;IServiceProvider,ICfgRoot&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Func_System.IServiceProvider,Apq.Cfg.ICfgRoot_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Func\<System\.IServiceProvider,Apq\.Cfg\.ICfgRoot\>\)') | 添加 Apq\.Cfg 配置服务（使用工厂方法） |
| [AddApqCfg&lt;TOptions&gt;\(this IServiceCollection, Action&lt;CfgBuilder&gt;, string\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder\>, string\)') | 添加 Apq\.Cfg 配置服务并绑定强类型配置 |

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder,System.IServiceProvider_)'></a>

## ServiceCollectionExtensions\.AddApqCfg\(this IServiceCollection, Action\<CfgBuilder,IServiceProvider\>\) Method

添加 Apq\.Cfg 配置服务（支持访问 IServiceProvider）

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddApqCfg(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Action<Apq.Cfg.CfgBuilder,System.IServiceProvider> configure);
```
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder,System.IServiceProvider_).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder,System.IServiceProvider_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')[System\.IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider 'System\.IServiceProvider')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-2 'System\.Action\`2')

配置构建器委托，接收 CfgBuilder 和 IServiceProvider

#### Returns
[Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')  
服务集合，支持链式调用

### Example

```csharp
// 使用 Data Protection 加密
services.AddDataProtection();
services.AddApqCfg((builder, sp) => builder
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddDataProtectionEncryption(sp.GetRequiredService<IDataProtectionProvider>())
    .AddSensitiveMasking());
```

### Remarks
此重载允许在配置构建过程中访问已注册的服务，
适用于需要依赖其他服务（如 IDataProtectionProvider）的场景。

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_)'></a>

## ServiceCollectionExtensions\.AddApqCfg\(this IServiceCollection, Action\<CfgBuilder\>\) Method

添加 Apq\.Cfg 配置服务

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddApqCfg(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Action<Apq.Cfg.CfgBuilder> configure);
```
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置构建器委托，用于配置各种配置源

#### Returns
[Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')  
服务集合，支持链式调用

### Example

```csharp
services.AddApqCfg(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddJson($"config.{environment}.json", level: 1)
    .AddEnvironmentVariables(prefix: "APP_", level: 2));
```

### Remarks
此方法会同时注册 ICfgRoot 和 IConfigurationRoot 服务，
使您可以在应用程序中同时使用 Apq\.Cfg 和 Microsoft\.Extensions\.Configuration 的 API。

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Func_System.IServiceProvider,Apq.Cfg.ICfgRoot_)'></a>

## ServiceCollectionExtensions\.AddApqCfg\(this IServiceCollection, Func\<IServiceProvider,ICfgRoot\>\) Method

添加 Apq\.Cfg 配置服务（使用工厂方法）

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddApqCfg(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Func<System.IServiceProvider,Apq.Cfg.ICfgRoot> factory);
```
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Func_System.IServiceProvider,Apq.Cfg.ICfgRoot_).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Func_System.IServiceProvider,Apq.Cfg.ICfgRoot_).factory'></a>

`factory` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.IServiceProvider](https://learn.microsoft.com/en-us/dotnet/api/system.iserviceprovider 'System\.IServiceProvider')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

配置根工厂方法，接收服务提供者并返回配置根实例

#### Returns
[Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')  
服务集合，支持链式调用

### Example

```csharp
services.AddApqCfg(sp => {
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new CfgBuilder()
        .AddJson("config.json", level: 0)
        .AddJson($"config.{env.EnvironmentName}.json", level: 1)
        .AddEnvironmentVariables(prefix: "APP_", level: 2)
        .Build();
});
```

### Remarks
使用工厂方法可以访问其他已注册的服务，实现更复杂的配置逻辑。
工厂方法只会在首次请求配置时调用一次。

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string)'></a>

## ServiceCollectionExtensions\.AddApqCfg\<TOptions\>\(this IServiceCollection, Action\<CfgBuilder\>, string\) Method

添加 Apq\.Cfg 配置服务并绑定强类型配置

```csharp
public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddApqCfg<TOptions>(this Microsoft.Extensions.DependencyInjection.IServiceCollection services, System.Action<Apq.Cfg.CfgBuilder> configure, string sectionKey)
    where TOptions : class, new();
```
#### Type parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string).TOptions'></a>

`TOptions`

配置选项类型
#### Parameters

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string).services'></a>

`services` [Microsoft\.Extensions\.DependencyInjection\.IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection 'Microsoft\.Extensions\.DependencyInjection\.IServiceCollection')

服务集合

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置构建器委托，用于配置各种配置源

<a name='Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string).sectionKey'></a>

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
}

// 注册配置服务并绑定强类型配置
services.AddApqCfg<DatabaseOptions>(cfg => cfg
    .AddJson("config.json", level: 0)
    .AddEnvironmentVariables(prefix: "APP_", level: 2),
    "Database");

// 使用配置
var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
```

### Remarks
此方法会注册 ICfgRoot、IConfigurationRoot 和 IOptions\<TOptions\> 服务。
配置选项类必须有无参构造函数。