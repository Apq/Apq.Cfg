using System.Globalization;

namespace Apq.Cfg.Internal;

/// <summary>
/// 值类型转换工具类
/// </summary>
internal static class ValueConverter
{
    /// <summary>
    /// 将字符串值转换为目标类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <returns>转换后的值，转换失败返回 default</returns>
    public static T? Convert<T>(string value)
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
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleVal) ? (T)(object)doubleVal : default;

        if (typeof(T) == typeof(decimal))
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalVal) ? (T)(object)decimalVal : default;

        if (typeof(T) == typeof(float))
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatVal) ? (T)(object)floatVal : default;

        if (typeof(T) == typeof(short))
            return short.TryParse(value, out var shortVal) ? (T)(object)shortVal : default;

        if (typeof(T) == typeof(byte))
            return byte.TryParse(value, out var byteVal) ? (T)(object)byteVal : default;

        if (typeof(T) == typeof(Guid))
            return Guid.TryParse(value, out var guidVal) ? (T)(object)guidVal : default;

        if (typeof(T) == typeof(DateTime))
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtVal) ? (T)(object)dtVal : default;

        if (typeof(T) == typeof(DateTimeOffset))
            return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtoVal) ? (T)(object)dtoVal : default;

        if (typeof(T) == typeof(TimeSpan))
            return TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var tsVal) ? (T)(object)tsVal : default;

        if (typeof(T) == typeof(Uri))
        {
            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uriVal) ? (T)(object)uriVal : default;
        }

        // 可空类型和其他类型走通用路径
        var targetType = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
            return (T)(object)value;

        if (underlyingType.IsEnum)
        {
            try
            {
                return (T)Enum.Parse(underlyingType, value, ignoreCase: true);
            }
            catch
            {
                return default;
            }
        }

        try
        {
            return (T)System.Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 尝试将字符串值转换为目标类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="value">字符串值</param>
    /// <param name="result">转换结果</param>
    /// <returns>转换是否成功</returns>
    public static bool TryConvert<T>(string? value, out T? result)
    {
        if (value == null)
        {
            result = default;
            return false;
        }

        try
        {
            result = Convert<T>(value);
            // 对于值类型，检查是否为 default 可能不准确（0 也是有效值）
            // 对于引用类型，null 表示转换失败
            if (typeof(T).IsValueType)
            {
                // 值类型总是返回 true（即使是 default 值）
                return true;
            }
            return result != null;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// 将字符串值转换为指定类型（非泛型版本，用于反射场景）
    /// </summary>
    /// <param name="value">字符串值</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>转换后的值，转换失败返回 null</returns>
    public static object? ConvertToType(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(int))
            return int.TryParse(value, out var i) ? i : null;

        if (underlyingType == typeof(long))
            return long.TryParse(value, out var l) ? l : null;

        if (underlyingType == typeof(bool))
            return bool.TryParse(value, out var b) ? b : null;

        if (underlyingType == typeof(double))
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : null;

        if (underlyingType == typeof(decimal))
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var m) ? m : null;

        if (underlyingType == typeof(float))
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : null;

        if (underlyingType == typeof(short))
            return short.TryParse(value, out var s) ? s : null;

        if (underlyingType == typeof(byte))
            return byte.TryParse(value, out var by) ? by : null;

        if (underlyingType == typeof(Guid))
            return Guid.TryParse(value, out var g) ? g : null;

        if (underlyingType == typeof(DateTime))
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : null;

        if (underlyingType == typeof(DateTimeOffset))
            return DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto) ? dto : null;

        if (underlyingType == typeof(TimeSpan))
            return TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var ts) ? ts : null;

        if (underlyingType == typeof(Uri))
            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri) ? uri : null;

        if (underlyingType.IsEnum)
        {
            try
            {
                return Enum.Parse(underlyingType, value, ignoreCase: true);
            }
            catch
            {
                return null;
            }
        }

        try
        {
            return System.Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }
}
