# 构建前端
FROM node:24-alpine AS frontend
WORKDIR /build
COPY ClientApp/package*.json ./
RUN npm ci --silent
COPY ClientApp/ ./
RUN npm run build

# 构建后端
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
COPY --from=frontend /wwwroot ./wwwroot
RUN dotnet publish -c Release -o /app --no-restore

# 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=backend /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Apq.Cfg.WebUI.dll"]
