@echo off
chcp 65001 >nul
title Apq.Cfg DocFX 文档生成

:: 获取脚本所在目录
set "SCRIPT_DIR=%~dp0"

:: 检查参数
if /i "%~1"=="-Serve" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%build-docfx.ps1" -Serve
) else if /i "%~1"=="-MetadataOnly" (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%build-docfx.ps1" -MetadataOnly
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%build-docfx.ps1"
)
