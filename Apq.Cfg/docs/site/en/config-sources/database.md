# Database Configuration Source

The database configuration source supports reading and writing configuration from relational databases, suitable for scenarios requiring centralized configuration management.

## Installation

```bash
dotnet add package Apq.Cfg.Database
```

## Default Level

The default level for this configuration source is `CfgSourceLevels.Database` (100).

If you don't specify the `level` parameter, the default level will be used:

```csharp
// Uses default level 100
.AddDatabase(options => { ... })

// Specify custom level
.AddDatabase(options => { ... }, level: 150)
```

## Supported Databases

| Database | Provider Value |
|----------|----------------|
| Microsoft SQL Server | `SqlServer` |
| MySQL | `MySql` |
| PostgreSQL | `PostgreSQL` or `Postgres` |
| Oracle | `Oracle` |
| SQLite | `SQLite` |

## Basic Usage

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
    })  // Uses default level 100
    .Build();

// Read configuration
var connStr = cfg["Database:ConnectionString"];
var timeout = cfg.GetValue<int>("Database:Timeout");
```

## Method Signature

```csharp
public static CfgBuilder AddDatabase(
    this CfgBuilder builder,
    Action<DatabaseOptions> configure,
    int? level = null,
    bool isPrimaryWriter = false)
```

## Configuration Options

### Full Configuration

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
    }, isPrimaryWriter: true)  // Uses default level 100
    .Build();
```

### Options Description

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Provider` | string? | null | Database provider name |
| `ConnectionString` | string? | null | Database connection string |
| `Table` | string? | null | Configuration table name |
| `KeyColumn` | string? | null | Key column name |
| `ValueColumn` | string? | null | Value column name |
| `CommandTimeoutMs` | int | 5000 | Command timeout (milliseconds) |

## Table Structure

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

## Database Examples

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
    })  // Uses default level 100
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
    })  // Uses default level 100
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
    })  // Uses default level 100
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
    })  // Uses default level 100
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
    })  // Uses default level 100
    .Build();
```

## Writable Configuration

The database configuration source supports writing configuration:

```csharp
var cfg = new CfgBuilder()
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, isPrimaryWriter: true)  // Uses default level 100
    .Build();

// Write configuration
cfg["App:Version"] = "2.0.0";
await cfg.SaveAsync();
```

## Mixed with Local Configuration

```csharp
var cfg = new CfgBuilder()
    // Local base configuration (uses default level 0)
    .AddJsonFile("config.json")
    // Database configuration (uses default level 100)
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, isPrimaryWriter: true)
    // Environment variables (uses default level 400)
    .AddEnvironmentVariables(prefix: "APP_")
    .Build();
```

## Initialize Data

You can use SQL scripts to initialize configuration data:

```sql
-- SQL Server
INSERT INTO AppConfig (ConfigKey, ConfigValue) VALUES
('Database:Host', 'localhost'),
('Database:Port', '5432'),
('Database:Name', 'mydb'),
('App:Name', 'MyApplication'),
('App:Version', '1.0.0');
```

## Next Steps

- [Redis Configuration Source](/en/config-sources/redis) - Redis key-value storage
- [Consul Configuration Source](/en/config-sources/consul) - HashiCorp Consul
