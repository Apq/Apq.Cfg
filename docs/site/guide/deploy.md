# 部署指南

本指南介绍如何将 VitePress 文档站点部署到各种平台，特别是国内有免费额度的平台。

## 构建站点

在部署之前，先构建静态站点：

```bash
cd docs/site
npm install
npm run docs:build
```

构建产物位于 `.vitepress/dist` 目录。

## 国内免费部署平台

### 1. Gitee Pages（推荐）

Gitee Pages 是国内访问速度最快的免费静态托管服务。

**优点：**
- 🚀 国内访问速度快
- 💰 完全免费
- 🔄 支持自动部署
- 🌐 支持自定义域名（需实名认证）

**部署步骤：**

1. 在 Gitee 创建仓库或使用现有仓库

2. 修改 `.vitepress/config.mts` 中的 `base`：
   ```ts
   export default defineConfig({
     base: '/Apq.Cfg/',  // 替换为你的仓库名
   })
   ```

3. 构建并推送到 Gitee：
   ```bash
   npm run docs:build
   
   # 将 dist 目录内容推送到 gh-pages 分支
   cd .vitepress/dist
   git init
   git add -A
   git commit -m 'deploy'
   git push -f git@gitee.com:你的用户名/Apq.Cfg.git main:gh-pages
   ```

4. 在 Gitee 仓库设置中启用 Gitee Pages：
   - 进入仓库 → 服务 → Gitee Pages
   - 选择 `gh-pages` 分支
   - 点击启动

5. 访问 `https://你的用户名.gitee.io/Apq.Cfg/`

**自动部署脚本：**

创建 `deploy-gitee.sh`：

```bash
#!/bin/bash

# 构建
npm run docs:build

# 进入构建目录
cd .vitepress/dist

# 初始化 git 并提交
git init
git add -A
git commit -m 'deploy'

# 推送到 Gitee gh-pages 分支
git push -f git@gitee.com:AlanPoon/Apq.Cfg.git main:gh-pages

cd -
```

::: warning 注意
Gitee Pages 更新后需要手动点击"更新"按钮，或使用 Gitee Pages Action 实现自动更新。
:::

---

### 2. Vercel（推荐）

Vercel 提供免费的静态站点托管，支持自动部署。

**优点：**
- 🌍 全球 CDN，国内访问尚可
- 🔄 Git 推送自动部署
- 🆓 免费额度充足（100GB/月带宽）
- 🔧 零配置部署

**部署步骤：**

