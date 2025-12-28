# 安装指南

本页面介绍如何安装 Apq.Cfg 及其各个配置源包。

## NuGet 包列表

### 核心包

| 包名 | 描述 | NuGet |
|-----|------|-------|
| `Apq.Cfg` | 核心库，包含基础功能和 JSON 支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.svg)](https://www.nuget.org/packages/Apq.Cfg) |

### 本地配置源

| 包名 | 描述 | NuGet |
|-----|------|-------|
| `Apq.Cfg.Yaml` | YAML 配置文件支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Yaml.svg)](https://www.nuget.org/packages/Apq.Cfg.Yaml) |
| `Apq.Cfg.Toml` | TOML 配置文件支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Toml.svg)](https://www.nuget.org/packages/Apq.Cfg.Toml) |
| `Apq.Cfg.Xml` | XML 配置文件支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Xml.svg)](https://www.nuget.org/packages/Apq.Cfg.Xml) |
| `Apq.Cfg.Ini` | INI 配置文件支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Ini.svg)](https://www.nuget.org/packages/Apq.Cfg.Ini) |
| `Apq.Cfg.Env` | ENV 环境变量文件支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Env.svg)](https://www.nuget.org/packages/Apq.Cfg.Env) |

### 远程配置源

| 包名 | 描述 | NuGet |
|-----|------|-------|
| `Apq.Cfg.Redis` | Redis 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Redis.svg)](https://www.nuget.org/packages/Apq.Cfg.Redis) |
| `Apq.Cfg.Database` | 数据库配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Database.svg)](https://www.nuget.org/packages/Apq.Cfg.Database) |
| `Apq.Cfg.Etcd` | Etcd 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Etcd.svg)](https://www.nuget.org/packages/Apq.Cfg.Etcd) |
| `Apq.Cfg.Consul` | Consul 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Consul.svg)](https://www.nuget.org/packages/Apq.Cfg.Consul) |
| `Apq.Cfg.Nacos` | Nacos 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Nacos.svg)](https://www.nuget.org/packages/Apq.Cfg.Nacos) |
| `Apq.Cfg.Apollo` | Apollo 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Apollo.svg)](https://www.nuget.org/packages/Apq.Cfg.Apollo) |
| `Apq.Cfg.Vault` | HashiCorp Vault 支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Vault.svg)](https://www.nuget.org/packages/Apq.Cfg.Vault) |
| `Apq.Cfg.Zookeeper` | Zookeeper 配置支持 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.Zookeeper.svg)](https://www.nuget.org/packages/Apq.Cfg.Zookeeper) |

### 扩展包

| 包名 | 描述 | NuGet |
|-----|------|-------|
| `Apq.Cfg.SourceGenerator` | 源代码生成器 | [![NuGet](https://img.shields.io/nuget/v/Apq.Cfg.SourceGenerator.svg)](https://www.nuget.org/packages/Apq.Cfg.SourceGenerator) |

## 安装方式

### 使用 .NET CLI

```bash
# 安装核心包
dotnet add package Apq.Cfg

# 安装配置源包
dotnet add package Apq.Cfg.Yaml
dotnet add package Apq.Cfg.Redis
```

### 使用 Package Manager Console

```powershell
# 安装核心包
Install-Package Apq.Cfg

# 安装配置源包
Install-Package Apq.Cfg.Yaml
Install-Package Apq.Cfg.Redis
```

### 使用 PackageReference

在 `.csproj` 文件中添加：

```xml
<ItemGroup>
  <PackageReference Include="Apq.Cfg" Version="1.0.*" />
  <PackageReference Include="Apq.Cfg.Yaml" Version="1.0.*" />
  <PackageReference Include="Apq.Cfg.Redis" Version="1.0.*" />
</ItemGroup>
```

## 版本兼容性

所有 Apq.Cfg 包遵循语义化版本控制，主版本号相同的包之间保证兼容。

建议在项目中使用相同版本的所有 Apq.Cfg 包：

```xml
<ItemGroup>
  <PackageReference Include="Apq.Cfg" Version="1.0.5" />
  <PackageReference Include="Apq.Cfg.Yaml" Version="1.0.5" />
  <PackageReference Include="Apq.Cfg.Redis" Version="1.0.5" />
</ItemGroup>
```

## 国内镜像

如果 NuGet 官方源访问较慢，可以使用国内镜像：

```bash
# 添加华为云镜像
dotnet nuget add source https://repo.huaweicloud.com/repository/nuget/v3/index.json -n huaweicloud

# 添加腾讯云镜像
dotnet nuget add source https://mirrors.cloud.tencent.com/nuget/ -n tencentcloud
```

或在 `nuget.config` 中配置：

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="huaweicloud" value="https://repo.huaweicloud.com/repository/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

## 下一步

- [快速开始](/guide/getting-started) - 开始使用
- [配置源](/sources/) - 了解各配置源的使用方法
