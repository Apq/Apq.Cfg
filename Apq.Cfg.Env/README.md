# Apq.Cfg.Env

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)

.env 文件配置源扩展包。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

## 依赖

- Apq.Cfg

## 用法

```csharp
using Apq.Cfg;
using Apq.Cfg.Env;

var cfg = new CfgBuilder()
    .AddEnv(".env", level: 0, writeable: true)
    .Build();

// 读取配置
var dbHost = cfg.Get("DATABASE__HOST");
var dbPort = cfg.Get("DATABASE__PORT");

// 使用配置节访问（__ 会自动转换为 :）
var dbSection = cfg.GetSection("DATABASE");
var host = dbSection.Get("HOST");
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
    bool isPrimaryWriter = false)
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

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
