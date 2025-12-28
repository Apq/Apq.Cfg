using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.FileProviders;

namespace Apq.Cfg.Ini;

/// <summary>
/// INI 文件配置源，支持读取和写入 INI 格式的配置文件
/// </summary>
internal sealed class IniFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    /// <summary>
    /// 初始化 IniFileCfgSource 实例
    /// </summary>
    /// <param name="path">INI 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">是否为可选文件</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    public IniFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的 INI 配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.Ini.IniConfigurationSource 实例</returns>
    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new IniConfigurationSource
        {
            FileProvider = fp,
            Path = file,
            Optional = _optional,
            ReloadOnChange = _reloadOnChange
        };
        src.ResolveFileProvider();
        return src;
    }

    /// <summary>
    /// 应用配置更改到 INI 文件
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="InvalidOperationException">当配置源不可写时抛出</exception>
    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        if (!IsWriteable)
            throw new InvalidOperationException($"配置源 (层级 {Level}) 不可写");

        EnsureDirectoryFor(_path);

        var sections = new Dictionary<string, Dictionary<string, string?>>(StringComparer.OrdinalIgnoreCase);
        string? currentSection = null;

        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            var lines = await File.ReadAllLinesAsync(_path, readEncoding, cancellationToken).ConfigureAwait(false);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(";")) continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    if (!sections.ContainsKey(currentSection))
                        sections[currentSection] = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                var idx = trimmed.IndexOf('=');
                if (idx > 0)
                {
                    var key = trimmed.Substring(0, idx).Trim();
                    var value = trimmed.Substring(idx + 1);

                    var section = currentSection ?? "";
                    if (!sections.ContainsKey(section))
                        sections[section] = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

                    sections[section][key] = value;
                }
            }
        }

        foreach (var (configKey, value) in changes)
        {
            var colonIdx = configKey.IndexOf(':');
            string section, key;

            if (colonIdx > 0)
            {
                section = configKey.Substring(0, colonIdx);
                key = configKey.Substring(colonIdx + 1);
            }
            else
            {
                section = "";
                key = configKey;
            }

            if (!sections.ContainsKey(section))
                sections[section] = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                sections[section].Remove(key);
            else
                sections[section][key] = value;
        }

        var sb = new System.Text.StringBuilder();

        if (sections.TryGetValue("", out var rootKeys) && rootKeys.Count > 0)
        {
            foreach (var kv in rootKeys)
                sb.Append(kv.Key).Append('=').Append(kv.Value).AppendLine();
        }

        foreach (var (sectionName, keys) in sections)
        {
            if (string.IsNullOrEmpty(sectionName) || keys.Count == 0) continue;

            if (sb.Length > 0) sb.AppendLine();
            sb.Append('[').Append(sectionName).Append(']').AppendLine();

            foreach (var kv in keys)
                sb.Append(kv.Key).Append('=').Append(kv.Value).AppendLine();
        }

        await File.WriteAllTextAsync(_path, sb.ToString(), GetWriteEncoding(), cancellationToken).ConfigureAwait(false);
    }
}