1. 访问 [vercel.com](https://vercel.com) 并使用 GitHub/GitLab 登录

2. 点击 "New Project" 导入你的仓库

3. 配置构建设置：
   - Framework Preset: `VitePress`
   - Root Directory: `docs/site`
   - Build Command: `npm run docs:build`
   - Output Directory: `.vitepress/dist`

4. 点击 "Deploy"

**vercel.json 配置：**

在 `docs/site/` 目录创建 `vercel.json`：

```json
{
  "buildCommand": "npm run docs:build",
  "outputDirectory": ".vitepress/dist",
  "framework": "vitepress",
  "headers": [
    {
      "source": "/assets/(.*)",
      "headers": [
        {
          "key": "Cache-Control",
          "value": "max-age=31536000, immutable"
        }
      ]
    }
  ]
}
```

---

### 3. Netlify

Netlify 也提供免费的静态站点托管。

**优点：**
- 🌍 全球 CDN
- 🔄 自动部署
- 🆓 免费额度（100GB/月带宽）
- 📝 支持表单处理

**部署步骤：**

1. 访问 [netlify.com](https://netlify.com) 并登录

2. 点击 "New site from Git"

3. 选择你的仓库

4. 配置构建设置：
   - Base directory: `docs/site`
   - Build command: `npm run docs:build`
   - Publish directory: `docs/site/.vitepress/dist`

5. 点击 "Deploy site"

**netlify.toml 配置：**

在项目根目录创建 `netlify.toml`：

```toml
[build]
  base = "docs/site"
  command = "npm run docs:build"
  publish = ".vitepress/dist"

[[headers]]
  for = "/assets/*"
  [headers.values]
    Cache-Control = "max-age=31536000, immutable"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
```

---

### 4. Cloudflare Pages

Cloudflare Pages 提供免费的静态站点托管，国内访问速度较好。

**优点：**
- 🚀 国内访问速度较好（有国内节点）
- 🆓 完全免费（无带宽限制）
- 🔄 自动部署
- 🛡️ 内置 DDoS 防护

**部署步骤：**

1. 访问 [pages.cloudflare.com](https://pages.cloudflare.com) 并登录

2. 点击 "Create a project" → "Connect to Git"

3. 选择你的仓库

4. 配置构建设置：
   - Framework preset: `VitePress`
   - Root directory: `docs/site`
   - Build command: `npm run docs:build`
   - Build output directory: `.vitepress/dist`

5. 点击 "Save and Deploy"

---

### 5. 腾讯云 Webify

腾讯云 Webify 是国内的静态站点托管服务。

**优点：**
- 🚀 国内访问速度快
- 🆓 有免费额度
- 🔄 支持自动部署
- 📱 支持微信小程序

**部署步骤：**

1. 访问 [腾讯云 Webify](https://webify.cloudbase.net/)

2. 创建应用，选择 "静态网站托管"

3. 关联 Git 仓库

4. 配置构建：
   - 构建目录: `docs/site`
   - 构建命令: `npm run docs:build`
   - 输出目录: `.vitepress/dist`

---

### 6. 阿里云 OSS + CDN

使用阿里云 OSS 存储静态文件，配合 CDN 加速。

**优点：**
- 🚀 国内访问速度极快
- 💰 按量付费，小站点成本很低
- 🌐 支持自定义域名
- 📊 详细的访问统计

**部署步骤：**

1. 创建 OSS Bucket，开启静态网站托管

2. 配置 CDN 加速（可选）

3. 使用 ossutil 上传：
   ```bash
   npm run docs:build
   ossutil cp -r .vitepress/dist/ oss://your-bucket/ --update
   ```

**自动部署脚本：**

```bash
#!/bin/bash

# 构建
npm run docs:build

# 上传到 OSS
ossutil cp -r .vitepress/dist/ oss://apq-cfg-docs/ --update

echo "部署完成！"
```

---

## GitHub Actions 自动部署

创建 `.github/workflows/deploy.yml`：

```yaml
name: Deploy VitePress site

on:
  push:
    branches: [main]
    paths:
      - 'docs/site/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Setup Node
        uses: actions/setup-node@v4
        with:
          node-version: 20
          cache: npm
          cache-dependency-path: docs/site/package-lock.json

      - name: Install dependencies
        run: npm ci
        working-directory: docs/site

      - name: Build
        run: npm run docs:build
        working-directory: docs/site

      # 部署到 GitHub Pages
      - name: Deploy to GitHub Pages
        uses: peaceiris/actions-gh-pages@v3
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: docs/site/.vitepress/dist

      # 或部署到 Vercel
      # - name: Deploy to Vercel
      #   uses: amondnet/vercel-action@v25
      #   with:
      #     vercel-token: ${{ secrets.VERCEL_TOKEN }}
      #     vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
      #     vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
```

---

## 平台对比

| 平台 | 国内速度 | 免费额度 | 自动部署 | 自定义域名 | 推荐指数 |
|-----|---------|---------|---------|-----------|---------|
| Gitee Pages | ⭐⭐⭐⭐⭐ | 完全免费 | 需手动 | ✅ | ⭐⭐⭐⭐⭐ |
| Cloudflare Pages | ⭐⭐⭐⭐ | 完全免费 | ✅ | ✅ | ⭐⭐⭐⭐⭐ |
| Vercel | ⭐⭐⭐ | 100GB/月 | ✅ | ✅ | ⭐⭐⭐⭐ |
| Netlify | ⭐⭐⭐ | 100GB/月 | ✅ | ✅ | ⭐⭐⭐⭐ |
| 腾讯云 Webify | ⭐⭐⭐⭐⭐ | 有限免费 | ✅ | ✅ | ⭐⭐⭐⭐ |
| 阿里云 OSS | ⭐⭐⭐⭐⭐ | 按量付费 | 需配置 | ✅ | ⭐⭐⭐ |

## 推荐方案

### 个人项目
1. **首选**：Gitee Pages（国内速度最快，完全免费）
2. **备选**：Cloudflare Pages（全球可访问，完全免费）

### 开源项目
1. **首选**：Cloudflare Pages + Gitee Pages 双部署
2. **备选**：Vercel（自动部署体验好）

### 企业项目
1. **首选**：阿里云 OSS + CDN（稳定可靠）
2. **备选**：腾讯云 Webify（一站式服务）

---

## 常见问题

### Q: Gitee Pages 为什么需要手动更新？

A: Gitee Pages 免费版不支持自动更新，需要在仓库设置中手动点击"更新"按钮。可以使用 Gitee Pages Action 实现自动化。

### Q: Vercel/Netlify 国内访问慢怎么办？

A: 可以配置自定义域名并使用国内 CDN 加速，或者同时部署到 Gitee Pages 作为国内镜像。

### Q: 如何实现多平台同步部署？

A: 使用 GitHub Actions，在一个 workflow 中配置多个部署步骤，推送到不同平台。
