# update-packages.ps1
# 检查并升级 NuGet 依赖包（排除按框架版本匹配的包）

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

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

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg 依赖包升级工具' 'Cyan'
Write-ColorText "========================================" 'Cyan'
Write-ColorText '  按 Q 随时退出' 'DarkGray'
Write-ColorText "========================================`n" 'Cyan'

# 需要排除的包（按框架版本匹配，不应升级到最新）
$excludePatterns = @(
    'Microsoft.Extensions.*'
)

# 检查过期包
Write-ColorText '正在检查过期的依赖包...' 'Cyan'
Write-Host ''

$outdatedOutput = & dotnet list "$ScriptDir\Apq.Cfg.sln" package --outdated --format json 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-ColorText '错误: 无法检查过期包' 'Red'
    exit 1
}

$outdatedData = $outdatedOutput | ConvertFrom-Json

# 收集需要升级的包（去重，排除按框架版本匹配的包）
$packagesToUpdate = @{}

foreach ($project in $outdatedData.projects) {
    foreach ($framework in $project.frameworks) {
        foreach ($package in $framework.topLevelPackages) {
            $packageName = $package.id
            $currentVersion = $package.resolvedVersion
            $latestVersion = $package.latestVersion

            # 检查是否在排除列表中
            $excluded = $false
            foreach ($pattern in $excludePatterns) {
                if ($packageName -like $pattern) {
                    $excluded = $true
                    break
                }
            }

            if (-not $excluded -and $currentVersion -ne $latestVersion) {
                # 检查最新版本是否为预览版（包含 preview、alpha、beta、rc 等）
                if ($latestVersion -match '-(preview|alpha|beta|rc)') {
                    continue
                }

                # 检查是否为 .NET 10 预览版（版本号 10.x）
                if ($latestVersion -match '^10\.') {
                    continue
                }

                $key = $packageName
                if (-not $packagesToUpdate.ContainsKey($key)) {
                    $packagesToUpdate[$key] = @{
                        Name = $packageName
                        CurrentVersion = $currentVersion
                        LatestVersion = $latestVersion
                        Projects = @()
                    }
                }

                $projectName = Split-Path $project.path -Leaf
                if ($packagesToUpdate[$key].Projects -notcontains $projectName) {
                    $packagesToUpdate[$key].Projects += $projectName
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

# 显示需要升级的包
Write-ColorText '发现以下包可以升级:' 'Yellow'
Write-Host ''

$index = 1
foreach ($pkg in $packagesToUpdate.Values) {
    Write-ColorText "  $index. $($pkg.Name)" 'White'
    Write-ColorText "     当前: $($pkg.CurrentVersion) -> 最新: $($pkg.LatestVersion)" 'Gray'
    Write-ColorText "     项目: $($pkg.Projects -join ', ')" 'DarkGray'
    $index++
}

Write-Host ''
Write-ColorText "排除的包 (按框架版本匹配):" 'DarkGray'
foreach ($pattern in $excludePatterns) {
    Write-ColorText "  - $pattern" 'DarkGray'
}
Write-Host ''

if (-not (Read-Confirm '是否升级这些包? (Y/n，默认为 Y): ' $true)) {
    Write-ColorText '已取消' 'Yellow'
    exit 0
}

Write-Host ''
Write-ColorText '开始升级...' 'Cyan'
Write-Host ''

$successCount = 0
$failCount = 0

foreach ($pkg in $packagesToUpdate.Values) {
    Write-ColorText "升级 $($pkg.Name) -> $($pkg.LatestVersion)" 'White'

    foreach ($projectName in $pkg.Projects) {
        # 查找项目文件
        $projectPath = Get-ChildItem -Path $ScriptDir -Filter $projectName -Recurse -Directory | Select-Object -First 1
        if ($projectPath) {
            $csprojFile = Join-Path $projectPath.FullName ($projectName -replace '\.csproj$', '.csproj')
            if (-not (Test-Path $csprojFile)) {
                $csprojFile = Get-ChildItem -Path $projectPath.FullName -Filter "*.csproj" | Select-Object -First 1 -ExpandProperty FullName
            }

            if ($csprojFile -and (Test-Path $csprojFile)) {
                try {
                    & dotnet add $csprojFile package $pkg.Name --version $pkg.LatestVersion 2>&1 | Out-Null
                    if ($LASTEXITCODE -eq 0) {
                        Write-ColorText "  $projectName 成功" 'Green'
                    } else {
                        Write-ColorText "  $projectName 失败" 'Red'
                        $failCount++
                        continue
                    }
                } catch {
                    Write-ColorText "  $projectName 失败: $($_.Exception.Message)" 'Red'
                    $failCount++
                    continue
                }
            }
        }
    }
    $successCount++
}

Write-Host ''
Write-ColorText "========================================" 'Cyan'
Write-ColorText "升级完成: 成功 $successCount 个包" $(if ($failCount -eq 0) { 'Green' } else { 'Yellow' })
Write-ColorText "========================================" 'Cyan'
Write-Host ''

# 验证构建
if (Read-Confirm '是否验证构建? (Y/n，默认为 Y): ' $true) {
    Write-Host ''
    Write-ColorText '正在构建解决方案...' 'Cyan'
    Write-Host ''

    & dotnet build "$ScriptDir\Apq.Cfg.sln" -c Release --verbosity minimal

    if ($LASTEXITCODE -eq 0) {
        Write-Host ''
        Write-ColorText '构建成功!' 'Green'
    } else {
        Write-Host ''
        Write-ColorText '构建失败，请检查错误信息' 'Red'
    }
}

Write-Host ''
