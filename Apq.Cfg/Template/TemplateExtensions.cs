using Apq.Cfg.Internal;
using Apq.Cfg.Template;

namespace Apq.Cfg;

/// <summary>
/// 配置模板扩展方法
/// </summary>
public static class TemplateExtensions
{
    private static TemplateEngine DefaultEngine => TemplateEngineRegistry.GetEngine();

    /// <summary>
    /// 获取解析变量后的配置值
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <returns>解析后的值</returns>
    /// <example>
    /// <code>
    /// // 配置: { "App:Name": "MyApp", "App:LogPath": "${App:Name}/logs" }
    /// var logPath = cfg.GetResolved("App:LogPath");
    /// // 返回: "MyApp/logs"
    /// </code>
    /// </example>
    public static string? GetResolved(this ICfgRoot cfg, string key)
    {
        var value = cfg.Get(key);
        return DefaultEngine.Resolve(value, cfg);
    }

    /// <summary>
    /// 获取解析变量后的配置值
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <param name="options">解析选项</param>
    /// <returns>解析后的值</returns>
    public static string? GetResolved(this ICfgRoot cfg, string key, VariableResolutionOptions options)
    {
        var engine = new TemplateEngine(options);
        var value = cfg.Get(key);
        return engine.Resolve(value, cfg);
    }

    /// <summary>
    /// 获取解析变量后的配置值，并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <returns>解析并转换后的值</returns>
    public static T? GetResolved<T>(this ICfgRoot cfg, string key)
    {
        var resolved = cfg.GetResolved(key);
        if (resolved == null)
            return default;

        return ValueConverter.Convert<T>(resolved);
    }

    /// <summary>
    /// 解析字符串中的变量
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="template">模板字符串</param>
    /// <returns>解析后的字符串</returns>
    /// <example>
    /// <code>
    /// var message = cfg.ResolveVariables("Application ${App:Name} is running on ${SYS:MachineName}");
    /// // 返回: "Application MyApp is running on SERVER01"
    /// </code>
    /// </example>
    public static string? ResolveVariables(this ICfgRoot cfg, string? template)
    {
        return DefaultEngine.Resolve(template, cfg);
    }

    /// <summary>
    /// 解析字符串中的变量
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="template">模板字符串</param>
    /// <param name="options">解析选项</param>
    /// <returns>解析后的字符串</returns>
    public static string? ResolveVariables(this ICfgRoot cfg, string? template, VariableResolutionOptions options)
    {
        var engine = new TemplateEngine(options);
        return engine.Resolve(template, cfg);
    }

    /// <summary>
    /// 批量获取解析变量后的配置值
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="keys">配置键列表</param>
    /// <returns>解析后的键值对字典</returns>
    public static IReadOnlyDictionary<string, string?> GetManyResolved(this ICfgRoot cfg, IEnumerable<string> keys)
    {
        var result = new Dictionary<string, string?>();
        foreach (var key in keys)
        {
            result[key] = cfg.GetResolved(key);
        }
        return result;
    }

    /// <summary>
    /// 尝试获取解析变量后的配置值
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <param name="value">解析后的值</param>
    /// <returns>是否成功获取</returns>
    public static bool TryGetResolved(this ICfgRoot cfg, string key, out string? value)
    {
        var rawValue = cfg.Get(key);
        if (rawValue == null)
        {
            value = null;
            return false;
        }

        value = DefaultEngine.Resolve(rawValue, cfg);
        return true;
    }

    /// <summary>
    /// 尝试获取解析变量后的配置值，并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="cfg">配置根</param>
    /// <param name="key">配置键</param>
    /// <param name="value">解析并转换后的值</param>
    /// <returns>是否成功获取</returns>
    public static bool TryGetResolved<T>(this ICfgRoot cfg, string key, out T? value)
    {
        if (!cfg.TryGetResolved(key, out var resolved) || resolved == null)
        {
            value = default;
            return false;
        }

        try
        {
            value = ValueConverter.Convert<T>(resolved);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }
}
