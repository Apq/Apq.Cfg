using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Tomlyn.Model;

namespace Apq.Cfg.Toml;

internal sealed class TomlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public TomlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new TomlSource
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

        TomlTable root;
        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            var text = await File.ReadAllTextAsync(_path, readEncoding, cancellationToken).ConfigureAwait(false);
            root = Tomlyn.Toml.ToModel(text) ?? new TomlTable();
        }
        else
        {
            root = new TomlTable();
        }

        foreach (var (key, value) in changes)
            SetTomlByColonKey(root, key, value);

        var output = Tomlyn.Toml.FromModel(root);
        await File.WriteAllTextAsync(_path, output, GetWriteEncoding(), cancellationToken).ConfigureAwait(false);
    }

    private static void SetTomlByColonKey(TomlTable root, string key, string? value)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var current = root;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (i == parts.Length - 1)
            {
                current[part] = value ?? string.Empty;
            }
            else
            {
                if (current.TryGetValue(part, out var next) && next is TomlTable table)
                {
                    current = table;
                }
                else
                {
                    var newTable = new TomlTable();
                    current[part] = newTable;
                    current = newTable;
                }
            }
        }
    }

    private sealed class TomlSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new TomlProvider(this);
        }
    }

    private sealed class TomlProvider : FileConfigurationProvider
    {
        public TomlProvider(TomlSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, true);
            var text = reader.ReadToEnd();
            var model = Tomlyn.Toml.ToModel(text);
            Visit(model, null, data);
            Data = data;
        }

        private static void Visit(object? node, string? prefix, IDictionary<string, string?> data)
        {
            switch (node)
            {
                case TomlTable table:
                    foreach (var key in table.Keys)
                        Visit(table[key], CombineKey(prefix, key), data);
                    break;
                case TomlArray array:
                    for (var i = 0; i < array.Count; i++)
                        Visit(array[i], CombineKey(prefix, i.ToString()), data);
                    break;
                default:
                    data[prefix ?? string.Empty] = node?.ToString();
                    break;
            }
        }

        private static string CombineKey(string? prefix, string key)
            => string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;
    }
}
