using Apq.Cfg.Changes;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 配置根接口，提供统一的配置访问和管理功能
/// </summary>
public interface ICfgRoot : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// 通过索引器获取或设置配置值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>配置值，不存在时返回null</returns>
    /// <example>
    /// <code>
    /// // 读取配置
    /// var name = cfg["App:Name"];
    ///
    /// // 写入配置
    /// cfg["App:Name"] = "NewName";
    /// </code>
    /// </example>
    string? this[string key] { get; set; }

    /// <summary>
    /// 获取配置值并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="key">配置键</param>
    /// <returns>转换后的配置值，不存在或转换失败时返回默认值</returns>
    /// <example>
    /// <code>
    /// var port = cfg.GetValue&lt;int&gt;("Server:Port");
    /// var enabled = cfg.GetValue&lt;bool&gt;("Features:NewUI");
    /// </code>
    /// </example>
    T? GetValue<T>(string key);

    /// <summary>
    /// 检查配置键是否存在
    /// </summary>
    /// <param name="key">配置键</param>
    /// <returns>存在返回true，否则返回false</returns>
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

    /// <summary>
    /// 设置配置值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <param name="targetLevel">目标层级，为null时使用可写的最高层级</param>
    /// <example>
    /// <code>
    /// cfg.SetValue("Server:Port", "8080");
    /// cfg.SetValue("Features:NewUI", "true");
    /// </code>
    /// </example>
    void SetValue(string key, string? value, int? targetLevel = null);

    /// <summary>
    /// 移除配置键
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="targetLevel">目标层级，为null时从所有层级移除</param>
    void Remove(string key, int? targetLevel = null);

    /// <summary>
    /// 保存配置更改到持久化存储
    /// </summary>
    /// <param name="targetLevel">目标层级，为null时保存所有可写层级</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <example>
    /// <code>
    /// await cfg.SaveAsync();
    /// // 或指定特定层级
    /// await cfg.SaveAsync(targetLevel: 1);
    /// </code>
    /// </example>
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
    /// 高性能批量获取：通过回调方式返回结果，零堆分配
    /// </summary>
    /// <param name="keys">要获取的键集合</param>
    /// <param name="onValue">每个键值对的回调处理函数</param>
    /// <remarks>
    /// 此方法避免了 Dictionary 分配开销，适合高频调用场景。
    /// 回调会按键的顺序依次调用。
    /// </remarks>
    void GetMany(IEnumerable<string> keys, Action<string, string?> onValue);

    /// <summary>
    /// 高性能批量获取：通过回调方式返回结果并转换类型，零堆分配
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="keys">要获取的键集合</param>
    /// <param name="onValue">每个键值对的回调处理函数</param>
    /// <remarks>
    /// 此方法避免了 Dictionary 分配开销，适合高频调用场景。
    /// 回调会按键的顺序依次调用。
    /// </remarks>
    void GetMany<T>(IEnumerable<string> keys, Action<string, T?> onValue);

    /// <summary>
    /// 批量设置多个配置值，减少锁竞争
    /// </summary>
    /// <param name="values">要设置的键值对</param>
    /// <param name="targetLevel">目标层级</param>
    void SetManyValues(IEnumerable<KeyValuePair<string, string?>> values, int? targetLevel = null);

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
