@echo off
chcp 65001 >nul
title Apq.Cfg NuGet 发布工具

:: 获取脚本所在目录
set "SCRIPT_DIR=%~dp0"

:: 检查是否提供了参数
if "%~1"=="" (
    :: 无参数，交互式运行
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%push-nuget.ps1"
) else (
    :: 有参数，传递给 PowerShell (第一个参数为 API Key)
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%push-nuget.ps1" -ApiKey "%~1"
)
