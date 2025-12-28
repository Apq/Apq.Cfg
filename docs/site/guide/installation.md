# 安装

本页介绍如何安装 Apq.Cfg 及其扩展包。

## 系统要求

- .NET 6.0 或更高版本
- 或 .NET Standard 2.0/2.1 兼容的运行时

## 核心包安装

### 使用 .NET CLI

```bash
dotnet add package Apq.Cfg
```

### 使用 Package Manager

```powershell
Install-Package Apq.Cfg
```

### 使用 PackageReference

```xml
<PackageReference Include="Apq.Cfg" Version="1.0.*" />
```

## 扩展包安装

根据需要安装对应的扩展包：

### 本地配置源

::: code-group

```bash [YAML]
dotnet add package Apq.Cfg.Yaml
```

```bash [XML]
dotnet add package Apq.Cfg.Xml
```

```bash [INI]
dotnet add package Apq.Cfg.Ini
```

```bash [TOML]
dotnet add package Apq.Cfg.Toml
```

:::

### 远程配置源

::: code-group

```bash [Consul]
dotnet add package Apq.Cfg.Consul
```

```bash [Redis]
dotnet add package Apq.Cfg.Redis
```

```bash [Apollo]
dotnet add package Apq.Cfg.Apollo
```

```bash [Vault]
dotnet add package Apq.Cfg.Vault
```

```bash [Etcd]
dotnet add package Apq.Cfg.Etcd
```

```bash [Zookeeper]
dotnet add package Apq.Cfg.Zookeeper
```

:::

### 源生成器

```bash
dotnet add package Apq.Cfg.SourceGenerator
```

## 完整安装示例

一个典型的企业应用可能需要以下包：

```xml
<ItemGroup>
  <!-- 核心包 -->
  <PackageReference Include="Apq.Cfg" Version="1.0.*" />
  
  <!-- 本地配置格式 -->
  <PackageReference Include="Apq.Cfg.Yaml" Version="1.0.*" />
  <PackageReference Include="Apq.Cfg.Toml" Version="1.0.*" />
  
  <!-- 远程配置中心 -->
  <PackageReference Include="Apq.Cfg.Consul" Version="1.0.*" />
  <PackageReference Include="Apq.Cfg.Vault" Version="1.0.*" />
  
  <!-- 源生成器 -->
  <PackageReference Include="Apq.Cfg.SourceGenerator" Version="1.0.*" />
</ItemGroup>
```

## 验证安装

创建一个简单的测试程序验证安装：

```csharp
using Apq.Cfg;

var cfg = new CfgBuilder()
    .AddInMemory(new Dictionary<string, string>
    {
        ["Test:Key"] = "Hello, Apq.Cfg!"
    })
    .Build();

Console.WriteLine(cfg["Test:Key"]);
// 输出: Hello, Apq.Cfg!
```

## 下一步

- [快速开始](/guide/quick-start) - 学习基本用法
- [基础用法](/guide/basic-usage) - 深入了解配置读取
