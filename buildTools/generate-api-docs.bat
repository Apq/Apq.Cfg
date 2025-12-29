@echo off
chcp 65001 >nul
title Apq.Cfg API 文档生成工具

:: 获取脚本所在目录
set "SCRIPT_DIR=%~dp0"

:: 检查是否提供了参数
if "%~1"=="" (
    :: 无参数，使用默认配置
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%generate-api-docs.ps1"
) else (
    :: 有参数，传递给 PowerShell
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%generate-api-docs.ps1" -Configuration "%~1"
)
