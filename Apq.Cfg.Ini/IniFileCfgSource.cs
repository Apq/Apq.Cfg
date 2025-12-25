using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.FileProviders;

namespace Apq.Cfg.Ini;

internal sealed class IniFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public IniFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

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
