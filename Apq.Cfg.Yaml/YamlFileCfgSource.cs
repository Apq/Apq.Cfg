using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using YamlDotNet.RepresentationModel;

namespace Apq.Cfg.Yaml;

/// <summary>
/// YAML 文件配置源，支持读取和写入 YAML 格式的配置文件
/// </summary>
internal sealed class YamlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    /// <summary>
    /// 初始化 YamlFileCfgSource 实例
    /// </summary>
    /// <param name="path">YAML 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">是否为可选文件</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    /// <param name="name">配置源名称（可选，默认使用文件名）</param>
    public YamlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, string? name = null)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, name: name)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        if (!File.Exists(_path))
            return Enumerable.Empty<KeyValuePair<string, string?>>();

        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var readEncoding = DetectEncodingEnhanced(_path);
        using var sr = new StreamReader(_path, readEncoding, true);
        var yaml = new YamlStream();
        yaml.Load(sr);

        if (yaml.Documents.Count == 0)
            return data;

        var root = (YamlMappingNode)yaml.Documents[0].RootNode;
        VisitNodeStatic(root, null, data);
        return data;
    }

    private static void VisitNodeStatic(YamlNode node, string? prefix, IDictionary<string, string?> data)
    {
        switch (node)
        {
            case YamlMappingNode map:
                foreach (var kv in map.Children)
                    VisitNodeStatic(kv.Value, CombineKeyStatic(prefix, kv.Key.ToString()), data);
                break;
            case YamlSequenceNode seq:
                var idx = 0;
                foreach (var item in seq.Children)
                    VisitNodeStatic(item, CombineKeyStatic(prefix, (idx++).ToString()), data);
                break;
            default:
                data[prefix ?? string.Empty] = node.ToString();
                break;
        }
    }

    private static string CombineKeyStatic(string? prefix, string key)
        => string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的 YAML 配置源
    /// </summary>
    /// <returns>YamlSource 实例，内部实现了 IConfigurationSource</returns>
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

    /// <summary>
    /// 应用配置更改到 YAML 文件
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

    /// <summary>
    /// 根据冒号分隔的键路径设置 YAML 映射节点中的值
    /// </summary>
    /// <param name="root">YAML 映射根节点</param>
    /// <param name="key">冒号分隔的键路径（如 "Database:Connection:Timeout"）</param>
    /// <param name="value">要设置的值，为 null 时设置为空字符串</param>
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
