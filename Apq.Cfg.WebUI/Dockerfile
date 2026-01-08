# 构建阶段
FROM node:24-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci --ignore-scripts
COPY . .
RUN npm run build

# 运行阶段 - 使用 static-web-server (Rust 高性能静态服务器)
FROM joseluisq/static-web-server:2-alpine
COPY --from=build /app/dist /public
ENV SERVER_ROOT=/public \
    SERVER_PORT=80 \
    SERVER_COMPRESSION=true \
    SERVER_COMPRESSION_LEVEL=default \
    SERVER_CACHE_CONTROL_HEADERS=true \
    SERVER_FALLBACK_PAGE=/index.html
EXPOSE 80
