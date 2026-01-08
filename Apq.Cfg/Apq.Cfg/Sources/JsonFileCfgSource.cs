using System.Text;
using System.Text.Encodings.Web;
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
internal sealed class JsonFileCfgSource : FileCfgSourceBase, IWritableCfgSource, IFileCfgSource
{
    // 缓存 JsonSerializerOptions，避免每次序列化都创建新实例
    private static readonly JsonSerializerOptions s_writeOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 初始化 JsonFileCfgSource 实例
    /// </summary>
    /// <param name="path">JSON 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">是否为可选文件</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    /// <param name="encodingOptions">编码选项</param>
    /// <param name="name">配置源名称，为 null 时使用文件名</param>
    public JsonFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, EncodingOptions? encodingOptions = null, string? name = null)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter, encodingOptions, name)
    {
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的 JSON 配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.Json.JsonConfigurationSource 实例</returns>
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

    /// <summary>
    /// 应用配置更改到 JSON 文件
    /// </summary>
    /// <param name="changes">要应用的配置更改</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="InvalidOperationException">当配置源不可写或 JSON 根节点不是对象时抛出</exception>
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

    /// <summary>
    /// 根据冒号分隔的键路径设置 JSON 对象中的值
    /// </summary>
    /// <param name="root">JSON 根对象</param>
    /// <param name="key">冒号分隔的键路径（如 "Database:Connection:Timeout"）</param>
    /// <param name="value">要设置的值，为 null 时删除该键</param>
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

    /// <inheritdoc />
    public override IEnumerable<KeyValuePair<string, string?>> GetAllValues()
    {
        if (!System.IO.File.Exists(_path))
            return [];

        var readEncoding = DetectEncodingEnhanced(_path);
        var text = System.IO.File.ReadAllText(_path, readEncoding);
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var root = JsonNode.Parse(text);
        if (root is not JsonObject rootObj)
            return [];

        var result = new List<KeyValuePair<string, string?>>();
        CollectValues(rootObj, string.Empty, result);
        return result;
    }

    private static void CollectValues(JsonObject obj, string prefix, List<KeyValuePair<string, string?>> result)
    {
        foreach (var prop in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? prop.Key : $"{prefix}:{prop.Key}";
            if (prop.Value is JsonObject childObj)
            {
                CollectValues(childObj, key, result);
            }
            else if (prop.Value is JsonArray arr)
            {
                for (var i = 0; i < arr.Count; i++)
                {
                    var itemKey = $"{key}:{i}";
                    if (arr[i] is JsonObject arrObj)
                    {
                        CollectValues(arrObj, itemKey, result);
                    }
                    else
                    {
                        result.Add(new KeyValuePair<string, string?>(itemKey, arr[i]?.ToString()));
                    }
                }
            }
            else
            {
                result.Add(new KeyValuePair<string, string?>(key, prop.Value?.ToString()));
            }
        }
    }
}
