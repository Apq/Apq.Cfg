using System.Text;
using System.Text.Json;

namespace Apq.Cfg.Snapshot;

/// <summary>
/// 配置快照导出委托
/// </summary>
/// <param name="data">扁平化的配置键值对</param>
/// <param name="context">导出上下文，包含元数据和选项</param>
/// <returns>导出的字符串</returns>
public delegate string SnapshotExporter(IReadOnlyDictionary<string, string?> data, ExportContext context);

/// <summary>
/// 内置配置快照导出器
/// </summary>
public static class SnapshotExporters
{
    /// <summary>
    /// JSON 格式导出器（嵌套结构）
    /// </summary>
    /// <remarks>
    /// 将配置导出为嵌套的 JSON 结构。
    /// 例如 "Database:Host" = "localhost" 会导出为 {"Database": {"Host": "localhost"}}
    /// </remarks>
    public static string Json(IReadOnlyDictionary<string, string?> data, ExportContext context)
    {
        // 构建嵌套的 JSON 结构
        var root = new Dictionary<string, object?>();

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            SetNestedValue(root, kvp.Key.Split(':'), kvp.Value);
        }

        // 添加元数据
        if (context.IncludeMetadata)
        {
            root["__metadata"] = new Dictionary<string, object?>
            {
                ["exportedAt"] = context.ExportedAt.ToString("O"),
                ["format"] = "Apq.Cfg.Snapshot",
                ["version"] = "1.0",
                ["keyCount"] = context.KeyCount
            };
        }

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = context.Indented,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        return JsonSerializer.Serialize(root, jsonOptions);
    }

    /// <summary>
    /// 键值对格式导出器
    /// </summary>
    /// <remarks>
    /// 将配置导出为扁平的键值对格式（key=value）。
    /// 例如：Database:Host=localhost
    /// </remarks>
    public static string KeyValue(IReadOnlyDictionary<string, string?> data, ExportContext context)
    {
        var sb = new StringBuilder();

        if (context.IncludeMetadata)
        {
            sb.AppendLine($"# Exported at: {context.ExportedAt:O}");
            sb.AppendLine($"# Key count: {context.KeyCount}");
            sb.AppendLine();
        }

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            sb.AppendLine($"{kvp.Key}={kvp.Value ?? ""}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// 环境变量格式导出器
    /// </summary>
    /// <remarks>
    /// 将配置导出为环境变量格式。
    /// 键名转换规则：转为大写，冒号替换为双下划线。
    /// 例如：Database:Host -> DATABASE__HOST=localhost
    /// </remarks>
    public static string Env(IReadOnlyDictionary<string, string?> data, ExportContext context)
    {
        var sb = new StringBuilder();

        if (context.IncludeMetadata)
        {
            sb.AppendLine($"# Exported at: {context.ExportedAt:O}");
            sb.AppendLine($"# Key count: {context.KeyCount}");
            sb.AppendLine();
        }

        var prefix = context.EnvPrefix ?? "";

        foreach (var kvp in data.OrderBy(x => x.Key))
        {
            // 转换键名：Database:Host -> DATABASE__HOST
            var envKey = prefix + kvp.Key.Replace(":", "__").ToUpperInvariant();
            var value = kvp.Value ?? "";

            // 如果值包含特殊字符，用引号包裹
            if (value.Contains(' ') || value.Contains('"') || value.Contains('\'') || value.Contains('\n'))
            {
                value = $"\"{value.Replace("\"", "\\\"")}\"";
            }

            sb.AppendLine($"{envKey}={value}");
        }

        return sb.ToString();
    }

    private static void SetNestedValue(Dictionary<string, object?> root, string[] keys, string? value)
    {
        var current = root;

        for (var i = 0; i < keys.Length - 1; i++)
        {
            var key = keys[i];
            if (!current.TryGetValue(key, out var existing) || existing is not Dictionary<string, object?> dict)
            {
                dict = new Dictionary<string, object?>();
                current[key] = dict;
            }
            current = (Dictionary<string, object?>)current[key]!;
        }

        current[keys[^1]] = value;
    }
}
