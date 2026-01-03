@echo off
chcp 65001 >nul 2>nul
title Apq.Cfg NuGet Version List/Unlist Tool

powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0set-unlist-versions.ps1' %*; exit $LASTEXITCODE"
