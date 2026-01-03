# 构建前端
# 注意：构建上下文必须是仓库根目录
# docker build -f Apq.Cfg.WebUI/Dockerfile .
FROM node:24-alpine AS frontend
WORKDIR /build
COPY Apq.Cfg.WebUI/ClientApp/package*.json ./
RUN npm ci --silent
COPY Apq.Cfg.WebUI/ClientApp/ ./
RUN npm run build

# 构建后端
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src

# 复制 Directory.Build.* 文件（版本号和公共配置）
COPY Directory.Build.props Directory.Build.Version.props ./

# 复制项目文件
COPY Apq.Cfg.WebUI/ ./Apq.Cfg.WebUI/
RUN dotnet restore Apq.Cfg.WebUI/Apq.Cfg.WebUI.csproj

# 复制前端构建输出
COPY --from=frontend /wwwroot ./Apq.Cfg.WebUI/wwwroot

# 发布
RUN dotnet publish Apq.Cfg.WebUI/Apq.Cfg.WebUI.csproj -c Release -o /app --no-restore -f net10.0

# 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=backend /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Apq.Cfg.WebUI.dll"]
