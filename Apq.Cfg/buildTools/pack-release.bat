@echo off
chcp 65001 >nul
title Apq.Cfg NuGet 包生成工具

:: 获取脚本所在目录
set "SCRIPT_DIR=%~dp0"

:: 检查是否提供了参数
if "%~1"=="" (
    :: 无参数，交互式运行
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%pack-release.ps1"
) else (
    :: 有参数，传递给 PowerShell
    if /i "%~1"=="-NoBuild" (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%pack-release.ps1" -NoBuild
    ) else (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%pack-release.ps1" -OutputDir "%~1"
    )
)
