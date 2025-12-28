import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  // 站点元数据
  title: 'Apq.Cfg',
  description: '一个强大、灵活的 .NET 配置管理框架',
  lang: 'zh-CN',
  
  // 部署基础路径（根据部署平台调整）
  // GitHub Pages: '/Apq.Cfg/'
  // Gitee Pages: '/Apq.Cfg/'
  // 其他平台: '/'
  base: '/',

  // 最后更新时间
  lastUpdated: true,

  // 清理 URL（移除 .html 后缀）
  cleanUrls: true,

  // Markdown 配置
  markdown: {
    lineNumbers: true,
    theme: {
      light: 'github-light',
      dark: 'github-dark'
    }
  },

  // 主题配置
  themeConfig: {
    // Logo
    logo: '/logo.svg',
    
    // 导航栏
    nav: [
      { text: '首页', link: '/' },
      { text: '指南', link: '/guide/getting-started' },
      { text: 'API', link: '/api/' },
      { text: '配置源', link: '/sources/' },
      {
        text: '更多',
        items: [
          { text: '最佳实践', link: '/best-practices/' },
          { text: '性能优化', link: '/performance/' },
          { text: '常见问题', link: '/faq/' }
        ]
      },
      {
        text: '相关链接',
        items: [
          { text: 'Gitee', link: 'https://gitee.com/AlanPoon/Apq.Cfg' },
          { text: 'NuGet', link: 'https://www.nuget.org/packages?q=Apq.Cfg' }
        ]
      }
    ],

    // 侧边栏
    sidebar: {
      '/guide/': [
        {
          text: '入门',
          items: [
            { text: '简介', link: '/guide/' },
            { text: '快速开始', link: '/guide/getting-started' },
            { text: '安装', link: '/guide/installation' }
          ]
        },
        {
          text: '核心概念',
          items: [
            { text: '架构设计', link: '/guide/architecture' },
            { text: '配置源', link: '/guide/config-sources' },
            { text: '类型转换', link: '/guide/type-conversion' },
            { text: '动态重载', link: '/guide/dynamic-reload' }
          ]
        },
        {
          text: '高级功能',
          items: [
            { text: '源代码生成器', link: '/guide/source-generator' },
            { text: '依赖注入', link: '/guide/dependency-injection' },
            { text: '编码处理', link: '/guide/encoding' }
          ]
        },
        {
          text: '部署',
          items: [
            { text: '部署指南', link: '/guide/deploy' }
          ]
        }
      ],
      '/sources/': [
        {
          text: '本地配置源',
          items: [
            { text: 'JSON', link: '/sources/json' },
            { text: 'YAML', link: '/sources/yaml' },
            { text: 'TOML', link: '/sources/toml' },
            { text: 'XML', link: '/sources/xml' },
            { text: 'INI', link: '/sources/ini' },
            { text: 'ENV', link: '/sources/env' }
          ]
        },
        {
          text: '远程配置源',
          items: [
            { text: 'Redis', link: '/sources/redis' },
            { text: 'Database', link: '/sources/database' },
            { text: 'Etcd', link: '/sources/etcd' },
            { text: 'Consul', link: '/sources/consul' },
            { text: 'Nacos', link: '/sources/nacos' },
            { text: 'Apollo', link: '/sources/apollo' },
            { text: 'Vault', link: '/sources/vault' },
            { text: 'Zookeeper', link: '/sources/zookeeper' }
          ]
        }
      ],
      '/api/': [
        {
          text: 'API 参考',
          items: [
            { text: '概览', link: '/api/' },
            { text: 'CfgBuilder', link: '/api/cfg-builder' },
            { text: 'CfgRoot', link: '/api/cfg-root' },
            { text: 'CfgSection', link: '/api/cfg-section' }
          ]
        }
      ],
      '/best-practices/': [
        {
          text: '最佳实践',
          items: [
            { text: '概览', link: '/best-practices/' },
            { text: '配置源选择', link: '/best-practices/source-selection' },
            { text: '安全性', link: '/best-practices/security' },
            { text: '生产环境', link: '/best-practices/production' }
          ]
        }
      ],
      '/performance/': [
        {
          text: '性能优化',
          items: [
            { text: '概览', link: '/performance/' },
            { text: '缓存策略', link: '/performance/caching' },
            { text: '基准测试', link: '/performance/benchmarks' }
          ]
        }
      ]
    },

    // 社交链接
    socialLinks: [
      { icon: 'github', link: 'https://gitee.com/AlanPoon/Apq.Cfg' }
    ],

    // 页脚
    footer: {
      message: '基于 MIT 许可发布',
      copyright: 'Copyright © 2024-present Apq'
    },

    // 搜索
    search: {
      provider: 'local',
      options: {
        translations: {
          button: {
            buttonText: '搜索文档',
            buttonAriaLabel: '搜索文档'
          },
          modal: {
            noResultsText: '无法找到相关结果',
            resetButtonTitle: '清除查询条件',
            footer: {
              selectText: '选择',
              navigateText: '切换',
              closeText: '关闭'
            }
          }
        }
      }
    },

    // 编辑链接
    editLink: {
      pattern: 'https://gitee.com/AlanPoon/Apq.Cfg/edit/main/docs/site/:path',
      text: '在 Gitee 上编辑此页面'
    },

    // 文档页脚
    docFooter: {
      prev: '上一页',
      next: '下一页'
    },

    // 大纲
    outline: {
      label: '页面导航',
      level: [2, 3]
    },

    // 最后更新时间
    lastUpdated: {
      text: '最后更新于',
      formatOptions: {
        dateStyle: 'short',
        timeStyle: 'short'
      }
    },

    // 返回顶部
    returnToTopLabel: '回到顶部',

    // 侧边栏菜单标签
    sidebarMenuLabel: '菜单',

    // 深色模式切换标签
    darkModeSwitchLabel: '主题',
    lightModeSwitchTitle: '切换到浅色模式',
    darkModeSwitchTitle: '切换到深色模式'
  },

  // 站点地图
  sitemap: {
    hostname: 'https://your-domain.com'
  },

  // Head 标签
  head: [
    ['link', { rel: 'icon', href: '/favicon.ico' }],
    ['meta', { name: 'theme-color', content: '#3eaf7c' }],
    ['meta', { name: 'og:type', content: 'website' }],
    ['meta', { name: 'og:locale', content: 'zh_CN' }],
    ['meta', { name: 'og:site_name', content: 'Apq.Cfg' }]
  ]
})
