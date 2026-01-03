namespace Apq.Cfg.Snapshot;

/// <summary>
/// 配置导出选项
/// </summary>
public sealed class ExportOptions
{
    /// <summary>
    /// 是否包含元数据（导出时间、版本等）
    /// </summary>
    public bool IncludeMetadata { get; set; }

    /// <summary>
    /// 是否对敏感值进行脱敏处理
    /// </summary>
    public bool MaskSensitiveValues { get; set; } = true;

    /// <summary>
    /// 仅包含指定的键（支持通配符 *），为 null 时包含所有键
    /// </summary>
    public string[]? IncludeKeys { get; set; }

    /// <summary>
    /// 排除指定的键（支持通配符 *）
    /// </summary>
    public string[]? ExcludeKeys { get; set; }

    /// <summary>
    /// JSON 格式化选项：是否缩进
    /// </summary>
    public bool Indented { get; set; } = true;

    /// <summary>
    /// 环境变量格式的键前缀
    /// </summary>
    public string? EnvPrefix { get; set; }
}
