#!/usr/bin/env pwsh
# 远程构建 Docker 镜像脚本
# 功能：连接远程服务器，拉取本仓库，构建多架构镜像并推送到 Docker Hub

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

#region 配置
$remoteUser   = "root"
$remoteHost   = "vps-apq2.zalhb.com"
$remotePort   = "22"
$repoUrl      = "https://gitee.com/apq/Apq.Cfg"
$repoDir      = "Apq.Cfg/Apq.Cfg.WebUI"
$imageName    = "amwpfiqvy/apqcfg-webui"
$tmuxSession  = "dbx"
$cacheDir     = "/root/.buildx-cache"
#endregion

#region 用户输入
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  远程构建 Docker 镜像" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$imageTagInput = Read-Host "请输入镜像标签 (多个用空格分隔，留空则仅推送 latest)"

if ($imageTagInput -eq "") {
    $imageTags = @("latest")
} else {
    $imageTags = @("latest") + ($imageTagInput -split '\s+' | Where-Object { $_ -ne "" }) | Select-Object -Unique
}

$tagParams = ($imageTags | ForEach-Object { "-t ${imageName}:$_" }) -join " "

Write-Host ""
Write-Host "镜像标签: $($imageTags -join ', ')" -ForegroundColor Yellow
Write-Host "连接到: ${remoteUser}@${remoteHost}:${remotePort}" -ForegroundColor Yellow
Write-Host ""
#endregion

#region 构建远程命令
# 远程 Shell 命令步骤说明：
# 1. 克隆仓库（如果不存在）
# 2. 进入目录并拉取最新代码
# 3. 创建缓存目录
# 4. 构建多架构镜像并推送

# 合并为单行命令（用 && 连接）
$buildCmd = @(
    "if [ ! -d ~/$repoDir ]; then cd ~ && git clone $repoUrl; fi",
    "cd ~/$repoDir",
    "git pull",
    "mkdir -p $cacheDir",
    "docker buildx build --platform linux/amd64,linux/arm64 --cache-from type=local,src=$cacheDir --cache-to type=local,dest=$cacheDir,mode=max $tagParams --push ."
) -join " && "

# tmux 命令：创建会话（如不存在）并发送构建命令
$tmuxCmd = @(
    "tmux has-session -t $tmuxSession 2>/dev/null || tmux new-session -d -s $tmuxSession",
    "tmux send-keys -t $tmuxSession '$buildCmd' Enter"
) -join "; "
#endregion

#region 执行
Write-Host "正在发送命令到 tmux 会话: $tmuxSession" -ForegroundColor Yellow

& ssh -p $remotePort "${remoteUser}@${remoteHost}" $tmuxCmd

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  命令已发送" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "推送目标:" -ForegroundColor Yellow
    foreach ($tag in $imageTags) {
        Write-Host "  - ${imageName}:${tag}" -ForegroundColor Cyan
    }
    Write-Host ""
    Write-Host "查看构建进度:" -ForegroundColor Yellow
    Write-Host "  ssh -t -p $remotePort ${remoteUser}@${remoteHost} `"tmux attach -t $tmuxSession`"" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "发送失败，请检查错误信息" -ForegroundColor Red
}
#endregion

Write-Host ""
Read-Host "按回车退出"
