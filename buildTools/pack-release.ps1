# pack-release.ps1
# 支持每个项目独立版本的打包脚本
param(
    [switch]$NoBuild,
    [string]$OutputDir,
    [string[]]$Projects  # 可选：指定要打包的项目，不指定则打包所有
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$PropsFile = Join-Path $RootDir 'Directory.Build.props'
$DefaultOutputDir = Join-Path $RootDir 'nupkgs'
$VersionsDir = Join-Path $RootDir 'versions'

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
}

# 读取 Y/N 确认，按Q立即退出，回车默认Y
function Read-Confirm {
    param([string]$Prompt)
    Write-Host $Prompt -NoNewline
    while ($true) {
        $key = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
        if ($key.Character -eq 'q' -or $key.Character -eq 'Q') {
            Write-Host ''
            Write-ColorText '已退出' 'Yellow'
            exit 0
        }
        if ($key.Character -eq 'y' -or $key.Character -eq 'Y' -or $key.VirtualKeyCode -eq 13) {
            Write-Host 'Y'
            return $true
        }
        if ($key.Character -eq 'n' -or $key.Character -eq 'N') {
            Write-Host 'N'
            return $false
        }
    }
}

# 获取项目版本（从 versions/{ProjectName}/v*.md 目录）
function Get-ProjectVersion {
    param([string]$ProjectName)
    
    $projectVersionDir = Join-Path $VersionsDir $ProjectName
    
    # 优先使用项目独立版本目录
    if (Test-Path $projectVersionDir) {
        $versionFiles = @(Get-ChildItem -Path $projectVersionDir -Filter 'v*.md' -ErrorAction SilentlyContinue)
    } else {
        # 回退到根版本目录
        $versionFiles = @(Get-ChildItem -Path $VersionsDir -Filter 'v*.md' -ErrorAction SilentlyContinue)
    }
    
    $versions = @($versionFiles | Where-Object { $_.BaseName -match '^v(\d+)\.(\d+)\.(\d+)' } | ForEach-Object {
        $fullVersion = $_.BaseName -replace '^v', ''
        $baseVersion = $_.BaseName -replace '^v(\d+\.\d+\.\d+).*', '$1'
        [PSCustomObject]@{
            Name = $fullVersion
            Version = [version]$baseVersion
        }
    } | Sort-Object Version -Descending)
    
    if ($versions.Count -gt 0) {
        return $versions[0].Name
    }
    return $null
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg NuGet 包生成工具' 'Cyan'
Write-ColorText '  支持独立版本管理' 'DarkCyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $PropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.props 文件' 'Red'
    Write-ColorText "路径: $PropsFile" 'Red'
    exit 1
}

if (-not (Test-Path $VersionsDir)) {
    Write-ColorText '错误: 找不到 versions 目录' 'Red'
    Write-ColorText "路径: $VersionsDir" 'Red'
    exit 1
}

# 定义所有可打包的项目
$AllProjects = @(
    'Apq.Cfg',
    'Apq.Cfg.Ini',
    'Apq.Cfg.Xml',
    'Apq.Cfg.Yaml',
    'Apq.Cfg.Toml',
    'Apq.Cfg.Env',
    'Apq.Cfg.Redis',
    'Apq.Cfg.Database',
    'Apq.Cfg.Consul',
    'Apq.Cfg.Etcd',
    'Apq.Cfg.Nacos',
    'Apq.Cfg.Apollo',
    'Apq.Cfg.Zookeeper',
    'Apq.Cfg.Vault',
    'Apq.Cfg.Crypto',
    'Apq.Cfg.Crypto.DataProtection',
    'Apq.Cfg.Crypto.Tool',
    'Apq.Cfg.SourceGenerator'
)

# 如果指定了项目，则只打包指定的项目
if ($Projects -and $Projects.Count -gt 0) {
    $TargetProjects = $Projects
} else {
    $TargetProjects = $AllProjects
}

# 设置输出目录
if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = $DefaultOutputDir
}

Write-Host ''
Write-ColorText '将要打包的项目及版本:' 'Cyan'

$projectVersions = @{}
foreach ($project in $TargetProjects) {
    $version = Get-ProjectVersion -ProjectName $project
    if ($version) {
        $projectVersions[$project] = $version
        Write-ColorText "  - $project @ v$version" 'White'
    } else {
        Write-ColorText "  - $project @ (未找到版本)" 'Yellow'
    }
}

Write-Host ''
Write-ColorText "输出目录: $OutputDir" 'Gray'
Write-Host ''

if (-not (Read-Confirm '确认开始打包? ([Y]/N): ')) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

# 清空并创建输出目录
if (Test-Path $OutputDir) {
    Write-ColorText "清空输出目录: $OutputDir" 'Gray'
    Remove-Item -Path "$OutputDir\*" -Force -Recurse -ErrorAction SilentlyContinue
} else {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-ColorText "已创建输出目录: $OutputDir" 'Gray'
}

Write-Host ''
Write-ColorText '开始打包...' 'Cyan'
Write-Host ''

$successCount = 0
$failCount = 0
$generatedPackages = @()

