# Web 管理界面

Apq.Cfg.WebUI 提供 Web 管理界面，用于集中管理多个应用的配置。

## 概述

WebUI 是一个独立的 Web 应用，通过 Docker 镜像部署，可以：
- 集中管理多个应用的配置
- 可视化配置编辑
- 配置版本历史
- 配置对比和回滚
- 多环境配置管理

## 部署方式

### Docker 部署

```bash
docker run -d \
  --name apqcfg-webui \
  -p 8080:8080 \
  -e APQCFG_ADMIN_PASSWORD=your-password \
  apq/apqcfg-webui:latest
```

### Docker Compose

```yaml
version: '3.8'
services:
  apqcfg-webui:
    image: apq/apqcfg-webui:latest
    ports:
      - "8080:8080"
    environment:
      - APQCFG_ADMIN_PASSWORD=your-password
      - APQCFG_DATABASE_TYPE=sqlite
      - APQCFG_DATABASE_PATH=/data/apqcfg.db
    volumes:
      - apqcfg-data:/data

volumes:
  apqcfg-data:
```

## 环境变量

| 变量 | 说明 | 默认值 |
|------|------|--------|
| `APQCFG_ADMIN_PASSWORD` | 管理员密码 | (必填) |
| `APQCFG_DATABASE_TYPE` | 数据库类型 (sqlite/mysql/postgres) | sqlite |
| `APQCFG_DATABASE_PATH` | SQLite 数据库路径 | /data/apqcfg.db |
| `APQCFG_DATABASE_CONNECTION` | 数据库连接字符串 | - |
| `APQCFG_JWT_SECRET` | JWT 密钥 | (自动生成) |
| `APQCFG_JWT_EXPIRY` | JWT 过期时间（小时） | 24 |

## 功能特性

### 应用管理

- 注册和管理多个应用
- 为每个应用配置独立的配置源
- 支持应用分组

### 配置编辑

- 可视化配置编辑器
- 支持 JSON/YAML/TOML 格式
- 语法高亮和验证
- 配置键搜索

### 版本控制

- 配置变更历史记录
- 版本对比
- 一键回滚

### 多环境支持

- 开发/测试/生产环境隔离
- 环境间配置复制
- 环境变量覆盖

### 安全特性

- 基于角色的访问控制
- 敏感值自动脱敏
- 操作审计日志

## 与 WebApi 集成

WebUI 通过 WebApi 与各应用通信。在应用中配置 WebApi：

```csharp
builder.Services.AddApqCfgWebApi(options =>
{
    options.Authentication = AuthenticationType.ApiKey;
    options.ApiKey = Environment.GetEnvironmentVariable("APQCFG_API_KEY");
    options.AllowRead = true;
    options.AllowWrite = true;
});
```

然后在 WebUI 中注册应用时，填入应用的 WebApi 地址和 API Key。

## 截图

> 截图待添加

## 注意事项

1. **生产环境安全**：确保使用强密码和 HTTPS
2. **数据备份**：定期备份 SQLite 数据库或使用外部数据库
3. **网络隔离**：WebUI 应部署在内网，不直接暴露到公网
