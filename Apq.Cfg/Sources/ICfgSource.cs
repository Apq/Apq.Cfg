using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Sources;

/// <summary>
/// 配置源接口，定义了配置源的基本行为
/// </summary>
public interface ICfgSource
{
    /// <summary>
    /// 获取配置层级，数值越大优先级越高
    /// </summary>
    int Level { get; }

    /// <summary>
    /// 获取是否可写，指示该配置源是否支持写入操作
    /// </summary>
    bool IsWriteable { get; }

    /// <summary>
    /// 获取是否为主要写入源，用于标识当多个可写源存在时的主要写入目标
    /// </summary>
    bool IsPrimaryWriter { get; }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.IConfigurationSource 实例</returns>
    IConfigurationSource BuildSource();
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