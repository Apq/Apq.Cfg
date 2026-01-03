# set-unlist-versions.ps1
# 将 NuGet 上指定版本范围的包设为 unlisted 或 listed
# 用法:
#   set-unlist-versions.bat                    # unlist 低于当前版本的所有包
#   set-unlist-versions.bat 1.1.6              # unlist 低于 1.1.6 的所有包
#   set-unlist-versions.bat 1.0.0-1.1.5        # unlist 1.0.0 到 1.1.5 范围内的包
#   set-unlist-versions.bat -List 1.0.0-1.1.5  # 将 1.0.0 到 1.1.5 范围内的包恢复为 listed
#   set-unlist-versions.bat -WhatIf            # 模拟模式，不实际执行
param(
    [string]$Version,
    [switch]$List,    # 恢复为 listed 状态（重新上架）
    [switch]$WhatIf   # 仅显示将要执行的操作，不实际执行
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$VersionPropsFile = Join-Path $RootDir 'Directory.Build.Version.props'
$ProjectsFile = Join-Path $ScriptDir 'projects.txt'
$ApiKeyFile = Join-Path $ScriptDir 'NuGet_Apq_Key.txt'

# NuGet API 基础 URL
$NuGetPackagePublishUrl = 'https://www.nuget.org/api/v2/package'

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
}

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

# 解析版本号为可比较的对象
function Parse-Version {
    param([string]$v)

    # 提取核心版本号和预发布标签
    $core = $v -replace '-.*$', ''
    $prerelease = if ($v -match '-(.+)$') { $Matches[1] } else { $null }

    try {
        return [PSCustomObject]@{
            Version = [version]$core
            Prerelease = $prerelease
            Original = $v
        }
    } catch {
        return $null
    }
}

# 比较版本号，返回 -1, 0, 1
function Compare-Versions {
    param([string]$v1, [string]$v2)

    $ver1 = Parse-Version $v1
    $ver2 = Parse-Version $v2

    if (-not $ver1 -or -not $ver2) { return 0 }

    # 先比较核心版本
    if ($ver1.Version -lt $ver2.Version) { return -1 }
    if ($ver1.Version -gt $ver2.Version) { return 1 }

    # 核心版本相同，比较预发布标签
    # 无预发布 > 有预发布（正式版 > beta）
    if (-not $ver1.Prerelease -and $ver2.Prerelease) { return 1 }
    if ($ver1.Prerelease -and -not $ver2.Prerelease) { return -1 }
    if ($ver1.Prerelease -and $ver2.Prerelease) {
        return [string]::Compare($ver1.Prerelease, $ver2.Prerelease, [StringComparison]::OrdinalIgnoreCase)
    }

    return 0
}

# 检查版本是否在范围内
function Test-VersionInRange {
    param(
        [string]$ver,
        [string]$minVersion,
        [string]$maxVersion
    )

    if ($minVersion -and (Compare-Versions $ver $minVersion) -lt 0) {
        return $false
    }
    if ($maxVersion -and (Compare-Versions $ver $maxVersion) -gt 0) {
        return $false
    }
    return $true
}

# 从 NuGet API 获取包的所有版本
function Get-NuGetPackageVersions {
    param([string]$PackageId)

    $url = "https://api.nuget.org/v3-flatcontainer/$($PackageId.ToLower())/index.json"
    try {
        $response = Invoke-RestMethod -Uri $url -Method Get -ErrorAction Stop
        return $response.versions
    } catch {
        if ($_.Exception.Response.StatusCode -eq 404) {
            return @()
        }
        throw
    }
}

# 解析版本范围参数（格式：1.0.0~1.1.5 或 1.0.0-1.1.5）
# 注意：1.0.0-beta 这种预发布版本不应被解析为范围
function Parse-VersionRange {
    param([string]$VersionStr)

    # 优先使用 ~ 作为范围分隔符
    if ($VersionStr -match '^(\d+\.\d+\.\d+)~(\d+\.\d+\.\d+)$') {
        return @{
            IsRange = $true
            MinVersion = $Matches[1]
            MaxVersion = $Matches[2]
        }
    }

    # 使用 - 作为范围分隔符，但要排除预发布版本格式（如 1.0.0-beta）
    # 版本范围格式：两边都是纯数字版本号（如 1.0.0-1.1.5）
    if ($VersionStr -match '^(\d+\.\d+\.\d+)-(\d+\.\d+\.\d+)$') {
        return @{
            IsRange = $true
            MinVersion = $Matches[1]
            MaxVersion = $Matches[2]
        }
    }

    # 不是范围，返回单个版本
    return @{
        IsRange = $false
        Version = $VersionStr -replace '^v', ''
    }
}

$actionText = if ($List) { 'List (重新上架)' } else { 'Unlist (下架)' }
$actionVerb = if ($List) { 'list' } else { 'unlist' }

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText "  Apq.Cfg NuGet 版本 $actionText 工具" 'Cyan'
Write-ColorText "========================================" 'Cyan'
if ($WhatIf) {
    Write-ColorText '  [模拟模式] 不会实际执行操作' 'Yellow'
}
Write-ColorText "========================================`n" 'Cyan'

