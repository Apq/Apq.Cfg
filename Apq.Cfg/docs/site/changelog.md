# 更新日志

## v1.2.1 (2026-01-18)

### 新功能

- **HOCON 配置源支持**：新增 `Apq.Cfg.Hcl` NuGet 包，支持 HOCON (Human-Optimized Config Object Notation) 格式
- **Properties 配置源支持**：新增 `Apq.Cfg.Properties` NuGet 包，支持 Java Properties 格式

### 改进

- **统一键路径分隔符**：所有配置源现在统一使用冒号 `:` 作为嵌套路径分隔符
  - HOCON 格式：`database.host` → `cfg["database:host"]`
  - Properties 格式：`[Database] Host` → `cfg["Database:Host"]`
- 更新文档站点，添加 HOCON 和 Properties 配置源页面
- 更新构建脚本，支持新项目的 NuGet 打包

### 文档

- 新增 HOCON 配置源文档
- 新增 Properties 配置源文档
- 更新 README 和配置源索引页面

## v1.2.0 (2026-01-09)

### 破坏性变更

- **文件类配置源方法重命名**：为与 `Microsoft.Extensions.Configuration` API 保持一致，文件类配置源方法名已更改：
  - `AddJson` → `AddJsonFile`
  - `AddIni` → `AddIniFile`
  - `AddXml` → `AddXmlFile`
  - `AddYaml` → `AddYamlFile`
  - `AddToml` → `AddTomlFile`
  - `AddEnv` → `AddEnvFile`

### 迁移指南

```csharp
// 旧代码
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddYaml("config.yaml")
    .AddEnv(".env")
    .Build();

// 新代码
var cfg = new CfgBuilder()
    .AddJsonFile("config.json")
    .AddYamlFile("config.yaml")
    .AddEnvFile(".env")
    .Build();
```

## v1.1.8 (2026-01-09)

### 新功能

- **WebUI 添加配置功能**：支持在可写配置源中添加新配置项，包括根级配置
- **只读配置源标识**：配置源列表中显示只读标识，区分可写和只读配置源

### 改进

- **中文内容保存优化**：JSON 文件保存时直接显示中文字符，不再使用 Unicode 转义序列
- **WebUI 界面优化**：
  - 配置源侧边栏支持折叠/展开
  - 刷新按钮分离，操作更清晰
  - 有值的叶子节点不再显示添加按钮
- **认证问题修复**：修复 WebApi 认证相关问题

## v1.1.7 (2026-01-06)

### 新功能

- **CSV 导出格式**：导入导出功能新增 CSV 格式支持
- **拖放导入**：支持拖放文件进行配置导入
- **WebApiDemo 示例**：新增完整的 WebApi 使用示例项目

### 改进

- **项目结构重组**：拆分为两个解决方案（核心库和 WebUI），优化构建流程
- **WebUI 架构调整**：改为纯静态站点，简化部署
- **CORS 配置**：添加 WebApi 时自动配置 CORS
- **API 文档**：根据框架版本自动选择 Swagger 或 Scalar 文档

## v1.1.6 (2026-01-05)

### 改进

- **NuGet 包依赖修复**：修复依赖包版本问题，确保依赖关系正确
- **构建优化**：添加 `-f net10.0` 指定目标框架，优化构建流程
- **包管理**：设置 NuGet 包的 list 状态

## v1.1.5 (2026-01-04)

### 新功能

- **OpenAPI 支持**：WebApi 项目集成 OpenAPI（Swagger）文档，方便 API 调试和集成
- **虚拟目录部署**：WebUI 支持从任意虚拟目录访问，无需额外配置，同一构建产物可部署到不同路径

### 改进

- **API 方法重命名**：`GetOrDefault` 方法重命名为 `GetValue`，语义更清晰
- **构建流程优化**：使用文本文件统一管理需要打包的项目列表
- **单元测试**：测试用例增加至 552 个，提高代码覆盖率
- **前端依赖更新**：更新 WebUI 前端依赖包至最新版本

## v1.1.4 (2026-01-03)

### 新功能

- **Web API 项目**：新增 `Apq.Cfg.WebApi` NuGet 包，提供 RESTful API 接口用于远程配置管理
- **Web 管理界面**：新增 `Apq.Cfg.WebUI` 项目，提供集中管理多个应用配置的 Web 界面（Docker 部署）

### 改进

- **ICfgSource 接口增强**：
  - 新增 `Name` 属性（配置源名称，同一层级内唯一）
  - 新增 `Type` 属性（配置源类型名称）
  - 新增 `KeyCount` 属性（配置项数量）
  - 新增 `TopLevelKeyCount` 属性（顶级配置键数量）
  - 新增 `GetAllValues()` 方法（获取所有配置值）
- **IWritableCfgSource 接口重构**：
  - 移除 `SetValue`、`Remove`、`SaveAsync` 方法
  - 新增 `ApplyChangesAsync(IReadOnlyDictionary<string, string?> changes, CancellationToken ct)` 方法
