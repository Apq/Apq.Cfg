# Apq.Cfg.WebUI

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Docker Hub](https://img.shields.io/badge/Docker-amwpfiqvy/apqcfg--webui-blue)](https://hub.docker.com/r/amwpfiqvy/apqcfg-webui)
[![Documentation](https://img.shields.io/badge/文档-Vercel-green)](https://apq-cfg.vercel.app/)

Apq.Cfg 配置管理 Web 界面，集中管理多个应用的配置。

## 功能特性

- 多应用管理、配置树视图、实时编辑
- 敏感值脱敏、多格式导出（JSON/ENV/CSV）
- 支持 API Key / JWT Bearer 认证
- **纯静态站点**，支持任意虚拟目录部署

## 快速开始

### Docker 部署（推荐）

```bash
docker run -d -p 8080:80 amwpfiqvy/apqcfg-webui:latest
# 访问 http://localhost:8080
```

### 本地开发

```bash
npm install && npm run dev
# 访问 http://localhost:38690
```

### 构建部署

```bash
npm run build
# 输出到 dist/，可部署到 Nginx/Vercel/OSS 等
```

## 远程应用要求

远程应用需启用 CORS 并暴露配置 API：

```csharp
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.EnableCors = true;
    options.CorsOrigins = ["*"];  // 或指定 WebUI 域名
});
```

## 相关链接

- [在线文档](https://apq-cfg.vercel.app/guide/webui.html)
- [Docker Hub](https://hub.docker.com/r/amwpfiqvy/apqcfg-webui)

## 许可证

MIT License
