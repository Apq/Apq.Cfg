namespace Apq.Cfg.Changes;

/// <summary>
/// 配置变更事件
/// </summary>
public sealed class ConfigChangeEvent
{
    /// <summary>
    /// 变更批次的唯一标识符，便于追踪和日志记录
    /// </summary>
    public Guid BatchId { get; }

    /// <summary>
    /// 变更的配置项集合
    /// </summary>
    public IReadOnlyDictionary<string, ConfigChange> Changes { get; }

    /// <summary>
    /// 变更发生的时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    public ConfigChangeEvent(IReadOnlyDictionary<string, ConfigChange> changes, DateTimeOffset timestamp, Guid? batchId = null)
    {
        BatchId = batchId ?? Guid.NewGuid();
        Changes = changes;
        Timestamp = timestamp;
    }

    public ConfigChangeEvent(IReadOnlyDictionary<string, ConfigChange> changes)
        : this(changes, DateTimeOffset.Now, Guid.NewGuid())
    {
    }
}
