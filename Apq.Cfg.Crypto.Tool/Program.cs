using System.CommandLine;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Apq.Cfg.Crypto.AesGcm;

namespace Apq.Cfg.Crypto.Tool;

/// <summary>
/// Apq.Cfg 配置加密命令行工具
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Apq.Cfg 配置加密命令行工具");

        // generate-key 命令
        var generateKeyCommand = CreateGenerateKeyCommand();
        rootCommand.AddCommand(generateKeyCommand);

        // encrypt 命令
        var encryptCommand = CreateEncryptCommand();
        rootCommand.AddCommand(encryptCommand);

        // decrypt 命令
        var decryptCommand = CreateDecryptCommand();
        rootCommand.AddCommand(decryptCommand);

        // encrypt-file 命令
        var encryptFileCommand = CreateEncryptFileCommand();
        rootCommand.AddCommand(encryptFileCommand);

        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// 创建 generate-key 命令
    /// </summary>
    static Command CreateGenerateKeyCommand()
    {
        var algorithmOption = new Option<string>(
            aliases: new[] { "--algorithm", "-a" },
            getDefaultValue: () => "aes-gcm",
            description: "加密算法 (aes-gcm)");

        var bitsOption = new Option<int>(
            aliases: new[] { "--bits", "-b" },
            getDefaultValue: () => 256,
            description: "密钥位数 (128, 192, 256)");

        var command = new Command("generate-key", "生成加密密钥")
        {
            algorithmOption,
            bitsOption
        };

        command.SetHandler((algorithm, bits) =>
        {
            if (algorithm.ToLower() != "aes-gcm")
            {
                Console.Error.WriteLine($"不支持的算法: {algorithm}，目前仅支持 aes-gcm");
                return;
            }

            if (bits != 128 && bits != 192 && bits != 256)
            {
                Console.Error.WriteLine($"不支持的密钥位数: {bits}，支持 128, 192, 256");
                return;
            }

            var keyBytes = new byte[bits / 8];
            RandomNumberGenerator.Fill(keyBytes);
            var base64Key = Convert.ToBase64String(keyBytes);

            Console.WriteLine($"算法: {algorithm.ToUpper()}");
            Console.WriteLine($"密钥位数: {bits}");
            Console.WriteLine($"Base64 密钥: {base64Key}");
            Console.WriteLine();
            Console.WriteLine("请妥善保管此密钥，不要将其存储在配置文件中！");
            Console.WriteLine("建议使用环境变量 APQ_CFG_ENCRYPTION_KEY 存储密钥。");
        }, algorithmOption, bitsOption);

        return command;
    }

    /// <summary>
    /// 创建 encrypt 命令
    /// </summary>
    static Command CreateEncryptCommand()
    {
        var keyOption = new Option<string>(
            aliases: new[] { "--key", "-k" },
            description: "Base64 编码的加密密钥")
        { IsRequired = true };

        var valueOption = new Option<string>(
            aliases: new[] { "--value", "-v" },
            description: "要加密的明文值")
        { IsRequired = true };

        var prefixOption = new Option<string>(
            aliases: new[] { "--prefix", "-p" },
            getDefaultValue: () => "{ENC}",
            description: "加密值前缀");

        var command = new Command("encrypt", "加密配置值")
        {
            keyOption,
            valueOption,
            prefixOption
        };

        command.SetHandler((key, value, prefix) =>
        {
            try
            {
                using var provider = new AesGcmCryptoProvider(key);
                var encrypted = provider.Encrypt(value);
                Console.WriteLine($"{prefix}{encrypted}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"加密失败: {ex.Message}");
            }
        }, keyOption, valueOption, prefixOption);

        return command;
    }

    /// <summary>
    /// 创建 decrypt 命令
    /// </summary>
    static Command CreateDecryptCommand()
    {
        var keyOption = new Option<string>(
            aliases: new[] { "--key", "-k" },
            description: "Base64 编码的加密密钥")
        { IsRequired = true };

        var valueOption = new Option<string>(
            aliases: new[] { "--value", "-v" },
            description: "要解密的密文值（包含前缀）")
        { IsRequired = true };

        var prefixOption = new Option<string>(
            aliases: new[] { "--prefix", "-p" },
            getDefaultValue: () => "{ENC}",
            description: "加密值前缀");

        var command = new Command("decrypt", "解密配置值")
        {
            keyOption,
            valueOption,
            prefixOption
        };

        command.SetHandler((key, value, prefix) =>
        {
            try
            {
                if (!value.StartsWith(prefix))
                {
                    Console.Error.WriteLine($"值不包含预期的前缀 '{prefix}'");
                    return;
                }

                var cipherText = value.Substring(prefix.Length);
                using var provider = new AesGcmCryptoProvider(key);
                var decrypted = provider.Decrypt(cipherText);
                Console.WriteLine(decrypted);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"解密失败: {ex.Message}");
            }
        }, keyOption, valueOption, prefixOption);

        return command;
    }

    /// <summary>
    /// 创建 encrypt-file 命令
    /// </summary>
    static Command CreateEncryptFileCommand()
    {
        var keyOption = new Option<string>(
            aliases: new[] { "--key", "-k" },
            description: "Base64 编码的加密密钥")
        { IsRequired = true };

        var fileOption = new Option<FileInfo>(
            aliases: new[] { "--file", "-f" },
            description: "要处理的 JSON 配置文件")
        { IsRequired = true };

        var patternsOption = new Option<string>(
            aliases: new[] { "--patterns" },
            getDefaultValue: () => "*Password*,*Secret*,*ApiKey*,*ConnectionString*,*Credential*,*Token*",
            description: "敏感键模式（逗号分隔，支持通配符）");

        var prefixOption = new Option<string>(
            aliases: new[] { "--prefix", "-p" },
            getDefaultValue: () => "{ENC}",
            description: "加密值前缀");

        var outputOption = new Option<FileInfo?>(
            aliases: new[] { "--output", "-o" },
            description: "输出文件路径（默认覆盖原文件）");

        var dryRunOption = new Option<bool>(
            aliases: new[] { "--dry-run" },
            getDefaultValue: () => false,
            description: "仅显示将要加密的键，不实际修改文件");

        var command = new Command("encrypt-file", "批量加密配置文件中的敏感值")
        {
            keyOption,
            fileOption,
            patternsOption,
            prefixOption,
            outputOption,
            dryRunOption
        };

        command.SetHandler((key, file, patterns, prefix, output, dryRun) =>
        {
            try
            {
                if (!file.Exists)
                {
                    Console.Error.WriteLine($"文件不存在: {file.FullName}");
                    return;
                }

                var patternList = patterns.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();

                var json = File.ReadAllText(file.FullName);
                using var doc = JsonDocument.Parse(json);

                using var provider = new AesGcmCryptoProvider(key);
                var encryptedCount = 0;

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                {
                    encryptedCount = ProcessJsonElement(doc.RootElement, writer, "", patternList, prefix, provider, dryRun);
                }

                if (dryRun)
                {
                    Console.WriteLine($"\n共发现 {encryptedCount} 个敏感键需要加密");
                    Console.WriteLine("使用 --dry-run=false 执行实际加密");
                }
                else
                {
                    var outputPath = output?.FullName ?? file.FullName;
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);
                    var result = reader.ReadToEnd();
                    File.WriteAllText(outputPath, result);
                    Console.WriteLine($"已加密 {encryptedCount} 个敏感值");
                    Console.WriteLine($"输出文件: {outputPath}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"处理失败: {ex.Message}");
            }
        }, keyOption, fileOption, patternsOption, prefixOption, outputOption, dryRunOption);

        return command;
    }

    /// <summary>
    /// 递归处理 JSON 元素
    /// </summary>
    static int ProcessJsonElement(
        JsonElement element,
        Utf8JsonWriter writer,
        string currentPath,
        List<string> patterns,
        string prefix,
        AesGcmCryptoProvider provider,
        bool dryRun)
    {
        var encryptedCount = 0;

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in element.EnumerateObject())
                {
                    var propertyPath = string.IsNullOrEmpty(currentPath)
                        ? property.Name
                        : $"{currentPath}:{property.Name}";

                    writer.WritePropertyName(property.Name);
                    encryptedCount += ProcessJsonElement(property.Value, writer, propertyPath, patterns, prefix, provider, dryRun);
                }
                writer.WriteEndObject();
                break;

            case JsonValueKind.Array:
                writer.WriteStartArray();
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var itemPath = $"{currentPath}[{index}]";
                    encryptedCount += ProcessJsonElement(item, writer, itemPath, patterns, prefix, provider, dryRun);
                    index++;
                }
                writer.WriteEndArray();
                break;

            case JsonValueKind.String:
                var stringValue = element.GetString();
                if (stringValue != null && !stringValue.StartsWith(prefix) && ShouldEncrypt(currentPath, patterns))
                {
                    if (dryRun)
                    {
                        Console.WriteLine($"  将加密: {currentPath}");
                        writer.WriteStringValue(stringValue);
                    }
                    else
                    {
                        var encrypted = provider.Encrypt(stringValue);
                        writer.WriteStringValue($"{prefix}{encrypted}");
                    }
                    encryptedCount++;
                }
                else
                {
                    writer.WriteStringValue(stringValue);
                }
                break;

            case JsonValueKind.Number:
                if (element.TryGetInt64(out var longValue))
                    writer.WriteNumberValue(longValue);
                else if (element.TryGetDouble(out var doubleValue))
                    writer.WriteNumberValue(doubleValue);
                break;

            case JsonValueKind.True:
                writer.WriteBooleanValue(true);
                break;

            case JsonValueKind.False:
                writer.WriteBooleanValue(false);
                break;

            case JsonValueKind.Null:
                writer.WriteNullValue();
                break;
        }

        return encryptedCount;
    }

    /// <summary>
    /// 判断是否应该加密
    /// </summary>
    static bool ShouldEncrypt(string key, List<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            var regex = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";
            if (Regex.IsMatch(key, regex, RegexOptions.IgnoreCase))
                return true;
        }
        return false;
    }
}
