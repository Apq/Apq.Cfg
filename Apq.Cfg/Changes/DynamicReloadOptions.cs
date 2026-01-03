namespace Apq.Cfg.Changes;

/// <summary>
/// 动态重载选项
/// </summary>
public sealed class DynamicReloadOptions
{
    /// <summary>
    /// 防抖时间窗口（毫秒），默认 100ms
    /// </summary>
    public int DebounceMs { get; set; } = 100;

    /// <summary>
    /// 是否启用动态重载，默认 true
    /// </summary>
    public bool EnableDynamicReload { get; set; } = true;

    /// <summary>
    /// 重载策略，默认 Eager（立即重载）
    /// </summary>
    public ReloadStrategy Strategy { get; set; } = ReloadStrategy.Eager;

    /// <summary>
    /// 键前缀过滤器，只监听匹配这些前缀的键变更（null 或空表示监听所有）
    /// </summary>
    public IReadOnlyList<string>? KeyPrefixFilters { get; set; }

    /// <summary>
    /// 重载失败时是否回滚到之前的配置，默认 true
    /// </summary>
    public bool RollbackOnError { get; set; } = true;

    /// <summary>
    /// 保留的变更历史记录数量（0 表示不记录），默认 0
    /// </summary>
    public int HistorySize { get; set; } = 0;
}