# 检查 API Key
$apiKey = $null
if (-not $WhatIf) {
    if (Test-Path $ApiKeyFile) {
        $apiKey = (Get-Content $ApiKeyFile -First 1 -Encoding UTF8).Trim()
        if ([string]::IsNullOrWhiteSpace($apiKey)) {
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

# 解析版本参数
$minVersion = $null
$maxVersion = $null
$mode = 'lessThan'  # lessThan 或 range

if ([string]::IsNullOrWhiteSpace($Version)) {
    # 使用当前版本
    $maxVersion = Get-CurrentVersion
    if (-not $maxVersion) {
        Write-ColorText '错误: 无法获取版本号，请通过 -Version 参数指定' 'Red'
        exit 1
    }
    $mode = 'lessThan'
} else {
    $parsed = Parse-VersionRange $Version
    if ($parsed.IsRange) {
        $minVersion = $parsed.MinVersion
        $maxVersion = $parsed.MaxVersion
        $mode = 'range'
    } else {
        $maxVersion = $parsed.Version
        $mode = 'lessThan'
    }
}

# 显示模式信息
if ($mode -eq 'range') {
    Write-ColorText "版本范围: v$minVersion ~ v$maxVersion" 'Green'
    Write-ColorText "此范围内的包将被设为 $actionVerb`n" 'Gray'
} else {
    Write-ColorText "目标版本: v$maxVersion" 'Green'
    Write-ColorText "低于此版本的包将被设为 $actionVerb`n" 'Gray'
}

# 读取项目列表
if (-not (Test-Path $ProjectsFile)) {
    Write-ColorText '错误: 找不到 projects.txt 文件' 'Red'
    exit 1
}

$projects = @(Get-Content $ProjectsFile | Where-Object { $_ -and $_ -notmatch '^\s*#' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })

Write-ColorText "正在查询 NuGet 上的包版本...`n" 'Cyan'

# 收集所有需要处理的包版本
$allTargets = @()

foreach ($project in $projects) {
    Write-ColorText "检查 $project..." 'Gray'

    $versions = Get-NuGetPackageVersions -PackageId $project

    if ($versions.Count -eq 0) {
        Write-ColorText "  未找到任何版本" 'DarkGray'
        continue
    }

    # 根据模式筛选版本
    if ($mode -eq 'range') {
        $targetVersions = @($versions | Where-Object { Test-VersionInRange -ver $_ -minVersion $minVersion -maxVersion $maxVersion })
    } else {
        # lessThan 模式：低于 maxVersion 的所有版本
        $targetVersions = @($versions | Where-Object { (Compare-Versions $_ $maxVersion) -lt 0 })
    }

    if ($targetVersions.Count -eq 0) {
        Write-ColorText "  没有需要 $actionVerb 的版本" 'DarkGray'
        continue
    }

    Write-ColorText "  找到 $($targetVersions.Count) 个版本需要 $actionVerb" 'Yellow'

    foreach ($ver in $targetVersions) {
        $allTargets += [PSCustomObject]@{
            Package = $project
            Version = $ver
        }
    }
}

if ($allTargets.Count -eq 0) {
    Write-ColorText "`n没有需要处理的包版本" 'Yellow'
    exit 0
}

Write-Host ''
Write-ColorText "共计 $($allTargets.Count) 个包版本需要 $actionVerb" 'Cyan'

if ($WhatIf) {
    Write-Host ''
    foreach ($target in $allTargets) {
        Write-ColorText "  [模拟] $($target.Package) $($target.Version)" 'DarkYellow'
    }
} else {
    Write-ColorText "开始处理...`n" 'Cyan'

    # 串行处理（避免 PowerShell 5.1 的并行处理兼容性问题）
    $totalProcessed = 0
    $totalFailed = 0

    # 临时改变错误处理策略，避免单个请求失败导致整个脚本终止
    $oldErrorAction = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'

    foreach ($target in $allTargets) {
        $url = "$NuGetPackagePublishUrl/$($target.Package)/$($target.Version)"
        $method = if ($List) { 'Post' } else { 'Delete' }

        Write-Host "  处理 $($target.Package) $($target.Version)... " -NoNewline

        $success = $false
        $errorMessage = $null

        try {
            $headers = @{
                'X-NuGet-ApiKey' = $apiKey
            }
            # 使用 Invoke-RestMethod 替代 Invoke-WebRequest
            $null = Invoke-RestMethod -Uri $url -Method $method -Headers $headers -ErrorAction Stop
            $success = $true
        } catch {
            $errorMessage = $_.Exception.Message
            if ($_.Exception.Response) {
                try {
                    $statusCode = [int]$_.Exception.Response.StatusCode
                    $errorMessage = "HTTP $statusCode"
                } catch {}
            }
        }

        if ($success) {
            Write-ColorText "✓ 已 $actionVerb" 'Green'
            $totalProcessed++
        } else {
            Write-ColorText "✗ 失败: $errorMessage" 'Red'
            $totalFailed++
        }
    }

    $ErrorActionPreference = $oldErrorAction
}

Write-Host ''
Write-ColorText "========================================" 'Cyan'
if ($WhatIf) {
    Write-ColorText "模拟完成!" 'Green'
    Write-ColorText "  将要 $actionVerb`: $($allTargets.Count) 个版本" 'Yellow'
    Write-ColorText "`n移除 -WhatIf 参数以实际执行操作" 'Gray'
} else {
    Write-ColorText "完成!" 'Green'
    Write-ColorText "  已 $actionVerb`: $totalProcessed 个版本" 'Green'
    if ($totalFailed -gt 0) {
        Write-ColorText "  失败: $totalFailed 个版本" 'Red'
    }
}
Write-ColorText "========================================" 'Cyan'
