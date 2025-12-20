using System.Text;
using System.Text.Json;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Apq.Cfg.Sources;

/// <summary>
/// JSON 文件配置源
/// </summary>
internal sealed class JsonFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public JsonFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new JsonConfigurationSource
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

        var text = string.Empty;
        if (System.IO.File.Exists(_path))
        {
            var readEncoding = DetectEncoding(_path);
            text = await System.IO.File.ReadAllTextAsync(_path, readEncoding, cancellationToken).ConfigureAwait(false);
        }

        var root = string.IsNullOrWhiteSpace(text)
            ? new Dictionary<string, object?>()
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(text) ?? new Dictionary<string, object?>();

        foreach (var (key, value) in changes)
            SetByColonKey(root, key, value);

        var jsonString = JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
        await System.IO.File.WriteAllTextAsync(_path, jsonString, WriteEncoding, cancellationToken).ConfigureAwait(false);
    }

    private static void SetByColonKey(Dictionary<string, object?> root, string key, string? value)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
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
                if (!current.TryGetValue(part, out var next) || next is not Dictionary<string, object?> dict)
                {
                    dict = new Dictionary<string, object?>();
                    current[part] = dict;
                }
                current = dict;
            }
        }
    }
}
