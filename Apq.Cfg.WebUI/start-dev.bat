@echo off
chcp 936 >nul
cd /d "%~dp0"
powershell -ExecutionPolicy Bypass -File "%~dp0start-dev.ps1"
::pause
