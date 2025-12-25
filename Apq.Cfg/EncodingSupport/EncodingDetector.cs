using System.Collections.Concurrent;
using System.Text;
using UtfUnknown;

namespace Apq.Cfg.EncodingSupport;

/// <summary>
/// 增强的编码检测器，支持 BOM 优先检测、缓存、日志和自定义映射
/// </summary>
public sealed class EncodingDetector
{
    /// <summary>
    /// 全局默认实例
    /// </summary>
    public static EncodingDetector Default { get; } = new();

    /// <summary>
    /// 默认写入编码（UTF-8 无 BOM）
    /// </summary>
    public static readonly Encoding DefaultWriteEncoding = new UTF8Encoding(false);

    // 编码检测缓存：文件路径 -> (检测结果, 文件最后修改时间)
    private readonly ConcurrentDictionary<string, (EncodingDetectionResult Result, DateTime LastModified)> _cache = new();

    /// <summary>
    /// 编码映射配置
    /// </summary>
    public EncodingMappingConfig MappingConfig { get; } = new();

    /// <summary>
    /// 编码检测日志事件
    /// </summary>
    public event Action<EncodingDetectionResult>? OnEncodingDetected;

    /// <summary>
    /// 默认编码选项
    /// </summary>
    public EncodingOptions DefaultOptions { get; set; } = EncodingOptions.Default;

    public EncodingDetector()
    {
        // 初始化常见扩展名的默认写入编码（PowerShell 需要 BOM）
        var utf8Bom = new UTF8Encoding(true);
        MappingConfig.AddWriteMapping("*.ps1", EncodingMappingType.Wildcard, utf8Bom, priority: -100);
        MappingConfig.AddWriteMapping("*.psm1", EncodingMappingType.Wildcard, utf8Bom, priority: -100);
        MappingConfig.AddWriteMapping("*.psd1", EncodingMappingType.Wildcard, utf8Bom, priority: -100);
    }

    /// <summary>
    /// 检测文件编码（用于读取）
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="options">编码选项（可选）</param>
    /// <returns>编码检测结果</returns>
    public EncodingDetectionResult Detect(string filePath, EncodingOptions? options = null)
    {
        options ??= DefaultOptions;
        var fullPath = Path.GetFullPath(filePath);

        // 1. 检查用户指定编码（EncodingOptions 中指定）
        if (options.ReadStrategy == EncodingReadStrategy.Specified && options.ReadEncoding != null)
        {
            var result = new EncodingDetectionResult(
                options.ReadEncoding,
                1.0f,
                EncodingDetectionMethod.UserSpecified,
                false,
                options.ReadEncoding.EncodingName,
                fullPath);

            LogDetection(result, options);
            return result;
        }

        // 2. 检查读取映射配置
        var mappedEncoding = MappingConfig.GetReadEncoding(fullPath);
        if (mappedEncoding != null)
        {
            // 验证映射编码是否能正确读取文件
            if (ValidateEncodingForFile(fullPath, mappedEncoding))
            {
                var result = new EncodingDetectionResult(
                    mappedEncoding,
                    1.0f,
                    EncodingDetectionMethod.UserSpecified,
                    false,
                    mappedEncoding.EncodingName,
                    fullPath);

                LogDetection(result, options);
                return result;
            }
            // 映射编码读取失败，回退到自动检测
        }

        // 3. 检查缓存
        if (options.EnableCache && TryGetFromCache(fullPath, out var cachedResult))
        {
            LogDetection(cachedResult!, options);
            return cachedResult!;
        }

        // 4. 自动检测编码
        var detectionResult = DetectInternal(fullPath, options);

        // 5. 更新缓存
        if (options.EnableCache && File.Exists(fullPath))
        {
            var lastModified = File.GetLastWriteTimeUtc(fullPath);
            _cache[fullPath] = (detectionResult, lastModified);
        }

        LogDetection(detectionResult, options);
        return detectionResult;
    }

    /// <summary>
    /// 获取文件的写入编码
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>写入编码（如果没有匹配的映射则返回 UTF-8 无 BOM）</returns>
    public Encoding GetWriteEncoding(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);

