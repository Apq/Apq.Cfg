# Apq.Cfg.Vault

[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)
[![国内文档](https://img.shields.io/badge/国内文档-CloudBase-green)](https://apq-9g6w58ii54088d8b-1251405840.tcloudbaseapp.com/)

Apq.Cfg 的 HashiCorp Vault 扩展，支持密钥管理和热重载功能。

**📖 在线文档**：
- 国际访问：https://apq-cfg.vercel.app/
- 国内访问：https://apq-9g6w58ii54088d8b-1251405840.tcloudbaseapp.com/

## 功能特性

- ✅ 支持 HashiCorp Vault KV Secrets Engine V1 和 V2
- ✅ 多种认证方式：Token、UserPass、AppRole
- ✅ 配置热重载（轮询检测变化）
- ✅ 写入支持（支持密钥更新）
- ✅ 命名空间支持（Vault Enterprise）
- ✅ Microsoft.Extensions.Configuration 集成

## 安装

```bash
dotnet add package Apq.Cfg.Vault
```

## 快速开始

### 使用 Token 认证（KV V2）

```csharp
using Apq.Cfg;
using Apq.Cfg.Vault;

var cfg = new CfgBuilder()
    .AddVaultV2(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "kv",
        path: "myapp/config",
        level: 0,
        enableHotReload: true
    )
    .Build();

// 读取配置
var dbHost = cfg.Get("Database:Host");
var apiKey = cfg.Get("Api:Key");
```

### 使用 UserPass 认证

```csharp
var cfg = new CfgBuilder()
    .AddVaultUserPass(
        address: "http://localhost:8200",
        username: "myapp",
        password: "secure-password",
        enginePath: "kv",
        path: "myapp/production",
        kvVersion: 2,
        level: 0
    )
    .Build();
```

### 使用 AppRole 认证

```csharp
var cfg = new CfgBuilder()
    .AddVaultAppRole(
        address: "http://localhost:8200",
        roleId: "role-id-value",
        roleSecret: "role-secret-value",
        enginePath: "kv",
        path: "myapp/staging",
        kvVersion: 2,
        level: 0
    )
    .Build();
```

### KV V1 引擎支持

```csharp
var cfg = new CfgBuilder()
    .AddVaultV1(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "secret",
        path: "myapp",
        level: 0
    )
    .Build();
```

### 高级配置

```csharp
var cfg = new CfgBuilder()
    .AddVault(options =>
    {
        options.Address = "http://localhost:8200";
        options.Token = "s.1234567890abcdef";
        options.EnginePath = "kv";
        options.Path = "myapp/config";
        options.Namespace = "my-namespace"; // Vault Enterprise
        options.KvVersion = 2;
        options.EnableHotReload = true;
        options.PollInterval = TimeSpan.FromSeconds(30);
        options.ReconnectInterval = TimeSpan.FromSeconds(60);
    }, level: 0)
    .Build();
```

### 写入配置

```csharp
var cfg = new CfgBuilder()
    .AddVaultV2(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "kv",
        path: "myapp/config",
        level: 0,
        isPrimaryWriter: true
    )
    .Build();

// 更新配置
cfg.Set("Database:Host", "new-db-host");
cfg.Set("Database:Port", "5433");
cfg.Set("Feature:NewFeature", "true");

// 保存到 Vault
await cfg.SaveAsync();
```

### 监听配置变化

```csharp
var cfg = new CfgBuilder()
    .AddVaultV2(
        address: "http://localhost:8200",
        token: "s.1234567890abcdef",
        enginePath: "kv",
        path: "myapp/config",
        level: 0,
        enableHotReload: true
    )
    .Build();

// 订阅配置变化事件
cfg.ConfigChanges.Subscribe(e =>
{
    foreach (var (key, change) in e.Changes)
    {
        Console.WriteLine($"[{change.Type}] {key}: {change.OldValue} -> {change.NewValue}");
    }
});
```

### 与 Microsoft.Extensions.Configuration 集成

```csharp
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.AddVault(options =>
{
    options.Address = "http://localhost:8200";
    options.Token = "s.1234567890abcdef";
    options.EnginePath = "kv";
    options.Path = "myapp/config";
    options.KvVersion = 2;
});

var configuration = builder.Build();
var value = configuration["SomeKey"];
```

## 配置选项

| 选项 | 类型 | 说明 |
|------|------|------|
| `Address` | `string` | Vault 服务地址 |
| `Token` | `string` | Vault Token（Token 认证方式） |
| `EnginePath` | `string` | KV 引擎路径，默认 "kv" |
| `Path` | `string` | 密钥路径 |
| `Namespace` | `string` | Vault Enterprise 命名空间 |
| `KvVersion` | `int` | KV 引擎版本（1 或 2），默认 2 |
| `EnableHotReload` | `bool` | 是否启用热重载，默认 true |
| `PollInterval` | `TimeSpan` | 轮询间隔，默认 30 秒 |
| `ReconnectInterval` | `TimeSpan` | 重连间隔，默认 60 秒 |
| `AuthMethod` | `VaultAuthMethod` | 认证方式：Token、UserPass、AppRole |
| `Username` | `string` | 用户名（UserPass 认证） |
| `Password` | `string` | 密码（UserPass 认证） |
| `RoleId` | `string` | Role ID（AppRole 认证） |
| `RoleSecret` | `string` | Role Secret（AppRole 认证） |

## Vault 认证方式

### Token 认证

最简单的认证方式，直接使用 Vault Token。

```csharp
options.AuthMethod = VaultAuthMethod.Token;
options.Token = "s.1234567890abcdef";
```

### UserPass 认证

使用用户名密码认证，适合需要密码轮换的场景。

```csharp
options.AuthMethod = VaultAuthMethod.UserPass;
options.Username = "myapp";
options.Password = "secure-password";
```

### AppRole 认证

推荐用于应用程序的认证方式，基于 Role ID 和 Secret ID。

```csharp
options.AuthMethod = VaultAuthMethod.AppRole;
options.RoleId = "role-id-value";
options.RoleSecret = "role-secret-value";
```

## Vault 准备

### 启用 KV Secrets Engine V2

```bash
vault secrets enable -path=kv kv-v2
```

### 启用 KV Secrets Engine V1

```bash
vault secrets enable -path=secret kv
```

### 创建 Token 策略

```bash
# 创建策略文件 policy.hcl
cat > policy.hcl <<EOF
path "kv/data/myapp/*" {
  capabilities = ["create", "read", "update", "delete", "list"]
}
EOF

# 创建策略
vault policy write myapp-policy policy.hcl

# 创建 Token
vault token create -policy=myapp-policy
```

### 配置 AppRole

```bash
# 启用 AppRole 认证
vault auth enable approle

# 创建 Role
vault write auth/approle/role/myapp \
    token_policies="myapp-policy" \
    token_ttl=1h \
    token_max_ttl=4h

# 获取 Role ID
vault read auth/approle/role/myapp/role-id

# 获取 Secret ID
vault write -f auth/approle/role/myapp/secret-id
```

### 配置 UserPass

```bash
# 启用 UserPass 认证
vault auth enable userpass

# 创建用户
vault write auth/userpass/users/myapp \
    password="secure-password" \
    policies="myapp-policy"
```

## 许可证

本项目遵循主项目的许可证。
