namespace Apq.Cfg.Changes;

/// <summary>
/// 配置变更事件
/// </summary>
public sealed class ConfigChangeEvent
{
    /// <summary>
    /// 变更的配置项集合
    /// </summary>
    public IReadOnlyDictionary<string, ConfigChange> Changes { get; init; } = new Dictionary<string, ConfigChange>();

    /// <summary>
    /// 变更发生的时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}
