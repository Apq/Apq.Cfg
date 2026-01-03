using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Apq.Cfg.SourceGenerator;

/// <summary>
/// 配置节源生成器，为标记了 [CfgSection] 的类生成零反射绑定代码
/// </summary>
[Generator]
public class CfgSectionGenerator : IIncrementalGenerator
{
    /// <summary>
    /// 特性的完全限定名
    /// </summary>
    private const string CfgSectionAttributeFullName = "Apq.Cfg.CfgSectionAttribute";

    /// <summary>
    /// 初始化源生成器，注册特性和代码生成逻辑
    /// </summary>
    /// <param name="context">增量生成器初始化上下文</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. 注册特性源代码（注入到用户项目）
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("CfgSectionAttribute.g.cs", SourceText.From(AttributeSourceCode, Encoding.UTF8));
        });

        // 2. 查找所有标记了 [CfgSection] 的类
        var classDeclarations = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                CfgSectionAttributeFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, ct) => GetConfigClassInfo(ctx, ct))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!);

        // 3. 收集所有配置类信息用于生成扩展方法
        var allClasses = classDeclarations.Collect();

        // 4. 为每个配置类生成绑定代码
        context.RegisterSourceOutput(classDeclarations, static (spc, classInfo) =>
        {
            var source = CodeEmitter.EmitBinderClass(classInfo);
            spc.AddSource($"{classInfo.FullTypeName.Replace(".", "_")}.Binder.g.cs", SourceText.From(source, Encoding.UTF8));
        });

        // 5. 生成统一的扩展方法类
        context.RegisterSourceOutput(allClasses, static (spc, classes) =>
        {
            if (classes.IsEmpty) return;

            var source = CodeEmitter.EmitExtensionsClass(classes);
            spc.AddSource("CfgRootGeneratedExtensions.g.cs", SourceText.From(source, Encoding.UTF8));
        });
    }

    /// <summary>
    /// 从语法上下文提取配置类信息
    /// </summary>
    private static ConfigClassInfo? GetConfigClassInfo(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
            return null;

        // 检查是否为 partial 类
        var classDeclaration = context.TargetNode as ClassDeclarationSyntax;
        if (classDeclaration == null)
            return null;

        bool isPartial = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
        if (!isPartial)
        {
            // 非 partial 类，报告诊断（可选）
            return null;
        }

        // 获取特性参数
        var attribute = context.Attributes.FirstOrDefault();
        if (attribute == null)
            return null;

        // 获取 SectionPath
        string sectionPath = "";
        if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is string path)
        {
            sectionPath = path;
        }

        // 如果 SectionPath 为空，从类名推断
        if (string.IsNullOrEmpty(sectionPath))
        {
            sectionPath = InferSectionPath(classSymbol.Name);
        }

        // 获取 GenerateExtension 属性
        bool generateExtension = true;
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == "GenerateExtension" && namedArg.Value.Value is bool genExt)
            {
                generateExtension = genExt;
            }
        }

        // 收集属性信息
        var properties = new List<PropertyInfo>();
        CollectProperties(classSymbol, properties, ct);

        // 获取命名空间
        var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? ""
            : classSymbol.ContainingNamespace.ToDisplayString();

        return new ConfigClassInfo(
            Namespace: namespaceName,
            ClassName: classSymbol.Name,
            FullTypeName: classSymbol.ToDisplayString(),
            SectionPath: sectionPath,
            GenerateExtension: generateExtension,
            Properties: properties.ToImmutableArray());
    }

    /// <summary>
    /// 从类名推断配置节路径
    /// </summary>
    private static string InferSectionPath(string className)
    {
        // 移除常见后缀
        string[] suffixes = { "Config", "Configuration", "Settings", "Options" };
        foreach (var suffix in suffixes)
        {
            if (className.EndsWith(suffix, StringComparison.Ordinal) && className.Length > suffix.Length)
            {
                return className.Substring(0, className.Length - suffix.Length);
            }
        }
        return className;
    }

    /// <summary>
    /// 收集类的所有可写公共属性
    /// </summary>
    private static void CollectProperties(INamedTypeSymbol classSymbol, List<PropertyInfo> properties, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is not IPropertySymbol prop)
                continue;

            // 只处理公共、可写、非索引器属性
            if (prop.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (prop.IsReadOnly || prop.IsWriteOnly)
                continue;
            if (prop.IsIndexer)
                continue;
            if (prop.IsStatic)
                continue;

            var propInfo = AnalyzeProperty(prop);
            properties.Add(propInfo);
        }
    }

    /// <summary>
    /// 分析属性类型
    /// </summary>
    private static PropertyInfo AnalyzeProperty(IPropertySymbol prop)
    {
        var propType = prop.Type;
        var typeKind = GetTypeKind(propType);
        var elementType = GetElementType(propType);
        var keyType = GetKeyType(propType);

        return new PropertyInfo(
            Name: prop.Name,
            TypeName: propType.ToDisplayString(),
            TypeKind: typeKind,
            ElementTypeName: elementType?.ToDisplayString(),
            KeyTypeName: keyType?.ToDisplayString(),
            IsNullable: propType.NullableAnnotation == NullableAnnotation.Annotated ||
                       (propType is INamedTypeSymbol named && named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T));
    }

    /// <summary>
    /// 判断类型种类
    /// </summary>
    private static TypeKind GetTypeKind(ITypeSymbol type)
    {
        // 处理 Nullable<T>
        if (type is INamedTypeSymbol nullable && nullable.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            type = nullable.TypeArguments[0];
        }

        // 简单类型
        if (IsSimpleType(type))
            return TypeKind.Simple;

        // 数组
        if (type is IArrayTypeSymbol)
            return TypeKind.Array;

        // 泛型集合
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var genericDef = namedType.OriginalDefinition.ToDisplayString();

            // Dictionary
            if (genericDef.StartsWith("System.Collections.Generic.Dictionary<", StringComparison.Ordinal) ||
                genericDef.StartsWith("System.Collections.Generic.IDictionary<", StringComparison.Ordinal))
            {
                return TypeKind.Dictionary;
            }

            // List/IList/ICollection/IEnumerable
            if (genericDef.StartsWith("System.Collections.Generic.List<", StringComparison.Ordinal) ||
                genericDef.StartsWith("System.Collections.Generic.IList<", StringComparison.Ordinal) ||
                genericDef.StartsWith("System.Collections.Generic.ICollection<", StringComparison.Ordinal) ||
                genericDef.StartsWith("System.Collections.Generic.IEnumerable<", StringComparison.Ordinal))
            {
                return TypeKind.List;
            }

            // HashSet/ISet
            if (genericDef.StartsWith("System.Collections.Generic.HashSet<", StringComparison.Ordinal) ||
                genericDef.StartsWith("System.Collections.Generic.ISet<", StringComparison.Ordinal))
            {
                return TypeKind.HashSet;
            }
        }

        // 复杂对象
        if (type.TypeKind == Microsoft.CodeAnalysis.TypeKind.Class ||
            type.TypeKind == Microsoft.CodeAnalysis.TypeKind.Struct)
        {
            return TypeKind.Complex;
        }

        return TypeKind.Unknown;
    }

    /// <summary>
    /// 获取集合元素类型
    /// </summary>
    private static ITypeSymbol? GetElementType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
            return arrayType.ElementType;

        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var args = namedType.TypeArguments;
            if (args.Length >= 1)
            {
                var genericDef = namedType.OriginalDefinition.ToDisplayString();
                if (genericDef.StartsWith("System.Collections.Generic.Dictionary<", StringComparison.Ordinal) ||
                    genericDef.StartsWith("System.Collections.Generic.IDictionary<", StringComparison.Ordinal))
                {
                    return args.Length >= 2 ? args[1] : null;
                }
                return args[0];
            }
        }

        return null;
    }

    /// <summary>
    /// 获取字典键类型
    /// </summary>
    private static ITypeSymbol? GetKeyType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var genericDef = namedType.OriginalDefinition.ToDisplayString();
            if ((genericDef.StartsWith("System.Collections.Generic.Dictionary<", StringComparison.Ordinal) ||
                 genericDef.StartsWith("System.Collections.Generic.IDictionary<", StringComparison.Ordinal)) &&
                namedType.TypeArguments.Length >= 1)
            {
                return namedType.TypeArguments[0];
            }
        }
        return null;
    }

    /// <summary>
    /// 判断是否为简单类型
    /// </summary>
    private static bool IsSimpleType(ITypeSymbol type)
    {
        // 基元类型
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_Char:
            case SpecialType.System_String:
                return true;
        }

        // 枚举
        if (type.TypeKind == Microsoft.CodeAnalysis.TypeKind.Enum)
            return true;

        // 其他常见简单类型
        var fullName = type.ToDisplayString();
        return fullName switch
        {
            "System.DateTime" => true,
            "System.DateTimeOffset" => true,
            "System.TimeSpan" => true,
            "System.Guid" => true,
            "System.Uri" => true,
            "System.DateOnly" => true,
            "System.TimeOnly" => true,
            _ => false
        };
    }

    /// <summary>
    /// 特性源代码（注入到用户项目）
    /// </summary>
    private const string AttributeSourceCode = @"// <auto-generated/>
