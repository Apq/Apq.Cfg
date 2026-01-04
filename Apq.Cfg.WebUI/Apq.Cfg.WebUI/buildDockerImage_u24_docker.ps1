# Apq.Cfg.WebUI 远程构建 Docker 镜像脚本

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# ============================================================
# 配置区
# ============================================================

# 远程服务器配置
$remoteUser = "root"
$remoteHost = "192.168.1.99"
$remotePort = "22"

# Git 仓库配置
$repoUrl = "https://gitee.com/apq/Apq.Cfg"
$repoDir = "Apq.Cfg"
$solutionDir = "Apq.Cfg.WebUI"  # WebUI 解决方案目录

# Docker 镜像配置
$imageName = "registry.cn-chengdu.aliyuncs.com/apq/apqcfg-webui"

# Docker 镜像加速源（随机选择一个）
$dockerMirrors = @(
    "docker.m.daocloud.io"
    "hub.rat.dev"
)

# tmux 会话名
$tmuxSession = "dbx"

# ============================================================
# 主程序
# ============================================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Apq.Cfg.WebUI 远程构建 Docker 镜像" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Docker 镜像标签：提示用户输入（默认包含 latest，-l 排除 latest）
Write-Host "请输入 Docker 镜像标签 (多个用空格分隔，-l=排除latest，留空则仅推送 latest): " -ForegroundColor Magenta -NoNewline
$imageTagInput = Read-Host

$inputTags = @()
$excludeLatest = $false

if ($imageTagInput -ne "") {
    $inputTags = $imageTagInput -split '\s+' | Where-Object { $_ -ne "" }
    if ($inputTags -contains "-l") {
        $excludeLatest = $true
        $inputTags = $inputTags | Where-Object { $_ -ne "-l" }
    }
}

# 构建最终标签列表
if ($excludeLatest) {
    $imageTags = $inputTags
} else {
    $imageTags = @("latest") + $inputTags | Select-Object -Unique
}

# 构建 -t 参数列表
$tagParams = ($imageTags | ForEach-Object { "-t ${imageName}:$_" }) -join " "

Write-Host "镜像标签: $($imageTags -join ', ')" -ForegroundColor Yellow
Write-Host ""

# 随机选择 Docker 镜像加速源
$dockerMirror = $dockerMirrors | Get-Random
Write-Host "Docker 镜像源: $dockerMirror" -ForegroundColor Yellow
Write-Host ""

# 支持命令行参数覆盖默认配置
if ($args.Count -ge 1) { $remoteUser = $args[0] }
if ($args.Count -ge 2) { $remoteHost = $args[1] }
if ($args.Count -ge 3) { $remotePort = $args[2] }

Write-Host "连接到: $remoteUser@$remoteHost`:$remotePort" -ForegroundColor Yellow
Write-Host ""

# ============================================================
# 构建远程执行的 Shell 脚本
# ============================================================

# 生成远程脚本内容（写入临时文件执行，避免引号嵌套问题）
$scriptContent = @"
#!/bin/bash
cd ~

# 克隆或更新仓库
if [ ! -d "$repoDir" ]; then
    echo "目录不存在，正在克隆仓库..."
    git clone $repoUrl
else
    echo "目录已存在，跳过克隆"
fi

cd $repoDir

# 拉取最新代码
echo "正在拉取最新代码..."
git pull

# 进入仓库根目录（构建上下文）
# cd $repoDir 已经在仓库根目录了

# 构建并推送 Docker 镜像（从仓库根目录构建）
echo "正在构建并推送 Docker 镜像..."
echo "使用 Docker 镜像源: $dockerMirror"
docker buildx build -f Apq.Cfg.WebUI/Apq.Cfg.WebUI/Dockerfile.cn \
    --build-arg DOCKER_MIRROR=$dockerMirror \
    --platform linux/amd64,linux/arm64 \
    $tagParams \
    --cache-from type=local,src=`$HOME/.buildx-cache \
    --cache-to type=local,dest=`$HOME/.buildx-cache,mode=max \
    --push .
"@

# 将脚本内容转为 base64，避免特殊字符问题
# 注意：需要将 CRLF 转换为 LF，否则 Linux 无法执行
$scriptContentLF = $scriptContent -replace "`r`n", "`n"
$scriptBase64 = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($scriptContentLF))

# tmux 命令：解码并执行脚本
$remoteCmd = "tmux has-session -t $tmuxSession 2>/dev/null || tmux new-session -d -s $tmuxSession; " +
             "tmux send-keys -t $tmuxSession 'echo $scriptBase64 | base64 -d > /tmp/build_webui.sh && bash /tmp/build_webui.sh' Enter"

# ============================================================
# 执行远程命令
# ============================================================

Write-Host "正在发送命令到 tmux 会话: $tmuxSession" -ForegroundColor Yellow
Write-Host ""

& ssh -p $remotePort "$remoteUser@$remoteHost" $remoteCmd

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  命令已发送到 tmux 会话: $tmuxSession" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "推送目标:" -ForegroundColor Yellow
    foreach ($tag in $imageTags) {
        Write-Host "  - ${imageName}:${tag}" -ForegroundColor Cyan
    }
    Write-Host ""
    Write-Host "查看构建进度:" -ForegroundColor Yellow
    Write-Host "  ssh -t -p $remotePort $remoteUser@$remoteHost `"tmux attach -t $tmuxSession`"" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "部署失败，请检查错误信息" -ForegroundColor Red
}

Write-Host ""
Read-Host "按回车退出"
