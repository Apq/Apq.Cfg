# gitee-pack-push.sh
# Gitee CI/CD 流水线打包并发布到 NuGet 的脚本
# 用法: sh buildTools/gitee-pack-push.sh
# 环境变量: NUGET_API_KEY - NuGet API 密钥

set -e

echo "=========================================="
echo "  Apq.Cfg CI/CD 打包发布脚本"
echo "=========================================="

# 项目列表（POSIX 兼容写法）
PROJECTS="
Apq.Cfg/Apq.Cfg.csproj
Apq.Cfg.Ini/Apq.Cfg.Ini.csproj
Apq.Cfg.Xml/Apq.Cfg.Xml.csproj
Apq.Cfg.Yaml/Apq.Cfg.Yaml.csproj
Apq.Cfg.Toml/Apq.Cfg.Toml.csproj
Apq.Cfg.Env/Apq.Cfg.Env.csproj
Apq.Cfg.Redis/Apq.Cfg.Redis.csproj
Apq.Cfg.Database/Apq.Cfg.Database.csproj
Apq.Cfg.Consul/Apq.Cfg.Consul.csproj
Apq.Cfg.Etcd/Apq.Cfg.Etcd.csproj
Apq.Cfg.Apollo/Apq.Cfg.Apollo.csproj
Apq.Cfg.Nacos/Apq.Cfg.Nacos.csproj
Apq.Cfg.Zookeeper/Apq.Cfg.Zookeeper.csproj
Apq.Cfg.Vault/Apq.Cfg.Vault.csproj
Apq.Cfg.Crypto/Apq.Cfg.Crypto.csproj
Apq.Cfg.Crypto.DataProtection/Apq.Cfg.Crypto.DataProtection.csproj
Apq.Cfg.Crypto.Tool/Apq.Cfg.Crypto.Tool.csproj
Apq.Cfg.SourceGenerator/Apq.Cfg.SourceGenerator.csproj
"

OUTPUT_DIR="./nupkgs"
NUGET_SOURCE="https://api.nuget.org/v3/index.json"

# 检查 API Key
if [ -z "$NUGET_API_KEY" ]; then
    echo "错误: 未设置 NUGET_API_KEY 环境变量"
    exit 1
fi

# 创建输出目录
mkdir -p "$OUTPUT_DIR"

# 还原所有项目依赖
echo ""
echo "=========================================="
echo "  步骤 1/3: 还原项目依赖"
echo "=========================================="
for project in $PROJECTS; do
    echo "还原: $project"
    dotnet restore "$project"
done

# 打包所有项目
echo ""
echo "=========================================="
echo "  步骤 2/3: 打包项目"
echo "=========================================="
for project in $PROJECTS; do
    echo "打包: $project"
    dotnet pack "$project" -c Release -o "$OUTPUT_DIR"
done

# 发布到 NuGet（并发）
echo ""
echo "=========================================="
echo "  步骤 3/3: 并发发布到 NuGet"
echo "=========================================="
for pkg in "$OUTPUT_DIR"/*.nupkg; do
    if [ -f "$pkg" ]; then
        echo "发布: $(basename "$pkg")"
        dotnet nuget push "$pkg" -s "$NUGET_SOURCE" -k "$NUGET_API_KEY" --skip-duplicate &
    fi
done
echo "等待所有发布任务完成..."
wait

echo ""
echo "=========================================="
echo "  完成!"
echo "=========================================="
