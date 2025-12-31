using Apq.Cfg.Sources;
using Apq.Cfg.Sources.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Apq.Cfg.Env;

/// <summary>
/// .env 文件配置源
/// </summary>
internal sealed class EnvFileCfgSource : FileCfgSourceBase, IWritableCfgSource
{
    private readonly bool _setEnvironmentVariables;

    public EnvFileCfgSource(string path, int level, bool writeable, bool optional, bool reloadOnChange,
        bool isPrimaryWriter, bool setEnvironmentVariables = false)
        : base(path, level, writeable, optional, reloadOnChange, isPrimaryWriter)
    {
        _setEnvironmentVariables = setEnvironmentVariables;
    }

    public override IConfigurationSource BuildSource()
    {
        var (fp, file) = CreatePhysicalFileProviderForEnv(_path);
        var src = new EnvConfigurationSource
        {
            FileProvider = fp,
            Path = file,
            Optional = _optional,
            ReloadOnChange = _reloadOnChange,
            SetEnvironmentVariables = _setEnvironmentVariables
        };
        src.ResolveFileProvider();
        return src;
    }

    /// <summary>
    /// 创建 PhysicalFileProvider，允许访问以点开头的文件（如 .env）
    /// </summary>
    private (PhysicalFileProvider Provider, string FileName) CreatePhysicalFileProviderForEnv(string path)
    {
        var fullPath = Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory();
        var fileName = Path.GetFileName(fullPath);
        
        // 使用 ExclusionFilters.None 来允许访问以点开头的文件
        var provider = new PhysicalFileProvider(dir, ExclusionFilters.None);
        return (provider, fileName);
    }

    public async Task ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken cancellationToken)
    {
        if (!IsWriteable)
            throw new InvalidOperationException($"配置源 (层级 {Level}) 不可写");

        EnsureDirectoryFor(_path);

        var entries = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (File.Exists(_path))
        {
            var readEncoding = DetectEncodingEnhanced(_path);
            var lines = await File.ReadAllLinesAsync(_path, readEncoding, cancellationToken).ConfigureAwait(false);

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // 跳过空行和注释
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var (key, value) = ParseEnvLine(trimmed);
                if (key != null)
                {
                    entries[key] = value;
                }
            }
        }

        // 应用变更
        foreach (var (key, value) in changes)
        {
            // 将配置键中的 : 转换为 __ (双下划线)，这是 .env 文件的常见约定
            var envKey = key.Replace(":", "__");
            
            if (value == null)
                entries.Remove(envKey);
            else
                entries[envKey] = value;
        }

        // 写入文件
        var sb = new System.Text.StringBuilder();
        foreach (var (key, value) in entries)
        {
            if (value != null)
            {
                // 如果值包含特殊字符，使用双引号包裹
                var formattedValue = NeedsQuoting(value) ? $"\"{EscapeValue(value)}\"" : value;
                sb.Append(key).Append('=').Append(formattedValue).AppendLine();
            }
        }

        await File.WriteAllTextAsync(_path, sb.ToString(), GetWriteEncoding(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 解析 .env 文件行
    /// </summary>
    private static (string? Key, string? Value) ParseEnvLine(string line)
    {
        // 支持 export KEY=VALUE 格式
        if (line.StartsWith("export ", StringComparison.OrdinalIgnoreCase))
        {
            line = line.Substring(7).TrimStart();
        }

        var idx = line.IndexOf('=');
        if (idx <= 0)
            return (null, null);

        var key = line.Substring(0, idx).Trim();
        var value = line.Substring(idx + 1);

        // 处理引号包裹的值
        value = UnquoteValue(value);

        return (key, value);
    }

    /// <summary>
    /// 移除值的引号并处理转义
    /// </summary>
    private static string UnquoteValue(string value)
    {
        value = value.Trim();

        if (value.Length >= 2)
        {
            // 双引号
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
                // 处理转义字符
                value = value
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "\t")
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\");
            }
            // 单引号（不处理转义）
            else if (value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }
        }

        return value;
    }

    /// <summary>
    /// 检查值是否需要引号包裹
    /// </summary>
    private static bool NeedsQuoting(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        // 包含空格、引号、换行符等特殊字符时需要引号
        return value.Contains(' ') ||
               value.Contains('"') ||
               value.Contains('\'') ||
               value.Contains('\n') ||
               value.Contains('\r') ||
               value.Contains('\t') ||
               value.Contains('#') ||
               value.StartsWith(" ") ||
               value.EndsWith(" ");
    }

    /// <summary>
    /// 转义值中的特殊字符
    /// </summary>
    private static string EscapeValue(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }
}

/// <summary>
/// .env 文件配置源（用于 Microsoft.Extensions.Configuration）
/// </summary>
internal sealed class EnvConfigurationSource : FileConfigurationSource
{
    /// <summary>
    /// 是否将配置写入系统环境变量
    /// </summary>
    public bool SetEnvironmentVariables { get; set; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        // 不调用 EnsureDefaults，因为它可能会覆盖我们设置的 FileProvider
        // 只需要确保 OnLoadException 有默认值
        if (OnLoadException == null && !Optional)
        {
            OnLoadException = context => { };
        }
        return new EnvConfigurationProvider(this);
    }
}

/// <summary>
/// .env 文件配置提供程序
/// </summary>
internal sealed class EnvConfigurationProvider : FileConfigurationProvider
{
    private readonly bool _setEnvironmentVariables;

    public EnvConfigurationProvider(EnvConfigurationSource source) : base(source)
    {
        _setEnvironmentVariables = source.SetEnvironmentVariables;
    }

    public override void Load(Stream stream)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        using var reader = new StreamReader(stream);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var trimmed = line.Trim();

            // 跳过空行和注释
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                continue;

            var (key, value) = ParseEnvLine(trimmed);
            if (key != null)
            {
                // 将 __ 转换为 : 以支持嵌套配置
                var configKey = key.Replace("__", ConfigurationPath.KeyDelimiter);
                data[configKey] = value;

                // 如果启用了写入环境变量，则设置系统环境变量
                if (_setEnvironmentVariables && value != null)
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        Data = data;
    }

    /// <summary>
    /// 解析 .env 文件行
    /// </summary>
    private static (string? Key, string? Value) ParseEnvLine(string line)
    {
        // 支持 export KEY=VALUE 格式
        if (line.StartsWith("export ", StringComparison.OrdinalIgnoreCase))
        {
            line = line.Substring(7).TrimStart();
        }

        var idx = line.IndexOf('=');
        if (idx <= 0)
            return (null, null);

        var key = line.Substring(0, idx).Trim();
        var value = line.Substring(idx + 1);

        // 处理引号包裹的值
        value = UnquoteValue(value);

        return (key, value);
    }

    /// <summary>
    /// 移除值的引号并处理转义
    /// </summary>
    private static string UnquoteValue(string value)
    {
        value = value.Trim();

        if (value.Length >= 2)
        {
            // 双引号
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                value = value.Substring(1, value.Length - 2);
                // 处理转义字符
                value = value
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "\t")
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\");
            }
            // 单引号（不处理转义）
            else if (value.StartsWith("'") && value.EndsWith("'"))
            {
                value = value.Substring(1, value.Length - 2);
            }
        }

        return value;
    }
}
