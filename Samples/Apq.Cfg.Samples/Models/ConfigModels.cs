namespace Apq.Cfg.Samples.Models;

/// <summary>
/// 数据库配置选项
/// </summary>
public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// 日志配置选项
/// </summary>
public class LoggingOptions
{
    public string? Level { get; set; }
    public bool EnableConsole { get; set; }
}

/// <summary>
/// 用于类型转换示例的日志级别枚举
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}
