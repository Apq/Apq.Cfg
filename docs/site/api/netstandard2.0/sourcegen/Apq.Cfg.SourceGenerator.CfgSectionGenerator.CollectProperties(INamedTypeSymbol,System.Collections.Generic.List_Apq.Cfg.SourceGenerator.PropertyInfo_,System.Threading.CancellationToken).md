#### [Apq\.Cfg\.SourceGenerator](index.md 'index')
### [Apq\.Cfg\.SourceGenerator](Apq.Cfg.SourceGenerator.md 'Apq\.Cfg\.SourceGenerator').[CfgSectionGenerator](Apq.Cfg.SourceGenerator.CfgSectionGenerator.md 'Apq\.Cfg\.SourceGenerator\.CfgSectionGenerator')

## CfgSectionGenerator\.CollectProperties\(INamedTypeSymbol, List\<PropertyInfo\>, CancellationToken\) Method

收集类的所有可写公共属性

```csharp
private static void CollectProperties(INamedTypeSymbol classSymbol, System.Collections.Generic.List<Apq.Cfg.SourceGenerator.PropertyInfo> properties, System.Threading.CancellationToken ct);
```
#### Parameters

<a name='Apq.Cfg.SourceGenerator.CfgSectionGenerator.CollectProperties(INamedTypeSymbol,System.Collections.Generic.List_Apq.Cfg.SourceGenerator.PropertyInfo_,System.Threading.CancellationToken).classSymbol'></a>

`classSymbol` [Microsoft\.CodeAnalysis\.INamedTypeSymbol](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.inamedtypesymbol 'Microsoft\.CodeAnalysis\.INamedTypeSymbol')

<a name='Apq.Cfg.SourceGenerator.CfgSectionGenerator.CollectProperties(INamedTypeSymbol,System.Collections.Generic.List_Apq.Cfg.SourceGenerator.PropertyInfo_,System.Threading.CancellationToken).properties'></a>

`properties` [System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[PropertyInfo](Apq.Cfg.SourceGenerator.PropertyInfo.md 'Apq\.Cfg\.SourceGenerator\.PropertyInfo')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='Apq.Cfg.SourceGenerator.CfgSectionGenerator.CollectProperties(INamedTypeSymbol,System.Collections.Generic.List_Apq.Cfg.SourceGenerator.PropertyInfo_,System.Threading.CancellationToken).ct'></a>

`ct` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')