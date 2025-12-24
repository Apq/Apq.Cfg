namespace Apq.Cfg;

/// <summary>
/// ICfgRoot 扩展方法
/// </summary>
public static class CfgRootExtensions
{
    /// <summary>
    /// 尝试获取配置值
    /// </summary>
    public static bool TryGet<T>(this ICfgRoot root, string key, out T? value)
    {
        var rawValue = root.Get(key);
        if (rawValue == null)
        {
            value = default;
            return false;
        }
        // 直接转换已获取的字符串值，避免二次查询
        value = ConvertValue<T>(rawValue);
        return true;
    }

    /// <summary>
    /// 获取必需的配置值，如果不存在则抛出异常
    /// </summary>
    public static T GetRequired<T>(this ICfgRoot root, string key)
    {
        var rawValue = root.Get(key);
        if (rawValue == null)
            throw new InvalidOperationException($"必需的配置键 '{key}' 不存在");
        // 直接转换已获取的字符串值，避免二次查询
        return ConvertValue<T>(rawValue)!;
    }

    /// <summary>
    /// 将字符串值转换为目标类型（与 MergedCfgRoot.ConvertValue 保持一致）
    /// </summary>
    private static T? ConvertValue<T>(string value)
    {
        // 常用类型特化处理，避免反射开销
        if (typeof(T) == typeof(string))
            return (T)(object)value;

        if (typeof(T) == typeof(int))
            return int.TryParse(value, out var intVal) ? (T)(object)intVal : default;

        if (typeof(T) == typeof(bool))
            return bool.TryParse(value, out var boolVal) ? (T)(object)boolVal : default;

        if (typeof(T) == typeof(long))
            return long.TryParse(value, out var longVal) ? (T)(object)longVal : default;

        if (typeof(T) == typeof(double))
            return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var doubleVal) ? (T)(object)doubleVal : default;

        if (typeof(T) == typeof(decimal))
            return decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var decimalVal) ? (T)(object)decimalVal : default;

        // 可空类型和其他类型走通用路径
        var targetType = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
            return (T)(object)value;

        if (underlyingType.IsEnum)
            return (T)Enum.Parse(underlyingType, value, ignoreCase: true);

        try
        {
            return (T)Convert.ChangeType(value, underlyingType, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch
        {
            return default;
        }
    }
}
