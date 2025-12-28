using System.Xml;
using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.FileProviders;

namespace Apq.Cfg.Xml;

/// <summary>
/// XML 文件配置源，支持读取和写入 XML 格式的配置文件
/// </summary>
internal sealed class XmlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    /// <summary>
    /// 初始化 XmlFileCfgSource 实例
    /// </summary>
    /// <param name="path">XML 文件路径</param>
    /// <param name="level">配置层级，数值越大优先级越高</param>
    /// <param name="writeable">是否可写</param>
    /// <param name="optional">是否为可选文件</param>
    /// <param name="reloadOnChange">文件变更时是否自动重载</param>
    /// <param name="isPrimaryWriter">是否为主要写入源</param>
    public XmlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

    /// <summary>
    /// 构建 Microsoft.Extensions.Configuration 的 XML 配置源
    /// </summary>
    /// <returns>Microsoft.Extensions.Configuration.Xml.XmlConfigurationSource 实例</returns>
    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProvider(_path);
        var src = new XmlConfigurationSource
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
    /// 应用配置更改到 XML 文件
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

        var doc = new XmlDocument();
        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            using var sr = new StreamReader(_path, readEncoding, true);
            doc.Load(sr);
        }
        else
        {
            var decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(decl);
            var root = doc.CreateElement("configuration");
            doc.AppendChild(root);
        }

        foreach (var (key, value) in changes)
            SetXmlByColonKey(doc, key, value);

        using var ms = new MemoryStream();
        using (var writer = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, Encoding = GetWriteEncoding() }))
        {
            doc.Save(writer);
        }

        var bytes = ms.ToArray();
        await File.WriteAllBytesAsync(_path, bytes, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据冒号分隔的键路径设置 XML 文档中的节点值
    /// </summary>
    /// <param name="doc">XML 文档</param>
    /// <param name="key">冒号分隔的键路径（如 "Database:Connection:Timeout"）</param>
    /// <param name="value">要设置的值，为 null 时设置为空字符串</param>
    private static void SetXmlByColonKey(XmlDocument doc, string key, string? value)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        var node = doc.DocumentElement!;
        for (var i = 0; i < parts.Length; i++)
        {
            var name = parts[i];
            var child = node.SelectSingleNode(name) as XmlElement;
            if (child == null)
            {
                child = doc.CreateElement(name);
                node.AppendChild(child);
            }

            node = child;
        }

        node.InnerText = value ?? string.Empty;
    }
}
