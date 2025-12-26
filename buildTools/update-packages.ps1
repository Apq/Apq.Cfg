# update-packages.ps1
# 检查并升级 NuGet 依赖包（智能查询框架兼容性）

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
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
        if ($key.VirtualKeyCode -eq 13) {
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

# 缓存已查询的包版本框架信息
$script:FrameworkCache = @{}

# 从 NuGet API 获取包版本支持的目标框架
function Get-PackageFrameworks {
    param(
        [string]$PackageId,
        [string]$Version
    )

    $cacheKey = "$PackageId|$Version"
    if ($script:FrameworkCache.ContainsKey($cacheKey)) {
        return $script:FrameworkCache[$cacheKey]
    }

    try {
        # 使用 NuGet API 获取包元数据
        $lowerPackageId = $PackageId.ToLower()
        $lowerVersion = $Version.ToLower()
        $url = "https://api.nuget.org/v3-flatcontainer/$lowerPackageId/$lowerVersion/$lowerPackageId.nuspec"

        # 使用 Invoke-RestMethod 直接获取 XML
        [xml]$nuspec = Invoke-RestMethod -Uri $url -TimeoutSec 10

        # 提取支持的框架
        $frameworks = @()

        # 使用 XmlNamespaceManager 处理命名空间
        $nsMgr = New-Object System.Xml.XmlNamespaceManager($nuspec.NameTable)
        $nsMgr.AddNamespace("nuget", $nuspec.DocumentElement.NamespaceURI)

        # 从 dependencies 节点获取框架
        $depGroups = $nuspec.SelectNodes('//nuget:dependencies/nuget:group', $nsMgr)
        if ($depGroups -and $depGroups.Count -gt 0) {
            foreach ($group in $depGroups) {
                $tfm = $group.GetAttribute('targetFramework')
                if ($tfm) {
                    $frameworks += $tfm
                }
            }
        }

        # 如果没有 group，可能是支持所有框架或者从 frameworkAssemblies 获取
        if ($frameworks.Count -eq 0) {
            # 尝试从 frameworkReferences 获取
            $fwRefs = $nuspec.SelectNodes('//nuget:frameworkReferences/nuget:group', $nsMgr)
            if ($fwRefs -and $fwRefs.Count -gt 0) {
                foreach ($group in $fwRefs) {
                    $tfm = $group.GetAttribute('targetFramework')
                    if ($tfm) {
                        $frameworks += $tfm
                    }
                }
            }
        }

        $script:FrameworkCache[$cacheKey] = $frameworks
        return $frameworks
    }
    catch {
        $script:FrameworkCache[$cacheKey] = @()
        return @()
    }
}

# 检查包版本是否支持指定的目标框架
function Test-FrameworkCompatibility {
    param(
        [string]$PackageId,
        [string]$Version,
        [string]$TargetFramework
    )

    $frameworks = Get-PackageFrameworks -PackageId $PackageId -Version $Version

    # 如果没有获取到框架信息，假设兼容（可能是 netstandard 包）
    if ($frameworks.Count -eq 0) {
        return $true
    }

    # 解析目标框架版本号
    $targetVersion = 0
    if ($TargetFramework -match 'net(\d+)\.(\d+)') {
        $targetVersion = [int]$Matches[1] * 100 + [int]$Matches[2]
    }
    elseif ($TargetFramework -match 'net(\d+)$') {
        $targetVersion = [int]$Matches[1] * 100
    }

    foreach ($fw in $frameworks) {
        $fwLower = $fw.ToLower()

        # 检查 netstandard 兼容性
        if ($fwLower -match 'netstandard') {
            return $true
        }

        # 检查 .NET Core / .NET 5+ 兼容性
        if ($fwLower -match '\.netcoreapp(\d+)\.(\d+)' -or $fwLower -match 'netcoreapp(\d+)\.(\d+)') {
            $fwVersion = [int]$Matches[1] * 100 + [int]$Matches[2]
            if ($targetVersion -ge $fwVersion) {
                return $true
            }
        }

        # 检查 .NET 5+ 格式 (net5.0, net6.0, etc.)
        if ($fwLower -match '\.net(\d+)\.(\d+)' -or $fwLower -match 'net(\d+)\.(\d+)') {
            $fwVersion = [int]$Matches[1] * 100 + [int]$Matches[2]
            if ($targetVersion -ge $fwVersion) {
                return $true
            }
        }

        # 检查简化格式 (net6, net7, etc.)
        if ($fwLower -match 'net(\d+)$') {
            $fwVersion = [int]$Matches[1] * 100
            if ($targetVersion -ge $fwVersion) {
                return $true
            }
        }
    }

    return $false
}

# 获取包的所有可用版本
function Get-PackageVersions {
    param([string]$PackageId)

    try {
        $lowerPackageId = $PackageId.ToLower()
        $url = "https://api.nuget.org/v3-flatcontainer/$lowerPackageId/index.json"

        # 使用 Invoke-RestMethod 直接获取 JSON
        $data = Invoke-RestMethod -Uri $url -TimeoutSec 10

        # 过滤掉预览版，返回稳定版本（降序排列）
        # 排除包含连字符的版本（预览版、alpha、beta、rc、dev、final 等）
        $stableVersions = $data.versions | Where-Object {
            $_ -notmatch '-' -and $_ -notmatch '^10\.'
        } | Sort-Object { [Version]($_ -replace '-.*$', '') } -Descending

        return $stableVersions
    }
    catch {
        return @()
    }
}

# 为指定框架找到最佳可用版本
function Get-BestVersionForFramework {
    param(
        [string]$PackageId,
        [string]$TargetFramework,
        [string]$CurrentVersion
    )

    $versions = Get-PackageVersions -PackageId $PackageId
    if ($versions.Count -eq 0) {
        return $null
    }

    foreach ($version in $versions) {
        # 跳过当前版本及更低版本
        try {
            $vCurrent = [Version]($CurrentVersion -replace '-.*$', '')
            $vCheck = [Version]($version -replace '-.*$', '')
            if ($vCheck -le $vCurrent) {
                return $null  # 没有更新的兼容版本
            }
        }
        catch {
            # 版本解析失败，继续检查
        }

        if (Test-FrameworkCompatibility -PackageId $PackageId -Version $version -TargetFramework $TargetFramework) {
            return $version
        }
    }

    return $null
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg 依赖包升级工具' 'Cyan'
Write-ColorText '  (智能框架兼容性检查)' 'DarkCyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

# 检查过期包
Write-ColorText '正在检查过期的依赖包...' 'Cyan'
Write-Host ''

$outdatedOutput = & dotnet list "$RootDir\Apq.Cfg.sln" package --outdated --format json 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-ColorText '错误: 无法检查过期包' 'Red'
    exit 1
}

$outdatedData = $outdatedOutput | ConvertFrom-Json

# 收集需要升级的包（按项目+框架分组）
$packagesToUpdate = @{}
$frameworkCheckNeeded = @{}

# 排除的包列表（这些包不应自动升级）
$excludedPackages = @(
    'Microsoft.CodeAnalysis.CSharp',      # Roslyn - 需要保持低版本以兼容编译器
    'Microsoft.CodeAnalysis.Analyzers'    # Roslyn Analyzers
)

foreach ($project in $outdatedData.projects) {
    $projectName = Split-Path $project.path -Leaf
    $projectPath = $project.path

    foreach ($framework in $project.frameworks) {
        $tfm = $framework.framework

        foreach ($package in $framework.topLevelPackages) {
            $packageName = $package.id
            $currentVersion = $package.resolvedVersion
            $latestVersion = $package.latestVersion

            if ($currentVersion -ne $latestVersion) {
                # 跳过排除的包
                if ($excludedPackages -contains $packageName) {
                    continue
                }

                # 检查最新版本是否为预览版
                if ($latestVersion -match '-(preview|alpha|beta|rc|dev)') {
                    continue
                }

                # 检查是否为 .NET 10 预览版
                if ($latestVersion -match '^10\.') {
                    continue
                }

                if (-not $packagesToUpdate.ContainsKey($packageName)) {
                    $packagesToUpdate[$packageName] = @{
                        Name = $packageName
                        LatestVersion = $latestVersion
                        ProjectFrameworks = @()
                    }
                }

                $packagesToUpdate[$packageName].ProjectFrameworks += @{
                    Project = $projectName
                    ProjectPath = $projectPath
                    Framework = $tfm
                    CurrentVersion = $currentVersion
                    BestVersion = $null
                }

                # 标记需要检查框架兼容性
                $frameworkCheckNeeded["$packageName|$tfm|$currentVersion"] = @{
                    PackageName = $packageName
                    Framework = $tfm
                    CurrentVersion = $currentVersion
                    LatestVersion = $latestVersion
                }
            }
        }
    }
}

if ($packagesToUpdate.Count -eq 0) {
    Write-ColorText '所有依赖包都是最新的!' 'Green'
    Write-Host ''
    exit 0
}

# 查询框架兼容性并确定最佳版本
Write-ColorText '正在查询 NuGet 框架兼容性...' 'Cyan'
$totalChecks = $frameworkCheckNeeded.Count
$currentCheck = 0

foreach ($key in $frameworkCheckNeeded.Keys) {
    $info = $frameworkCheckNeeded[$key]
    $currentCheck++
    Write-Host "`r  检查 $currentCheck/$totalChecks : $($info.PackageName) ($($info.Framework))    " -NoNewline

    $bestVersion = Get-BestVersionForFramework -PackageId $info.PackageName -TargetFramework $info.Framework -CurrentVersion $info.CurrentVersion

    # 更新 packagesToUpdate 中对应的 BestVersion
    foreach ($pf in $packagesToUpdate[$info.PackageName].ProjectFrameworks) {
        if ($pf.Framework -eq $info.Framework -and $pf.CurrentVersion -eq $info.CurrentVersion) {
            $pf.BestVersion = $bestVersion
        }
    }
}
Write-Host ''
Write-Host ''

# 整理升级计划
$upgradeActions = @()

foreach ($pkg in $packagesToUpdate.Values) {
    # 按项目路径分组 - 使用 ScriptBlock 访问哈希表属性
    $projectGroups = $pkg.ProjectFrameworks | Group-Object -Property { $_.ProjectPath }

    foreach ($group in $projectGroups) {
        $projectPath = $group.Name
        if (-not $projectPath) { continue }
        $projectName = Split-Path $projectPath -Leaf
        $frameworks = $group.Group

        # 检查该项目下所有框架的最佳版本
        $versionsToApply = @{}
        foreach ($fw in $frameworks) {
            if ($fw.BestVersion) {
                if (-not $versionsToApply.ContainsKey($fw.BestVersion)) {
                    $versionsToApply[$fw.BestVersion] = @()
                }
                $versionsToApply[$fw.BestVersion] += $fw.Framework
            }
        }

        # 如果所有框架都能用同一个最新版本，直接升级
        # 否则需要按框架条件引用
        if ($versionsToApply.Count -eq 1) {
            $version = ($versionsToApply.Keys | Select-Object -First 1)
            $upgradeActions += @{
                Package = $pkg.Name
                Project = $projectName
                ProjectPath = $projectPath
                Type = 'Simple'
                Version = $version
                CurrentVersions = ($frameworks | ForEach-Object { $_.CurrentVersion } | Select-Object -Unique) -join ', '
                Frameworks = $versionsToApply[$version] -join ', '
            }
        }
        elseif ($versionsToApply.Count -gt 1) {
            # 需要按框架分别升级
            foreach ($version in $versionsToApply.Keys) {
                $fws = $versionsToApply[$version]
                $currentVer = ($frameworks | Where-Object { $fws -contains $_.Framework } | Select-Object -First 1).CurrentVersion
                $upgradeActions += @{
                    Package = $pkg.Name
                    Project = $projectName
                    ProjectPath = $projectPath
                    Type = 'PerFramework'
                    Version = $version
                    CurrentVersions = $currentVer
                    Frameworks = $fws -join ', '
                }
            }
        }
    }
}

if ($upgradeActions.Count -eq 0) {
    Write-ColorText '没有可升级的包（所有包的最新版本都不兼容当前框架）' 'Yellow'
    Write-Host ''
    exit 0
}

# 显示升级计划
Write-ColorText '升级计划:' 'Yellow'
Write-Host ''

$simpleUpgrades = $upgradeActions | Where-Object { $_.Type -eq 'Simple' }
$perFrameworkUpgrades = $upgradeActions | Where-Object { $_.Type -eq 'PerFramework' }

if ($simpleUpgrades.Count -gt 0) {
    Write-ColorText '  [统一升级] 所有框架使用相同版本:' 'Green'
    $index = 1
    foreach ($action in $simpleUpgrades) {
        Write-ColorText "    $index. $($action.Package)" 'White'
        Write-ColorText "       $($action.CurrentVersions) -> $($action.Version)" 'Gray'
        Write-ColorText "       项目: $($action.Project)" 'DarkGray'
        $index++
    }
    Write-Host ''
}

if ($perFrameworkUpgrades.Count -gt 0) {
    Write-ColorText '  [按框架升级] 不同框架使用不同版本:' 'Cyan'
    $grouped = $perFrameworkUpgrades | Group-Object -Property Package
    foreach ($group in $grouped) {
        Write-ColorText "    $($group.Name):" 'White'
        foreach ($action in $group.Group) {
            Write-ColorText "       [$($action.Frameworks)] $($action.CurrentVersions) -> $($action.Version)" 'Gray'
            Write-ColorText "       项目: $($action.Project)" 'DarkGray'
        }
    }
    Write-Host ''
}

if (-not (Read-Confirm '是否执行升级? (Y/n，默认为 Y): ' $true)) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

Write-Host ''
Write-ColorText '开始升级...' 'Cyan'
Write-Host ''

$successCount = 0
$failCount = 0
$skipCount = 0

# 执行简单升级（所有框架用同一版本）
foreach ($action in $simpleUpgrades) {
    Write-ColorText "升级 $($action.Package) -> $($action.Version)" 'White'
    Write-ColorText "  $($action.Project) ..." 'Gray'

    $csprojFile = $action.ProjectPath

    if ($csprojFile -and (Test-Path $csprojFile)) {
        try {
            $output = & dotnet add $csprojFile package $action.Package --version $action.Version 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-ColorText "  $($action.Project) 成功" 'Green'
                $successCount++
            } else {
                Write-ColorText "  $($action.Project) 失败" 'Red'
                Write-ColorText "    $output" 'DarkGray'
                $failCount++
            }
        } catch {
            Write-ColorText "  $($action.Project) 失败: $($_.Exception.Message)" 'Red'
            $failCount++
        }
    } else {
        Write-ColorText "  $($action.Project) 未找到项目文件: $csprojFile" 'Red'
        $failCount++
    }
}

