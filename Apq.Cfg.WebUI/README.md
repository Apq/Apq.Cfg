# Apq.Cfg.WebUI

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apq.Cfg 配置管理 Web 界面，集中管理多个应用的配置。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 功能特性

- 多应用管理、配置树视图、实时编辑
- 敏感值脱敏、多格式导出（JSON/ENV/KV）
- 支持 API Key / JWT Bearer 认证

## 技术栈

- **后端**：ASP.NET Core 8.0/10.0
- **前端**：Vue 3 + TypeScript + Element Plus

## 快速开始

### Docker

```bash
# 本地构建
docker build -t apqcfg-webui .
docker run -p 8080:80 apqcfg-webui

# 或使用阿里云镜像
docker run -p 8080:80 registry.cn-chengdu.aliyuncs.com/apq/apqcfg-webui
```

### 本地开发

```bash
# 前端
cd ClientApp && npm install && npm run dev

# 后端
dotnet run
```

## 应用端点配置

WebUI 转发请求到各应用的配置 API：

```json
{
    "id": "app-1",
    "name": "订单服务",
    "url": "http://localhost:5000/api/apqcfg",
    "authType": "ApiKey",
    "apiKey": "your-api-key"
}
```

## API 端点

| 方法 | 路径 | 说明 |
|------|------|------|
| GET/POST/PUT/DELETE | `/api/apps` | 应用管理 |
| GET | `/api/proxy/{appId}/tree` | 获取配置树 |
| PUT | `/api/proxy/{appId}/values/{key}` | 设置配置值 |
| POST | `/api/proxy/{appId}/save` | 保存配置 |
| GET | `/api/proxy/{appId}/export` | 导出配置 |

## 许可证

MIT License
