using System.Text;

namespace Apq.Cfg.EncodingSupport;

/// <summary>
/// 编码读取策略
/// </summary>
public enum EncodingReadStrategy
{
    /// <summary>
    /// 自动检测编码（BOM 优先，然后使用 UTF.Unknown）
    /// </summary>
    AutoDetect,

    /// <summary>
    /// 使用指定的编码
    /// </summary>
    Specified,

    /// <summary>
    /// 保持原编码（读取时检测，写入时保持）
    /// </summary>
    Preserve
}

/// <summary>
/// 编码写入策略
/// </summary>
public enum EncodingWriteStrategy
{
    /// <summary>
    /// 统一转换为 UTF-8（无 BOM）
    /// </summary>
    Utf8NoBom,

    /// <summary>
    /// 统一转换为 UTF-8（带 BOM）
    /// </summary>
    Utf8WithBom,

    /// <summary>
    /// 保持原文件编码
    /// </summary>
    Preserve,

    /// <summary>
    /// 使用指定的编码
    /// </summary>
    Specified
}

/// <summary>
/// 编码选项配置
/// </summary>
public sealed class EncodingOptions
{
    /// <summary>
    /// 默认编码选项（自动检测读取，UTF-8 无 BOM 写入）
    /// </summary>
    public static readonly EncodingOptions Default = new();

    /// <summary>
    /// PowerShell 脚本编码选项（UTF-8 带 BOM）
    /// </summary>
    public static readonly EncodingOptions PowerShell = new()
    {
        WriteStrategy = EncodingWriteStrategy.Utf8WithBom
    };

    /// <summary>
    /// 读取策略，默认自动检测
    /// </summary>
    public EncodingReadStrategy ReadStrategy { get; set; } = EncodingReadStrategy.AutoDetect;

    /// <summary>
    /// 写入策略，默认 UTF-8 无 BOM
    /// </summary>
    public EncodingWriteStrategy WriteStrategy { get; set; } = EncodingWriteStrategy.Utf8NoBom;

    /// <summary>
    /// 指定的读取编码（当 ReadStrategy 为 Specified 时使用）
    /// </summary>
    public System.Text.Encoding? ReadEncoding { get; set; }

    /// <summary>
    /// 指定的写入编码（当 WriteStrategy 为 Specified 时使用）
    /// </summary>
    public System.Text.Encoding? WriteEncoding { get; set; }

    /// <summary>
    /// 回退编码（自动检测失败时使用），默认 UTF-8
    /// </summary>
    public System.Text.Encoding FallbackEncoding { get; set; } = System.Text.Encoding.UTF8;

    /// <summary>
    /// 编码检测置信度阈值（0.0-1.0），默认 0.6
    /// </summary>
    public float ConfidenceThreshold { get; set; } = 0.6f;

    /// <summary>
    /// 是否启用编码检测缓存，默认 true
    /// </summary>
    public bool EnableCache { get; set; } = true;

    /// <summary>
    /// 是否启用编码检测日志，默认 false
    /// </summary>
    public bool EnableLogging { get; set; } = false;

    /// <summary>
    /// 获取写入时使用的编码
    /// </summary>
    /// <param name="detectedEncoding">检测到的原文件编码（用于 Preserve 策略）</param>
    /// <returns>写入编码</returns>
    public System.Text.Encoding GetWriteEncoding(System.Text.Encoding? detectedEncoding = null)
    {
        return WriteStrategy switch
        {
            EncodingWriteStrategy.Utf8NoBom => new UTF8Encoding(false),
            EncodingWriteStrategy.Utf8WithBom => new UTF8Encoding(true),
            EncodingWriteStrategy.Preserve => detectedEncoding ?? new UTF8Encoding(false),
            EncodingWriteStrategy.Specified => WriteEncoding ?? new UTF8Encoding(false),
            _ => new UTF8Encoding(false)
        };
    }

    /// <summary>
    /// 获取读取时使用的编码
    /// </summary>
    /// <param name="detectedEncoding">自动检测到的编码</param>
    /// <returns>读取编码</returns>
    public System.Text.Encoding GetReadEncoding(System.Text.Encoding? detectedEncoding = null)
    {
        return ReadStrategy switch
        {
            EncodingReadStrategy.AutoDetect => detectedEncoding ?? FallbackEncoding,
            EncodingReadStrategy.Specified => ReadEncoding ?? FallbackEncoding,
            EncodingReadStrategy.Preserve => detectedEncoding ?? FallbackEncoding,
            _ => FallbackEncoding
        };
    }
}
