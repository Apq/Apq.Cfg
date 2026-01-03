namespace Apq.Cfg.Snapshot;

/// <summary>
/// 导出上下文，提供导出时的元数据和选项
/// </summary>
public sealed class ExportContext
{
    /// <summary>
    /// 导出时间（UTC）
    /// </summary>
    public DateTime ExportedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 是否包含元数据
    /// </summary>
    public bool IncludeMetadata { get; init; }

    /// <summary>
    /// 是否缩进格式化
    /// </summary>
    public bool Indented { get; init; } = true;

    /// <summary>
    /// 环境变量格式的键前缀
    /// </summary>
    public string? EnvPrefix { get; init; }

    /// <summary>
    /// 配置键数量
    /// </summary>
    public int KeyCount { get; init; }

    /// <summary>
    /// 自定义属性，供自定义导出器使用
    /// </summary>
    public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

    /// <summary>
    /// 从 ExportOptions 创建上下文
    /// </summary>
    internal static ExportContext FromOptions(ExportOptions options, int keyCount)
    {
        return new ExportContext
        {
            IncludeMetadata = options.IncludeMetadata,
            Indented = options.Indented,
            EnvPrefix = options.EnvPrefix,
            KeyCount = keyCount
        };
    }
}