        // 检查写入映射配置
        var mappedEncoding = MappingConfig.GetWriteEncoding(fullPath);
        if (mappedEncoding != null)
        {
            return mappedEncoding;
        }

        // 默认返回 UTF-8 无 BOM
        return DefaultWriteEncoding;
    }

    /// <summary>
    /// 内部检测逻辑
    /// </summary>
    private EncodingDetectionResult DetectInternal(string filePath, EncodingOptions options)
    {
        if (!File.Exists(filePath))
        {
            return new EncodingDetectionResult(
                options.FallbackEncoding,
                0f,
                EncodingDetectionMethod.Fallback,
                false,
                null,
                filePath);
        }

        try
        {
            // 1. BOM 优先检测（更快更准确）
            var bomResult = DetectByBom(filePath);
            if (bomResult != null)
            {
                return new EncodingDetectionResult(
                    bomResult.Value.Encoding,
                    1.0f,
                    EncodingDetectionMethod.Bom,
                    true,
                    bomResult.Value.Encoding.EncodingName,
                    filePath);
            }

            // 2. 使用 UTF.Unknown 检测
            var utfResult = CharsetDetector.DetectFromFile(filePath);
            if (utfResult.Detected != null && utfResult.Detected.Confidence >= options.ConfidenceThreshold)
            {
                try
                {
                    var encoding = Encoding.GetEncoding(utfResult.Detected.EncodingName);
                    return new EncodingDetectionResult(
                        encoding,
                        utfResult.Detected.Confidence,
                        EncodingDetectionMethod.UtfUnknown,
                        false,
                        utfResult.Detected.EncodingName,
                        filePath);
                }
                catch
                {
                    // 编码名称无法识别，继续使用回退
                }
            }
        }
        catch
        {
            // 检测失败，使用回退
        }

        // 3. 使用回退编码
        return new EncodingDetectionResult(
            options.FallbackEncoding,
            0f,
            EncodingDetectionMethod.Fallback,
            false,
            null,
            filePath);
    }

    /// <summary>
    /// 验证指定编码是否能正确读取文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="encoding">要验证的编码</param>
    /// <returns>如果能正确读取返回 true，否则返回 false</returns>
    private static bool ValidateEncodingForFile(string filePath, Encoding encoding)
    {
        if (!File.Exists(filePath))
            return true; // 文件不存在时不验证，让后续流程处理

        try
        {
            // 创建一个带有异常回退的解码器来验证
            var decoder = encoding.GetDecoder();
            decoder.Fallback = DecoderFallback.ExceptionFallback;

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 只读取前 4KB 进行验证，避免大文件性能问题
            var bufferSize = (int)Math.Min(4096, fs.Length);
            if (bufferSize == 0)
                return true; // 空文件

            var bytes = new byte[bufferSize];
            var bytesRead = fs.Read(bytes, 0, bufferSize);

            // 尝试解码，如果编码不匹配会抛出异常
            var chars = new char[encoding.GetMaxCharCount(bytesRead)];
            decoder.GetChars(bytes, 0, bytesRead, chars, 0, flush: true);

            return true;
        }
        catch (DecoderFallbackException)
        {
            // 解码失败，编码不匹配
            return false;
        }
        catch
        {
            // 其他错误（如文件访问问题），假设编码有效
            return true;
        }
    }

    /// <summary>
    /// 通过 BOM 检测编码
    /// </summary>
    private static (Encoding Encoding, int BomLength)? DetectByBom(string filePath)
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (fs.Length < 2) return null;

            var bom = new byte[4];
            var bytesRead = fs.Read(bom, 0, Math.Min(4, (int)fs.Length));

            if (bytesRead < 2) return null;

            // UTF-32 BE: 00 00 FE FF
            if (bytesRead >= 4 && bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
                return (new UTF32Encoding(true, true), 4);

            // UTF-32 LE: FF FE 00 00
            if (bytesRead >= 4 && bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                return (new UTF32Encoding(false, true), 4);

            // UTF-8 BOM: EF BB BF
            if (bytesRead >= 3 && bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                return (new UTF8Encoding(true), 3);

            // UTF-16 BE: FE FF
            if (bom[0] == 0xFE && bom[1] == 0xFF)
                return (new UnicodeEncoding(true, true), 2);

            // UTF-16 LE: FF FE
            if (bom[0] == 0xFF && bom[1] == 0xFE)
                return (new UnicodeEncoding(false, true), 2);
        }
        catch
        {
            // 读取失败
        }

        return null;
    }

    /// <summary>
    /// 尝试从缓存获取
    /// </summary>
    private bool TryGetFromCache(string filePath, out EncodingDetectionResult? result)
    {
        result = null;

        if (!_cache.TryGetValue(filePath, out var cached))
            return false;

        // 检查文件是否被修改
        if (!File.Exists(filePath))
        {
            _cache.TryRemove(filePath, out _);
            return false;
        }

        var currentModified = File.GetLastWriteTimeUtc(filePath);
        if (currentModified != cached.LastModified)
        {
            _cache.TryRemove(filePath, out _);
            return false;
        }

        // 返回带有 Cached 方法标记的新结果
        result = new EncodingDetectionResult(
            cached.Result.Encoding,
            cached.Result.Confidence,
            EncodingDetectionMethod.Cached,
            cached.Result.HasBom,
            cached.Result.RawEncodingName,
            filePath);

        return true;
    }

    /// <summary>
    /// 记录检测日志
    /// </summary>
    private void LogDetection(EncodingDetectionResult result, EncodingOptions options)
    {
        if (options.EnableLogging)
        {
            OnEncodingDetected?.Invoke(result);
        }
    }

    #region 便捷方法（向后兼容）

    /// <summary>
    /// 添加读取编码映射（完整路径）
    /// </summary>
    public void SetCustomReadMapping(string filePath, Encoding encoding)
    {
        MappingConfig.AddReadMapping(filePath, EncodingMappingType.ExactPath, encoding, priority: 100);
        // 清除该文件的缓存
        var fullPath = Path.GetFullPath(filePath);
        _cache.TryRemove(fullPath, out _);
    }

    /// <summary>
    /// 添加写入编码映射（完整路径）
    /// </summary>
    public void SetCustomWriteMapping(string filePath, Encoding encoding)
    {
        MappingConfig.AddWriteMapping(filePath, EncodingMappingType.ExactPath, encoding, priority: 100);
    }

    /// <summary>
    /// 移除读取编码映射
    /// </summary>
    public void RemoveCustomReadMapping(string filePath)
    {
        MappingConfig.RemoveReadMapping(filePath);
    }

    /// <summary>
    /// 移除写入编码映射
    /// </summary>
    public void RemoveCustomWriteMapping(string filePath)
    {
        MappingConfig.RemoveWriteMapping(filePath);
    }

    /// <summary>
    /// 获取文件的自定义写入编码（向后兼容）
    /// </summary>
    public Encoding? GetCustomWriteEncoding(string filePath)
    {
        return MappingConfig.GetWriteEncoding(filePath);
    }

    /// <summary>
    /// 设置文件扩展名的默认编码（向后兼容，添加到写入映射）
    /// </summary>
    public void SetExtensionMapping(string extension, Encoding encoding)
    {
        var ext = extension.StartsWith('.') ? extension : $".{extension}";
        MappingConfig.AddWriteMapping($"*{ext}", EncodingMappingType.Wildcard, encoding, priority: -50);
    }

    /// <summary>
    /// 获取文件扩展名的默认写入编码（向后兼容）
    /// </summary>
    public Encoding? GetExtensionEncoding(string filePath)
    {
        return MappingConfig.GetWriteEncoding(filePath);
    }

    #endregion

    #region 缓存管理

    /// <summary>
    /// 清除所有缓存
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// 清除指定文件的缓存
    /// </summary>
    public void InvalidateCache(string filePath)
    {
        var fullPath = Path.GetFullPath(filePath);
        _cache.TryRemove(fullPath, out _);
    }

    /// <summary>
    /// 获取统计信息
    /// </summary>
    public (int CacheCount, int ReadMappingCount, int WriteMappingCount) GetStats()
    {
        var (readCount, writeCount) = MappingConfig.GetStats();
        return (_cache.Count, readCount, writeCount);
    }

    #endregion
}
