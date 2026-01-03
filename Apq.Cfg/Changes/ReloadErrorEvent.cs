namespace Apq.Cfg.Changes;

/// <summary>
/// 配置重载错误事件
/// </summary>
public sealed class ReloadErrorEvent
{
    /// <summary>
    /// 错误发生的时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// 发生错误的配置源层级
    /// </summary>
    public IReadOnlySet<int> AffectedLevels { get; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// 是否已回滚到之前的配置
    /// </summary>
    public bool RolledBack { get; }

    public ReloadErrorEvent(IReadOnlySet<int> affectedLevels, Exception exception, bool rolledBack)
    {
        Timestamp = DateTimeOffset.Now;
        AffectedLevels = affectedLevels;
        Exception = exception;
        RolledBack = rolledBack;
    }
}
