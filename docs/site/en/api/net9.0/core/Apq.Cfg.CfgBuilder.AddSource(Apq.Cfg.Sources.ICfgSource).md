#### [Apq\.Cfg](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')

## CfgBuilder\.AddSource\(ICfgSource\) Method

添加自定义配置源（供扩展包使用）

```csharp
public Apq.Cfg.CfgBuilder AddSource(Apq.Cfg.Sources.ICfgSource source);
```
#### Parameters

<a name='Apq.Cfg.CfgBuilder.AddSource(Apq.Cfg.Sources.ICfgSource).source'></a>

`source` [ICfgSource](Apq.Cfg.Sources.ICfgSource.md 'Apq\.Cfg\.Sources\.ICfgSource')

配置源实例，实现 ICfgSource 接口

#### Returns
[CfgBuilder](Apq.Cfg.CfgBuilder.md 'Apq\.Cfg\.CfgBuilder')  
配置构建器实例，支持链式调用

### Example

```csharp
// 添加自定义配置源
var customSource = new CustomCfgSource();
var cfg = new CfgBuilder()
    .AddJson("config.json", level: 0)
    .AddSource(customSource)
    .Build();
```