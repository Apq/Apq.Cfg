# 生成 API 文档脚本
# 使用 xmldoc2md 将 XML 文档注释转换为 Markdown

param(
    [string]$Configuration = "Release",
    [string]$TargetFramework = "net8.0",
    [string]$OutputDir = "docs/site/api"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Apq.Cfg API 文档生成 ===" -ForegroundColor Cyan

# 检查并安装 xmldoc2markdown
$toolInstalled = dotnet tool list -g | Select-String "xmldoc2markdown"
if (-not $toolInstalled) {
    Write-Host "安装 xmldoc2markdown..." -ForegroundColor Yellow
    dotnet tool install -g xmldoc2markdown
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
    @{ Name = "Apq.Cfg.Database"; Output = "database" }
)

# 生成每个项目的文档
foreach ($project in $projects) {
    $xmlPath = Join-Path $PSScriptRoot "..\" "$($project.Name)\bin\$Configuration\$TargetFramework\$($project.Name).xml"
    $projectOutputDir = Join-Path $outputPath $project.Output

    if (Test-Path $xmlPath) {
        Write-Host "生成 $($project.Name) 文档..." -ForegroundColor Green

        # 创建项目输出目录
        if (-not (Test-Path $projectOutputDir)) {
            New-Item -ItemType Directory -Path $projectOutputDir -Force | Out-Null
        }

        # 运行 xmldoc2md
        xmldoc2md $xmlPath $projectOutputDir

        Write-Host "  -> $projectOutputDir" -ForegroundColor Gray
    } else {
        Write-Host "跳过 $($project.Name) (未找到 XML 文件: $xmlPath)" -ForegroundColor DarkYellow
    }
}

# 生成 API 索引页
$indexContent = @"
# API 参考

本节包含 Apq.Cfg 所有公开 API 的详细文档，由代码注释自动生成。

## 核心库

- [Apq.Cfg](./core/) - 核心配置库（JSON、环境变量、DI 集成）

## 加密脱敏

- [Apq.Cfg.Crypto](./crypto/) - 配置加密脱敏

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
[System.IO.File]::WriteAllText($indexPath, $indexContent, [System.Text.Encoding]::UTF8)

Write-Host ""
Write-Host "=== 文档生成完成 ===" -ForegroundColor Cyan
Write-Host "输出目录: $outputPath" -ForegroundColor Green
