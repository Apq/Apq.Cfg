using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Apq.Cfg.Sources;
using Apq.Cfg.WebApi.Models;
using Microsoft.Extensions.Options;

namespace Apq.Cfg.WebApi.Services;

/// <summary>
/// 配置 API 服务实现
/// </summary>
public sealed class ConfigApiService : IConfigApiService
{
    private readonly ICfgRoot _cfgRoot;
    private readonly WebApiOptions _options;

    public ConfigApiService(ICfgRoot cfgRoot, IOptions<WebApiOptions> options)
    {
        _cfgRoot = cfgRoot;
        _options = options.Value;
    }

    // ========== 合并后配置（Merged）==========

    public Dictionary<string, string?> GetMergedConfig()
    {
        var result = new Dictionary<string, string?>();
        var msConfig = _cfgRoot.ToMicrosoftConfiguration();
        CollectAllValues(msConfig, string.Empty, result);
        return result;
    }

    public ConfigTreeNode GetMergedTree()
    {
        var values = GetMergedConfig();
        return BuildTree(values);
    }

    public ConfigValueResponse GetMergedValue(string key)
    {
        var value = _cfgRoot[key];
        var isSensitive = IsSensitiveKey(key);
        return new ConfigValueResponse
        {
            Key = key,
            Value = isSensitive && value != null ? "***" : value,
            Exists = value != null,
            IsMasked = isSensitive && value != null
        };
    }

    public Dictionary<string, string?> GetMergedSection(string section)
    {
        var result = new Dictionary<string, string?>();
        var msConfig = _cfgRoot.ToMicrosoftConfiguration();
        var sectionConfig = msConfig.GetSection(section);
        CollectAllValues(sectionConfig, section, result);
        return result;
    }

    // ========== 合并前配置（Sources）==========

    public List<ConfigSourceInfo> GetSources()
    {
        return _cfgRoot.GetSources().Select(s => s.ToInfo()).ToList();
    }

    public Dictionary<string, string?>? GetSourceConfig(int level, string name)
    {
        var source = _cfgRoot.GetSource(level, name);
        if (source == null) return null;

        var result = new Dictionary<string, string?>();
        foreach (var kvp in source.GetAllValues())
        {
            result[kvp.Key] = MaskIfSensitive(kvp.Key, kvp.Value);
        }
        return result;
    }

    public ConfigTreeNode? GetSourceTree(int level, string name)
    {
        var config = GetSourceConfig(level, name);
        if (config == null) return null;
        return BuildTree(config);
    }

    public ConfigValueResponse? GetSourceValue(int level, string name, string key)
    {
        var source = _cfgRoot.GetSource(level, name);
        if (source == null) return null;

        var values = source.GetAllValues().ToDictionary(x => x.Key, x => x.Value);
        var exists = values.TryGetValue(key, out var value);
        var isSensitive = IsSensitiveKey(key);

        return new ConfigValueResponse
        {
            Key = key,
            Value = isSensitive && value != null ? "***" : value,
            Exists = exists,
            IsMasked = isSensitive && value != null
        };
    }

    // ========== 写入操作 ==========

    public bool SetValue(string key, string? value)
    {
        try
        {
            _cfgRoot.SetValue(key, value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool SetSourceValue(int level, string name, string key, string? value)
    {
        try
        {
            var source = _cfgRoot.GetSource(level, name);
            if (source == null) return false;

            _cfgRoot.SetValue(key, value, level);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool BatchUpdate(BatchUpdateRequest request)
    {
        try
        {
            if (request.TargetLevel.HasValue)
            {
                _cfgRoot.SetManyValues(request.Values, request.TargetLevel);
            }
            else
            {
                _cfgRoot.SetManyValues(request.Values);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteKey(string key)
    {
        try
        {
            _cfgRoot.Remove(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteSourceKey(int level, string name, string key)
    {
        try
        {
            var source = _cfgRoot.GetSource(level, name);
            if (source == null) return false;

            _cfgRoot.Remove(key, level);
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ========== 管理操作 ==========

    public async Task<bool> SaveAsync()
    {
        try
        {
            await _cfgRoot.SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool Reload()
    {
        try
        {
            _cfgRoot.Reload();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string Export(string format, int? level = null, string? name = null)
    {
        Dictionary<string, string?> values;

        if (level.HasValue && name != null)
        {
            values = GetSourceConfig(level.Value, name) ?? new();
        }
        else
        {
            values = GetMergedConfig();
        }

        return format.ToLowerInvariant() switch
        {
            "json" => ExportAsJson(values),
            "env" => ExportAsEnv(values),
            "keyvalue" or "kv" => ExportAsKeyValue(values),
            _ => ExportAsJson(values)
        };
    }

    // ========== 辅助方法 ==========

    private bool IsSensitiveKey(string key)
    {
        if (!_options.MaskSensitiveValues) return false;

        return _options.SensitiveKeyPatterns.Any(pattern =>
            MatchWildcard(key, pattern));
    }

    private string? MaskIfSensitive(string key, string? value)
    {
        if (value == null || !IsSensitiveKey(key)) return value;
        return "***";
    }

    private static bool MatchWildcard(string text, string pattern)
    {
        var regex = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
    }

    private ConfigTreeNode BuildTree(Dictionary<string, string?> values)
    {
        var root = new ConfigTreeNode { Key = "" };

        foreach (var (key, value) in values)
        {
            var parts = key.Split(':');
            var current = root;

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var child = current.Children.FirstOrDefault(c => c.Key == part);

                if (child == null)
                {
                    child = new ConfigTreeNode { Key = part };
                    current.Children.Add(child);
                }

                if (i == parts.Length - 1)
                {
                    var isSensitive = IsSensitiveKey(key);
                    child.Value = isSensitive && value != null ? "***" : value;
                    child.HasValue = true;
                    child.IsMasked = isSensitive && value != null;
                }

                current = child;
            }
        }

        return root;
    }

    private static void CollectAllValues(Microsoft.Extensions.Configuration.IConfiguration config, string parentPath, Dictionary<string, string?> result)
    {
        foreach (var child in config.GetChildren())
        {
            var fullKey = string.IsNullOrEmpty(parentPath) ? child.Key : $"{parentPath}:{child.Key}";

            if (child.Value != null)
            {
                result[fullKey] = child.Value;
            }

            CollectAllValues(child, fullKey, result);
        }
    }

    private static string ExportAsJson(Dictionary<string, string?> values)
    {
        // 构建嵌套的 JSON 结构
        var root = new Dictionary<string, object?>();

        foreach (var (key, value) in values)
        {
            var parts = key.Split(':');
            var current = root;

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];

                if (i == parts.Length - 1)
                {
                    current[part] = value;
                }
                else
                {
                    if (!current.TryGetValue(part, out var next) || next is not Dictionary<string, object?> nextDict)
                    {
                        nextDict = new Dictionary<string, object?>();
                        current[part] = nextDict;
                    }
                    current = (Dictionary<string, object?>)current[part]!;
                }
            }
        }

        return JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string ExportAsEnv(Dictionary<string, string?> values)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in values)
        {
            var envKey = key.Replace(":", "__").ToUpperInvariant();
            sb.AppendLine($"{envKey}={value}");
        }
        return sb.ToString();
    }

    private static string ExportAsKeyValue(Dictionary<string, string?> values)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in values)
        {
            sb.AppendLine($"{key}={value}");
        }
        return sb.ToString();
    }
}
