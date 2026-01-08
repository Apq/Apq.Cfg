# Web 管理界面

Apq.Cfg.WebUI 提供 Web 管理界面，用于集中管理多个应用的配置。

## 概述

WebUI 是一个**纯静态** Web 应用，可以部署到任何静态文件托管服务：

- 多应用配置集中管理
- 配置树形视图展示
- 实时编辑与自动保存
- 敏感值脱敏显示
- 导出为 JSON/ENV/CSV 格式
- 支持 API Key / JWT Bearer 认证

## 部署方式

### Docker 部署

```bash
docker run -d \
  --name apqcfg-webui \
  -p 8080:80 \
  amwpfiqvy/apqcfg-webui:latest
```

访问 http://localhost:8080

### Docker Compose

```yaml
version: '3.8'
services:
  apqcfg-webui:
    image: amwpfiqvy/apqcfg-webui:latest
    ports:
      - "8080:80"
    restart: unless-stopped
```

### 静态文件部署

构建产物是纯静态文件，可部署到：

- Nginx / Apache
- GitHub Pages / GitLab Pages
- Vercel / Netlify
- 阿里云 OSS / 腾讯云 COS
- 任何 HTTP 服务器

```bash
# 构建
cd Apq.Cfg.WebUI
npm install
npm run build
# 输出到 dist/ 目录
```

### 虚拟目录部署

WebUI 使用相对路径构建（`base: './'`），支持部署到任意虚拟目录：

```
http://example.com/                    # 根目录
http://example.com/apqcfg/             # 虚拟目录
http://example.com/admin/config/       # 多级虚拟目录
```

## 数据存储

应用端点信息（包括认证凭据）保存在浏览器 **localStorage**，不上传到任何服务器。

```typescript
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
// 添加 WebApi 时配置 CORS
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.EnableCors = true;  // 默认已启用
    options.CorsOrigins = ["http://your-webui-domain"];  // 默认 ["*"]
});
```

## 功能特性

### 应用管理

- 添加、编辑、删除应用端点
- 保存前测试连接
- 支持多种认证方式

### 配置编辑

- 树形视图展示配置层级
- 实时编辑配置值
- 添加新配置项
- 删除配置项
- 只读配置源标识

### 导入导出

- 导出为 JSON/ENV/CSV/KV 格式
- 拖放文件导入
- 批量操作

### 安全特性

- 敏感值自动脱敏显示
- 认证凭据本地存储
- 支持 API Key / JWT Bearer 认证

## 支持架构

Docker 镜像支持以下架构：

- `linux/amd64`
- `linux/arm64`

## 相关链接

- [Docker Hub](https://hub.docker.com/r/amwpfiqvy/apqcfg-webui)
- [源代码](https://gitee.com/apq/Apq.Cfg/tree/master/Apq.Cfg.WebUI)

## 注意事项

1. **CORS 配置**：确保远程应用正确配置 CORS，允许 WebUI 域名访问
2. **HTTPS**：生产环境建议使用 HTTPS
3. **认证安全**：API Key 和 Token 存储在浏览器本地，注意保护
