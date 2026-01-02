# 环境变量配置源

环境变量是一种常用的配置方式，特别适合容器化部署和 CI/CD 环境。

## 核心包支持

环境变量配置源包含在核心包中，无需额外安装：

```bash
dotnet add package Apq.Cfg
```

## 默认层级

环境变量和 .env 文件的默认层级为 `CfgSourceLevels.EnvironmentVariables` / `CfgSourceLevels.Env` (400)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 400
.AddEnvironmentVariables(prefix: "APP_")
.AddEnv(".env")

// 指定自定义层级
.AddEnvironmentVariables(level: 500, prefix: "APP_")
.AddEnv(".env", level: 450)
```

## 基本用法

### 读取系统环境变量

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddEnvironmentVariables(prefix: "APP_")  // 使用默认层级 400
    .Build();
```

### 带前缀过滤

```csharp
// 只读取以 MYAPP_ 开头的环境变量
var cfg = new CfgBuilder()
    .AddEnvironmentVariables(prefix: "MYAPP_")
    .Build();

// 环境变量 MYAPP_DATABASE__HOST 映射为 Database:Host
var host = cfg["Database:Host"];
```

## .env 文件支持

Apq.Cfg.Env 包提供 `.env` 文件支持：

```bash
dotnet add package Apq.Cfg.Env
```

```csharp
using Apq.Cfg;
using Apq.Cfg.Env;

var cfg = new CfgBuilder()
    .AddEnv(".env")  // 使用默认层级 400
    .AddEnv(".env.local", level: 401, optional: true)
    .Build();
```

## .env 文件格式

```bash
# 这是注释
APP_NAME=MyApp
APP_DEBUG=true

# 数据库配置（使用 __ 表示嵌套）
DATABASE__HOST=localhost
DATABASE__PORT=5432
DATABASE__NAME=mydb

# 支持引号包裹的值
MESSAGE="Hello, World!"
MULTILINE="Line1\nLine2"

# 支持单引号（不处理转义）
RAW_VALUE='Hello\nWorld'

# 支持 export 前缀
export API_KEY=secret123
```

## 键路径映射

环境变量使用双下划线 `__` 来表示配置层级：

| 环境变量 | 配置键 |
|----------|--------|
| `APP_NAME` | `APP_NAME` |
| `DATABASE__HOST` | `DATABASE:HOST` |
| `DATABASE__CONNECTION__STRING` | `DATABASE:CONNECTION:STRING` |

### 带前缀的映射

当使用 `prefix` 参数时，前缀会被移除：

| 环境变量 | 前缀 | 配置键 |
|----------|------|--------|
| `MYAPP_NAME` | `MYAPP_` | `NAME` |
| `MYAPP_DATABASE__HOST` | `MYAPP_` | `DATABASE:HOST` |

## 方法签名

### AddEnvironmentVariables

```csharp
public static CfgBuilder AddEnvironmentVariables(
    this CfgBuilder builder,
    int level,
    string? prefix = null)
```

### AddEnv (.env 文件)

```csharp
public static CfgBuilder AddEnv(
    this CfgBuilder builder,
    string path,
    int level,
    bool writeable = false,
    bool optional = true,
    bool reloadOnChange = true,
    bool isPrimaryWriter = false,
    bool setEnvironmentVariables = false)
```

## 参数说明

| 参数 | 说明 |
|------|------|
| `level` | 配置层级，数值越大优先级越高 |
| `prefix` | 环境变量前缀过滤 |
| `path` | .env 文件路径 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `setEnvironmentVariables` | 是否将配置写入系统环境变量（默认为 false） |

## 典型用法

### 开发环境

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddJson("config.Development.json", level: 1, optional: true)
    .AddEnv(".env", level: 2, optional: true)
    .AddEnv(".env.local", level: 3, optional: true)
    .AddEnvironmentVariables(prefix: "APP_")  // 使用默认层级 400
    .Build();
```

### Docker 部署

```dockerfile
# Dockerfile
ENV APP_DATABASE__HOST=db
ENV APP_DATABASE__PORT=5432
ENV APP_REDIS__HOST=redis
```

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddEnvironmentVariables(prefix: "APP_")  // 使用默认层级 400
    .Build();
```

### Kubernetes 部署

```yaml
# ConfigMap
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
data:
  APP_DATABASE__HOST: "postgres-service"
  APP_DATABASE__PORT: "5432"
```

```yaml
# Deployment
spec:
  containers:
  - name: app
    envFrom:
    - configMapRef:
        name: app-config
```

## .env 文件特性

| 特性 | 支持 |
|------|------|
| 注释（`#` 开头） | ✅ |
| 双引号包裹的值 | ✅ |
| 单引号包裹的值 | ✅ |
| `export` 前缀 | ✅ |
| 嵌套配置（`__`） | ✅ |
| 文件变更自动重载 | ✅ |
| 可写配置源 | ✅ |
| 转义字符（双引号内） | ✅ |
| 写入系统环境变量 | ✅ |

### 写入系统环境变量

默认情况下，`.env` 文件中的配置只会加载到配置系统中，不会写入系统环境变量。如果需要将配置同时写入系统环境变量（例如供子进程使用），可以启用 `setEnvironmentVariables` 参数：

```csharp
var cfg = new CfgBuilder()
    .AddEnv(".env", setEnvironmentVariables: true)  // 使用默认层级 400
    .Build();

// .env 文件中的 DATABASE__HOST=localhost 会：
// 1. 作为配置键 "DATABASE:HOST" 可通过 cfg.Get("DATABASE:HOST") 访问
// 2. 同时设置系统环境变量 DATABASE__HOST=localhost
```

> **注意**：启用此选项后，配置会写入当前进程的环境变量，子进程可以继承这些环境变量。但这不会影响系统级别的环境变量。

## 最佳实践

### 1. 敏感信息使用环境变量

```csharp
var cfg = new CfgBuilder()
    .AddJson("config.json")
    .AddEnvironmentVariables(prefix: "APP_")  // 使用默认层级 400
    .Build();

// 敏感信息通过环境变量注入
// APP_DATABASE__PASSWORD=secret
```

### 2. .env 文件不提交到版本控制

```bash
# .gitignore
.env
.env.local
.env.*.local
```

### 3. 提供 .env.example 模板

```bash
# .env.example
APP_NAME=MyApp
DATABASE__HOST=localhost
DATABASE__PORT=5432
DATABASE__PASSWORD=your-password-here
```

## 下一步

- [Consul](/config-sources/consul) - Consul 配置中心
- [Redis](/config-sources/redis) - Redis 配置源
