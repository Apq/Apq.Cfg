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
    // 输出到 dist 文件夹
    outDir: 'dist',
    emptyOutDir: true
  },
  server: {
    port: 38690
  }
})
