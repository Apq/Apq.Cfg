# 版本管理说明

## 目录结构

每个项目有独立的版本目录：

```
versions/
├── Apq.Cfg/
│   ├── v1.0.4.md
│   └── v1.0.5.md      # 新版本
├── Apq.Cfg.Nacos/
│   ├── v1.0.4.md
│   └── v1.1.0.md      # 独立升级
├── Apq.Cfg.Apollo/
│   └── v1.0.4.md
└── ...
```

## 版本文件格式

版本文件命名格式：`v{major}.{minor}.{patch}[-prerelease].md`

示例：
- `v1.0.4.md` - 正式版本
- `v1.1.0-beta1.md` - 预发布版本
- `v2.0.0-rc.1.md` - 候选发布版本

## 版本文件内容

版本文件内容将作为 NuGet 包的 README，建议包含：

```markdown
# {ProjectName} v{version}

## 更新内容

- 功能1
- 功能2
- 修复问题

## 破坏性变更（如有）

- 变更说明
```

## 发布新版本

### 1. 创建版本文件

在对应项目的版本目录下创建新的版本文件：

```bash
# 例如：发布 Apq.Cfg.Nacos v1.1.0
echo "# Apq.Cfg.Nacos v1.1.0" > versions/Apq.Cfg.Nacos/v1.1.0.md
```

### 2. 打包

```powershell
# 打包所有项目
.\buildTools\pack-release.ps1

# 只打包指定项目
.\buildTools\pack-release.ps1 -Projects Apq.Cfg.Nacos

# 打包多个项目
.\buildTools\pack-release.ps1 -Projects Apq.Cfg,Apq.Cfg.Nacos,Apq.Cfg.Apollo
```

### 3. 发布

```powershell
.\buildTools\push-nuget.ps1
```

## 版本检测逻辑

1. 优先查找 `versions/{ProjectName}/v*.md`
2. 如果项目目录不存在，回退到 `versions/v*.md`（全局版本）
3. 选择版本号最高的文件作为当前版本

## 项目列表

| 项目 | 说明 |
|------|------|
| Apq.Cfg | 核心库 |
| Apq.Cfg.Ini | INI 文件支持 |
| Apq.Cfg.Xml | XML 文件支持 |
| Apq.Cfg.Yaml | YAML 文件支持 |
| Apq.Cfg.Toml | TOML 文件支持 |
| Apq.Cfg.Env | .env 文件支持 |
| Apq.Cfg.Redis | Redis 配置源 |
| Apq.Cfg.Database | 数据库配置源 |
| Apq.Cfg.Consul | Consul 配置中心 |
| Apq.Cfg.Etcd | Etcd 配置中心 |
| Apq.Cfg.Nacos | Nacos 配置中心 |
| Apq.Cfg.Apollo | Apollo 配置中心 |
| Apq.Cfg.Zookeeper | Zookeeper 配置中心 |
| Apq.Cfg.Vault | HashiCorp Vault |
| Apq.Cfg.SourceGenerator | 源生成器 |
