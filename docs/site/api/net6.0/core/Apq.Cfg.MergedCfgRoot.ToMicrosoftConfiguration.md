#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[MergedCfgRoot](Apq.Cfg.MergedCfgRoot.md 'Apq\.Cfg\.MergedCfgRoot')

## MergedCfgRoot\.ToMicrosoftConfiguration Method

| Overloads | |
| :--- | :--- |
| [ToMicrosoftConfiguration\(\)](Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration() 'Apq\.Cfg\.MergedCfgRoot\.ToMicrosoftConfiguration\(\)') | 转换为 Microsoft Configuration（静态快照） |
| [ToMicrosoftConfiguration\(DynamicReloadOptions\)](Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions) 'Apq\.Cfg\.MergedCfgRoot\.ToMicrosoftConfiguration\(Apq\.Cfg\.Changes\.DynamicReloadOptions\)') | 转换为支持动态重载的 Microsoft Configuration |

<a name='Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration()'></a>

## MergedCfgRoot\.ToMicrosoftConfiguration\(\) Method

转换为 Microsoft Configuration（静态快照）

```csharp
public Microsoft.Extensions.Configuration.IConfigurationRoot ToMicrosoftConfiguration();
```

Implements [ToMicrosoftConfiguration\(\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration() 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(\)')

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationRoot](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot 'Microsoft\.Extensions\.Configuration\.IConfigurationRoot')  
Microsoft\.Extensions\.Configuration\.IConfigurationRoot 实例

### Example

```csharp
// 转换为 Microsoft Configuration 并使用
var msConfig = cfg.ToMicrosoftConfiguration();
var connectionString = msConfig.GetConnectionString("DefaultConnection");
```

### Remarks
返回的配置根是静态快照，不会自动更新配置变更。
如需支持动态重载，请使用带 DynamicReloadOptions 参数的重载方法。

<a name='Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions)'></a>

## MergedCfgRoot\.ToMicrosoftConfiguration\(DynamicReloadOptions\) Method

转换为支持动态重载的 Microsoft Configuration

```csharp
public Microsoft.Extensions.Configuration.IConfigurationRoot ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions? options);
```
#### Parameters

<a name='Apq.Cfg.MergedCfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions).options'></a>

`options` [DynamicReloadOptions](Apq.Cfg.Changes.DynamicReloadOptions.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions')

动态重载选项，为 null 时使用默认选项

Implements [ToMicrosoftConfiguration\(DynamicReloadOptions\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions) 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(Apq\.Cfg\.Changes\.DynamicReloadOptions\)')

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationRoot](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot 'Microsoft\.Extensions\.Configuration\.IConfigurationRoot')  
Microsoft\.Extensions\.Configuration\.IConfigurationRoot 实例

### Example

```csharp
// 创建支持动态重载的配置
var options = new DynamicReloadOptions
{
    EnableDynamicReload = true,
    DebounceMs = 500,
    HistoryLimit = 100
};

var msConfig = cfg.ToMicrosoftConfiguration(options);

// 监听配置变更
ChangeToken.OnChange(
    () => msConfig.GetReloadToken(),
    () => Console.WriteLine("配置已更新"));
```

### Remarks
当 EnableDynamicReload 为 true 时，返回的配置根会自动跟踪配置变更。
配置变更会通过 Microsoft\.Extensions\.Configuration 的重载机制传播。
此方法只会创建一个动态配置实例，多次调用会返回相同的实例。