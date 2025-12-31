# create-version.ps1
# 为所有包创建指定版本文件
param(
    [string]$Version
)

$projects = @(
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

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$VersionsDir = Join-Path $RootDir 'versions'

# 获取当前最大版本号
function Get-MaxVersion {
    $maxVersion = $null
    $maxPatch = -1

    # 从 Apq.Cfg 目录获取版本文件
    $versionFiles = Get-ChildItem -Path (Join-Path $VersionsDir 'Apq.Cfg') -Filter 'v*.md' -ErrorAction SilentlyContinue

    foreach ($file in $versionFiles) {
        if ($file.BaseName -match '^v(\d+)\.(\d+)\.(\d+)') {
            $major = [int]$Matches[1]
            $minor = [int]$Matches[2]
            $patch = [int]$Matches[3]
            $totalPatch = $major * 10000 + $minor * 100 + $patch

            if ($totalPatch -gt $maxPatch) {
                $maxPatch = $totalPatch
                $maxVersion = "$major.$minor.$patch"
            }
        }
    }

    return $maxVersion
}

# 计算下一个版本号
function Get-NextVersion {
    param([string]$CurrentVersion)

    if ($CurrentVersion -match '^(\d+)\.(\d+)\.(\d+)$') {
        $major = [int]$Matches[1]
        $minor = [int]$Matches[2]
        $patch = [int]$Matches[3] + 1
        return "$major.$minor.$patch"
    }
    return "1.0.0"
}

# 如果没有提供版本号，交互式输入
if ([string]::IsNullOrWhiteSpace($Version)) {
    $currentMax = Get-MaxVersion
    $defaultVersion = Get-NextVersion -CurrentVersion $currentMax

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Apq.Cfg 版本文件创建工具" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "当前最大版本: v$currentMax" -ForegroundColor Gray
    Write-Host "默认新版本:   v$defaultVersion" -ForegroundColor Green
    Write-Host ""
    Write-Host "版本格式: X.X.X 或 X.X.X-beta1 或 X.X.X-rc.1" -ForegroundColor DarkGray
    Write-Host ""

    $input = Read-Host "请输入版本号 (直接回车使用默认值 v$defaultVersion)"

    if ([string]::IsNullOrWhiteSpace($input)) {
        $Version = $defaultVersion
    } else {
        # 去掉开头的 v（如果有）
        $Version = $input -replace '^v', ''
    }
}

# 去掉开头的 v（如果有）
$Version = $Version -replace '^v', ''

# 验证版本号格式
if ($Version -notmatch '^\d+\.\d+\.\d+(-[a-zA-Z0-9.]+)?$') {
    Write-Host "错误: 版本号格式不正确！" -ForegroundColor Red
    Write-Host "正确格式: X.X.X 或 X.X.X-beta1 或 X.X.X-rc.1" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "将创建版本: v$Version" -ForegroundColor Cyan
Write-Host ""

foreach ($proj in $projects) {
    $path = Join-Path $VersionsDir "$proj\v$Version.md"
    [System.IO.File]::WriteAllText($path, '', [System.Text.Encoding]::UTF8)
    Write-Host "  已创建: $proj\v$Version.md" -ForegroundColor Green
}

Write-Host ""
Write-Host "完成! 已为 $($projects.Count) 个包创建 v$Version 版本文件。" -ForegroundColor Cyan