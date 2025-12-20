using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using UtfUnknown;

namespace Apq.Cfg.Sources.File;

/// <summary>
/// 文件配置源基类，提供编码检测和统一写入编码
/// </summary>
public abstract class FileCfgSourceBase : ICfgSource
{
    /// <summary>
    /// 写入时统一使用的编码：UTF-8 无 BOM
    /// </summary>
    public static readonly Encoding WriteEncoding = new UTF8Encoding(false);

    /// <summary>
    /// 编码检测置信度阈值（0.0-1.0），可在运行时修改。
    /// 可通过环境变量 APQ_CFG_ENCODING_CONFIDENCE 设置默认值。
    /// </summary>
    public static float EncodingConfidenceThreshold { get; set; } = GetDefaultThreshold();

    private static float GetDefaultThreshold()
    {
        var envValue = System.Environment.GetEnvironmentVariable("APQ_CFG_ENCODING_CONFIDENCE");
        if (!string.IsNullOrEmpty(envValue) && float.TryParse(envValue, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var threshold))
        {
            return Math.Clamp(threshold, 0f, 1f);
        }

        return 0.6f;
    }

    protected readonly bool _optional;
    protected readonly string _path;
    protected readonly bool _reloadOnChange;

    protected FileCfgSourceBase(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter)
    {
        _path = path;
        Level = level;
        IsWriteable = writeable;
        _optional = optional;
        _reloadOnChange = reloadOnChange;
        IsPrimaryWriter = isPrimaryWriter;
    }

    public int Level { get; }
    public bool IsWriteable { get; }
    public bool IsPrimaryWriter { get; }

    public abstract IConfigurationSource BuildSource();

    protected static (PhysicalFileProvider Provider, string FileName) CreatePhysicalFileProvider(string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path)) ?? Directory.GetCurrentDirectory();
        return (new PhysicalFileProvider(dir), Path.GetFileName(path));
    }

    protected static void EnsureDirectoryFor(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
    }

    /// <summary>
    /// 检测文件编码（用于读取）
    /// 使用 UTF.Unknown 库进行智能编码检测，支持各种编码格式
    /// </summary>
    public static Encoding DetectEncoding(string path)
    {
        if (!System.IO.File.Exists(path)) return Encoding.UTF8;

        try
        {
            var result = CharsetDetector.DetectFromFile(path);
            if (result.Detected != null && result.Detected.Confidence >= EncodingConfidenceThreshold)
            {
                try
                {
                    return Encoding.GetEncoding(result.Detected.EncodingName);
                }
                catch
                {
                    // 如果编码名称无法识别，回退到 UTF-8
                }
            }
        }
        catch { }

        return Encoding.UTF8;
    }
}
