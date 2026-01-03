# Database 配置源

数据库配置源支持从关系型数据库读取和写入配置，适合需要集中管理配置的场景。

## 安装

```bash
dotnet add package Apq.Cfg.Database
```

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Database` (100)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 100
.AddDatabase(options => { ... })

// 指定自定义层级
.AddDatabase(options => { ... }, level: 150)
```

## 支持的数据库

| 数据库 | Provider 值 |
|--------|-------------|
| Microsoft SQL Server | `SqlServer` |
| MySQL | `MySql` |
| PostgreSQL | `PostgreSQL` 或 `Postgres` |
| Oracle | `Oracle` |
| SQLite | `SQLite` |

## 基本用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Database;

var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;Trusted_Connection=True;";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    })  // 使用默认层级 100
    .Build();

// 读取配置
var connStr = cfg["Database:ConnectionString"];
var timeout = cfg.GetValue<int>("Database:Timeout");
```

## 方法签名

```csharp
public static CfgBuilder AddDatabase(
    this CfgBuilder builder,
    Action<DatabaseOptions> configure,
    int level,
    bool isPrimaryWriter = false)
```

## 配置选项

### 完整配置

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
        options.CommandTimeoutMs = 5000;
    }, isPrimaryWriter: true)  // 使用默认层级 100
    .Build();
```

### 选项说明

| 选项 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Provider` | string? | null | 数据库提供程序名称 |
| `ConnectionString` | string? | null | 数据库连接字符串 |
| `Table` | string? | null | 配置表名 |
| `KeyColumn` | string? | null | 键列名 |
| `ValueColumn` | string? | null | 值列名 |
| `CommandTimeoutMs` | int | 5000 | 命令超时时间（毫秒） |

## 数据表结构

### SQL Server

```sql
CREATE TABLE AppConfig (
    ConfigKey NVARCHAR(256) PRIMARY KEY,
    ConfigValue NVARCHAR(MAX)
);
```

### MySQL

```sql
CREATE TABLE app_config (
    config_key VARCHAR(256) PRIMARY KEY,
    config_value TEXT
);
```

### PostgreSQL

```sql
CREATE TABLE app_config (
    config_key VARCHAR(256) PRIMARY KEY,
    config_value TEXT
);
```

### SQLite

```sql
CREATE TABLE Config (
    Key TEXT PRIMARY KEY,
    Value TEXT
);
```

## 各数据库示例

### SQL Server

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=.;Database=MyDb;Trusted_Connection=True;";
        options.Table = "AppConfig";
        options.KeyColumn = "Key";
        options.ValueColumn = "Value";
    })  // 使用默认层级 100
    .Build();
```

### MySQL

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "MySql";
        options.ConnectionString = "Server=localhost;Database=mydb;User=root;Password=...;";
        options.Table = "app_config";
        options.KeyColumn = "config_key";
        options.ValueColumn = "config_value";
    })  // 使用默认层级 100
    .Build();
```

### PostgreSQL

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "PostgreSQL";
        options.ConnectionString = "Host=localhost;Database=mydb;Username=postgres;Password=...;";
        options.Table = "app_config";
        options.KeyColumn = "config_key";
        options.ValueColumn = "config_value";
    })  // 使用默认层级 100
    .Build();
```

### Oracle

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "Oracle";
        options.ConnectionString = "Data Source=localhost:1521/ORCL;User Id=user;Password=...;";
        options.Table = "APP_CONFIG";
        options.KeyColumn = "CONFIG_KEY";
        options.ValueColumn = "CONFIG_VALUE";
    })  // 使用默认层级 100
    .Build();
```

### SQLite

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SQLite";
        options.ConnectionString = "Data Source=config.db";
        options.Table = "Config";
        options.KeyColumn = "Key";
        options.ValueColumn = "Value";
    })  // 使用默认层级 100
    .Build();
```

## 可写配置

数据库配置源支持写入配置：

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, isPrimaryWriter: true)  // 使用默认层级 100
    .Build();

// 写入配置
cfg["App:Version"] = "2.0.0";
await cfg.SaveAsync();
```

## 与本地配置混合使用

```csharp
var cfg = new CfgBuilder()
    // 本地基础配置
    .AddJson("config.json")
    // 数据库配置（使用默认层级 100）
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, isPrimaryWriter: true)
    // 环境变量（使用默认层级 400）
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## 初始化数据

可以使用 SQL 脚本初始化配置数据：

```sql
-- SQL Server
INSERT INTO AppConfig (ConfigKey, ConfigValue) VALUES
('Database:Host', 'localhost'),
('Database:Port', '5432'),
('Database:Name', 'mydb'),
('App:Name', 'MyApplication'),
('App:Version', '1.0.0');
```

## 下一步

- [Redis 配置源](/config-sources/redis) - Redis 键值存储
- [Consul 配置源](/config-sources/consul) - HashiCorp Consul
