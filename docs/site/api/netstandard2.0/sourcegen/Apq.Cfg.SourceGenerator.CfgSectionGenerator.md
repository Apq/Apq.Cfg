#### [Apq\.Cfg\.SourceGenerator](index.md 'index')
### [Apq\.Cfg\.SourceGenerator](Apq.Cfg.SourceGenerator.md 'Apq\.Cfg\.SourceGenerator')

## CfgSectionGenerator Class

配置节源生成器，为标记了 \[CfgSection\] 的类生成零反射绑定代码

```csharp
public class CfgSectionGenerator
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [Microsoft\.CodeAnalysis\.IIncrementalGenerator](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iincrementalgenerator 'Microsoft\.CodeAnalysis\.IIncrementalGenerator') &#129106; CfgSectionGenerator

| Fields | |
| :--- | :--- |
| [AttributeSourceCode](Apq.Cfg.SourceGenerator.CfgSectionGenerator.AttributeSourceCode.md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.AttributeSourceCode') | 特性源代码（注入到用户项目） |
| [CfgSectionAttributeFullName](Apq.Cfg.SourceGenerator.CfgSectionGenerator.CfgSectionAttributeFullName.md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.CfgSectionAttributeFullName') | 特性的完全限定名 |

| Methods | |
| :--- | :--- |
| [AnalyzeProperty\(IPropertySymbol\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.AnalyzeProperty(IPropertySymbol).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.AnalyzeProperty\(IPropertySymbol\)') | 分析属性类型 |
| [CollectProperties\(INamedTypeSymbol, List&lt;PropertyInfo&gt;, CancellationToken\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.CollectProperties(INamedTypeSymbol,System.Collections.Generic.List_Apq.Cfg.SourceGenerator.PropertyInfo_,System.Threading.CancellationToken).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.CollectProperties\(INamedTypeSymbol, System\.Collections\.Generic\.List\<Apq\.Cfg\.SourceGenerator\.PropertyInfo\>, System\.Threading\.CancellationToken\)') | 收集类的所有可写公共属性 |
| [GetConfigClassInfo\(GeneratorAttributeSyntaxContext, CancellationToken\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.GetConfigClassInfo(GeneratorAttributeSyntaxContext,System.Threading.CancellationToken).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.GetConfigClassInfo\(GeneratorAttributeSyntaxContext, System\.Threading\.CancellationToken\)') | 从语法上下文提取配置类信息 |
| [GetElementType\(ITypeSymbol\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.GetElementType(ITypeSymbol).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.GetElementType\(ITypeSymbol\)') | 获取集合元素类型 |
| [GetKeyType\(ITypeSymbol\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.GetKeyType(ITypeSymbol).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.GetKeyType\(ITypeSymbol\)') | 获取字典键类型 |
| [GetTypeKind\(ITypeSymbol\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.GetTypeKind(ITypeSymbol).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.GetTypeKind\(ITypeSymbol\)') | 判断类型种类 |
| [InferSectionPath\(string\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.InferSectionPath(string).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.InferSectionPath\(string\)') | 从类名推断配置节路径 |
| [IsSimpleType\(ITypeSymbol\)](Apq.Cfg.SourceGenerator.CfgSectionGenerator.IsSimpleType(ITypeSymbol).md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator\.IsSimpleType\(ITypeSymbol\)') | 判断是否为简单类型 |
