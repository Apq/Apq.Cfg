#### [Apq\.Cfg\.SourceGenerator](index.md 'index')
### [Apq\.Cfg\.SourceGenerator](Apq.Cfg.SourceGenerator.md 'Apq\.Cfg\.SourceGenerator')

## CodeEmitter Class

代码生成器，生成配置绑定代码

```csharp
internal static class CodeEmitter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CodeEmitter

| Methods | |
| :--- | :--- |
| [EmitArrayPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitArrayPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitArrayPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成数组属性绑定 |
| [EmitBinderClass\(ConfigClassInfo\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitBinderClass(Apq.Cfg.SourceGenerator.ConfigClassInfo).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitBinderClass\(Apq\.Cfg\.SourceGenerator\.ConfigClassInfo\)') | 生成配置类的绑定代码 |
| [EmitBindFromMethod\(StringBuilder, ConfigClassInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitBindFromMethod(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.ConfigClassInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitBindFromMethod\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.ConfigClassInfo, string\)') | 生成 BindFrom 静态方法 |
| [EmitBindToMethod\(StringBuilder, ConfigClassInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitBindToMethod(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.ConfigClassInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitBindToMethod\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.ConfigClassInfo, string\)') | 生成 BindTo 静态方法 |
| [EmitComplexPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitComplexPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitComplexPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成复杂对象属性绑定 |
| [EmitDictionaryPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitDictionaryPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitDictionaryPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成 Dictionary 属性绑定 |
| [EmitElementBinding\(StringBuilder, string, string, string, string, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitElementBinding(System.Text.StringBuilder,string,string,string,string,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitElementBinding\(System\.Text\.StringBuilder, string, string, string, string, string\)') | 生成元素绑定代码（用于数组） |
| [EmitElementBindingForCollection\(StringBuilder, string, string, string, string, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitElementBindingForCollection(System.Text.StringBuilder,string,string,string,string,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitElementBindingForCollection\(System\.Text\.StringBuilder, string, string, string, string, string\)') | 生成元素绑定代码（用于 List） |
| [EmitElementBindingForDictionary\(StringBuilder, string, string, string, string, string, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitElementBindingForDictionary(System.Text.StringBuilder,string,string,string,string,string,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitElementBindingForDictionary\(System\.Text\.StringBuilder, string, string, string, string, string, string\)') | 生成元素绑定代码（用于 Dictionary） |
| [EmitElementBindingForSet\(StringBuilder, string, string, string, string, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitElementBindingForSet(System.Text.StringBuilder,string,string,string,string,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitElementBindingForSet\(System\.Text\.StringBuilder, string, string, string, string, string\)') | 生成元素绑定代码（用于 HashSet） |
| [EmitExtensionsClass\(ImmutableArray&lt;ConfigClassInfo&gt;\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitExtensionsClass(System.Collections.Immutable.ImmutableArray_Apq.Cfg.SourceGenerator.ConfigClassInfo_).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitExtensionsClass\(System\.Collections\.Immutable\.ImmutableArray\<Apq\.Cfg\.SourceGenerator\.ConfigClassInfo\>\)') | 生成扩展方法类 |
| [EmitHashSetPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitHashSetPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitHashSetPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成 HashSet 属性绑定 |
| [EmitListPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitListPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitListPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成 List 属性绑定 |
| [EmitPropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitPropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitPropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成单个属性的绑定代码 |
| [EmitSimplePropertyBinding\(StringBuilder, PropertyInfo, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.EmitSimplePropertyBinding(System.Text.StringBuilder,Apq.Cfg.SourceGenerator.PropertyInfo,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.EmitSimplePropertyBinding\(System\.Text\.StringBuilder, Apq\.Cfg\.SourceGenerator\.PropertyInfo, string\)') | 生成简单类型属性绑定 |
| [GetConvertCode\(string, string\)](Apq.Cfg.SourceGenerator.CodeEmitter.GetConvertCode(string,string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.GetConvertCode\(string, string\)') | 获取类型转换代码 |
| [IsSimpleTypeName\(string\)](Apq.Cfg.SourceGenerator.CodeEmitter.IsSimpleTypeName(string).md 'Apq\.Cfg\.SourceGenerator\.CodeEmitter\.IsSimpleTypeName\(string\)') | 判断是否为简单类型名 |
