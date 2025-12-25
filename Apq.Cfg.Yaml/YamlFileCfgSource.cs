using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace Apq.Cfg.Yaml;

internal sealed class YamlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public YamlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new YamlSource
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

        var yaml = new YamlStream();
        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            using var sr = new StreamReader(_path, readEncoding, true);
            yaml.Load(sr);
        }
        else
        {
            yaml.Add(new YamlDocument(new YamlMappingNode()));
        }

        var root = yaml.Documents.Count > 0 ? (YamlMappingNode)yaml.Documents[0].RootNode : new YamlMappingNode();

        foreach (var (key, val) in changes)
            SetYamlByColonKey(root, key, val);

        using var writer = new StreamWriter(_path, false, GetWriteEncoding());
        yaml.Save(writer, false);
        await writer.FlushAsync().ConfigureAwait(false);
    }

    private static void SetYamlByColonKey(YamlMappingNode root, string key, string? value)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var current = root;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (i == parts.Length - 1)
            {
                current.Children[new YamlScalarNode(part)] = new YamlScalarNode(value ?? string.Empty);
            }
            else
            {
                if (!current.Children.TryGetValue(new YamlScalarNode(part), out var child) ||
                    child is not YamlMappingNode childMap)
                {
                    childMap = new YamlMappingNode();
                    current.Children[new YamlScalarNode(part)] = childMap;
                }

                current = childMap;
            }
        }
    }

    private sealed class YamlSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new YamlProvider(this);
        }
    }

    private sealed class YamlProvider : FileConfigurationProvider
    {
        public YamlProvider(YamlSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true);
            var yaml = new YamlStream();
            yaml.Load(reader);
            if (yaml.Documents.Count == 0)
            {
                Data = data;
                return;
            }

            var root = (YamlMappingNode)yaml.Documents[0].RootNode;
            VisitNode(root, null, data);
            Data = data;
        }

        private static void VisitNode(YamlNode node, string? prefix, IDictionary<string, string?> data)
        {
            switch (node)
            {
                case YamlMappingNode map:
                    foreach (var kv in map.Children)
                        VisitNode(kv.Value, CombineKey(prefix, kv.Key.ToString()), data);
                    break;
                case YamlSequenceNode seq:
                    var idx = 0;
                    foreach (var item in seq.Children)
                        VisitNode(item, CombineKey(prefix, (idx++).ToString()), data);
                    break;
                default:
                    data[prefix ?? string.Empty] = node.ToString();
                    break;
            }
        }

        private static string CombineKey(string? prefix, string key)
            => string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;
    }
}
