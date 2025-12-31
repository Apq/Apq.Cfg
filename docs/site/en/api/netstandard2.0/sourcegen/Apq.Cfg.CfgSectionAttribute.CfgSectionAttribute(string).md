#### [Apq\.Cfg\.SourceGenerator](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg').[CfgSectionAttribute](Apq.Cfg.CfgSectionAttribute.md 'Apq\.Cfg\.CfgSectionAttribute')

## CfgSectionAttribute\(string\) Constructor

创建配置节特性

```csharp
public CfgSectionAttribute(string sectionPath="");
```
#### Parameters

<a name='Apq.Cfg.CfgSectionAttribute.CfgSectionAttribute(string).sectionPath'></a>

`sectionPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

配置节路径，为空时使用类名（去掉 Config/Settings 后缀）