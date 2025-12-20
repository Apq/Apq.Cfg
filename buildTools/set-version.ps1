# set-version.ps1
param(
    [string]$Version,
    [string]$Suffix
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$PropsFile = Join-Path $RootDir 'Directory.Build.props'

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
}

# 读取单个按键输入，按Q立即退出
function Read-KeyOrInput {
    param([string]$Prompt)
    Write-Host $Prompt -NoNewline
    $input = ''
    while ($true) {
        $key = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
        if ($key.Character -eq 'q' -or $key.Character -eq 'Q') {
            Write-Host ''
            Write-ColorText '已退出' 'Yellow'
            exit 0
        }
        if ($key.VirtualKeyCode -eq 13) {  # Enter
            Write-Host ''
            return $input
        }
        if ($key.VirtualKeyCode -eq 8) {  # Backspace
            if ($input.Length -gt 0) {
                $input = $input.Substring(0, $input.Length - 1)
                Write-Host "`b `b" -NoNewline
            }
        } elseif ($key.Character -match '[\x20-\x7E]') {
            $input += $key.Character
            Write-Host $key.Character -NoNewline
        }
    }
}

# 读取 Y/N 确认，按Q立即退出
function Read-Confirm {
    param([string]$Prompt, [bool]$DefaultYes = $false)
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
        if ($key.VirtualKeyCode -eq 13) {  # Enter - 使用默认值
            if ($DefaultYes) {
                Write-Host 'Y'
                return $true
            } else {
                Write-Host 'N'
                return $false
            }
        }
    }
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg 版本号设置工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $PropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.props 文件' 'Red'
    Write-ColorText "路径: $PropsFile" 'Red'
    exit 1
}

$fileContent = Get-Content $PropsFile -Raw -Encoding UTF8
if ($fileContent -match '<Version>([^<]+)</Version>') {
    $currentVersion = $Matches[1]
    Write-ColorText "当前版本: $currentVersion" 'Yellow'
} else {
    Write-ColorText '警告: 无法读取当前版本号' 'Yellow'
    $currentVersion = '未知'
}

if ([string]::IsNullOrWhiteSpace($Version)) {
    Write-Host ''
    do {
        $Version = Read-KeyOrInput '请输入新版本号 (X.X.X 格式): '
        if ([string]::IsNullOrWhiteSpace($Version)) {
            Write-ColorText '版本号不能为空，请重新输入' 'Yellow'
        }
    } while ([string]::IsNullOrWhiteSpace($Version))
}

# 严格验证 X.X.X 格式
if ($Version -notmatch '^[0-9]+\.[0-9]+\.[0-9]+$') {
    Write-ColorText '错误: 版本号格式不正确，必须为 X.X.X 格式 (例如: 1.0.0)' 'Red'
    exit 1
}

if ([string]::IsNullOrWhiteSpace($Suffix)) {
    Write-Host ''
    Write-ColorText '是否添加预发布后缀? (留空跳过)' 'Gray'
    Write-ColorText '  示例: alpha1, beta1, rc1, preview1' 'Gray'
    $Suffix = Read-KeyOrInput '预发布后缀: '
}

if ([string]::IsNullOrWhiteSpace($Suffix)) {
    $fullVersion = $Version
} else {
    if ($Suffix -notmatch '^[a-zA-Z][a-zA-Z0-9]*$') {
        Write-ColorText '错误: 后缀格式不正确' 'Red'
        exit 1
    }
    $fullVersion = "$Version-$Suffix"
}

Write-Host ''
Write-ColorText "新版本号: $fullVersion" 'Green'
Write-Host ''

if (-not (Read-Confirm '确认更新版本号? (Y/N): ')) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

try {
    $newContent = $fileContent -replace '<Version>[^<]+</Version>', "<Version>$fullVersion</Version>"
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($PropsFile, $newContent, $utf8NoBom)

    Write-Host ''
    Write-ColorText "版本号已更新: $currentVersion -> $fullVersion" 'Green'
    Write-Host ''
    Write-ColorText '受影响的 NuGet 包:' 'Cyan'
    Write-ColorText '  - Apq.Cfg' 'White'
    Write-ColorText '  - Apq.Cfg.Ini' 'White'
    Write-ColorText '  - Apq.Cfg.Xml' 'White'
    Write-ColorText '  - Apq.Cfg.Yaml' 'White'
    Write-ColorText '  - Apq.Cfg.Toml' 'White'
    Write-ColorText '  - Apq.Cfg.Redis' 'White'
    Write-ColorText '  - Apq.Cfg.Database' 'White'
    Write-Host ''

    # 询问是否生成新版本包
    $doPack = Read-Confirm '是否生成新版本包? (Y/n，默认为 Y): ' $true

    if ($doPack) {
        Write-Host ''
        Write-ColorText '开始生成新版本包...' 'Cyan'
        Write-Host ''

        $PackScript = Join-Path $ScriptDir 'pack-release.ps1'
        & $PackScript
    } else {
        Write-ColorText '下一步操作:' 'Yellow'
        Write-ColorText '  1. 运行 pack-release.bat 生成新版本包' 'Gray'
        Write-ColorText '  2. 运行 push-nuget.bat 发布到 NuGet' 'Gray'
        Write-Host ''
    }
} catch {
    Write-ColorText '错误: 更新版本号失败' 'Red'
    Write-ColorText $_.Exception.Message 'Red'
    exit 1
}