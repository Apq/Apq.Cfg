# Apq.Cfg.Samples

[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apq.Cfg 配置库的完整功能示例项目。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 项目结构

```
Apq.Cfg.Samples/
├── Program.cs                          # 入口程序，运行所有示例
├── Models/
│   └── ConfigModels.cs                 # 强类型配置模型
└── Demos/
    ├── BasicUsageDemo.cs               # 示例 1: 基础用法
    ├── MultiFormatDemo.cs              # 示例 2: 多格式支持
    ├── ConfigSectionDemo.cs            # 示例 3: 配置节访问
    ├── BatchOperationsDemo.cs          # 示例 4: 批量操作
    ├── TypeConversionDemo.cs           # 示例 5: 类型转换
    ├── DynamicReloadDemo.cs            # 示例 6: 动态重载
    ├── DependencyInjectionDemo.cs      # 示例 7: 依赖注入
    ├── EncodingMappingDemo.cs          # 示例 8: 编码映射
    ├── RedisDemo.cs                    # 示例 9: Redis 配置源
    ├── DatabaseDemo.cs                 # 示例 10: 数据库配置源
    ├── ConsulDemo.cs                   # 示例 11: Consul 配置源
    ├── EtcdDemo.cs                     # 示例 12: Etcd 配置源
    ├── NacosDemo.cs                    # 示例 13: Nacos 配置源
    ├── ApolloDemo.cs                   # 示例 14: Apollo 配置源
    ├── ZookeeperDemo.cs                # 示例 15: Zookeeper 配置源
    ├── VaultDemo.cs                    # 示例 16: HashiCorp Vault 配置源
    └── SourceGeneratorDemo.cs          # 示例 17: 源代码生成器
```

## 运行示例

```bash
# 在项目根目录运行
dotnet run --project Samples/Apq.Cfg.Samples

# 或进入示例目录运行
cd Samples/Apq.Cfg.Samples
dotnet run
```

## 示例说明

### 示例 1: 基础用法 (BasicUsageDemo)

演示 Apq.Cfg 的基本功能：
- JSON 配置文件加载
- 多层级配置覆盖（level 参数）
- 环境变量配置源
- 配置读取、写入、删除
- 转换为 Microsoft.Extensions.Configuration

### 示例 2: 多格式支持 (MultiFormatDemo)

演示不同配置文件格式的支持：
- INI 格式 (`Apq.Cfg.Ini`)
- XML 格式 (`Apq.Cfg.Xml`)
- YAML 格式 (`Apq.Cfg.Yaml`)
- TOML 格式 (`Apq.Cfg.Toml`)
- 混合多种格式的层级覆盖

### 示例 3: 配置节访问 (ConfigSectionDemo)

演示配置节（GetSection）功能：
- 使用 `GetSection` 简化嵌套配置访问
- 枚举配置节的子键 (`GetChildKeys`)
- 通过配置节修改值

### 示例 4: 批量操作 (BatchOperationsDemo)

演示批量读写操作：
- `GetMany` 批量获取配置
- `GetMany<T>` 批量获取并转换类型
- `SetMany` 批量设置配置

### 示例 5: 类型转换 (TypeConversionDemo)

演示 `Get<T>` 类型转换功能：
- 基本类型：int、long、double、decimal、bool
- 复杂类型：DateTime、Guid、枚举
- 可空类型与默认值处理

### 示例 6: 动态重载 (DynamicReloadDemo)

演示配置动态重载功能：
- `reloadOnChange` 参数启用文件监听
- `DynamicReloadOptions` 配置防抖、策略等
- `IChangeToken` 监听配置变更
- `ConfigChanges` (Rx) 订阅配置变更事件

### 示例 7: 依赖注入 (DependencyInjectionDemo)

演示与 Microsoft.Extensions.DependencyInjection 集成：
- `AddApqCfg` 注册配置服务
- `ConfigureApqCfg<T>` 绑定强类型配置
- 通过 DI 获取 `ICfgRoot`、`IConfigurationRoot`
- 通过 `IOptions<T>` 获取强类型配置

### 示例 8: 编码映射 (EncodingMappingDemo)

演示编码检测和映射功能：
- `WithEncodingConfidenceThreshold` 设置检测置信度
- `WithEncodingDetectionLogging` 启用检测日志
- `AddReadEncodingMapping` 指定读取编码
- `AddWriteEncodingMappingWildcard` 通配符映射

### 示例 9: Redis 配置源 (RedisDemo)

演示 Redis 作为配置源：
- 连接 Redis 服务器
- 从 Redis Hash 读取配置
- 配置键前缀和数据库选择
- 支持配置变更监听

### 示例 10: 数据库配置源 (DatabaseDemo)

演示数据库作为配置源：
- 支持 SqlSugar 多数据库（MySQL、PostgreSQL、SQLite 等）
- 从数据库表读取配置
- 自定义表名、键列、值列
- 支持配置变更监听

### 示例 11: Consul 配置源 (ConsulDemo)

演示 Consul KV 作为配置源：
- 连接 Consul 服务器
- 从 Consul KV 存储读取配置
- 配置键前缀过滤
- 支持 ACL Token 认证
- 支持配置变更监听

### 示例 12: Etcd 配置源 (EtcdDemo)

演示 Etcd 作为配置源：
- 连接 Etcd 集群
- 从 Etcd 键值存储读取配置
- 配置键前缀过滤
- 支持用户名密码认证
- 支持配置变更监听

### 示例 13: Nacos 配置源 (NacosDemo)

演示 Nacos 作为配置源：
- 连接 Nacos 配置中心
- 从 Nacos 读取配置
- 配置命名空间、分组、数据ID
- 支持用户名密码认证
- 支持配置变更监听

### 示例 14: Apollo 配置源 (ApolloDemo)

演示 Apollo 作为配置源：
- 连接 Apollo 配置中心
- 从 Apollo 读取配置
- 配置应用ID、集群、命名空间
- 支持 Secret 认证
- 支持配置变更监听

### 示例 15: Zookeeper 配置源 (ZookeeperDemo)

演示 Zookeeper 作为配置源：
- 连接 Zookeeper 集群
- 从 Zookeeper 节点读取配置
- 配置根路径
- 支持认证
- 支持配置变更监听

### 示例 16: HashiCorp Vault 配置源 (VaultDemo)

演示 HashiCorp Vault 作为配置源：
- 连接 Vault 服务器
- 从 Vault KV 存储读取密钥配置
- 配置挂载点和路径
- 支持 Token 认证
- 支持配置变更监听

### 示例 17: 源代码生成器 (SourceGeneratorDemo)

演示 Apq.Cfg.SourceGenerator 功能：
- 使用 `[CfgSection]` 特性标记配置类
- 自动生成配置绑定代码
- 编译时类型安全
- 减少运行时反射开销

## 配置模型

示例中使用的强类型配置模型定义在 `Models/ConfigModels.cs`：

```csharp
public class DatabaseOptions
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Name { get; set; }
}

public class LoggingOptions
{
    public string? Level { get; set; }
    public bool EnableConsole { get; set; }
}

public enum LogLevel
{
    Debug, Info, Warning, Error
}
```

## 支持的框架

- .NET 8.0
- .NET 9.0

> **注意**：由于 `dotnet-etcd` 依赖 `Microsoft.Extensions.DependencyInjection >= 10.0.1`，示例项目只支持 .NET 8.0+。
> 核心库 `Apq.Cfg` 和所有扩展库仍然支持 .NET 6.0+。

## 依赖

### 核心库
- Apq.Cfg（核心库）

### 文件格式扩展
- Apq.Cfg.Ini
- Apq.Cfg.Xml
- Apq.Cfg.Yaml
- Apq.Cfg.Toml

### 分布式配置源
- Apq.Cfg.Redis
- Apq.Cfg.Database
- Apq.Cfg.Consul
- Apq.Cfg.Etcd
- Apq.Cfg.Nacos
- Apq.Cfg.Apollo
- Apq.Cfg.Zookeeper
- Apq.Cfg.Vault

### 代码生成
- Apq.Cfg.SourceGenerator

## 注意事项

1. **分布式配置源示例**（示例 9-16）需要对应的服务运行才能正常工作
2. 示例代码中的连接地址、端口、认证信息仅为演示用途，请根据实际环境修改
3. 源代码生成器示例需要 .NET 6.0 或更高版本
