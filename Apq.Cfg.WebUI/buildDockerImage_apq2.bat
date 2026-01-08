@echo off
powershell -ExecutionPolicy Bypass -File "%~dp0buildDockerImage_apq2.ps1" %*
