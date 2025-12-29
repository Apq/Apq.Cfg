#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## ServiceCollectionExtensions Class

IServiceCollection 扩展方法

```csharp
public static class ServiceCollectionExtensions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ServiceCollectionExtensions

| Methods | |
| :--- | :--- |
| [AddApqCfg\(this IServiceCollection, Action&lt;CfgBuilder,IServiceProvider&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder,System.IServiceProvider_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder,System\.IServiceProvider\>\)') | 添加 Apq\.Cfg 配置服务（支持访问 IServiceProvider） |
| [AddApqCfg\(this IServiceCollection, Action&lt;CfgBuilder&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder\>\)') | 添加 Apq\.Cfg 配置服务 |
| [AddApqCfg\(this IServiceCollection, Func&lt;IServiceProvider,ICfgRoot&gt;\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Func_System.IServiceProvider,Apq.Cfg.ICfgRoot_) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Func\<System\.IServiceProvider,Apq\.Cfg\.ICfgRoot\>\)') | 添加 Apq\.Cfg 配置服务（使用工厂方法） |
| [AddApqCfg&lt;TOptions&gt;\(this IServiceCollection, Action&lt;CfgBuilder&gt;, string\)](Apq.Cfg.ServiceCollectionExtensions.AddApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.AddApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,System.Action_Apq.Cfg.CfgBuilder_,string) 'Apq\.Cfg\.ServiceCollectionExtensions\.AddApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, System\.Action\<Apq\.Cfg\.CfgBuilder\>, string\)') | 添加 Apq\.Cfg 配置服务并绑定强类型配置 |
| [ConfigureApqCfg&lt;TOptions&gt;\(this IServiceCollection, string\)](Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string) 'Apq\.Cfg\.ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, string\)') | 配置强类型选项（从 ICfgRoot 绑定），支持嵌套对象和集合 |
| [ConfigureApqCfg&lt;TOptions&gt;\(this IServiceCollection, string, Action&lt;TOptions&gt;\)](Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg.md#Apq.Cfg.ServiceCollectionExtensions.ConfigureApqCfg_TOptions_(thisMicrosoft.Extensions.DependencyInjection.IServiceCollection,string,System.Action_TOptions_) 'Apq\.Cfg\.ServiceCollectionExtensions\.ConfigureApqCfg\<TOptions\>\(this Microsoft\.Extensions\.DependencyInjection\.IServiceCollection, string, System\.Action\<TOptions\>\)') | 配置强类型选项并启用配置变更监听 |