# 执行按框架升级（需要修改 csproj 中的条件引用）
if ($perFrameworkUpgrades.Count -gt 0) {
    Write-Host ''
    Write-ColorText '按框架升级:' 'Cyan'

    $grouped = $perFrameworkUpgrades | Group-Object -Property { "$($_.Package)|$($_.ProjectPath)" }
    foreach ($group in $grouped) {
        $actions = $group.Group
        $firstAction = $actions | Select-Object -First 1
        $packageName = $firstAction.Package
        $projectName = $firstAction.Project

        Write-ColorText "  $packageName ($projectName):" 'White'

        $csprojFile = $firstAction.ProjectPath

        if ($csprojFile -and (Test-Path $csprojFile)) {
            # 读取项目文件
            $content = [System.IO.File]::ReadAllText($csprojFile, [System.Text.Encoding]::UTF8)

            $modified = $false
            foreach ($action in $actions) {
                $frameworks = $action.Frameworks -split ', '
                $newVersion = $action.Version

                foreach ($fw in $frameworks) {
                    # 匹配带条件的 ItemGroup 中的 PackageReference
                    # 格式: <ItemGroup Condition="'$(TargetFramework)' == 'net6.0'">
                    #         <PackageReference Include="PackageName" Version="x.x.x" />
                    $pattern = "(<ItemGroup[^>]*Condition\s*=\s*[`"'][^`"']*\`$\(TargetFramework\)[^`"']*==\s*'$fw'[^`"']*[`"'][^>]*>[\s\S]*?<PackageReference\s+Include\s*=\s*[`"']$packageName[`"'][^>]*Version\s*=\s*[`"'])([^`"']+)([`"'])"

                    if ($content -match $pattern) {
                        $content = $content -replace $pattern, "`${1}$newVersion`${3}"
                        $modified = $true
                        Write-ColorText "    [$fw] 已更新到 $newVersion" 'Green'
                    }
                }
            }

            if ($modified) {
                # 检测原文件编码（是否有 BOM）
                $bytes = [System.IO.File]::ReadAllBytes($csprojFile)
                $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)

                if ($hasBom) {
                    $encoding = New-Object System.Text.UTF8Encoding($true)
                } else {
                    $encoding = New-Object System.Text.UTF8Encoding($false)
                }

                [System.IO.File]::WriteAllText($csprojFile, $content, $encoding)
                $successCount++
            } else {
                # 如果没有找到条件引用，提示用户手动处理
                Write-ColorText "    未找到条件引用，请手动修改:" 'Yellow'
                foreach ($action in $actions) {
                    Write-ColorText "      [$($action.Frameworks)] -> $($action.Version)" 'Gray'
                }
                $skipCount++
            }
        } else {
            Write-ColorText "    未找到项目文件" 'Red'
            $failCount++
        }
    }
}

Write-Host ''
Write-ColorText "========================================" 'Cyan'
$resultColor = if ($failCount -eq 0 -and $skipCount -eq 0) { 'Green' } else { 'Yellow' }
Write-ColorText "升级完成: 成功 $successCount, 失败 $failCount, 跳过 $skipCount" $resultColor
Write-ColorText "========================================" 'Cyan'
Write-Host ''

# 验证构建
if (Read-Confirm '是否验证构建? (Y/n，默认为 Y): ' $true) {
    Write-Host ''
    Write-ColorText '正在构建解决方案...' 'Cyan'
    Write-Host ''

    & dotnet build "$RootDir\Apq.Cfg.sln" -c Release --verbosity minimal

    if ($LASTEXITCODE -eq 0) {
        Write-Host ''
        Write-ColorText '构建成功!' 'Green'
    } else {
        Write-Host ''
        Write-ColorText '构建失败，请检查错误信息' 'Red'
    }
}

Write-Host ''
