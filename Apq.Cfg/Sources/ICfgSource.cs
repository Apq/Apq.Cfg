using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Sources;

/// <summary>
/// 配置源接口
/// </summary>
public interface ICfgSource
{
    int Level { get; }
    bool IsWriteable { get; }
    bool IsPrimaryWriter { get; }

    IConfigurationSource BuildSource();
}

/// <summary>
/// 可写配置源接口
/// </summary>
public interface IWritableCfgSource : ICfgSource
{
    Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken);
}