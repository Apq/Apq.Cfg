# pack-release.ps1
param(
    [switch]$NoBuild,
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$PropsFile = Join-Path $RootDir 'Directory.Build.props'
$DefaultOutputDir = Join-Path $RootDir 'nupkgs'

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

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg NuGet 包生成工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $PropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.props 文件' 'Red'
    Write-ColorText "路径: $PropsFile" 'Red'
    exit 1
}

# 从 versions 目录获取版本号（与 Directory.Build.props 保持一致）
$VersionsDir = Join-Path $RootDir 'versions'
if (-not (Test-Path $VersionsDir)) {
    Write-ColorText '错误: 找不到 versions 目录' 'Red'
    Write-ColorText "路径: $VersionsDir" 'Red'
    exit 1
}

$versionFiles = @(Get-ChildItem -Path $VersionsDir -Filter 'v*.md' -ErrorAction SilentlyContinue)
$versions = @($versionFiles | Where-Object { $_.BaseName -match '^v(\d+)\.(\d+)\.(\d+)' } | ForEach-Object {
    $fullVersion = $_.BaseName -replace '^v', ''
    $baseVersion = $_.BaseName -replace '^v(\d+\.\d+\.\d+).*', '$1'
    [PSCustomObject]@{
        Name = $fullVersion
        Version = [version]$baseVersion
    }
} | Sort-Object Version -Descending)

if ($versions.Count -gt 0) {
    $currentVersion = $versions[0].Name
    Write-ColorText "当前版本: $currentVersion" 'Yellow'
} else {
    Write-ColorText '错误: 无法从 versions 目录获取版本号' 'Red'
    Write-ColorText '请确保 versions 目录中存在 v*.*.*.md 格式的版本文件' 'Red'
    exit 1
}

# 设置输出目录
if ([string]::IsNullOrWhiteSpace($OutputDir)) {
    $OutputDir = $DefaultOutputDir
}

Write-Host ''
Write-ColorText '将要打包的项目:' 'Cyan'
Write-ColorText '  - Apq.Cfg' 'White'
Write-ColorText '  - Apq.Cfg.Ini' 'White'
Write-ColorText '  - Apq.Cfg.Xml' 'White'
Write-ColorText '  - Apq.Cfg.Yaml' 'White'
Write-ColorText '  - Apq.Cfg.Toml' 'White'
Write-ColorText '  - Apq.Cfg.Redis' 'White'
Write-ColorText '  - Apq.Cfg.Database' 'White'
Write-Host ''
Write-ColorText "输出目录: $OutputDir" 'Gray'
Write-Host ''

if (-not (Read-Confirm '确认开始打包? ([Y]/N): ')) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

# 创建输出目录
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
    Write-ColorText "已创建输出目录: $OutputDir" 'Gray'
}

Write-Host ''
Write-ColorText '开始打包...' 'Cyan'
Write-Host ''

# 删除当前版本的旧包（避免 NuGet 缓存冲突）
$oldPackages = Get-ChildItem -Path $OutputDir -Filter "Apq.Cfg*.$currentVersion.*pkg" -ErrorAction SilentlyContinue
if ($oldPackages.Count -gt 0) {
    Write-ColorText "清理当前版本 ($currentVersion) 的旧包..." 'Gray'
    foreach ($pkg in $oldPackages) {
        Remove-Item $pkg.FullName -Force
        Write-ColorText "  已删除: $($pkg.Name)" 'DarkGray'
    }
    Write-Host ''
}

# 构建打包参数
$packArgs = @(
    'pack'
    $RootDir
    '-c', 'Release'
    '-o', $OutputDir
)

if ($NoBuild) {
    $packArgs += '--no-build'
    Write-ColorText '跳过构建 (使用 --no-build)' 'Gray'
}

try {
    # 执行打包
    & dotnet @packArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Host ''
        Write-ColorText '错误: 打包失败' 'Red'
        exit 1
    }

    Write-Host ''
    Write-ColorText '打包完成!' 'Green'
    Write-Host ''

    # 列出生成的包
    $packages = Get-ChildItem -Path $OutputDir -Filter "*.nupkg" | Where-Object { $_.Name -match "Apq\.Cfg.*\.$([regex]::Escape($currentVersion))\.nupkg$" }
    $symbolPackages = Get-ChildItem -Path $OutputDir -Filter "*.snupkg" | Where-Object { $_.Name -match "Apq\.Cfg.*\.$([regex]::Escape($currentVersion))\.snupkg$" }

    if ($packages.Count -gt 0) {
        Write-ColorText '生成的 NuGet 包:' 'Cyan'
        foreach ($pkg in $packages) {
            $size = [math]::Round($pkg.Length / 1KB, 1)
            Write-ColorText "  $($pkg.Name) ($size KB)" 'White'
        }
        Write-Host ''

        if ($symbolPackages.Count -gt 0) {
            Write-ColorText '生成的符号包:' 'Cyan'
            foreach ($pkg in $symbolPackages) {
                $size = [math]::Round($pkg.Length / 1KB, 1)
                Write-ColorText "  $($pkg.Name) ($size KB)" 'White'
            }
            Write-Host ''
        }
    }

    Write-ColorText '下一步操作:' 'Yellow'
    Write-ColorText '  运行 push-nuget.bat 发布到 NuGet' 'Gray'
    Write-Host ''

} catch {
    Write-Host ''
    Write-ColorText '错误: 打包过程中发生异常' 'Red'
    Write-ColorText $_.Exception.Message 'Red'
    exit 1
}
