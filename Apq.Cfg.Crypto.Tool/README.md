# Apq.Cfg.Crypto.Tool

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apq.Cfg 配置加密命令行工具。

**仓库地址**：https://gitee.com/apq/Apq.Cfg

**📖 在线文档**：https://apq-cfg.vercel.app/

## 安装

```bash
dotnet tool install -g Apq.Cfg.Crypto.Tool
```

## 命令

### generate-key - 生成密钥

```bash
# 生成 256 位 AES-GCM 密钥（默认）
apqenc generate-key

# 生成 128 位密钥
apqenc generate-key --bits 128

# 生成 192 位密钥
apqenc generate-key -b 192
```

输出示例：
```
算法: AES-GCM
密钥位数: 256
Base64 密钥: abc123...xyz789==

请妥善保管此密钥，不要将其存储在配置文件中！
建议使用环境变量 APQ_CFG_ENCRYPTION_KEY 存储密钥。
```

### encrypt - 加密值

```bash
# 加密单个值
apqenc encrypt --key "base64key..." --value "mySecretPassword"
# 输出: {ENC}base64ciphertext...

# 使用自定义前缀
apqenc encrypt -k "base64key..." -v "mySecret" --prefix "[ENCRYPTED]"
# 输出: [ENCRYPTED]base64ciphertext...
```

### decrypt - 解密值

```bash
# 解密值
apqenc decrypt --key "base64key..." --value "{ENC}base64ciphertext..."
# 输出: mySecretPassword

# 使用自定义前缀
apqenc decrypt -k "base64key..." -v "[ENCRYPTED]base64cipher..." -p "[ENCRYPTED]"
```

### encrypt-file - 批量加密配置文件

```bash
# 加密配置文件中的敏感值
apqenc encrypt-file --key "base64key..." --file config.json

# 预览将要加密的键（不实际修改）
apqenc encrypt-file -k "base64key..." -f config.json --dry-run

# 指定输出文件
apqenc encrypt-file -k "base64key..." -f config.json -o config.encrypted.json

# 自定义敏感键模式
apqenc encrypt-file -k "base64key..." -f config.json --patterns "*Password*,*Secret*,*ApiKey*"

# 使用自定义前缀
apqenc encrypt-file -k "base64key..." -f config.json --prefix "[ENC]"
```

## 敏感键模式

默认的敏感键模式（支持通配符 `*` 和 `?`）：

- `*Password*` - 匹配包含 Password 的键
- `*Secret*` - 匹配包含 Secret 的键
- `*ApiKey*` - 匹配包含 ApiKey 的键
- `*ConnectionString*` - 匹配包含 ConnectionString 的键
- `*Credential*` - 匹配包含 Credential 的键
- `*Token*` - 匹配包含 Token 的键

## 使用示例

### 完整工作流

```bash
# 1. 生成密钥
apqenc generate-key
# 输出: Base64 密钥: abc123...xyz789==

# 2. 设置环境变量
export APQ_CFG_ENCRYPTION_KEY="abc123...xyz789=="

# 3. 预览将要加密的键
apqenc encrypt-file -k "$APQ_CFG_ENCRYPTION_KEY" -f appsettings.json --dry-run

# 4. 执行加密
apqenc encrypt-file -k "$APQ_CFG_ENCRYPTION_KEY" -f appsettings.json

# 5. 验证加密结果
cat appsettings.json
```

### 配置文件示例

加密前：
```json
{
    "Database": {
        "ConnectionString": "Server=localhost;Database=mydb;User=admin;Password=secret123",
        "Timeout": 30
    },
    "Api": {
        "Key": "my-api-key-12345",
        "Endpoint": "https://api.example.com"
    }
}
```

加密后：
```json
{
    "Database": {
        "ConnectionString": "{ENC}base64ciphertext...",
        "Timeout": 30
    },
    "Api": {
        "Key": "{ENC}base64ciphertext...",
        "Endpoint": "https://api.example.com"
    }
}
```

## 安全最佳实践

1. **不要**将加密密钥存储在配置文件中
2. 使用环境变量存储密钥
3. 在 CI/CD 中使用密钥管理服务
4. 定期轮换密钥
5. 使用 256 位密钥获得最高安全性

## 许可证

MIT License

## 作者

- 邮箱：amwpfiqvy@163.com

## 仓库

- Gitee：https://gitee.com/apq/Apq.Cfg
