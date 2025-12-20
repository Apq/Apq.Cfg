# bump-version.ps1
param(
    [ValidateSet('major', 'minor', 'patch')]
    [string]$Part,
    [string]$Suffix
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$PropsFile = Join-Path $RootDir 'Directory.Build.props'
$SetVersionScript = Join-Path $ScriptDir 'set-version.ps1'

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

# 读取单个选择键
function Read-Choice {
    param([string]$Prompt, [string[]]$ValidKeys)
    Write-Host $Prompt -NoNewline
    while ($true) {
        $key = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown')
        if ($key.Character -eq 'q' -or $key.Character -eq 'Q') {
            Write-Host ''
            Write-ColorText '已退出' 'Yellow'
            exit 0
        }
        $char = $key.Character.ToString()
        if ($ValidKeys -contains $char -or $key.VirtualKeyCode -eq 13) {
            Write-Host $char
            return $char
        }
    }
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg 版本号自动增长工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

if (-not (Test-Path $SetVersionScript)) {
    Write-ColorText '错误: 找不到 set-version.ps1 文件' 'Red'
    Write-ColorText "路径: $SetVersionScript" 'Red'
    exit 1
}

if (-not (Test-Path $PropsFile)) {
    Write-ColorText '错误: 找不到 Directory.Build.props 文件' 'Red'
    Write-ColorText "路径: $PropsFile" 'Red'
    exit 1
}

$fileContent = Get-Content $PropsFile -Raw -Encoding UTF8
if ($fileContent -match '<Version>([^<]+)</Version>') {
    $currentVersionFull = $Matches[1]
    if ($currentVersionFull -match '^(\d+\.\d+\.\d+)(-(.+))?$') {
        $currentVersion = $Matches[1]
        $currentSuffix = $Matches[3]
    } else {
        Write-ColorText "错误: 无法解析版本号格式: $currentVersionFull" 'Red'
        exit 1
    }
    Write-ColorText "当前版本: $currentVersionFull" 'Yellow'
} else {
    Write-ColorText '错误: 无法读取当前版本号' 'Red'
    exit 1
}

$versionParts = $currentVersion.Split('.')
$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$patch = [int]$versionParts[2]

if ([string]::IsNullOrWhiteSpace($Part)) {
    Write-Host ''
    Write-ColorText '请选择要增长的版本部分:' 'White'
    Write-ColorText "  [1] Major (主版本号): $major.0.0 -> $($major + 1).0.0" 'Gray'
    Write-ColorText "  [2] Minor (次版本号): $major.$minor.0 -> $major.$($minor + 1).0" 'Gray'
    Write-ColorText "  [3] Patch (修订号):   $major.$minor.$patch -> $major.$minor.$($patch + 1)" 'Gray'
    Write-Host ''

    $choice = Read-Choice '请选择 (1/2/3，默认为 3): ' @('1', '2', '3')

    switch ($choice) {
        '1' { $Part = 'major' }
        '2' { $Part = 'minor' }
        default { $Part = 'patch' }
    }
}

switch ($Part) {
    'major' {
        $major++
        $minor = 0
        $patch = 0
    }
    'minor' {
        $minor++
        $patch = 0
    }
    'patch' {
        $patch++
    }
}

$newVersion = "$major.$minor.$patch"

if (-not $PSBoundParameters.ContainsKey('Suffix')) {
    Write-Host ''
    Write-ColorText '是否添加预发布后缀? (留空保留现有后缀，输入 clear 清除)' 'Gray'
    Write-ColorText '  示例: alpha1, beta1, rc1, preview1' 'Gray'
    if ($currentSuffix) {
        Write-ColorText "  当前后缀: $currentSuffix" 'Yellow'
    }
    $Suffix = Read-KeyOrInput '预发布后缀: '
}

if ($Suffix -eq 'clear') {
    $finalSuffix = ''
} elseif ([string]::IsNullOrWhiteSpace($Suffix)) {
    $finalSuffix = $currentSuffix
} else {
    $finalSuffix = $Suffix
}

Write-Host ''
Write-ColorText '即将调用 set-version.ps1 设置新版本...' 'Cyan'
Write-Host ''

if ([string]::IsNullOrWhiteSpace($finalSuffix)) {
    & $SetVersionScript -Version $newVersion
} else {
    & $SetVersionScript -Version $newVersion -Suffix $finalSuffix
}