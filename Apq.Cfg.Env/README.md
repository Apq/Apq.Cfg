# Apq.Cfg.Env

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

.env 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 依赖

- Apq.Cfg

## 默认层级

该配置源的默认层级为 `CfgSourceLevels.Env` (400)。

如果不指定 `level` 参数，将使用默认层级：

```csharp
// 使用默认层级 400
.AddEnv(".env")

// 指定自定义层级
.AddEnv(".env", level: 450)
```

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Env;

var cfg = new CfgBuilder()
    .AddEnv(".env", level: 0, writeable: true)
    .Build();

// 使用索引器访问（__ 会自动转换为 :）
var dbHost = cfg["DATABASE:HOST"];

// 使用配置节
var db = cfg.GetSection("DATABASE");
var host = db["HOST"];
var port = db.Get<int>("PORT");
```

## 方法签名

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
| `path` | .env 文件路径 |
| `level` | 配置层级，数值越大优先级越高 |
| `writeable` | 是否可写 |
| `optional` | 文件不存在时是否忽略 |
| `reloadOnChange` | 文件变更时是否自动重载 |
| `isPrimaryWriter` | 是否为默认写入目标 |
| `setEnvironmentVariables` | 是否将配置写入系统环境变量（默认为 false） |

## .env 格式示例

```env
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

## 配置键映射

.env 文件使用双下划线 `__` 来表示配置层级，读取时会自动转换为 `:`：

| .env 键 | 配置键 |
|---------|--------|
| `APP_NAME` | `APP_NAME` |
| `DATABASE__HOST` | `DATABASE:HOST` |
| `DATABASE__CONNECTION__STRING` | `DATABASE:CONNECTION:STRING` |

## 支持的特性

- ✅ 注释（以 `#` 开头）
- ✅ 双引号包裹的值（支持转义字符）
- ✅ 单引号包裹的值（原样保留）
- ✅ `export` 前缀
- ✅ 嵌套配置（使用 `__`）
- ✅ 文件变更自动重载
- ✅ 可写配置源
- ✅ 写入系统环境变量

## 写入系统环境变量

默认情况下，`.env` 文件中的配置只会加载到配置系统中。如果需要将配置同时写入系统环境变量（例如供子进程使用），可以启用 `setEnvironmentVariables` 参数：

```csharp
var cfg = new CfgBuilder()
    .AddEnv(".env", level: 0, writeable: false, setEnvironmentVariables: true)
    .Build();

// .env 文件中的 DATABASE__HOST=localhost 会：
// 1. 作为配置键 "DATABASE:HOST" 可通过 cfg.Get("DATABASE:HOST") 访问
// 2. 同时设置系统环境变量 DATABASE__HOST=localhost
```

> **注意**：启用此选项后，配置会写入当前进程的环境变量，子进程可以继承这些环境变量。但这不会影响系统级别的环境变量。

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
