using System.Text;
using Apq.Cfg.EncodingSupport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using UtfUnknown;

namespace Apq.Cfg.Sources.File;

/// <summary>
/// 文件配置源基类，提供编码检测和写入编码功能
/// </summary>
public abstract class FileCfgSourceBase : ICfgSource, IDisposable
{
    /// <summary>
    /// 全局编码检测器实例
    /// </summary>
    public static EncodingDetector EncodingDetector { get; } = EncodingDetector.Default;

    private static float _encodingConfidenceThreshold = GetDefaultThreshold();

    /// <summary>
    /// 编码检测置信度阈值（0.0-1.0），可在运行时修改。
    /// 可通过环境变量 APQ_CFG_ENCODING_CONFIDENCE 设置默认值。
    /// </summary>
    public static float EncodingConfidenceThreshold
    {
        get => Volatile.Read(ref _encodingConfidenceThreshold);
        set => Volatile.Write(ref _encodingConfidenceThreshold, Math.Clamp(value, 0f, 1f));
    }

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
    protected readonly EncodingOptions _encodingOptions;
    private PhysicalFileProvider? _fileProvider; // 跟踪创建的 FileProvider 以便释放
    private int _disposed;

    // 缓存检测到的编码，用于 Preserve 策略
    private System.Text.Encoding? _detectedEncoding;

    protected FileCfgSourceBase(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, EncodingOptions? encodingOptions = null, string? name = null)
    {
        _path = path;
        Level = level;
        IsWriteable = writeable;
        _optional = optional;
        _reloadOnChange = reloadOnChange;
        IsPrimaryWriter = isPrimaryWriter;
        _encodingOptions = encodingOptions ?? EncodingOptions.Default;
        // 默认使用文件名作为配置源名称
        Name = name ?? Path.GetFileName(path);
    }

    /// <inheritdoc />
    public string Name { get; set; }

    public int Level { get; }
    public bool IsWriteable { get; }
    public bool IsPrimaryWriter { get; }

    /// <summary>
    /// 获取文件路径
    /// </summary>
    public string FilePath => _path;

    /// <summary>
    /// 编码选项
    /// </summary>
    public EncodingOptions EncodingOptionsValue => _encodingOptions;

    public abstract IConfigurationSource BuildSource();

    /// <inheritdoc />
    public abstract IEnumerable<KeyValuePair<string, string?>> GetAllValues();

    /// <summary>
    /// 创建 PhysicalFileProvider 并跟踪以便后续释放
    /// </summary>
    protected (PhysicalFileProvider Provider, string FileName) CreatePhysicalFileProvider(string path)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path)) ?? Directory.GetCurrentDirectory();
        var provider = new PhysicalFileProvider(dir);
        _fileProvider = provider; // 跟踪以便释放
        return (provider, Path.GetFileName(path));
    }

    protected static void EnsureDirectoryFor(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
    }

    /// <summary>
    /// 检测文件编码（用于读取）- 使用增强的编码检测器
    /// </summary>
    public System.Text.Encoding DetectEncodingEnhanced(string path)
    {
        var result = EncodingDetector.Detect(path, _encodingOptions);
        _detectedEncoding = result.Encoding; // 缓存用于 Preserve 策略
        return _encodingOptions.GetReadEncoding(result.Encoding);
    }

    /// <summary>
    /// 获取写入编码
    /// </summary>
    public System.Text.Encoding GetWriteEncoding()
    {
        // 1. 优先使用 EncodingOptions 的策略
        var writeStrategy = _encodingOptions.WriteStrategy;

        // 如果是 Utf8NoBom 或 Utf8WithBom，直接使用 EncodingOptions 的方法
        if (writeStrategy == EncodingWriteStrategy.Utf8NoBom || writeStrategy == EncodingWriteStrategy.Utf8WithBom)
        {
            return _encodingOptions.GetWriteEncoding();
        }

        // 2. 如果 EncodingOptions 指定了写入编码，使用它
        if (writeStrategy == EncodingWriteStrategy.Specified && _encodingOptions.WriteEncoding != null)
        {
            return _encodingOptions.WriteEncoding;
        }

        // 3. 如果是 Preserve 策略，使用检测到的编码
        if (writeStrategy == EncodingWriteStrategy.Preserve && _detectedEncoding != null)
        {
            return _detectedEncoding;
        }

        // 4. 检查映射配置（包括通配符、正则等）
        var mappedEncoding = EncodingDetector.GetWriteEncoding(_path);
        return mappedEncoding;
    }

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposed, 1, 0) != 0)
            return;

        _fileProvider?.Dispose();
        _fileProvider = null;
    }
}
