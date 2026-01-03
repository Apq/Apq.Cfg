using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Tomlyn.Model;

namespace Apq.Cfg.Toml;

/// <summary>
/// TOML 文件配置源，支持读取和写入 TOML 格式的配置文件
/// </summary>
internal sealed class TomlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    /// <summary>
    /// 初始化 TomlFileCfgSource 实例
    /// </summary>
    /// <param name="path">TOML 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">是否为可选文件</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    /// <param name="name">配置源名称（可选，默认使用文件名）</param>
    public TomlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
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
        var text = File.ReadAllText(_path, readEncoding);
        var model = Tomlyn.Toml.ToModel(text);
        VisitToml(model, null, data);
        return data;
    }

    private static void VisitToml(object? node, string? prefix, IDictionary<string, string?> data)
    {
        switch (node)
        {
            case TomlTable table:
                foreach (var key in table.Keys)
                    VisitToml(table[key], CombineTomlKey(prefix, key), data);
                break;
            case TomlArray array:
                for (var i = 0; i < array.Count; i++)
                    VisitToml(array[i], CombineTomlKey(prefix, i.ToString()), data);
                break;
            default:
                data[prefix ?? string.Empty] = node?.ToString();
                break;
        }
    }

    private static string CombineTomlKey(string? prefix, string key)
        => string.IsNullOrEmpty(prefix) ? key : prefix + ":" + key;

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的 TOML 配置源
    /// </summary>
    /// <returns>TomlSource 实例，内部实现了 IConfigurationSource</returns>
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

    /// <summary>
    /// 应用配置更改到 TOML 文件
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

    /// <summary>
    /// 根据冒号分隔的键路径设置 TOML 表中的值
    /// </summary>
    /// <param name="root">TOML 根表</param>
    /// <param name="key">冒号分隔的键路径（如 "Database:Connection:Timeout"）</param>
    /// <param name="value">要设置的值，为 null 时设置为空字符串</param>
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
