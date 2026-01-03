import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import path from 'path'

// 开发时的虚拟目录（留空或设为 '/' 表示根目录访问）
// 如需测试虚拟目录，设为如 '/xxx/'
const DEV_BASE = process.env.VITE_BASE || '/'

export default defineConfig(({ command }) => ({
  // 开发模式使用 DEV_BASE，生产构建使用相对路径支持任意虚拟目录部署
  base: command === 'serve' ? DEV_BASE : './',
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
    port: 38690,
    proxy: {
      // 匹配任意虚拟目录下的 /api/ 请求，但排除 /src/ 开头的源文件路径
      // 例如: /api/*, /xxx/api/*, /a/b/api/*
      // 不匹配: /src/api/*, /@fs/*, /@vite/*
      '^(?!/src/|/@).*(/api/)': {
        target: 'http://localhost:38678',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^(.*)(\/api\/)/, '/api/')
      }
    }
  }
}))
