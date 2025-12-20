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
        if (!root.Exists(key))
        {
            value = default;
            return false;
        }
        value = root.Get<T>(key);
        return true;
    }

    /// <summary>
    /// 获取必需的配置值，如果不存在则抛出异常
    /// </summary>
    public static T GetRequired<T>(this ICfgRoot root, string key)
    {
        if (!root.Exists(key))
            throw new InvalidOperationException($"必需的配置键 '{key}' 不存在");
        return root.Get<T>(key)!;
    }
}
