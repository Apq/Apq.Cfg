using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Properties;

internal sealed class PropertiesFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public PropertiesFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, string? name = null)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, name: name)
    {
    }

    public override IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        if (!File.Exists(_path))
            return Enumerable.Empty<KeyValuePair<string, string?>>();

        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var readEncoding = DetectEncodingEnhanced(_path);
        var lines = File.ReadAllLines(_path, readEncoding);
        string? currentSection = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#') || trimmed.StartsWith('!'))
                continue;

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentSection = trimmed.Substring(1, trimmed.Length - 2);
                continue;
            }

            var idx = FindKeyValueSeparator(line);
            if (idx > 0)
            {
                var key = line.Substring(0, idx).Trim();
                var value = UnescapeValue(line.Substring(idx + 1).Trim());
                var configKey = string.IsNullOrEmpty(currentSection) ? key : $"{currentSection}.{key}";
                data[configKey] = value;
            }
        }

        return data;
    }

    private static int FindKeyValueSeparator(string line)
    {
        var inEscape = false;
        var firstColon = -1;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '\\' && !inEscape)
            {
                inEscape = true;
                continue;
            }
            if (c == '=')
            {
                if (i > 0 && line[i - 1] == '\\')
                    continue;
                return i;
            }
            if (c == ':' && firstColon < 0)
            {
                if (i > 0 && line[i - 1] == '\\')
                    continue;
                firstColon = i;
            }
            inEscape = false;
        }
        return firstColon;
    }

    private static string UnescapeValue(string value)
    {
        var sb = new System.Text.StringBuilder();
        var inEscape = false;

        foreach (var c in value)
        {
            if (inEscape)
            {
                sb.Append(c switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    _ => c
                });
                inEscape = false;
            }
            else if (c == '\\')
            {
                inEscape = true;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new PropertiesSource
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
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#') || trimmed.StartsWith('!'))
                    continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    if (!sections.ContainsKey(currentSection))
                        sections[currentSection] = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                    continue;
                }

                var idx = FindKeyValueSeparator(line);
                if (idx > 0)
                {
                    var key = line.Substring(0, idx).Trim();
                    var value = UnescapeValue(line.Substring(idx + 1).Trim());
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
                sections[section][key] = EscapeValue(value);
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

    private static string EscapeValue(string value)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var c in value)
        {
            if (c == '\\')
                sb.Append("\\\\");
            else if (c == '\n')
                sb.Append("\\n");
            else if (c == '\r')
                sb.Append("\\r");
            else if (c == '\t')
                sb.Append("\\t");
            else if (c == '#' || c == '!')
                sb.Append('\\').Append(c);
            else
                sb.Append(c);
        }

        return sb.ToString();
    }

    private sealed class PropertiesSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new PropertiesProvider(this);
        }
    }

    private sealed class PropertiesProvider : FileConfigurationProvider
    {
        public PropertiesProvider(PropertiesSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true);
            var lines = reader.ReadToEnd().Split('\n');
            string? currentSection = null;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#') || trimmed.StartsWith('!'))
                    continue;

                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2);
                    continue;
                }

                var idx = FindKeyValueSeparator(line);
                if (idx > 0)
                {
                    var key = line.Substring(0, idx).Trim();
                    var value = UnescapeValue(line.Substring(idx + 1).Trim());
                    var configKey = string.IsNullOrEmpty(currentSection) ? key : $"{currentSection}.{key}";
                    data[configKey] = value;
                }
            }

            Data = data;
        }

        private static int FindKeyValueSeparator(string line)
        {
            var inEscape = false;
            var firstColon = -1;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '\\' && !inEscape)
                {
                    inEscape = true;
                    continue;
                }
                if (c == '=')
                {
                    if (i > 0 && line[i - 1] == '\\')
                        continue;
                    return i;
                }
                if (c == ':' && firstColon < 0)
                {
                    if (i > 0 && line[i - 1] == '\\')
                        continue;
                    firstColon = i;
                }
                inEscape = false;
            }
            return firstColon;
        }

        private static string UnescapeValue(string value)
        {
            var sb = new System.Text.StringBuilder();
            var inEscape = false;

            foreach (var c in value)
            {
                if (inEscape)
                {
                    sb.Append(c switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        'r' => '\r',
                        '\\' => '\\',
                        _ => c
                    });
                    inEscape = false;
                }
                else if (c == '\\')
                {
                    inEscape = true;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
