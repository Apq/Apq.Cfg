#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[ICfgRoot](Apq.Cfg.ICfgRoot.md 'Apq\.Cfg\.ICfgRoot')

## ICfgRoot\.ToMicrosoftConfiguration Method

| Overloads | |
| :--- | :--- |
| [ToMicrosoftConfiguration\(\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration() 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(\)') | 转换为 Microsoft Configuration（静态快照） |
| [ToMicrosoftConfiguration\(DynamicReloadOptions\)](Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration.md#Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions) 'Apq\.Cfg\.ICfgRoot\.ToMicrosoftConfiguration\(Apq\.Cfg\.Changes\.DynamicReloadOptions\)') | 转换为支持动态重载的 Microsoft Configuration |

<a name='Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration()'></a>

## ICfgRoot\.ToMicrosoftConfiguration\(\) Method

转换为 Microsoft Configuration（静态快照）

```csharp
Microsoft.Extensions.Configuration.IConfigurationRoot ToMicrosoftConfiguration();
```

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationRoot](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot 'Microsoft\.Extensions\.Configuration\.IConfigurationRoot')

<a name='Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions)'></a>

## ICfgRoot\.ToMicrosoftConfiguration\(DynamicReloadOptions\) Method

转换为支持动态重载的 Microsoft Configuration

```csharp
Microsoft.Extensions.Configuration.IConfigurationRoot ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions? options);
```
#### Parameters

<a name='Apq.Cfg.ICfgRoot.ToMicrosoftConfiguration(Apq.Cfg.Changes.DynamicReloadOptions).options'></a>

`options` [DynamicReloadOptions](Apq.Cfg.Changes.DynamicReloadOptions.md 'Apq\.Cfg\.Changes\.DynamicReloadOptions')

动态重载选项，为 null 时使用默认选项

#### Returns
[Microsoft\.Extensions\.Configuration\.IConfigurationRoot](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfigurationroot 'Microsoft\.Extensions\.Configuration\.IConfigurationRoot')