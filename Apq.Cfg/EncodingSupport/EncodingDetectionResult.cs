using System.Text;

namespace Apq.Cfg.EncodingSupport;

/// <summary>
/// 编码检测结果
/// </summary>
public sealed class EncodingDetectionResult
{
    /// <summary>
    /// 检测到的编码
    /// </summary>
    public System.Text.Encoding Encoding { get; }

    /// <summary>
    /// 检测置信度（0.0-1.0）
    /// </summary>
    public float Confidence { get; }

    /// <summary>
    /// 检测方法
    /// </summary>
    public EncodingDetectionMethod Method { get; }

    /// <summary>
    /// 是否有 BOM 标记
    /// </summary>
    public bool HasBom { get; }

    /// <summary>
    /// 原始编码名称（来自检测库）
    /// </summary>
    public string? RawEncodingName { get; }

    /// <summary>
    /// 文件路径
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 检测时间
    /// </summary>
    public DateTimeOffset DetectedAt { get; }

    public EncodingDetectionResult(
        System.Text.Encoding encoding,
        float confidence,
        EncodingDetectionMethod method,
        bool hasBom,
        string? rawEncodingName,
        string filePath)
    {
        Encoding = encoding;
        Confidence = confidence;
        Method = method;
        HasBom = hasBom;
        RawEncodingName = rawEncodingName;
        FilePath = filePath;
        DetectedAt = DateTimeOffset.Now;
    }

    public override string ToString()
    {
        var bomInfo = HasBom ? " (BOM)" : "";
        return $"[{Method}] {Encoding.EncodingName}{bomInfo} ({Confidence:P0})";
    }
}

/// <summary>
/// 编码检测方法
/// </summary>
public enum EncodingDetectionMethod
{
    /// <summary>
    /// 通过 BOM 标记检测
    /// </summary>
    Bom,

    /// <summary>
    /// 通过 UTF.Unknown 库检测
    /// </summary>
    UtfUnknown,

    /// <summary>
    /// 使用回退编码
    /// </summary>
    Fallback,

    /// <summary>
    /// 使用用户指定编码
    /// </summary>
    UserSpecified,

    /// <summary>
    /// 从缓存获取
    /// </summary>
    Cached
}
