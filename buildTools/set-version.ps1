# set-version.ps1
# 更新 Directory.Build.Version.props 中的版本号
param(
    [string]$Version
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$VersionPropsFile = Join-Path $RootDir 'Directory.Build.Version.props'

# 从 Directory.Build.Version.props 获取当前版本号
function Get-CurrentVersion {
    if (-not (Test-Path $VersionPropsFile)) {
        return $null
    }
    $content = [System.IO.File]::ReadAllText($VersionPropsFile)
    if ($content -match '<ApqCfgVersion>([^<]+)</ApqCfgVersion>') {
        return $Matches[1]
    }
    return $null
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
    $currentVersion = Get-CurrentVersion
    $defaultVersion = Get-NextVersion -CurrentVersion $currentVersion

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Apq.Cfg 版本管理工具" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "当前版本: v$currentVersion" -ForegroundColor Gray
    Write-Host "默认新版本: v$defaultVersion" -ForegroundColor Green
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
Write-Host "将设置版本: v$Version" -ForegroundColor Cyan
Write-Host ""

# 更新 Directory.Build.Version.props 文件
$newContent = @"
<Project>
  <!-- 中央版本管理 -->
  <PropertyGroup>
    <ApqCfgVersion>$Version</ApqCfgVersion>
  </PropertyGroup>

  <!-- 版本号映射 -->
  <PropertyGroup>
    <Version>`$(ApqCfgVersion)</Version>
    <PackageVersion>`$(ApqCfgVersion)</PackageVersion>
    <AssemblyVersion>`$(ApqCfgVersion).0</AssemblyVersion>
    <FileVersion>`$(ApqCfgVersion).0</FileVersion>
  </PropertyGroup>

  <!-- NuGet 包公共元数据 -->
  <PropertyGroup>
    <Authors>Apq</Authors>
    <Company>Apq</Company>
    <Copyright>Copyright © Apq `$([System.DateTime]::Now.Year)</Copyright>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>

    <!-- 启用包生成 -->
    <IsPackable>true</IsPackable>

    <!-- 源码链接支持 -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>
</Project>
"@

[System.IO.File]::WriteAllText($VersionPropsFile, $newContent, [System.Text.Encoding]::UTF8)
Write-Host "  已更新: Directory.Build.Version.props" -ForegroundColor Green

Write-Host ""
Write-Host "完成! 版本已设置为 v$Version" -ForegroundColor Cyan
