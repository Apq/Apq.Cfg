using Apq.Cfg.Internal;

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
        value = ValueConverter.Convert<T>(rawValue);
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
        return ValueConverter.Convert<T>(rawValue)!;
    }

    /// <summary>
    /// 获取配置值，如果不存在则返回默认值
    /// </summary>
    public static T? GetOrDefault<T>(this ICfgRoot root, string key, T? defaultValue = default)
    {
        var rawValue = root.Get(key);
        if (rawValue == null)
            return defaultValue;
        return ValueConverter.Convert<T>(rawValue);
    }
}
