using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 配置根接口，提供统一的配置访问和管理功能
/// </summary>
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    // 读取方法
    string? Get(string key);
    T? Get<T>(string key);
    bool Exists(string key);

    // 写入方法
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default);

    // 转换方法
    /// <summary>
    /// 转换为 Microsoft Configuration（静态快照）
    /// </summary>
    IConfigurationRoot ToMicrosoftConfiguration();

    /// <summary>
    /// 转换为支持动态重载的 Microsoft Configuration
    /// </summary>
    /// <param name="options">动态重载选项，为 null 时使用默认选项</param>
    IConfigurationRoot ToMicrosoftConfiguration(DynamicReloadOptions? options);

    /// <summary>
    /// 获取配置变更的可观察序列
    /// </summary>
    IObservable<ConfigChangeEvent> ConfigChanges { get; }
}
