# 启动 Apq.Cfg.WebApiDemo 和 Apq.Cfg.WebUI 项目
# 使用方法: .\start-dev.ps1

$ErrorActionPreference = 'Stop'
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir

Write-Host '========================================' -ForegroundColor Cyan
Write-Host '  Apq.Cfg 开发环境启动脚本' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ''

# 启动 WebApiDemo (.NET 项目)
$webApiPath = Join-Path $rootDir 'Apq.Cfg\Samples\Apq.Cfg.WebApiDemo'
Write-Host '[1/2] 正在启动 WebApiDemo...' -ForegroundColor Yellow
Start-Process powershell -ArgumentList '-NoExit', '-Command', "cd '$webApiPath'; dotnet run --framework net10.0"

# 等待 API 启动
Write-Host '等待 API 启动 (3秒)...' -ForegroundColor Gray
Start-Sleep -Seconds 3

# 启动 WebUI (Vue 项目)
$webUIPath = $scriptDir
Write-Host '[2/2] 正在启动 WebUI...' -ForegroundColor Yellow
Start-Process powershell -ArgumentList '-NoExit', '-Command', "cd '$webUIPath'; npm run dev"

Write-Host ''
Write-Host '========================================' -ForegroundColor Green
Write-Host '  两个项目已在新窗口中启动' -ForegroundColor Green
Write-Host '  WebApiDemo: https://localhost:5001' -ForegroundColor Green
Write-Host '  WebUI: http://localhost:5173' -ForegroundColor Green
Write-Host '========================================' -ForegroundColor Green
