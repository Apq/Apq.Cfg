namespace Apq.Cfg.Snapshot;

/// <summary>
/// ICfgRoot 配置快照导出扩展方法
/// </summary>
public static class CfgRootSnapshotExtensions
{
    #region 自定义导出器

    /// <summary>
    /// 使用自定义导出器导出配置快照
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="exporter">导出器委托</param>
    /// <param name="options">导出选项</param>
    /// <returns>导出的配置字符串</returns>
    /// <example>
    /// <code>
    /// // 使用内置导出器
    /// var json = cfg.ExportSnapshot(SnapshotExporters.Json);
    /// var env = cfg.ExportSnapshot(SnapshotExporters.Env);
    ///
    /// // 使用自定义导出器
    /// var custom = cfg.ExportSnapshot((data, ctx) => string.Join("\n", data.Select(x => $"{x.Key}={x.Value}")));
    /// </code>
    /// </example>
    public static string ExportSnapshot(this ICfgRoot cfg, SnapshotExporter exporter, ExportOptions? options = null)
    {
        return ConfigExporter.Export(cfg, exporter, options);
    }

    /// <summary>
    /// 使用自定义导出器导出配置快照到文件
    /// </summary>
    public static Task ExportSnapshotToFileAsync(this ICfgRoot cfg, SnapshotExporter exporter, string filePath, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        return ConfigExporter.ExportToFileAsync(cfg, exporter, filePath, options, cancellationToken);
    }

    /// <summary>
    /// 使用自定义导出器导出配置快照到流
    /// </summary>
    public static Task ExportSnapshotAsync(this ICfgRoot cfg, SnapshotExporter exporter, Stream stream, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        return ConfigExporter.ExportAsync(cfg, exporter, stream, options, cancellationToken);
    }

    #endregion

    #region 内置格式快捷方法

    /// <summary>
    /// 导出配置快照为字符串（默认 JSON 格式）
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="options">导出选项</param>
    /// <returns>导出的配置字符串</returns>
    /// <example>
    /// <code>
    /// // 导出为 JSON（默认）
    /// var json = cfg.ExportSnapshot();
    ///
    /// // 导出时排除敏感配置
    /// var safe = cfg.ExportSnapshot(new ExportOptions
    /// {
    ///     ExcludeKeys = new[] { "Secrets:*", "Database:Password" }
    /// });
    /// </code>
    /// </example>
    public static string ExportSnapshot(this ICfgRoot cfg, ExportOptions? options = null)
    {
        return ConfigExporter.Export(cfg, options);
    }

    /// <summary>
    /// 导出配置快照为 JSON 字符串
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="indented">是否缩进格式化</param>
    /// <param name="maskSensitive">是否脱敏敏感值</param>
    /// <returns>JSON 格式的配置字符串</returns>
    public static string ExportSnapshotAsJson(this ICfgRoot cfg, bool indented = true, bool maskSensitive = true)
    {
        return ConfigExporter.Export(cfg, SnapshotExporters.Json, new ExportOptions
        {
            Indented = indented,
            MaskSensitiveValues = maskSensitive
        });
    }

    /// <summary>
    /// 导出配置快照为环境变量格式
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="prefix">环境变量前缀</param>
    /// <param name="maskSensitive">是否脱敏敏感值</param>
    /// <returns>环境变量格式的配置字符串</returns>
    public static string ExportSnapshotAsEnv(this ICfgRoot cfg, string? prefix = null, bool maskSensitive = true)
    {
        return ConfigExporter.Export(cfg, SnapshotExporters.Env, new ExportOptions
        {
            EnvPrefix = prefix,
            MaskSensitiveValues = maskSensitive
        });
    }

    /// <summary>
    /// 导出配置快照为键值对格式
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="maskSensitive">是否脱敏敏感值</param>
    /// <returns>键值对格式的配置字符串</returns>
    public static string ExportSnapshotAsKeyValue(this ICfgRoot cfg, bool maskSensitive = true)
    {
        return ConfigExporter.Export(cfg, SnapshotExporters.KeyValue, new ExportOptions
        {
            MaskSensitiveValues = maskSensitive
        });
    }

    /// <summary>
    /// 导出配置快照为字典
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="maskSensitive">是否脱敏敏感值</param>
    /// <returns>扁平化的配置字典</returns>
    public static IReadOnlyDictionary<string, string?> ExportSnapshotAsDictionary(this ICfgRoot cfg, bool maskSensitive = true)
    {
        return ConfigExporter.ExportToDictionary(cfg, new ExportOptions
        {
            MaskSensitiveValues = maskSensitive
        });
    }

    #endregion

    #region 文件和流导出

    /// <summary>
    /// 导出配置快照到文件
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="options">导出选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static Task ExportSnapshotToFileAsync(this ICfgRoot cfg, string filePath, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        return ConfigExporter.ExportToFileAsync(cfg, filePath, options, cancellationToken);
    }

    /// <summary>
    /// 导出配置快照到流
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="stream">输出流</param>
    /// <param name="options">导出选项</param>
    /// <param name="cancellationToken">取消令牌</param>
    public static Task ExportSnapshotAsync(this ICfgRoot cfg, Stream stream, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        return ConfigExporter.ExportAsync(cfg, stream, options, cancellationToken);
    }

    #endregion

    #region 构建器模式

    /// <summary>
    /// 使用构建器模式配置导出选项
    /// </summary>
    /// <param name="cfg">配置根</param>
    /// <param name="configure">配置委托</param>
    /// <returns>导出的配置字符串</returns>
    /// <example>
    /// <code>
    /// var json = cfg.ExportSnapshot(options =>
    /// {
    ///     options.IncludeMetadata = true;
    ///     options.ExcludeKeys = new[] { "Secrets:*" };
    /// });
    /// </code>
    /// </example>
    public static string ExportSnapshot(this ICfgRoot cfg, Action<ExportOptions> configure)
    {
        var options = new ExportOptions();
        configure(options);
        return ConfigExporter.Export(cfg, options);
    }

    #endregion
}
