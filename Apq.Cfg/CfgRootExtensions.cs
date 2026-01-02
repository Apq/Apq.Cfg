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
    public static bool TryGetValue<T>(this ICfgRoot root, string key, out T? value)
    {
        var rawValue = root[key];
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
        var rawValue = root[key];
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
        var rawValue = root[key];
        if (rawValue == null)
            return defaultValue;
        return ValueConverter.Convert<T>(rawValue);
    }

    /// <summary>
    /// 获取脱敏后的配置值（用于日志输出）
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <returns>脱敏后的值</returns>
    /// <example>
    /// <code>
    /// // 日志输出时使用脱敏值
    /// logger.LogInformation("连接字符串: {ConnectionString}", cfg.GetMasked("Database:ConnectionString"));
    /// // 输出: 连接字符串: Ser***ion
    /// </code>
    /// </example>
    public static string GetMasked(this ICfgRoot cfg, string key)
    {
        if (cfg is MergedCfgRoot merged)
        {
            return merged.GetMasked(key);
        }
        return cfg[key] ?? "[null]";
    }

    /// <summary>
    /// 获取所有配置的脱敏快照（用于调试）
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <returns>脱敏后的配置键值对字典</returns>
    /// <example>
    /// <code>
    /// // 获取脱敏快照用于调试
    /// var snapshot = cfg.GetMaskedSnapshot();
    /// foreach (var (key, value) in snapshot)
    /// {
    ///     Console.WriteLine($"{key}: {value}");
    /// }
    /// </code>
    /// </example>
    public static IReadOnlyDictionary<string, string> GetMaskedSnapshot(this ICfgRoot cfg)
    {
        if (cfg is MergedCfgRoot merged)
        {
            return merged.GetMaskedSnapshot();
        }

        var result = new Dictionary<string, string>();
        foreach (var key in cfg.GetChildKeys())
        {
            result[key] = cfg[key] ?? "[null]";
        }
        return result;
    }
}