foreach ($project in $TargetProjects) {
    $version = $projectVersions[$project]
    if (-not $version) {
        Write-ColorText "跳过 $project (未找到版本)" 'Yellow'
        continue
    }
    
    # 查找项目文件
    $projectPath = Join-Path $RootDir "$project/$project.csproj"
    if (-not (Test-Path $projectPath)) {
        Write-ColorText "跳过 $project (项目文件不存在)" 'Yellow'
        continue
    }
    
    Write-ColorText "打包 $project v$version..." 'Gray'
    
    # 删除当前版本的旧包
    $oldPackages = Get-ChildItem -Path $OutputDir -Filter "$project.$version.*pkg" -ErrorAction SilentlyContinue
    foreach ($pkg in $oldPackages) {
        Remove-Item $pkg.FullName -Force
    }
    
    # 构建打包参数
    $packArgs = @(
        'pack'
        $projectPath
        '-c', 'Release'
        '-o', $OutputDir
    )
    
    if ($NoBuild) {
        $packArgs += '--no-build'
    }
    
    try {
        & dotnet @packArgs 2>&1 | Out-Null
        
        if ($LASTEXITCODE -eq 0) {
            $successCount++
            $generatedPackages += "$project.$version.nupkg"
            Write-ColorText "  ✓ $project v$version" 'Green'
        } else {
            $failCount++
            Write-ColorText "  ✗ $project 打包失败" 'Red'
        }
    } catch {
        $failCount++
        Write-ColorText "  ✗ $project 打包异常: $($_.Exception.Message)" 'Red'
    }
}

Write-Host ''
Write-ColorText "========================================" 'Cyan'
Write-ColorText "打包完成!" 'Green'
Write-ColorText "  成功: $successCount" 'Green'
if ($failCount -gt 0) {
    Write-ColorText "  失败: $failCount" 'Red'
}
Write-ColorText "========================================" 'Cyan'
Write-Host ''

# 生成 API 文档
Write-ColorText '生成 API 文档...' 'Cyan'

$apiDocsDir = Join-Path $RootDir 'docs\site\api\net9.0'
$docProjects = @(
    @{ Name = 'Apq.Cfg'; Output = 'core' },
    @{ Name = 'Apq.Cfg.Crypto'; Output = 'crypto' },
    @{ Name = 'Apq.Cfg.Ini'; Output = 'ini' },
    @{ Name = 'Apq.Cfg.Xml'; Output = 'xml' },
    @{ Name = 'Apq.Cfg.Yaml'; Output = 'yaml' },
    @{ Name = 'Apq.Cfg.Toml'; Output = 'toml' },
    @{ Name = 'Apq.Cfg.Env'; Output = 'env' },
    @{ Name = 'Apq.Cfg.Redis'; Output = 'redis' },
    @{ Name = 'Apq.Cfg.Consul'; Output = 'consul' },
    @{ Name = 'Apq.Cfg.Etcd'; Output = 'etcd' },
    @{ Name = 'Apq.Cfg.Nacos'; Output = 'nacos' },
    @{ Name = 'Apq.Cfg.Apollo'; Output = 'apollo' },
    @{ Name = 'Apq.Cfg.Zookeeper'; Output = 'zookeeper' },
    @{ Name = 'Apq.Cfg.Vault'; Output = 'vault' },
    @{ Name = 'Apq.Cfg.Database'; Output = 'database' },
    @{ Name = 'Apq.Cfg.Crypto.DataProtection'; Output = 'crypto-dp' }
)

# 检查 DefaultDocumentation 是否安装
$toolInstalled = dotnet tool list -g | Select-String 'defaultdocumentation'
if (-not $toolInstalled) {
    Write-ColorText '  安装 DefaultDocumentation...' 'Yellow'
    dotnet tool install -g DefaultDocumentation.Console 2>&1 | Out-Null
}

# 创建 API 文档目录
if (-not (Test-Path $apiDocsDir)) {
    New-Item -ItemType Directory -Path $apiDocsDir -Force | Out-Null
}

$docSuccessCount = 0
$docFailCount = 0

foreach ($proj in $docProjects) {
    $dllPath = Join-Path $RootDir "$($proj.Name)\bin\Release\net9.0\$($proj.Name).dll"
    $projOutputDir = Join-Path $apiDocsDir $proj.Output

    if (Test-Path $dllPath) {
        # 清空并创建输出目录
        if (Test-Path $projOutputDir) {
            Remove-Item -Path "$projOutputDir\*" -Force -Recurse -ErrorAction SilentlyContinue
        } else {
            New-Item -ItemType Directory -Path $projOutputDir -Force | Out-Null
        }

        # 运行 DefaultDocumentation
        & defaultdocumentation -a $dllPath -o $projOutputDir 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0) {
            $docSuccessCount++
        } else {
            $docFailCount++
        }
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

$indexPath = Join-Path $apiDocsDir 'index.md'
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($indexPath, $indexContent, $utf8NoBom)

Write-ColorText "  API 文档: $docSuccessCount 成功" 'Green'
if ($docFailCount -gt 0) {
    Write-ColorText "  API 文档: $docFailCount 失败" 'Yellow'
}
Write-Host ''

# 列出生成的包
$packages = Get-ChildItem -Path $OutputDir -Filter "Apq.Cfg*.nupkg" -ErrorAction SilentlyContinue | Sort-Object Name
if ($packages.Count -gt 0) {
    Write-ColorText '生成的 NuGet 包:' 'Cyan'
    foreach ($pkg in $packages) {
        $size = [math]::Round($pkg.Length / 1KB, 1)
        Write-ColorText "  $($pkg.Name) ($size KB)" 'White'
    }
    Write-Host ''
}

$symbolPackages = Get-ChildItem -Path $OutputDir -Filter "Apq.Cfg*.snupkg" -ErrorAction SilentlyContinue | Sort-Object Name
if ($symbolPackages.Count -gt 0) {
    Write-ColorText '生成的符号包:' 'Cyan'
    foreach ($pkg in $symbolPackages) {
        $size = [math]::Round($pkg.Length / 1KB, 1)
        Write-ColorText "  $($pkg.Name) ($size KB)" 'White'
    }
    Write-Host ''
}

Write-ColorText '下一步操作:' 'Yellow'
Write-ColorText '  运行 push-nuget.bat 发布到 NuGet' 'Gray'
Write-Host ''
