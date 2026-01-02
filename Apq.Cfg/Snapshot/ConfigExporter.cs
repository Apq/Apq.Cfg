using System.Text;
using System.Text.RegularExpressions;

namespace Apq.Cfg.Snapshot;

/// <summary>
/// 配置快照导出器
/// </summary>
/// <remarks>
/// 提供配置快照导出的核心功能。支持内置格式和自定义导出器。
/// </remarks>
public static class ConfigExporter
{
    /// <summary>
    /// 使用自定义导出器导出配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="exporter">导出器委托</param>
    /// <param name="options">导出选项</param>
    /// <returns>导出的配置字符串</returns>
    /// <example>
    /// <code>
    /// // 使用内置导出器
    /// var json = ConfigExporter.Export(cfg, SnapshotExporters.Json);
    /// var env = ConfigExporter.Export(cfg, SnapshotExporters.Env);
    ///
    /// // 使用自定义导出器
    /// var custom = ConfigExporter.Export(cfg, (data, ctx) => string.Join("\n", data.Select(x => $"{x.Key}={x.Value}")));
    /// </code>
    /// </example>
    public static string Export(ICfgRoot cfg, SnapshotExporter exporter, ExportOptions? options = null)
    {
        options ??= new ExportOptions();
        var data = CollectData(cfg, options);
        var context = ExportContext.FromOptions(options, data.Count);
        return exporter(data, context);
    }

    /// <summary>
    /// 使用默认 JSON 格式导出配置
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="options">导出选项</param>
    /// <returns>导出的配置字符串</returns>
    public static string Export(ICfgRoot cfg, ExportOptions? options = null)
    {
        return Export(cfg, SnapshotExporters.Json, options);
    }

    /// <summary>
    /// 使用自定义导出器导出配置到流
    /// </summary>
    public static async Task ExportAsync(ICfgRoot cfg, SnapshotExporter exporter, Stream stream, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = Export(cfg, exporter, options);
        var bytes = Encoding.UTF8.GetBytes(content);
        await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 导出配置到流
    /// </summary>
    public static async Task ExportAsync(ICfgRoot cfg, Stream stream, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = Export(cfg, options);
        var bytes = Encoding.UTF8.GetBytes(content);
        await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 使用自定义导出器导出配置到文件
    /// </summary>
    public static async Task ExportToFileAsync(ICfgRoot cfg, SnapshotExporter exporter, string filePath, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = Export(cfg, exporter, options);
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 导出配置到文件
    /// </summary>
    public static async Task ExportToFileAsync(ICfgRoot cfg, string filePath, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = Export(cfg, options);
        await File.WriteAllTextAsync(filePath, content, Encoding.UTF8, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 导出配置为字典
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="options">导出选项（仅使用过滤和脱敏选项）</param>
    /// <returns>扁平化的配置字典</returns>
    public static IReadOnlyDictionary<string, string?> ExportToDictionary(ICfgRoot cfg, ExportOptions? options = null)
    {
        options ??= new ExportOptions();
        return CollectData(cfg, options);
    }

    /// <summary>
    /// 收集配置数据（应用过滤和脱敏）
    /// </summary>
    internal static Dictionary<string, string?> CollectData(ICfgRoot cfg, ExportOptions options)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var useMasked = options.MaskSensitiveValues;

        // 递归收集所有配置键
        CollectKeysRecursive(cfg, "", result, useMasked);

        // 应用过滤
        if (options.IncludeKeys != null && options.IncludeKeys.Length > 0)
        {
            var includePatterns = options.IncludeKeys.Select(CompileWildcard).ToArray();
            result = result
                .Where(kvp => includePatterns.Any(p => p.IsMatch(kvp.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        if (options.ExcludeKeys != null && options.ExcludeKeys.Length > 0)
        {
            var excludePatterns = options.ExcludeKeys.Select(CompileWildcard).ToArray();
            result = result
                .Where(kvp => !excludePatterns.Any(p => p.IsMatch(kvp.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        return result;
    }

    private static void CollectKeysRecursive(ICfgRoot cfg, string prefix, Dictionary<string, string?> result, bool useMasked)
    {
        var section = string.IsNullOrEmpty(prefix) ? null : cfg.GetSection(prefix);
        var childKeys = section?.GetChildKeys() ?? cfg.GetChildKeys();

        foreach (var childKey in childKeys)
        {
            var fullKey = string.IsNullOrEmpty(prefix) ? childKey : $"{prefix}:{childKey}";

            // 尝试获取值
            var value = useMasked ? cfg.GetMasked(fullKey) : cfg.Get(fullKey);

            // 检查是否有子节点
            var childSection = cfg.GetSection(fullKey);
            var hasChildren = childSection.GetChildKeys().Any();

            if (hasChildren)
            {
                // 递归处理子节点
                CollectKeysRecursive(cfg, fullKey, result, useMasked);
            }

            // 只添加叶子节点或有值的节点
            if (value != null && value != "[null]")
            {
                result[fullKey] = value;
            }
        }
    }

    private static Regex CompileWildcard(string pattern)
    {
        // 将通配符模式转换为正则表达式
        var regexPattern = "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$";
        return new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
