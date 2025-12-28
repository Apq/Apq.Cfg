# Apq.Cfg 测试

本目录包含 Apq.Cfg 的单元测试项目。

## 项目结构

```text
tests/
├── appsettings.json         # 共享测试配置文件（三个测试项目共用）
├── Directory.Build.props    # 共享构建配置
├── Apq.Cfg.Tests.Shared/    # 共享测试代码
├── Apq.Cfg.Tests.Net6/      # .NET 6 测试项目
├── Apq.Cfg.Tests.Net8/      # .NET 8 测试项目
└── Apq.Cfg.Tests.Net9/      # .NET 9 测试项目
```

## 运行测试

```bash
# 运行所有测试
dotnet test

# 运行特定框架的测试
dotnet test tests/Apq.Cfg.Tests.Net9/

# 运行特定测试类
dotnet test --filter "FullyQualifiedName~JsonCfgTests"
```

## 测试统计（共 346 个测试，41 个需外部服务）

| 测试类 | 测试数量 | 跳过 | 说明 |
|--------|----------|------|------|
| JsonCfgTests | 15 | 0 | JSON 配置源测试 |
| EnvVarsCfgTests | 4 | 0 | 环境变量配置源测试 |
| EnvCfgTests | 15 | 0 | .env 文件配置源测试 |
| IniCfgTests | 5 | 0 | INI 文件配置源测试 |
| XmlCfgTests | 5 | 0 | XML 文件配置源测试 |
| YamlCfgTests | 6 | 0 | YAML 文件配置源测试 |
| TomlCfgTests | 6 | 0 | TOML 文件配置源测试 |
| RedisCfgTests | 5 | 0 | Redis 配置源测试（✅ 已配置）|
| DatabaseCfgTests | 5 | 0 | 数据库配置源测试（✅ 已配置）|
| ZookeeperCfgTests | 6 | 6 | Zookeeper 配置中心测试（需要 Zookeeper 服务）|
| ApolloCfgTests | 6 | 6 | Apollo 配置中心测试（需要 Apollo 服务）|
| ConsulCfgTests | 6 | 6 | Consul 配置中心测试（需要 Consul 服务）|
| EtcdCfgTests | 6 | 6 | Etcd 配置中心测试（需要 Etcd 服务）|
| NacosCfgTests | 9 | 9 | Nacos 配置中心测试（需要 Nacos 服务）|
| VaultCfgTests | 8 | 8 | Vault 密钥管理测试（需要 Vault 服务）|
| CfgRootExtensionsTests | 4 | 0 | 扩展方法测试（TryGet/GetRequired）|
| CfgBuilderAdvancedTests | 14 | 0 | 高级功能测试 |
| DynamicReloadTests | 22 | 0 | 动态配置重载测试 |
| EncodingDetectionTests | 14 | 0 | 编码检测测试 |
| ConcurrencyTests | 9 | 0 | 并发安全测试 |
| BoundaryConditionTests | 25 | 0 | 边界条件测试 |
| ExceptionHandlingTests | 18 | 0 | 异常处理测试 |
| ConfigChangesSubscriptionTests | 28 | 0 | 配置变更订阅测试 |
| CfgSectionTests | 13 | 0 | 配置节（GetSection/GetChildKeys/GetOrDefault）测试 |
| ServiceCollectionExtensionsTests | 21 | 0 | 依赖注入扩展测试（IOptions/IOptionsMonitor/IOptionsSnapshot/嵌套对象/集合绑定）|
| EncodingTests | 33 | 0 | 编码映射测试 |
| PerformanceOptimizationTests | 30 | 0 | 性能优化测试（GetMany/SetMany/GetMany回调/缓存）|
| SourceGeneratorTests | 8 | 0 | 源生成器测试（[CfgSection] 特性/BindFrom/BindTo）|

### 跳过测试说明

