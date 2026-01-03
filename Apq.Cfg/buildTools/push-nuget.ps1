# push-nuget.ps1
param(
    [string]$ApiKey,
    [string]$Source = 'https://api.nuget.org/v3/index.json',
    [string]$PackageDir,
    [switch]$SkipConfirm
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$ApqNewDir = Split-Path -Parent $RootDir
$PropsFile = Join-Path $RootDir 'Directory.Build.props'
$DefaultPackageDir = Join-Path $RootDir 'nupkgs'
$ApiKeyFile = Join-Path $ScriptDir 'NuGet_Apq_Key.txt'

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
}

# 读取 Y/N 确认，按Q立即退出
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
        if ($key.Character -eq 'y' -or $key.Character -eq 'Y') {
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
Write-ColorText '  Apq.Cfg NuGet 发布工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $PropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.props 文件' 'Red'
    Write-ColorText "路径: $PropsFile" 'Red'
    exit 1
}

# 读取当前版本
$fileContent = Get-Content $PropsFile -Raw -Encoding UTF8
if ($fileContent -match '<Version>([^<]+)</Version>') {
    $currentVersion = $Matches[1]
    Write-ColorText "当前版本: $currentVersion" 'Yellow'
} else {
    Write-ColorText '错误: 无法读取当前版本号' 'Red'
    exit 1
}

# 设置包目录
if ([string]::IsNullOrWhiteSpace($PackageDir)) {
    $PackageDir = $DefaultPackageDir
}

if (-not (Test-Path $PackageDir)) {
    Write-ColorText '错误: 包目录不存在' 'Red'
    Write-ColorText "路径: $PackageDir" 'Red'
    Write-ColorText '请先运行 pack-release.bat 生成包' 'Yellow'
    exit 1
}

# 查找当前版本的包
$packages = Get-ChildItem -Path $PackageDir -Filter "*.nupkg" | Where-Object { $_.Name -match "Apq\.Cfg.*\.$([regex]::Escape($currentVersion))\.nupkg$" }
$symbolPackages = Get-ChildItem -Path $PackageDir -Filter "*.snupkg" | Where-Object { $_.Name -match "Apq\.Cfg.*\.$([regex]::Escape($currentVersion))\.snupkg$" }

if ($packages.Count -eq 0) {
    Write-ColorText "错误: 未找到版本 $currentVersion 的包" 'Red'
    Write-ColorText "路径: $PackageDir" 'Red'
    Write-ColorText '请先运行 pack-release.bat 生成包' 'Yellow'
    exit 1
}

Write-Host ''
Write-ColorText '将要发布的包:' 'Cyan'
foreach ($pkg in $packages) {
    $size = [math]::Round($pkg.Length / 1KB, 1)
    Write-ColorText "  $($pkg.Name) ($size KB)" 'White'
}

if ($symbolPackages.Count -gt 0) {
    Write-Host ''
    Write-ColorText '符号包 (将自动上传):' 'Cyan'
    foreach ($pkg in $symbolPackages) {
        $size = [math]::Round($pkg.Length / 1KB, 1)
        Write-ColorText "  $($pkg.Name) ($size KB)" 'Gray'
    }
}
Write-Host ''
Write-ColorText "目标源: $Source" 'Gray'
Write-Host ''

# 获取 API Key
if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    # 从文件读取 API Key
    if (Test-Path $ApiKeyFile) {
        $ApiKey = (Get-Content $ApiKeyFile -First 1 -Encoding UTF8).Trim()
        if ([string]::IsNullOrWhiteSpace($ApiKey)) {
            Write-ColorText '错误: API Key 文件内容为空' 'Red'
            Write-ColorText "路径: $ApiKeyFile" 'Red'
            exit 1
        }
        Write-ColorText "已从文件读取 API Key: $ApiKeyFile" 'Gray'
    } else {
        Write-ColorText '错误: 找不到 API Key 文件' 'Red'
        Write-ColorText "路径: $ApiKeyFile" 'Red'
        exit 1
    }
}

Write-Host ''
if (-not $SkipConfirm) {
    if (-not (Read-Confirm '确认发布到 NuGet? (Y/N): ')) {
        Write-ColorText '已取消' 'Yellow'
        exit 0
    }
}

Write-Host ''
Write-ColorText '开始并行发布...' 'Cyan'
Write-Host ''

$successCount = 0
$failCount = 0

# 使用 ForEach-Object -Parallel 并行发布（线程池，无额外进程）
$results = $packages | ForEach-Object -Parallel {
    $pkg = $_
    $result = & dotnet nuget push $pkg.FullName -s $using:Source -k $using:ApiKey --skip-duplicate 2>&1
    [PSCustomObject]@{
        ExitCode = $LASTEXITCODE
        Output = ($result | Out-String)
        PkgName = $pkg.Name
    }
} -ThrottleLimit 8

# 处理结果
foreach ($result in $results) {
    if ($result.ExitCode -eq 0) {
        Write-ColorText "  $($result.PkgName) - 成功" 'Green'
        $successCount++
    } else {
        if ($result.Output -match 'already exists') {
            Write-ColorText "  $($result.PkgName) - 跳过 (已存在)" 'Yellow'
            $successCount++
        } else {
            Write-ColorText "  $($result.PkgName) - 失败: $($result.Output)" 'Red'
            $failCount++
        }
    }
}

Write-Host ''
Write-ColorText "========================================" 'Cyan'
Write-ColorText "发布完成: 成功 $successCount, 失败 $failCount" $(if ($failCount -eq 0) { 'Green' } else { 'Yellow' })
Write-ColorText "========================================" 'Cyan'
Write-Host ''

if ($failCount -eq 0) {
    Write-ColorText '所有包已成功发布!' 'Green'
    Write-Host ''
    Write-ColorText '查看已发布的包:' 'Yellow'
    Write-ColorText '  https://www.nuget.org/profiles/Apq' 'DarkGray'
} else {
    Write-ColorText "有 $failCount 个包发布失败，请检查错误信息" 'Red'
}
Write-Host ''
