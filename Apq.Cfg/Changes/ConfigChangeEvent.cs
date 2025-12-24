namespace Apq.Cfg.Changes;

/// <summary>
/// 配置变更事件
/// </summary>
public sealed class ConfigChangeEvent
{
    /// <summary>
    /// 变更的配置项集合
    /// </summary>
    public IReadOnlyDictionary<string, ConfigChange> Changes { get; }

    /// <summary>
    /// 变更发生的时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    public ConfigChangeEvent(IReadOnlyDictionary<string, ConfigChange> changes, DateTimeOffset timestamp)
    {
        Changes = changes;
        Timestamp = timestamp;
    }

    public ConfigChangeEvent(IReadOnlyDictionary<string, ConfigChange> changes)
        : this(changes, DateTimeOffset.Now)
    {
    }
}
