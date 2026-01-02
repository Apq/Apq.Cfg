using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Apq.Cfg.Internal;

namespace Apq.Cfg.DependencyInjection;

/// <summary>
/// 对象绑定器，支持嵌套对象和集合绑定
/// </summary>
/// <remarks>
/// 此类使用反射进行配置绑定，不支持 Native AOT。
/// 对于 AOT 场景，请使用 Apq.Cfg.SourceGenerator 包中的 [CfgSection] 特性。
/// </remarks>
[RequiresUnreferencedCode("ObjectBinder uses reflection for configuration binding. Use Apq.Cfg.SourceGenerator with [CfgSection] attribute for AOT compatibility.")]
#if NET7_0_OR_GREATER
[RequiresDynamicCode("ObjectBinder uses MakeGenericType and Activator.CreateInstance. Use Apq.Cfg.SourceGenerator with [CfgSection] attribute for AOT compatibility.")]
#endif
internal static class ObjectBinder
{
    /// <summary>
    /// 将配置节绑定到对象
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="section">配置节</param>
    /// <param name="target">目标对象</param>
    public static void BindSection<T>(ICfgSection section, T target) where T : class
    {
        if (target == null) return;
        BindObject(section, target, typeof(T));
    }

    /// <summary>
    /// 将配置节绑定到对象（非泛型版本）
    /// </summary>
    /// <param name="section">配置节</param>
    /// <param name="target">目标对象</param>
    /// <param name="targetType">目标类型</param>
    public static void BindObject(ICfgSection section, object target, Type targetType)
    {
        if (target == null) return;

        var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

        foreach (var prop in properties)
        {
            try
            {
                BindProperty(section, target, prop);
            }
            catch
            {
                // 忽略单个属性绑定失败
            }
        }
    }

    private static void BindProperty(ICfgSection section, object target, PropertyInfo prop)
    {
        var propType = prop.PropertyType;
        var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;

        // 1. 简单类型：直接从配置值转换
        if (IsSimpleType(underlyingType))
        {
            var value = section[prop.Name];
            if (value != null)
            {
                var convertedValue = ValueConverter.ConvertToType(value, propType);
                if (convertedValue != null)
                {
                    prop.SetValue(target, convertedValue);
                }
            }
            return;
        }

        // 2. 数组类型
        if (propType.IsArray)
        {
            var elementType = propType.GetElementType()!;
            var array = BindArray(section.GetSection(prop.Name), elementType);
            if (array != null)
            {
                prop.SetValue(target, array);
            }
            return;
        }

        // 3. 泛型集合类型 (List<T>, IList<T>, ICollection<T>, IEnumerable<T>)
        if (propType.IsGenericType)
        {
            var genericDef = propType.GetGenericTypeDefinition();
            var elementType = propType.GetGenericArguments()[0];

            // Dictionary<TKey, TValue>
            if (genericDef == typeof(Dictionary<,>) ||
                genericDef == typeof(IDictionary<,>))
            {
                var keyType = propType.GetGenericArguments()[0];
                var valueType = propType.GetGenericArguments()[1];
                var dict = BindDictionary(section.GetSection(prop.Name), keyType, valueType);
                if (dict != null)
                {
                    prop.SetValue(target, dict);
                }
                return;
            }

            // List<T>, IList<T>, ICollection<T>, IEnumerable<T>
            if (genericDef == typeof(List<>) ||
                genericDef == typeof(IList<>) ||
                genericDef == typeof(ICollection<>) ||
                genericDef == typeof(IEnumerable<>))
            {
                var list = BindList(section.GetSection(prop.Name), elementType);
                if (list != null)
                {
                    prop.SetValue(target, list);
                }
                return;
            }

            // HashSet<T>, ISet<T>
            if (genericDef == typeof(HashSet<>) ||
                genericDef == typeof(ISet<>))
            {
                var set = BindHashSet(section.GetSection(prop.Name), elementType);
                if (set != null)
                {
                    prop.SetValue(target, set);
                }
                return;
            }
        }

        // 4. 嵌套复杂对象
        if (propType.IsClass && propType != typeof(string))
        {
            var childSection = section.GetSection(prop.Name);
            var childKeys = childSection.GetChildKeys().ToList();

            if (childKeys.Count > 0)
            {
                // 获取或创建嵌套对象
                var nestedObj = prop.GetValue(target);
                if (nestedObj == null)
                {
                    nestedObj = CreateInstance(propType);
                    if (nestedObj == null) return;
                }

                BindObject(childSection, nestedObj, propType);
                prop.SetValue(target, nestedObj);
            }
        }
    }

