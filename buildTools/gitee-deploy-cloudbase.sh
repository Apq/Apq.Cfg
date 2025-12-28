# gitee-deploy-cloudbase.sh
# Gitee CI/CD 流水线部署文档站点到腾讯云 CloudBase 的脚本
# 用法: sh buildTools/gitee-deploy-cloudbase.sh
# 环境变量:
#   - TENCENT_SECRET_ID: 腾讯云 SecretId
#   - TENCENT_SECRET_KEY: 腾讯云 SecretKey
#   - CLOUDBASE_ENV_ID: CloudBase 环境 ID

set -e

echo "=========================================="
echo "  Apq.Cfg 文档站点部署脚本"
echo "=========================================="

# 检查环境变量
if [ -z "$TENCENT_SECRET_ID" ]; then
    echo "错误: 未设置 TENCENT_SECRET_ID 环境变量"
    exit 1
fi

if [ -z "$TENCENT_SECRET_KEY" ]; then
    echo "错误: 未设置 TENCENT_SECRET_KEY 环境变量"
    exit 1
fi

if [ -z "$CLOUDBASE_ENV_ID" ]; then
    echo "错误: 未设置 CLOUDBASE_ENV_ID 环境变量"
    exit 1
fi

SITE_DIR="docs/site"
OUTPUT_DIR="$SITE_DIR/.vitepress/dist"

# 步骤 0: 配置国内镜像源
echo ""
echo "=========================================="
echo "  步骤 0/4: 配置国内镜像源"
echo "=========================================="

# 配置 Alpine apk 使用阿里云镜像
sed -i 's/dl-cdn.alpinelinux.org/mirrors.aliyun.com/g' /etc/apk/repositories

# 配置 npm 使用淘宝镜像
npm config set registry https://registry.npmmirror.com

echo "Alpine APK 镜像: mirrors.aliyun.com"
echo "NPM 镜像: registry.npmmirror.com"

# 步骤 1: 安装系统依赖（git 用于 VitePress 获取文件最后更新时间）
echo ""
echo "=========================================="
echo "  步骤 1/4: 安装系统依赖"
echo "=========================================="
apk add --no-cache git

# 步骤 2: 安装 Node.js 依赖
echo ""
echo "=========================================="
echo "  步骤 2/4: 安装 npm 依赖"
echo "=========================================="
cd "$SITE_DIR"
npm install

# 步骤 3: 构建文档站点
echo ""
echo "=========================================="
echo "  步骤 3/4: 构建文档站点"
echo "=========================================="
npm run build

# 步骤 4: 安装 CloudBase CLI 并部署
echo ""
echo "=========================================="
echo "  步骤 4/4: 部署到 CloudBase"
echo "=========================================="

# 安装 CloudBase CLI
npm install -g @cloudbase/cli

# 使用密钥登录
tcb login --apiKeyId "$TENCENT_SECRET_ID" --apiKey "$TENCENT_SECRET_KEY"

# 部署静态文件
tcb hosting deploy .vitepress/dist -e "$CLOUDBASE_ENV_ID"

echo ""
echo "=========================================="
echo "  部署完成!"
echo "=========================================="
echo "访问地址: https://$CLOUDBASE_ENV_ID.tcloudbaseapp.com"
