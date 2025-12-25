namespace Apq.Cfg.Changes;

/// <summary>
/// 配置重载策略
/// </summary>
public enum ReloadStrategy
{
    /// <summary>
    /// 立即重载：文件变更后自动重载（默认行为）
    /// </summary>
    Eager,

    /// <summary>
    /// 延迟重载：访问配置时才检查并重载
    /// </summary>
    Lazy,

    /// <summary>
    /// 手动重载：只有调用 Reload() 方法时才重载
    /// </summary>
    Manual
}
