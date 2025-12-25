using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Apq.Cfg.EncodingSupport;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Apq.Cfg.Sources;

/// <summary>
/// JSON 文件配置源
/// </summary>
internal sealed class JsonFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    // 缓存 JsonSerializerOptions，避免每次序列化都创建新实例
    private static readonly JsonSerializerOptions s_writeOptions = new() { WriteIndented = true };

    public JsonFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, EncodingOptions? encodingOptions = null)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, encodingOptions)
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

        JsonNode? root = null;
        if (System.IO.File.Exists(_path))
        {
            // 使用增强的编码检测
            var readEncoding = DetectEncodingEnhanced(_path);
            var text = await System.IO.File.ReadAllTextAsync(_path, readEncoding, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(text))
            {
                root = JsonNode.Parse(text);
            }
        }

        root ??= new JsonObject();

        if (root is not JsonObject rootObj)
            throw new InvalidOperationException("JSON 根节点必须是对象");

        foreach (var (key, value) in changes)
            SetByColonKey(rootObj, key, value);

        var jsonString = root.ToJsonString(s_writeOptions);
        // 使用配置的写入编码
        var writeEncoding = GetWriteEncoding();
        await System.IO.File.WriteAllTextAsync(_path, jsonString, writeEncoding, cancellationToken).ConfigureAwait(false);
    }

    private static void SetByColonKey(JsonObject root, string key, string? value)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var current = root;
        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (i == parts.Length - 1)
            {
                // 最后一个部分，设置值
                if (value == null)
                    current.Remove(part);
                else
                    current[part] = JsonValue.Create(value);
            }
            else
            {
                // 中间部分，确保是对象
                if (!current.TryGetPropertyValue(part, out var next) || next is not JsonObject nextObj)
                {
                    nextObj = new JsonObject();
                    current[part] = nextObj;
                }
                current = nextObj;
            }
        }
    }
}