共 41 个测试被跳过，原因是需要外部服务支持。这些测试使用 `[SkippableFact]` 特性，在未配置相应服务时自动跳过。

> **已配置服务**: Redis、Database（MySQL）

配置服务连接信息后可完整运行，配置文件位于 `tests/appsettings.json`（三个测试项目共用）：

```json
{
  "TestConnections": {
    "Redis": "localhost:6379",
    "Database": "Server=localhost;Database=ApqCfgTest;...",
    "DatabaseProvider": "SqlServer",
    "Zookeeper": "localhost:2181",
    "Apollo": {
      "AppId": "your-app-id",
      "MetaServer": "http://localhost:8080"
    },
    "Consul": {
      "Address": "http://localhost:8500"
    },
    "Etcd": {
      "ConnectionString": "http://localhost:2379"
    },
    "Nacos": {
      "ServerAddress": "localhost:8848"
    },
    "Vault": {
      "Address": "http://localhost:8200",
      "Token": "your-token"
    }
  }
}
```

## 公开 API 覆盖矩阵

| API | Json | EnvVar | .env | Ini | Xml | Yaml | Toml | Redis | DB | Zk | Apollo | Consul | Etcd | Nacos | Vault |
|-----|:----:|:------:|:----:|:---:|:---:|:----:|:----:|:-----:|:--:|:--:|:------:|:------:|:----:|:-----:|:-----:|
| **ICfgRoot** |
| `Get(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Get<T>(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | - | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `Exists(key)` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `GetMany(keys)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `GetMany<T>(keys)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `GetMany(keys, callback)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `GetMany<T>(keys, callback)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `Set(key, value)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | - | ✅ | ✅ | - | ✅ |
| `SetMany(values)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `Set(key, value, targetLevel)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `Remove(key)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | - | ✅ | ✅ | - | ✅ |
| `Remove(key, targetLevel)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `SaveAsync()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | - | ✅ | ✅ | - | ✅ |
| `SaveAsync(targetLevel)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ToMicrosoftConfiguration(options)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ConfigChanges` | ✅ | - | - | - | - | - | - | - | - | ✅ | - | ✅ | ✅ | - | - |
| `GetSection(path)` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - |
| `GetChildKeys()` | ✅ | - | ✅ | ✅ | ✅ | ✅ | ✅ | - | - | - | - | - | - | - | - |
| `Dispose/DisposeAsync` | ✅ | - | - | - | - | - | - | - | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CfgBuilder** |
| `AddJson()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddEnvironmentVariables()` | - | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddEnv()` | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddSource()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `WithEncodingConfidenceThreshold()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddReadEncodingMapping()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddWriteEncodingMapping()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ConfigureEncodingMapping()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `WithEncodingDetectionLogging()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `Build()` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **CfgRootExtensions** |
| `TryGet<T>()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `GetRequired<T>()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `GetOrDefault<T>()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| **FileCfgSourceBase** |
| `EncodingDetector` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `EncodingConfidenceThreshold` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| **扩展包** |
| `AddIni()` | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - | - |
| `AddXml()` | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - | - |
| `AddYaml()` | - | - | - | - | - | ✅ | - | - | - | - | - | - | - | - | - |
| `AddToml()` | - | - | - | - | - | - | ✅ | - | - | - | - | - | - | - | - |
| `AddRedis()` | - | - | - | - | - | - | - | ✅ | - | - | - | - | - | - | - |
| `AddDatabase()` | - | - | - | - | - | - | - | - | ✅ | - | - | - | - | - | - |
| `AddZookeeper()` | - | - | - | - | - | - | - | - | - | ✅ | - | - | - | - | - |
| `AddApollo()` | - | - | - | - | - | - | - | - | - | - | ✅ | - | - | - | - |
| `AddConsul()` | - | - | - | - | - | - | - | - | - | - | - | ✅ | - | - | - |
| `AddEtcd()` | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | - | - |
| `AddNacos()` | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ | - |
| `AddVault()` | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ |
| `AddVaultV1()` | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ |
| `AddVaultV2()` | - | - | - | - | - | - | - | - | - | - | - | - | - | - | ✅ |
| **依赖注入扩展** |
| `AddApqCfg()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `AddApqCfg<T>()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ConfigureApqCfg<T>()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `ConfigureApqCfg<T>(onChange)` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `IOptions<T>` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `IOptionsMonitor<T>` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `IOptionsSnapshot<T>` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| **源生成器** |
| `[CfgSection]` 特性 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `BindFrom()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| `BindTo()` | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 简单类型绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 嵌套对象绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 数组绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 列表绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 字典绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| 枚举绑定 | ✅ | - | - | - | - | - | - | - | - | - | - | - | - | - | - |
| **多层级覆盖** |
| 高层级覆盖低层级 | ✅ | ✅ | ✅ | - | - | - | - | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

