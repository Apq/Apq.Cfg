# 生成 API 文档脚本
# 使用 DefaultDocumentation 将 XML 文档注释转换为 Markdown

param(
    [string]$Configuration = "Release",
    [string]$TargetFramework = "net9.0",
    [string]$OutputDir = "docs/site/api/net9.0"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Apq.Cfg API 文档生成 ===" -ForegroundColor Cyan

# 检查并安装 DefaultDocumentation
$toolInstalled = dotnet tool list -g | Select-String "defaultdocumentation"
if (-not $toolInstalled) {
    Write-Host "安装 DefaultDocumentation..." -ForegroundColor Yellow
    dotnet tool install -g DefaultDocumentation.Console
}

# 构建项目
Write-Host "构建项目 ($Configuration)..." -ForegroundColor Yellow
dotnet build -c $Configuration --no-incremental

# 创建输出目录
$outputPath = Join-Path $PSScriptRoot "..\$OutputDir"
if (-not (Test-Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath -Force | Out-Null
}

# 要生成文档的项目列表
$projects = @(
    @{ Name = "Apq.Cfg"; Output = "core" },
    @{ Name = "Apq.Cfg.Crypto"; Output = "crypto" },
    @{ Name = "Apq.Cfg.Ini"; Output = "ini" },
    @{ Name = "Apq.Cfg.Xml"; Output = "xml" },
    @{ Name = "Apq.Cfg.Yaml"; Output = "yaml" },
    @{ Name = "Apq.Cfg.Toml"; Output = "toml" },
    @{ Name = "Apq.Cfg.Env"; Output = "env" },
    @{ Name = "Apq.Cfg.Redis"; Output = "redis" },
    @{ Name = "Apq.Cfg.Consul"; Output = "consul" },
    @{ Name = "Apq.Cfg.Etcd"; Output = "etcd" },
    @{ Name = "Apq.Cfg.Nacos"; Output = "nacos" },
    @{ Name = "Apq.Cfg.Apollo"; Output = "apollo" },
    @{ Name = "Apq.Cfg.Zookeeper"; Output = "zookeeper" },
    @{ Name = "Apq.Cfg.Vault"; Output = "vault" },
    @{ Name = "Apq.Cfg.Database"; Output = "database" },
    @{ Name = "Apq.Cfg.Crypto.DataProtection"; Output = "crypto-dp" }
)

$successCount = 0
$failCount = 0

# 生成每个项目的文档
foreach ($project in $projects) {
    $dllPath = Join-Path $PSScriptRoot "..\" "$($project.Name)\bin\$Configuration\$TargetFramework\$($project.Name).dll"
    $projectOutputDir = Join-Path $outputPath $project.Output

    if (Test-Path $dllPath) {
        Write-Host "生成 $($project.Name) 文档..." -ForegroundColor Green

        # 清空并创建项目输出目录
        if (Test-Path $projectOutputDir) {
            Remove-Item -Path "$projectOutputDir\*" -Force -Recurse -ErrorAction SilentlyContinue
        } else {
            New-Item -ItemType Directory -Path $projectOutputDir -Force | Out-Null
        }

        # 运行 DefaultDocumentation
        try {
            $result = & defaultdocumentation -a $dllPath -o $projectOutputDir 2>&1
            if ($LASTEXITCODE -eq 0) {
                $successCount++
                Write-Host "  -> $projectOutputDir" -ForegroundColor Gray
            } else {
                $failCount++
                Write-Host "  ✗ 生成失败" -ForegroundColor Red
            }
        } catch {
            $failCount++
            Write-Host "  ✗ 异常: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "跳过 $($project.Name) (未找到 DLL 文件: $dllPath)" -ForegroundColor DarkYellow
    }
}

# 生成 API 索引页
$indexContent = @"
# API 参考 (.NET 9.0)

本节包含 Apq.Cfg 所有公开 API 的详细文档，由代码注释自动生成。

> 当前文档基于 .NET 9.0 版本生成。各版本 API 基本一致，仅内部实现有差异。

## 核心库

- [Apq.Cfg](./core/) - 核心配置库（JSON、环境变量、DI 集成）

## 加密脱敏

- [Apq.Cfg.Crypto](./crypto/) - 配置加密脱敏
- [Apq.Cfg.Crypto.DataProtection](./crypto-dp/) - ASP.NET Core DataProtection 集成

## 本地配置源

- [Apq.Cfg.Ini](./ini/) - INI 格式支持
- [Apq.Cfg.Xml](./xml/) - XML 格式支持
- [Apq.Cfg.Yaml](./yaml/) - YAML 格式支持
- [Apq.Cfg.Toml](./toml/) - TOML 格式支持
- [Apq.Cfg.Env](./env/) - .env 文件支持

## 远程配置源

- [Apq.Cfg.Redis](./redis/) - Redis 配置源
- [Apq.Cfg.Consul](./consul/) - Consul 配置中心
- [Apq.Cfg.Etcd](./etcd/) - Etcd 配置中心
- [Apq.Cfg.Nacos](./nacos/) - Nacos 配置中心
- [Apq.Cfg.Apollo](./apollo/) - Apollo 配置中心
- [Apq.Cfg.Zookeeper](./zookeeper/) - Zookeeper 配置中心
- [Apq.Cfg.Vault](./vault/) - HashiCorp Vault
- [Apq.Cfg.Database](./database/) - 数据库配置源
"@

$indexPath = Join-Path $outputPath "index.md"
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($indexPath, $indexContent, $utf8NoBom)

Write-Host ""
Write-Host "=== 文档生成完成 ===" -ForegroundColor Cyan
Write-Host "  成功: $successCount" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "  失败: $failCount" -ForegroundColor Red
}
Write-Host "输出目录: $outputPath" -ForegroundColor Green
