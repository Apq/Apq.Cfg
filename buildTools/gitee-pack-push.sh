# gitee-pack-push.sh
# Gitee CI/CD 流水线打包并发布到 NuGet 的脚本
# 用法: sh buildTools/gitee-pack-push.sh
# 环境变量: NUGET_API_KEY - NuGet API 密钥

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECTS_FILE="$SCRIPT_DIR/projects.txt"

echo "=========================================="
echo "  Apq.Cfg CI/CD 打包发布脚本"
echo "=========================================="

# 检查 projects.txt 是否存在
if [ ! -f "$PROJECTS_FILE" ]; then
    echo "错误: 找不到 projects.txt 文件"
    echo "路径: $PROJECTS_FILE"
    exit 1
fi

# 从 projects.txt 读取项目列表（过滤注释和空行，转换为 csproj 路径）
PROJECTS=""
while IFS= read -r line || [ -n "$line" ]; do
    # 跳过注释和空行
    case "$line" in
        \#*|"") continue ;;
    esac
    # 去除首尾空格
    project=$(echo "$line" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')
    if [ -n "$project" ]; then
        PROJECTS="$PROJECTS $project/$project.csproj"
    fi
done < "$PROJECTS_FILE"

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