> 说明：
> - `✅` 表示已有测试覆盖
> - `-` 表示该配置源不支持此功能（如环境变量只读、Apollo/Nacos 通常只读）或该功能只需测试一次
> - `EnvVar` = 环境变量, `.env` = .env 文件, `Zk` = Zookeeper, `DB` = Database

## 测试场景覆盖

| 场景类别 | 测试文件 | 测试数量 | 跳过 |
|----------|----------|----------|------|
| 基本读写 | JsonCfgTests, EnvCfgTests, 各格式测试 | 107 | 41 |
| 类型转换 | JsonCfgTests | 15 | 0 |
| 编码检测 | EncodingDetectionTests | 14 | 0 |
| 编码映射 | EncodingTests | 33 | 0 |
| 并发安全 | ConcurrencyTests | 9 | 0 |
| 边界条件 | BoundaryConditionTests | 25 | 0 |
| 异常处理 | ExceptionHandlingTests | 18 | 0 |
| 动态重载 | DynamicReloadTests | 22 | 0 |
| 变更订阅 | ConfigChangesSubscriptionTests | 28 | 0 |
| 配置节访问 | CfgSectionTests | 13 | 0 |
| 依赖注入 | ServiceCollectionExtensionsTests | 21 | 0 |
| 批量操作 | PerformanceOptimizationTests | 30 | 0 |
| 源生成器 | SourceGeneratorTests | 8 | 0 |

> 注：基本读写测试中 41 个跳过的测试需要外部服务（Zookeeper/Apollo/Consul/Etcd/Nacos/Vault），Redis 和 Database 已配置

## 性能基准测试

性能基准测试位于 `benchmarks/Apq.Cfg.Benchmarks/` 目录，使用 BenchmarkDotNet 框架。

| 基准测试文件 | 说明 |
|--------------|------|
| ReadWriteBenchmarks | 不同配置源的 Get/Set/Exists 性能对比 |
| CacheBenchmarks | 缓存效果测试（热路径、缓存命中/未命中）|
| TypeConversionBenchmarks | 类型转换性能测试（含 TryGet/GetRequired/GetOrDefault）|
| ConcurrencyBenchmarks | 并发读写性能测试 |
| GetSectionBenchmarks | GetSection/GetChildKeys 性能测试 |
| SaveBenchmarks | SaveAsync 持久化性能测试 |
| RemoveBenchmarks | Remove 操作性能测试 |
| MultiSourceBenchmarks | 多配置源合并性能测试 |
| LargeFileBenchmarks | 大文件配置性能测试 |
| KeyPathBenchmarks | 键路径解析性能测试 |
| BatchOperationBenchmarks | GetMany/SetMany 批量操作性能测试 |
| MicrosoftConfigBenchmarks | ToMicrosoftConfiguration/ConfigChanges 性能测试 |

运行性能测试：

```bash
cd benchmarks/Apq.Cfg.Benchmarks
dotnet run -c Release
```

## 测试覆盖率

**100%** - 所有公开 API 均已覆盖测试
