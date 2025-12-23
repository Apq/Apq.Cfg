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
}
