### [Apq\.Cfg\.Vault](Apq.Cfg.Vault.md 'Apq\.Cfg\.Vault').[CfgBuilderExtensions](Apq.Cfg.Vault.CfgBuilderExtensions.md 'Apq\.Cfg\.Vault\.CfgBuilderExtensions')

## CfgBuilderExtensions\.AddVault\(this CfgBuilder, Action\<VaultCfgOptions\>, int, bool\) Method

添加 Vault 配置源（通用方法）

```csharp
public static Apq.Cfg.CfgBuilder AddVault(this Apq.Cfg.CfgBuilder builder, System.Action<Apq.Cfg.Vault.VaultCfgOptions> configure, int level=0, bool isPrimaryWriter=false);
```
#### Parameters

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVault(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Vault.VaultCfgOptions_,int,bool).builder'></a>

`builder` [Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')

配置构建器

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVault(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Vault.VaultCfgOptions_,int,bool).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[VaultCfgOptions](Apq.Cfg.Vault.VaultCfgOptions.md 'Apq\.Cfg\.Vault\.VaultCfgOptions')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

配置选项

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVault(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Vault.VaultCfgOptions_,int,bool).level'></a>

`level` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

配置层级，数值越大优先级越高

<a name='Apq.Cfg.Vault.CfgBuilderExtensions.AddVault(thisApq.Cfg.CfgBuilder,System.Action_Apq.Cfg.Vault.VaultCfgOptions_,int,bool).isPrimaryWriter'></a>

`isPrimaryWriter` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

是否为主写入源

#### Returns
[Apq\.Cfg\.CfgBuilder](https://learn.microsoft.com/en-us/dotnet/api/apq.cfg.cfgbuilder 'Apq\.Cfg\.CfgBuilder')  
配置构建器