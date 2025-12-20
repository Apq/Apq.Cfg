@echo off
chcp 65001 >nul
title Apq.Cfg 版本号自动增长工具

:: 获取脚本所在目录
set "SCRIPT_DIR=%~dp0"

:: 检查是否提供了参数
if "%~1"=="" (
    :: 无参数，交互式运行
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%bump-version.ps1"
) else (
    :: 有参数，传递给 PowerShell
    if "%~2"=="" (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%bump-version.ps1" -Part "%~1"
    ) else (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%bump-version.ps1" -Part "%~1" -Suffix "%~2"
    )
)

