using System.Collections;
using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Microsoft.Extensions.Configuration;

namespace Apq.Cfg.Hcl;

internal sealed class HclFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public HclFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
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
        var text = File.ReadAllText(_path, readEncoding);
        var config = ConfigurationFactory.ParseString(text);

        void Traverse(HoconValue value, string prefix)
        {
            if (value.IsObject())
            {
                var obj = value.GetObject();
                foreach (var key in obj.Items.Keys)
                {
                    var newPrefix = string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;
                    Traverse(obj.GetKey(key), newPrefix);
                }
            }
            else if (value.IsArray())
            {
                var arr = value.GetArray();
                for (var i = 0; i < arr.Count; i++)
                {
                    Traverse(arr[i], prefix + ":" + i);
                }
            }
            else
            {
                data[prefix] = value.GetString();
            }
        }

        Traverse(config.Root, "");
        return data;
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new HoconSource
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

        var allValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            var text = File.ReadAllText(_path, readEncoding);
            var config = ConfigurationFactory.ParseString(text);

            void Traverse(HoconValue value, string prefix)
            {
                if (value.IsObject())
                {
                    var obj = value.GetObject();
                    foreach (var key in obj.Items.Keys)
                    {
                        var newPrefix = string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;
                        Traverse(obj.GetKey(key), newPrefix);
                    }
                }
                else if (value.IsArray())
                {
                    var arr = value.GetArray();
                    for (var i = 0; i < arr.Count; i++)
                    {
                        Traverse(arr[i], prefix + "." + i);
                    }
                }
                else
                {
                    allValues[prefix] = value.GetString();
                }
            }

            Traverse(config.Root, "");
        }

        foreach (var (key, value) in changes)
        {
            allValues[key] = value;
        }

        var sb = new System.Text.StringBuilder();
        WriteDictionaryToHocon(allValues, sb, "");
        await File.WriteAllTextAsync(_path, sb.ToString(), GetWriteEncoding(), cancellationToken).ConfigureAwait(false);
    }

    private static void WriteDictionaryToHocon(Dictionary<string, string?> data, System.Text.StringBuilder sb, string prefix)
    {
        var grouped = new Dictionary<string, Dictionary<string, string?>>();
        var leafValues = new Dictionary<string, string?>();

        foreach (var kvp in data)
        {
            if (kvp.Value == null)
                continue;

            if (kvp.Key.StartsWith(prefix))
            {
                var relativeKey = prefix.Length > 0 ? kvp.Key.Substring(prefix.Length + 1) : kvp.Key;
                var parts = relativeKey.Split('.', 2);

                if (parts.Length == 1)
                {
                    leafValues[parts[0]] = kvp.Value;
                }
                else
                {
                    if (!grouped.ContainsKey(parts[0]))
                    {
                        grouped[parts[0]] = new Dictionary<string, string?>();
                    }
                    grouped[parts[0]][prefix.Length > 0 ? prefix + "." + parts[0] + "." + parts[1] : parts[0] + "." + parts[1]] = kvp.Value;
                }
            }
        }

        foreach (var kvp in leafValues)
        {
            sb.Append(kvp.Key).Append(" = ").AppendLine(kvp.Value ?? "");
        }

        foreach (var kvp in grouped)
        {
            sb.AppendLine(kvp.Key + " {");
            WriteDictionaryToHocon(kvp.Value, sb, prefix.Length > 0 ? prefix + "." + kvp.Key : kvp.Key);
            sb.AppendLine("}");
        }
    }

    private sealed class HoconSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new HoconProvider(this);
        }
    }

    private sealed class HoconProvider : FileConfigurationProvider
    {
        public HoconProvider(HoconSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true);
            var text = reader.ReadToEnd();
            try
            {
                var config = ConfigurationFactory.ParseString(text);

                void Traverse(HoconValue value, string prefix)
                {
                    if (value.IsObject())
                    {
                        var obj = value.GetObject();
                        foreach (var key in obj.Items.Keys)
                        {
                            var newPrefix = string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;
                            Traverse(obj.GetKey(key), newPrefix);
                        }
                    }
                    else if (value.IsArray())
                    {
                        var arr = value.GetArray();
                        for (var i = 0; i < arr.Count; i++)
                        {
                            Traverse(arr[i], prefix + "." + i);
                        }
                    }
                    else
                    {
                        data[prefix] = value.GetString();
                    }
                }

                Traverse(config.Root, "");
            }
            catch
            {
            }
            Data = data;
        }
    }
}
