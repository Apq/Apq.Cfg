# build-docfx.ps1
# DocFX API 文档生成脚本

param(
    [switch]$Serve,      # 启动本地预览服务器
    [switch]$MetadataOnly # 仅生成元数据，不构建站点
)

$ErrorActionPreference = 'Stop'
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

function Write-ColorText {
    param([string]$Text, [string]$Color = 'White')
    Write-Host $Text -ForegroundColor $Color
}

Write-ColorText "`n========================================" 'Cyan'
Write-ColorText '  Apq.Cfg DocFX 文档生成' 'Cyan'
Write-ColorText "========================================`n" 'Cyan'

# 检查 DocFX 是否安装
Write-ColorText '检查 DocFX 工具...' 'Gray'
$docfxInstalled = dotnet tool list -g | Select-String 'docfx'
if (-not $docfxInstalled) {
    Write-ColorText '  安装 DocFX...' 'Yellow'
    dotnet tool install -g docfx
    if ($LASTEXITCODE -ne 0) {
        Write-ColorText '  DocFX 安装失败' 'Red'
        exit 1
    }
}
Write-ColorText '  DocFX 工具就绪' 'Green'
Write-Host ''

# 切换到 DocFX 目录
$DocfxDir = Join-Path $RootDir 'docs\docfx'
Push-Location $DocfxDir

try {
    # 生成元数据
    Write-ColorText '生成 API 元数据...' 'Cyan'
    docfx metadata
    if ($LASTEXITCODE -ne 0) {
        Write-ColorText '  元数据生成失败' 'Red'
        exit 1
    }
    Write-ColorText '  元数据生成完成' 'Green'
    Write-Host ''

    if ($MetadataOnly) {
        Write-ColorText '仅生成元数据模式，跳过站点构建' 'Yellow'
        exit 0
    }

    # 构建文档站点
    Write-ColorText '构建文档站点...' 'Cyan'
    docfx build
    if ($LASTEXITCODE -ne 0) {
        Write-ColorText '  站点构建失败' 'Red'
        exit 1
    }
    Write-ColorText '  站点构建完成' 'Green'
    Write-Host ''

    # 输出目录信息
    $outputDir = Join-Path $RootDir 'docs\site\public\api-reference'

    # 复制 favicon（DocFX 不支持自定义 favicon 路径）
    $faviconSrc = Join-Path $DocfxDir 'favicon.svg'
    $faviconDest = Join-Path $outputDir 'favicon.svg'
    if (Test-Path $faviconSrc) {
        Copy-Item $faviconSrc $faviconDest -Force
        # 替换所有 HTML 文件中的 favicon.ico 引用为 favicon.svg
        Get-ChildItem -Path $outputDir -Filter '*.html' -Recurse | ForEach-Object {
            $content = [IO.File]::ReadAllText($_.FullName, [Text.Encoding]::UTF8)
            $newContent = $content -replace 'href="([^"]*?)favicon\.ico"', 'href="$1favicon.svg"'
            if ($content -ne $newContent) {
                [IO.File]::WriteAllText($_.FullName, $newContent, [Text.Encoding]::UTF8)
            }
        }
        Write-ColorText '  已更新 favicon 引用' 'Green'
    }

    Write-ColorText "输出目录: $outputDir" 'Gray'
    Write-Host ''

    if ($Serve) {
        Write-ColorText '启动本地预览服务器...' 'Cyan'
        Write-ColorText '  按 Ctrl+C 停止服务器' 'DarkGray'
        docfx serve $outputDir
    }

    Write-ColorText "========================================" 'Cyan'
    Write-ColorText 'DocFX 文档生成完成!' 'Green'
    Write-ColorText "========================================`n" 'Cyan'
}
finally {
    Pop-Location
}
