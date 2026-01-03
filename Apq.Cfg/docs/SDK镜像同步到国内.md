# Docker 镜像同步指南

本文档记录将 .NET SDK 镜像从 DaoCloud 镜像源同步到阿里云容器镜像服务的操作步骤。

## 镜像信息

| 项目 | 值 |
|------|-----|
| 源镜像 | `mcr.m.daocloud.io/dotnet/sdk:9.0` |
| 目标镜像 | `registry.cn-chengdu.aliyuncs.com/apq/dotnet_sdk:9.0_amd64` |

## 操作步骤

### 1. 拉取源镜像

```bash
docker pull mcr.m.daocloud.io/dotnet/sdk:9.0
```

### 2. 打标签

```bash
docker tag mcr.m.daocloud.io/dotnet/sdk:9.0 registry.cn-chengdu.aliyuncs.com/apq/dotnet_sdk:9.0_amd64
```

### 3. 登录阿里云镜像仓库

```bash
docker login registry.cn-chengdu.aliyuncs.com
```

按提示输入用户名和密码。

### 4. 推送镜像

```bash
docker push registry.cn-chengdu.aliyuncs.com/apq/dotnet_sdk:9.0_amd64
```

## 一键执行脚本

将以下内容保存为 `sync-dotnet-sdk.sh`：

```bash
#!/bin/bash

# 镜像同步脚本：DaoCloud -> 阿里云

SOURCE_IMAGE="mcr.m.daocloud.io/dotnet/sdk:9.0"
TARGET_IMAGE="registry.cn-chengdu.aliyuncs.com/apq/dotnet_sdk:9.0_amd64"

echo "=== 拉取源镜像 ==="
docker pull $SOURCE_IMAGE

echo "=== 打标签 ==="
docker tag $SOURCE_IMAGE $TARGET_IMAGE

echo "=== 登录阿里云镜像仓库 ==="
docker login registry.cn-chengdu.aliyuncs.com

echo "=== 推送镜像 ==="
docker push $TARGET_IMAGE

echo "=== 完成 ==="
docker images | grep dotnet
```

执行脚本：

```bash
chmod +x sync-dotnet-sdk.sh
./sync-dotnet-sdk.sh
```

## 验证

推送完成后，可以通过以下命令验证：

```bash
docker pull registry.cn-chengdu.aliyuncs.com/apq/dotnet_sdk:9.0_amd64
```

## 注意事项

1. 确保已安装 Docker 并且服务正在运行
2. 需要有阿里云容器镜像服务的访问权限
3. 首次登录阿里云镜像仓库需要在阿里云控制台设置访问凭证
