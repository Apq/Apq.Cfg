# Apq.Cfg 项目指南

## 项目概述

Apq.Cfg 是一个统一配置管理系统，支持多种配置格式（JSON、INI、XML、YAML、TOML）和远程配置中心（Redis、Database、Consul、Etcd）。

## 代码规范

### 示例代码规范

- **不要使用** `appsettings` 作为配置文件名示例
- 使用 `config.json`、`config.local.json`、`settings.json` 等作为示例文件名
- 远程配置中心的键前缀示例使用 `app/config/` 或 `/app/config/`

### 命名约定

- 配置源类：`{Format}CfgSource`（如 `JsonCfgSource`、`ConsulCfgSource`）
- 配置选项类：`{Format}CfgOptions`（如 `ConsulCfgOptions`、`EtcdCfgOptions`）
- 扩展方法：`Add{Format}`（如 `AddJson`、`AddConsul`）

### 多目标框架

所有项目支持 `net6.0;net7.0;net8.0;net9.0` 四个目标框架。

## 文档规范

### 扩展项目文档

创建扩展项目（如 `Apq.Cfg.Xxx`）时，必须在扩展项目目录下创建 `README.md` 文档，包含：
- 项目简介
- 安装方式（NuGet 包）
- 配置选项说明
- 使用示例代码

### 主项目文档

主项目 `Apq.Cfg/README.md` 需要包含所有扩展项目的使用示例代码。

### 解决方案文档

解决方案根目录下的 `README.md` 是主文档，需要包含：

- 项目概述和特性
- 所有 NuGet 包列表
- 所有扩展项目的使用示例代码
- 构建和测试说明

## 测试规范

### 新功能测试要求

增加新的公开功能后，必须：

1. **添加单元测试**：在 `tests/Apq.Cfg.Tests.Shared/` 中添加对应的测试类
2. **添加性能测试**：在 `benchmarks/Apq.Cfg.Benchmarks/` 中添加对应的基准测试
3. **更新测试文档**：更新 `tests/README.md`，添加新测试类的说明
4. **更新性能文档**：更新 `benchmarks/README.md`，添加新基准测试的说明

## 项目结构

```
Apq.Cfg/                    # 核心库
Apq.Cfg.Ini/               # INI 格式支持
Apq.Cfg.Xml/               # XML 格式支持
Apq.Cfg.Yaml/              # YAML 格式支持
Apq.Cfg.Toml/              # TOML 格式支持
Apq.Cfg.Redis/             # Redis 配置源
Apq.Cfg.Database/          # 数据库配置源
Apq.Cfg.Consul/            # Consul 配置中心
Apq.Cfg.Etcd/              # Etcd 配置中心
Apq.Cfg.SourceGenerator/   # 源生成器（Native AOT）
tests/                     # 单元测试
benchmarks/                # 性能基准测试
Samples/                   # 示例项目
```

## 构建和测试

```bash
# 构建
dotnet build

# 运行测试
dotnet test

# 运行性能测试
dotnet run -c Release --project benchmarks/Apq.Cfg.Benchmarks -f net9.0
```
