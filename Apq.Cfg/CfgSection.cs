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

    /// <inheritdoc />
    public string? this[string key]
    {
        get => _root[GetFullKey(key)];
        set => SetValue(key, value);
    }

    /// <summary>
    /// 组合完整的配置键
    /// </summary>
    private string GetFullKey(string key)
    {
        return string.IsNullOrEmpty(Path) ? key : $"{Path}:{key}";
    }

    public T? GetValue<T>(string key)
    {
        return _root.GetValue<T>(GetFullKey(key));
    }

    public bool Exists(string key)
    {
        return _root.Exists(GetFullKey(key));
    }

    public void SetValue(string key, string? value, int? targetLevel = null)
    {
        _root.SetValue(GetFullKey(key), value, targetLevel);
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
