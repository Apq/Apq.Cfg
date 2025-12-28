# Apq.Cfg 文档站点部署指南

本文档介绍如何将 Apq.Cfg 文档站点部署到腾讯云 CloudBase 静态网站托管。

## 技术栈

- **框架**: VitePress (Vue 3 + Vite)
- **运行时**: Node.js 20+
- **托管**: 腾讯云 CloudBase 静态网站托管

## 本地开发

### 环境要求

- Node.js 20.x 或更高版本
- npm 10.x 或更高版本

### 安装依赖

```bash
cd docs/site
npm install
```

### 启动开发服务器

```bash
npm run dev
```

访问 http://localhost:5173 查看文档站点。

### 构建生产版本

```bash
npm run build
```

构建产物位于 `docs/site/.vitepress/dist` 目录。

### 本地预览

```bash
npm run preview
```

## 腾讯云 CloudBase 部署

### 前置条件

1. 腾讯云账号
2. 已开通 CloudBase 服务
3. 已创建 CloudBase 环境

### 方式一：手动部署（推荐）

#### 1. 安装 CloudBase CLI

```bash
npm install -g @cloudbase/cli
```

#### 2. 登录

```bash
tcb login
```

#### 3. 初始化项目

```bash
cd docs/site
tcb init
```

选择已有环境或创建新环境。

#### 4. 部署

```bash
# 构建
npm run build

# 部署到 CloudBase
tcb hosting deploy .vitepress/dist -e your-env-id
```

### 方式二：使用 cloudbaserc.json

项目已包含 `cloudbaserc.json` 配置文件：

```json
{
  "envId": "your-env-id",
  "version": "2.0",
  "framework": {
    "name": "vitepress",
    "plugins": {
      "client": {
        "use": "@cloudbase/framework-plugin-website",
        "inputs": {
          "buildCommand": "npm run build",
          "outputPath": ".vitepress/dist",
          "cloudPath": "/"
        }
      }
    }
  }
}
```

修改 `envId` 为您的环境 ID，然后执行：

```bash
cd docs/site
tcb framework deploy
```

## 自定义域名

### 配置步骤

1. 登录 [CloudBase 控制台](https://console.cloud.tencent.com/tcb)
2. 选择您的环境
3. 进入「静态网站托管」-「设置」
4. 添加自定义域名
5. 配置 DNS 解析（CNAME 记录）
6. 等待 SSL 证书自动签发

### DNS 配置示例

```
类型: CNAME
主机记录: docs
记录值: your-env-id.tcloudbaseapp.com
```

## 目录结构

```
docs/site/
├── .vitepress/
│   ├── config.ts          # VitePress 配置
│   ├── theme/
│   │   ├── index.ts       # 主题配置
│   │   └── custom.css     # 自定义样式
│   ├── cache/             # 构建缓存（gitignore）
│   └── dist/              # 构建产物（gitignore）
├── public/
│   └── logo.svg           # 站点 Logo
├── guide/                 # 指南文档
├── config-sources/        # 配置源文档
├── api/                   # API 文档
├── examples/              # 示例文档
├── index.md               # 首页
├── changelog.md           # 更新日志
├── package.json           # 项目配置
├── cloudbaserc.json       # CloudBase 配置
└── .gitignore
```

## 常见问题

### Q: 部署后页面 404？

确保 CloudBase 静态网站托管已开启，并且部署路径正确。

### Q: 自定义域名无法访问？

1. 检查 DNS 解析是否生效（可能需要等待几分钟）
2. 确认 SSL 证书已签发
3. 检查域名是否已备案（国内域名需要）

### Q: 构建失败？

1. 检查 Node.js 版本是否 >= 20
2. 删除 `node_modules` 和 `package-lock.json` 后重新安装
3. 检查是否有语法错误

## 相关链接

- [VitePress 官方文档](https://vitepress.dev/)
- [腾讯云 CloudBase 文档](https://cloud.tencent.com/document/product/876)
- [CloudBase CLI 文档](https://docs.cloudbase.net/cli-v1/intro)
