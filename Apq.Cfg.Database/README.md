# Apq.Cfg.Database

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

数据库配置源扩展包，支持多种数据库。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 依赖

- Apq.Cfg
- SqlSugarCore 5.1.4.210

## 支持的数据库

- SQL Server
- MySQL
- PostgreSQL
- Oracle
- SQLite

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Database;

var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, level: 1, isPrimaryWriter: true)
    .Build();
```

## 方法签名

```csharp
public static CfgBuilder AddDatabase(
    this CfgBuilder builder,
    Action<DatabaseOptions> configure,
    int level,
    bool isPrimaryWriter = false)
```

## DatabaseOptions

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `Provider` | `string?` | - | 数据库提供程序名称 |
| `ConnectionString` | `string?` | - | 数据库连接字符串 |
| `Table` | `string?` | - | 配置表名 |
| `KeyColumn` | `string?` | - | 键列名 |
| `ValueColumn` | `string?` | - | 值列名 |
| `CommandTimeoutMs` | `int` | 5000 | 命令超时（毫秒） |

## Provider 值

| 值 | 数据库 |
|------|------|
| `SqlServer` | Microsoft SQL Server |
| `MySql` | MySQL |
| `PostgreSQL` 或 `Postgres` | PostgreSQL |
| `Oracle` | Oracle |
| `SQLite` | SQLite |

## 数据表结构

```sql
CREATE TABLE AppConfig (
    ConfigKey NVARCHAR(256) PRIMARY KEY,
    ConfigValue NVARCHAR(MAX)
);
```

## 示例

### SQL Server

```csharp
.AddDatabase(options =>
{
    options.Provider = "SqlServer";
    options.ConnectionString = "Server=.;Database=MyDb;Trusted_Connection=True;";
    options.Table = "AppConfig";
    options.KeyColumn = "Key";
    options.ValueColumn = "Value";
}, level: 1)
```

### MySQL

```csharp
.AddDatabase(options =>
{
    options.Provider = "MySql";
    options.ConnectionString = "Server=localhost;Database=mydb;User=root;Password=...;";
    options.Table = "app_config";
    options.KeyColumn = "config_key";
    options.ValueColumn = "config_value";
}, level: 1)
```

### SQLite

```csharp
.AddDatabase(options =>
{
    options.Provider = "SQLite";
    options.ConnectionString = "Data Source=config.db";
    options.Table = "Config";
    options.KeyColumn = "Key";
    options.ValueColumn = "Value";
}, level: 1)
```

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
