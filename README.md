# Apq.Cfg

统一配置管理系统，支持多种配置格式和多层级配置合并。

## 项目结构

```text
Apq.Cfg/
├── Apq.Cfg/                 # 核心库（JSON + 环境变量）
├── Apq.Cfg.Ini/             # INI 文件扩展
├── Apq.Cfg.Xml/             # XML 文件扩展
├── Apq.Cfg.Yaml/            # YAML 文件扩展
├── Apq.Cfg.Toml/            # TOML 文件扩展
├── Apq.Cfg.Redis/           # Redis 扩展
├── Apq.Cfg.Database/        # 数据库扩展
├── Samples/                 # 示例项目
│   └── Apq.Cfg.Samples/
├── tests/                   # 单元测试
│   ├── Apq.Cfg.Tests.Shared/    # 共享测试代码
│   ├── Apq.Cfg.Tests.Net6/      # .NET 6 测试项目
│   ├── Apq.Cfg.Tests.Net8/      # .NET 8 测试项目
│   └── Apq.Cfg.Tests.Net9/      # .NET 9 测试项目
├── benchmarks/              # 性能测试
│   └── Apq.Cfg.Benchmarks/      # 性能测试项目（多目标框架）
├── buildTools/              # 构建工具脚本
└── nupkgs/                  # NuGet 包输出目录
```

## 特性

- 多格式支持（JSON、INI、XML、YAML、TOML、Redis、数据库）
- 智能编码检测与统一 UTF-8 写入
- 多层级配置合并
- 可写配置与热重载
- 线程安全（支持多线程并发读写）
- Microsoft.Extensions.Configuration 兼容

## 支持的框架

.NET 6.0 / 7.0 / 8.0 / 9.0

## 快速开始

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddJson("appsettings.json", level: 0, writeable: false)
    .AddJson("appsettings.local.json", level: 1, writeable: true, isPrimaryWriter: true)
    .AddEnvironmentVariables(level: 2, prefix: "APP_")
    .Build();

// 读取配置
var value = cfg.Get("Database:ConnectionString");

// 修改配置
cfg.Set("App:LastRun", DateTime.Now.ToString());
await cfg.SaveAsync();
```

## 构建与测试

```bash
# 构建
dotnet build

# 运行单元测试
dotnet test

# 运行性能测试（需要管理员权限以获得准确结果）
cd benchmarks/Apq.Cfg.Benchmarks
dotnet run -c Release
```

## 测试覆盖情况

### 测试统计（共 54 个测试）

| 测试类 | 测试数量 | 说明 |
|--------|----------|------|
| JsonCfgTests | 8 | JSON 配置源测试 |
| EnvVarsCfgTests | 4 | 环境变量配置源测试 |
| IniCfgTests | 5 | INI 文件配置源测试 |
| XmlCfgTests | 5 | XML 文件配置源测试 |
| YamlCfgTests | 6 | YAML 文件配置源测试 |
| TomlCfgTests | 6 | TOML 文件配置源测试 |
| RedisCfgTests | 5 | Redis 配置源测试 |
| DatabaseCfgTests | 5 | 数据库配置源测试 |
| CfgRootExtensionsTests | 4 | 扩展方法测试 |
| CfgBuilderAdvancedTests | 6 | 高级功能测试 |

### 公开 API 覆盖矩阵

| API | Json | Env | Ini | Xml | Yaml | Toml | Redis | DB |
|-----|:----:|:---:|:---:|:---:|:----:|:----:|:-----:|:--:|
| **ICfgRoot** |
| `Get(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Get<T>(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | - | - |
| `Exists(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Set(key, value)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Set(key, value, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `Remove(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Remove(key, targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `SaveAsync()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `SaveAsync(targetLevel)` | ✅ | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration()` | ✅ | - | - | - | - | - | - | - |
| **CfgBuilder** |
| `AddJson()` | ✅ | - | - | - | - | - | - | - |
| `AddEnvironmentVariables()` | - | ✅ | - | - | - | - | - | - |
| `WithEncodingConfidenceThreshold()` | ✅ | - | - | - | - | - | - | - |
| `Build()` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CfgRootExtensions** |
| `TryGet<T>()` | ✅ | - | - | - | - | - | - | - |
| `GetRequired<T>()` | ✅ | - | - | - | - | - | - | - |
| **扩展包** |
| `AddIni()` | - | - | ✅ | - | - | - | - | - |
| `AddXml()` | - | - | - | ✅ | - | - | - | - |
| `AddYaml()` | - | - | - | - | ✅ | - | - | - |
| `AddToml()` | - | - | - | - | - | ✅ | - | - |
| `AddRedis()` | - | - | - | - | - | - | ✅ | - |
| `AddDatabase()` | - | - | - | - | - | - | - | ✅ |
| **多层级覆盖** |
| 高层级覆盖低层级 | ✅ | ✅ | - | - | - | - | ✅ | ✅ |

> 说明：`-` 表示该配置源不支持此功能（如环境变量只读）或该功能只需测试一次

### 测试覆盖率

**100%** - 所有公开 API 均已覆盖测试

### 多框架测试通过情况

| 框架 | 测试数量 | 状态 | 测试日期 |
|------|----------|------|----------|
| .NET 6.0 | 54 | ✅ 全部通过 | 2025-12-23 |
| .NET 8.0 | 54 | ✅ 全部通过 | 2025-12-23 |
| .NET 9.0 | 54 | ✅ 全部通过 | 2025-12-23 |

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com