    private static Array? BindArray(ICfgSection section, Type elementType)
    {
        var childKeys = section.GetChildKeys()
            .Where(k => int.TryParse(k, out _))
            .OrderBy(k => int.Parse(k))
            .ToList();

        if (childKeys.Count == 0) return null;

        var array = Array.CreateInstance(elementType, childKeys.Count);

        for (int i = 0; i < childKeys.Count; i++)
        {
            var key = childKeys[i];
            var value = BindElement(section, key, elementType);
            if (value != null)
            {
                array.SetValue(value, i);
            }
        }

        return array;
    }

    private static IList? BindList(ICfgSection section, Type elementType)
    {
        var childKeys = section.GetChildKeys()
            .Where(k => int.TryParse(k, out _))
            .OrderBy(k => int.Parse(k))
            .ToList();

        if (childKeys.Count == 0) return null;

        var listType = typeof(List<>).MakeGenericType(elementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var key in childKeys)
        {
            var value = BindElement(section, key, elementType);
            list.Add(value);
        }

        return list;
    }

    private static object? BindHashSet(ICfgSection section, Type elementType)
    {
        var childKeys = section.GetChildKeys()
            .Where(k => int.TryParse(k, out _))
            .OrderBy(k => int.Parse(k))
            .ToList();

        if (childKeys.Count == 0) return null;

        var setType = typeof(HashSet<>).MakeGenericType(elementType);
        var set = Activator.CreateInstance(setType)!;
        var addMethod = setType.GetMethod("Add")!;

        foreach (var key in childKeys)
        {
            var value = BindElement(section, key, elementType);
            if (value != null)
            {
                addMethod.Invoke(set, new[] { value });
            }
        }

        return set;
    }

    private static IDictionary? BindDictionary(ICfgSection section, Type keyType, Type valueType)
    {
        var childKeys = section.GetChildKeys().ToList();

        if (childKeys.Count == 0) return null;

        var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
        var dict = (IDictionary)Activator.CreateInstance(dictType)!;

        foreach (var key in childKeys)
        {
            var convertedKey = ConvertKey(key, keyType);
            if (convertedKey == null) continue;

            var value = BindElement(section, key, valueType);
            dict[convertedKey] = value;
        }

        return dict;
    }

    private static object? BindElement(ICfgSection section, string key, Type elementType)
    {
        var underlyingType = Nullable.GetUnderlyingType(elementType) ?? elementType;

        // 简单类型
        if (IsSimpleType(underlyingType))
        {
            var value = section[key];
            if (value != null)
            {
                return ValueConverter.ConvertToType(value, elementType);
            }
            return null;
        }

        // 复杂类型
        var childSection = section.GetSection(key);
        var childKeys = childSection.GetChildKeys().ToList();

        if (childKeys.Count > 0)
        {
            var obj = CreateInstance(elementType);
            if (obj != null)
            {
                BindObject(childSection, obj, elementType);
            }
            return obj;
        }

        return null;
    }

    private static object? ConvertKey(string key, Type keyType)
    {
        if (keyType == typeof(string))
            return key;

        return ValueConverter.ConvertToType(key, keyType);
    }

    private static object? CreateInstance(Type type)
    {
        try
        {
            // 尝试使用无参构造函数
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                return Activator.CreateInstance(type);
            }

            // 对于没有无参构造函数的类型，返回 null
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               type == typeof(Uri);
    }
}
