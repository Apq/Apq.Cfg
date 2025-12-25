using Apq.Cfg.Internal;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg;

/// <summary>
/// 配置节实现
/// </summary>
internal sealed class CfgSection : ICfgSection
{
    private readonly ICfgRoot _root;
    private readonly IConfigurationRoot _msConfig;

    public CfgSection(ICfgRoot root, IConfigurationRoot msConfig, string path)
    {
        _root = root;
        _msConfig = msConfig;
        Path = path;
    }

    public string Path { get; }

    /// <summary>
    /// 组合完整的配置键
    /// </summary>
    private string GetFullKey(string key)
    {
        return string.IsNullOrEmpty(Path) ? key : $"{Path}:{key}";
    }

    public string? Get(string key)
    {
        return _root.Get(GetFullKey(key));
    }

    public T? Get<T>(string key)
    {
        return _root.Get<T>(GetFullKey(key));
    }

    public bool Exists(string key)
    {
        return _root.Exists(GetFullKey(key));
    }

    public void Set(string key, string? value, int? targetLevel = null)
    {
        _root.Set(GetFullKey(key), value, targetLevel);
    }

    public void Remove(string key, int? targetLevel = null)
    {
        _root.Remove(GetFullKey(key), targetLevel);
    }

    public ICfgSection GetSection(string key)
    {
        return new CfgSection(_root, _msConfig, GetFullKey(key));
    }

    public IEnumerable<string> GetChildKeys()
    {
        var section = _msConfig.GetSection(Path);
        return section.GetChildren().Select(c => c.Key);
    }
}