#nullable enable

namespace Apq.Cfg
{
    /// <summary>
    /// 标记一个类为配置节，源生成器将为其生成零反射的绑定代码
    /// </summary>
    [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class CfgSectionAttribute : global::System.Attribute
    {
        /// <summary>
        /// 配置节路径
        /// </summary>
        public string SectionPath { get; }

        /// <summary>
        /// 是否生成扩展方法
        /// </summary>
        public bool GenerateExtension { get; set; } = true;

        /// <summary>
        /// 创建配置节特性
        /// </summary>
        public CfgSectionAttribute(string sectionPath = """")
        {
            SectionPath = sectionPath;
        }
    }
}
";
}

/// <summary>
/// 配置类信息
/// </summary>
internal sealed record ConfigClassInfo(
    string Namespace,
    string ClassName,
    string FullTypeName,
    string SectionPath,
    bool GenerateExtension,
    ImmutableArray<PropertyInfo> Properties);

/// <summary>
/// 属性信息
/// </summary>
internal sealed record PropertyInfo(
    string Name,
    string TypeName,
    TypeKind TypeKind,
    string? ElementTypeName,
    string? KeyTypeName,
    bool IsNullable);

/// <summary>
/// 类型种类
/// </summary>
internal enum TypeKind
{
    Unknown,
    Simple,
    Array,
    List,
    HashSet,
    Dictionary,
    Complex
}
