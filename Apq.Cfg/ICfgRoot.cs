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

    /// <summary>
    /// 获取配置节
    /// </summary>
    /// <param name="key">节的键名（如 "Database"）</param>
    /// <returns>配置节对象</returns>
    ICfgSection GetSection(string key);

    /// <summary>
    /// 获取所有顶级配置键
    /// </summary>
    IEnumerable<string> GetChildKeys();

    // 写入方法
    void Set(string key, string? value, int? targetLevel = null);
    void Remove(string key, int? targetLevel = null);
    Task SaveAsync(int? targetLevel = null, CancellationToken cancellationToken = default);

    // 批量操作方法
    /// <summary>
    /// 批量获取多个配置值，减少锁竞争
    /// </summary>
    /// <param name="keys">要获取的键集合</param>
    /// <returns>键值对字典</returns>
    IReadOnlyDictionary<string, string?> GetMany(IEnumerable<string> keys);

    /// <summary>
    /// 批量获取多个配置值并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="keys">要获取的键集合</param>
    /// <returns>键值对字典</returns>
    IReadOnlyDictionary<string, T?> GetMany<T>(IEnumerable<string> keys);

    /// <summary>
    /// 批量设置多个配置值，减少锁竞争
    /// </summary>
    /// <param name="values">要设置的键值对</param>
    /// <param name="targetLevel">目标层级</param>
    void SetMany(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);

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
