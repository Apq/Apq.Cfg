# pack-release.ps1
# NuGet 包打包脚本
param(
    [switch]$NoBuild,
    [string]$OutputDir,
    [string[]]$Projects  # 可选：指定要打包的项目，不指定则打包所有
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$VersionPropsFile = Join-Path $RootDir 'Directory.Build.Version.props'
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

# 从 Directory.Build.Version.props 获取版本号
function Get-Version {
    if (-not (Test-Path $VersionPropsFile)) {
        return $null
    }
    $content = [System.IO.File]::ReadAllText($VersionPropsFile)
    if ($content -match '<ApqCfgVersion>([^<]+)</ApqCfgVersion>') {
        return $Matches[1]
    }
    return $null
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg NuGet 包生成工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $VersionPropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.Version.props 文件' 'Red'
    Write-ColorText "路径: $VersionPropsFile" 'Red'
    exit 1
}

$version = Get-Version
if (-not $version) {
    Write-ColorText '错误: 无法从 Directory.Build.Version.props 读取版本号' 'Red'
    exit 1
}

# 从 projects.txt 读取项目列表
$ProjectsFile = Join-Path $ScriptDir 'projects.txt'
if (-not (Test-Path $ProjectsFile)) {
    Write-ColorText '错误: 找不到 projects.txt 文件' 'Red'
    Write-ColorText "路径: $ProjectsFile" 'Red'
    exit 1
}
$AllProjects = @(Get-Content $ProjectsFile | Where-Object { $_ -and $_ -notmatch '^\s*#' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })

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
Write-ColorText "当前版本: v$version" 'Green'
Write-Host ''
Write-ColorText '将要打包的项目:' 'Cyan'
foreach ($project in $TargetProjects) {
    Write-ColorText "  - $project" 'White'
}

Write-Host ''
Write-ColorText "输出目录: $OutputDir" 'Gray'
Write-Host ''

if (-not (Read-Confirm '确认开始打包? ([Y]/N): ')) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

# 询问是否生成 DocFX 文档
$generateDocFx = Read-Confirm '是否生成 DocFX API 文档? ([Y]/N): '

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

# 生成 DocFX 文档（如果用户选择）
if ($generateDocFx) {
    $docfxScript = Join-Path $ScriptDir 'build-docfx.ps1'
    if (Test-Path $docfxScript) {
        & $docfxScript
    } else {
        Write-ColorText '错误: 找不到 build-docfx.ps1 脚本' 'Red'
    }
    Write-Host ''
}

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
Write-Host
