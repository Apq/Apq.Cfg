#### [Apq\.Cfg\.SourceGenerator](index.md 'index')
### [Apq\.Cfg](Apq.Cfg.md 'Apq\.Cfg')

## CfgSectionAttribute Class

标记一个类为配置节，源生成器将为其生成零反射的绑定代码

```csharp
public sealed class CfgSectionAttribute : System.Attribute
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.attribute 'System\.Attribute') &#129106; CfgSectionAttribute

### Remarks
使用示例：

```csharp
[CfgSection("Database")]
public partial class DatabaseConfig
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; } = 30;
}

// 使用生成的绑定方法
var config = DatabaseConfig.BindFrom(cfg.GetSection("Database"));
```

| Constructors | |
| :--- | :--- |
| [CfgSectionAttribute\(string\)](Apq.Cfg.CfgSectionAttribute.CfgSectionAttribute(string).md 'Apq\.Cfg\.CfgSectionAttribute\.CfgSectionAttribute\(string\)') | 创建配置节特性 |

| Properties | |
| :--- | :--- |
| [GenerateExtension](Apq.Cfg.CfgSectionAttribute.GenerateExtension.md 'Apq\.Cfg\.CfgSectionAttribute\.GenerateExtension') | 是否生成扩展方法，默认为 true |
| [SectionPath](Apq.Cfg.CfgSectionAttribute.SectionPath.md 'Apq\.Cfg\.CfgSectionAttribute\.SectionPath') | 配置节路径，如 "Database" 或 "App:Settings" |
