# Apq.Cfg.WebUI

[![Gitee](https://img.shields.io/badge/Gitee-Apq.Cfg-red)](https://gitee.com/apq/Apq.Cfg)
[![Documentation](https://img.shields.io/badge/文档-Vercel-blue)](https://apq-cfg.vercel.app/)

Apq.Cfg 配置管理 Web 界面，集中管理多个应用的配置。

**📖 在线文档**：https://apq-cfg.vercel.app/

## 功能特性

- 多应用管理、配置树视图、实时编辑
- 敏感值脱敏、多格式导出（JSON/ENV/KV）
- 支持 API Key / JWT Bearer 认证
- **纯静态站点**，可部署到任何静态文件托管服务

## 技术栈

- Vue 3.5 + TypeScript 5.9 + Vite 7
- Element Plus 2.13 + Pinia 3.0
- Axios 1.13

## 快速开始

### 本地开发

```bash
cd Apq.Cfg.WebUI
npm install
npm run dev

# 访问 http://localhost:38690
```

### 构建

```bash
npm run build
# 输出到 dist/ 目录
```

### 部署

构建产物是纯静态文件，可部署到：

- Nginx / Apache
- GitHub Pages / GitLab Pages
- Vercel / Netlify
- 阿里云 OSS / 腾讯云 COS
- 任何 HTTP 服务器

## 数据存储

应用端点信息（包括认证凭据）保存在浏览器 **localStorage**，不上传到任何服务器。

```typescript
// localStorage 中的数据结构
interface AppEndpoint {
  id: string           // 唯一标识
  name: string         // 应用名称
  url: string          // API 地址（如 http://localhost:5000/api/apqcfg）
  authType: AuthType   // 认证方式：None | ApiKey | JwtBearer
  apiKey?: string      // API Key
  token?: string       // JWT Token
  description?: string // 备注
}
```

## 远程应用要求

WebUI 直接从浏览器访问远程应用的配置 API，因此远程应用需要：

1. **启用 CORS**，允许 WebUI 的来源访问
2. **暴露配置 API**（`/api/apqcfg/*`）

```csharp
// 远程应用 CORS 配置示例
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://your-webui-domain")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 虚拟目录部署

WebUI 使用相对路径构建（`base: './'`），支持部署到任意虚拟目录：

```
http://example.com/                    # 根目录
http://example.com/apqcfg/             # 虚拟目录
http://example.com/admin/config/       # 多级虚拟目录
```

### Nginx 配置示例

```nginx
server {
    listen 80;
    server_name webui.example.com;
    root /var/www/apqcfg-webui;
    index index.html;

    # SPA 路由支持
    location / {
        try_files $uri $uri/ /index.html;
    }

    # 缓存静态资源
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

## 许可证

MIT License