- **ConfigSourceInfo 类**：移动到 `Apq.Cfg.Sources` 命名空间
- 更新文档站点，添加 WebApi 和 WebUI 项目说明
- 更新构建脚本，支持 WebApi 项目的 NuGet 打包

## v1.1.3 (2026-01-03)

### 重大变更

- **API 简化**：移除 `ICfgRoot` 和 `ICfgSection` 接口中的 `Get(string key)` 方法
  - 使用索引器 `cfg["key"]` 或 `section["key"]` 替代
  - 索引器现在同时支持读取和写入操作
- **方法重命名**：
  - `Set(string key, string? value, int? targetLevel)` → `SetValue(...)`
  - `SetMany(...)` → `SetManyValues(...)`

### 改进

- 简化 `GetValue<T>` 实现，内部复用索引器
- 更新源生成器（CodeEmitter）使用索引器语法

### 迁移指南

```csharp
// 旧代码
var value = cfg.Get("Key");
cfg.Set("Key", "Value");
cfg.SetMany(dict);

// 新代码
var value = cfg["Key"];
cfg.SetValue("Key", "Value");
cfg.SetManyValues(dict);
```

## v1.1.2 (2026-01-02)

### 新功能

- **默认层级功能**：新增 `CfgSourceLevels` 静态类，为每种配置源定义默认层级
  - 本地文件配置源（Json、Ini、Xml、Yaml、Toml）：层级 0
  - 远程存储（Redis、Database）：层级 100
  - 配置中心（Consul、Etcd、Nacos、Apollo、Zookeeper）：层级 200
  - 密钥管理（Vault）：层级 300
  - 环境相关（Env、EnvironmentVariables）：层级 400
- **索引器支持**：`ICfgSection` 新增索引器访问方式，支持通过 `section[index]` 访问数组元素
- **配置验证功能**：支持多种验证规则（Required、Range、Regex、OneOf、Length、DependsOn、Custom）
- **变量替换功能**：模板引擎支持 `${Key}`、`${ENV:Name}`、`${SYS:Property}` 语法引用配置值

### 改进

- 更新所有配置源文档，添加默认层级说明
- 更新中英文文档站点的 config-sources 和 examples 目录
- 代码示例统一使用默认层级

## v1.1.1 (2026-01-01)

### 改进
- 修正文档中的 .NET 版本描述
- 移除文档中对 .NET 6.0/7.0/9.0 的过时引用

## v1.1.0 (2026-01-01)

### 重大变更
- **目标框架调整**：仅支持 .NET 8.0 和 .NET 10.0 (LTS 版本)
- 移除 .NET 6.0/7.0/9.0 支持

### 改进
- 依赖版本策略优化：`Microsoft.Extensions.*` 包根据目标框架使用匹配版本
  - net8.0 → 8.0.0
  - net10.0 → 10.0.1
- 性能测试更新至 .NET 10.0

## v1.0.6 (2025-12-30)

### 新功能
- 添加英文文档支持
- SEO 优化（meta 标签、sitemap）

### 改进
- 完善加密脱敏文档
- 优化文档结构

## v1.0.5 (2025-12-29)

### 新功能
- 添加配置加密和脱敏功能（Apq.Cfg.Crypto）
- 支持多种加密算法（AES-GCM、AES-CBC、ChaCha20、RSA、SM4）
- 添加敏感配置脱敏功能

### 改进
- 性能优化
- 提高单元测试覆盖率
- 完善性能测试

## v1.0.4 (2025-12-28)

### 新功能
- 添加 Nacos 配置源支持
- 添加 Apollo 配置源支持
- 添加 Etcd 配置源支持
- 添加 Zookeeper 配置源支持

### 改进
- 优化远程配置源的热重载机制

## v1.0.3 (2025-12-26)

### 新功能
- 添加 Vault 配置源支持
- 添加 Redis 配置源支持
- 添加 Database 配置源支持

### 改进
- 优化编码检测准确性
- 改进错误消息

## v1.0.2 (2025-12-24)

### 新功能
- 添加 TOML 配置源支持
- 添加 INI 配置源支持
- 添加 XML 配置源支持
- 支持配置变更事件

### 改进
- 优化内存使用
- 改进动态重载稳定性

## v1.0.1 (2025-12-22)

### 新功能
- 添加 Consul 配置源支持
- 添加 YAML 配置源支持
- 添加 .env 文件支持
- 支持配置节绑定

### 修复
- 修复并发读取问题
- 修复配置重载内存泄漏

## v1.0.0 (2025-12-22)

### 首次发布
- 核心配置组件功能
- JSON 配置源支持
- 环境变量配置源支持
- 多配置源层级合并
- 动态重载支持
- 可写配置支持
- 依赖注入集成
- 源生成器支持（Native AOT）

## v0.0.1 (2025-12-20)

### 初始版本
- 项目初始化
