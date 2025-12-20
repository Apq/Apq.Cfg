using System.Xml;
using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;
using Microsoft.Extensions.FileProviders;

namespace Apq.Cfg.Xml;

internal sealed class XmlFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    public XmlFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
    }

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

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        if (!IsWriteable)
            throw new InvalidOperationException($"配置源 (层级 {Level}) 不可写");

        EnsureDirectoryFor(_path);

        var doc = new XmlDocument();
        if (File.Exists(_path))
        {
            var readEncoding = DetectEncoding(_path);
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
        using (var writer = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, Encoding = WriteEncoding }))
        {
            doc.Save(writer);
        }

        var bytes = ms.ToArray();
        await File.WriteAllBytesAsync(_path, bytes, cancellationToken).ConfigureAwait(false);
    }

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
