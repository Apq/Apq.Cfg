import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  // 使用相对路径，支持任意虚拟目录部署
  base: './',
  plugins: [vue()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  },
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true
  },
  server: {
    port: 5173,
    proxy: {
      // 匹配任意虚拟目录下的 /api 请求
      // 例如: /api/*, /xxx/api/*, /a/b/api/*
      '^(/[^/]+)*/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^(\/[^/]+)*\/api/, '/api')
      }
    }
  }
})
