using Apq.Cfg.WebApi.Models;

namespace Apq.Cfg.WebApi.Services;

/// <summary>
/// 配置 API 服务接口
/// </summary>
public interface IConfigApiService
{
    // ========== 合并后配置（Merged）==========

    /// <summary>
    /// 获取合并后的所有配置（扁平化键值对）
    /// </summary>
    Dictionary<string, string?> GetMergedConfig();

    /// <summary>
    /// 获取合并后的配置树结构
    /// </summary>
    ConfigTreeNode GetMergedTree();

    /// <summary>
    /// 获取合并后的单个配置值
    /// </summary>
    ConfigValueResponse GetMergedValue(string key);

    /// <summary>
    /// 获取合并后的配置节
    /// </summary>
    Dictionary<string, string?> GetMergedSection(string section);

    // ========== 合并前配置（Sources）==========

    /// <summary>
    /// 获取所有配置源列表
    /// </summary>
    List<ConfigSourceInfo> GetSources();

    /// <summary>
    /// 获取指定配置源的内容
    /// </summary>
    Dictionary<string, string?>? GetSourceConfig(int level, string name);

    /// <summary>
    /// 获取指定配置源的配置树
    /// </summary>
    ConfigTreeNode? GetSourceTree(int level, string name);

    /// <summary>
    /// 获取指定配置源的单个配置值
    /// </summary>
    ConfigValueResponse? GetSourceValue(int level, string name, string key);

    // ========== 写入操作 ==========

    /// <summary>
    /// 设置配置值（写入主写入源）
    /// </summary>
    bool SetValue(string key, string? value);

    /// <summary>
    /// 设置指定配置源的配置值
    /// </summary>
    bool SetSourceValue(int level, string name, string key, string? value);

    /// <summary>
    /// 批量设置配置
    /// </summary>
    bool BatchUpdate(BatchUpdateRequest request);

    /// <summary>
    /// 删除配置
    /// </summary>
    bool DeleteKey(string key);

    /// <summary>
    /// 删除指定配置源的配置
    /// </summary>
    bool DeleteSourceKey(int level, string name, string key);

    // ========== 管理操作 ==========

    /// <summary>
    /// 保存配置到持久化存储
    /// </summary>
    Task<bool> SaveAsync();

    /// <summary>
    /// 重新加载配置
    /// </summary>
    bool Reload();

    /// <summary>
    /// 导出配置
    /// </summary>
    string Export(string format, int? level = null, string? name = null);
}
