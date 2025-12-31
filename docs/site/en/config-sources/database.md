# Database Configuration Source

The database configuration source supports reading and writing configuration from relational databases, suitable for scenarios requiring centralized configuration management.

## Installation

```bash
dotnet add package Apq.Cfg.Database
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
    }, level: 1)
    .Build();

// Read configuration
var connStr = cfg.Get("Database:ConnectionString");
var timeout = cfg.Get<int>("Database:Timeout");
```

## Method Signature

```csharp
public static CfgBuilder AddDatabase(
    this CfgBuilder builder,
    Action<DatabaseOptions> configure,
    int level,
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
    }, level: 1, isPrimaryWriter: true)
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
    }, level: 1)
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
    }, level: 1)
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
    }, level: 1)
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
    }, level: 1)
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
    }, level: 1)
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
    }, level: 1, isPrimaryWriter: true)
    .Build();

// Write configuration
cfg.Set("App:Version", "2.0.0");
await cfg.SaveAsync();
```

## Mixed with Local Configuration

```csharp
var cfg = new CfgBuilder()
    // Local base configuration
    .AddJson("config.json", level: 0, writeable: false)
    // Database configuration (higher priority)
    .AddDatabase(options =>
    {
        options.Provider = "SqlServer";
        options.ConnectionString = "Server=localhost;Database=ConfigDb;...";
        options.Table = "AppConfig";
        options.KeyColumn = "ConfigKey";
        options.ValueColumn = "ConfigValue";
    }, level: 10, isPrimaryWriter: true)
    // Environment variables (highest priority)
    .AddEnvironmentVariables(level: 20, prefix: "APP_")
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
