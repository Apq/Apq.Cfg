using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Sources;

/// <summary>
/// 配置源接口，定义了配置源的基本行为
/// </summary>
public interface ICfgSource
{
    /// <summary>
    /// 获取或设置配置源名称（同一层级内唯一）
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 获取配置层级，数值越大优先级越高
    /// </summary>
    int Level { get; }

    /// <summary>
    /// 获取配置源类型名称
    /// </summary>
    string Type { get; }

    /// <summary>
    /// 获取是否可写，指示该配置源是否支持写入操作
    /// </summary>
    bool IsWriteable { get; }

    /// <summary>
    /// 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标
    /// </summary>
    bool IsPrimaryWriter { get; }

    /// <summary>
    /// 获取配置项数量（所有叶子节点的总数）
    /// </summary>
    int KeyCount { get; }

    /// <summary>
    /// 获取顶级配置键数量（只统计第一层节点）
    /// </summary>
    int TopLevelKeyCount { get; }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.IConfigurationSource 实例</returns>
    IConfigurationSource BuildSource();

    /// <summary>
    /// 获取该配置源的所有配置值
    /// </summary>
    /// <returns>配置键值对集合</returns>
    IEnumerable<KeyValuePair<string, string?>> GetAllValues();
}

/// <summary>
/// 文件配置源接口，用于标识基于文件的配置源
/// </summary>
public interface IFileCfgSource : ICfgSource
{
    /// <summary>
    /// 获取文件路径
    /// </summary>
    string FilePath { get; }
}

/// <summary>
/// 可写配置源接口，继承自 ICfgSource，增加了写入配置的能力
/// </summary>
public interface IWritableCfgSource : ICfgSource
{
    /// <summary>
    /// 应用配置更改
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken);
}

/// <summary>
/// ICfgSource 扩展方法
/// </summary>
public static class CfgSourceExtensions
{
    /// <summary>
    /// 将配置源转换为可序列化的信息对象
    /// </summary>
    public static ConfigSourceInfo ToInfo(this ICfgSource source)
    {
        return new ConfigSourceInfo
        {
            Level = source.Level,
            Name = source.Name,
            Type = source.Type,
            IsWriteable = source.IsWriteable,
            IsPrimaryWriter = source.IsPrimaryWriter,
            KeyCount = source.KeyCount,
            TopLevelKeyCount = source.TopLevelKeyCount
        };
    }
}