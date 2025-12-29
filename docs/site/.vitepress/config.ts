import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'Apq.Cfg',
  description: '强大、灵活、高性能的 .NET 配置管理库',
  lang: 'zh-CN',
  
  head: [
    ['link', { rel: 'icon', href: '/logo.svg' }],
    // SEO meta 标签
    ['meta', { name: 'keywords', content: 'Apq.Cfg, .NET, 配置管理, Configuration, C#, JSON, YAML, Consul, Nacos, Redis, Vault, 热重载, 依赖注入' }],
    ['meta', { name: 'author', content: 'Apq' }],
    // Open Graph 标签（社交媒体分享）
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:title', content: 'Apq.Cfg - 高性能 .NET 配置管理库' }],
    ['meta', { property: 'og:description', content: '支持多种配置源、动态重载、加密脱敏、依赖注入集成的 .NET 配置管理库' }],
    ['meta', { property: 'og:image', content: '/logo.svg' }],
    ['meta', { property: 'og:locale', content: 'zh_CN' }],
    // Twitter Card 标签
    ['meta', { name: 'twitter:card', content: 'summary' }],
    ['meta', { name: 'twitter:title', content: 'Apq.Cfg - 高性能 .NET 配置管理库' }],
    ['meta', { name: 'twitter:description', content: '支持多种配置源、动态重载、加密脱敏、依赖注入集成的 .NET 配置管理库' }],
    // Sitemap 链接
    ['link', { rel: 'sitemap', type: 'application/xml', href: '/sitemap.xml' }]
  ],
  
  themeConfig: {
    logo: '/logo.svg',
    
    nav: [
      { text: '指南', link: '/guide/', activeMatch: '/guide/' },
      { text: '配置源', link: '/config-sources/', activeMatch: '/config-sources/' },
      { text: 'API', link: '/api/', activeMatch: '/api/' },
      { text: '示例', link: '/examples/', activeMatch: '/examples/' },
      { text: '更新日志', link: '/changelog' }
    ],
    
    sidebar: {
      '/guide/': [
        {
          text: '入门',
          items: [
            { text: '简介', link: '/guide/' },
            { text: '快速开始', link: '/guide/quick-start' },
            { text: '安装', link: '/guide/installation' }
          ]
        },
        {
          text: '基础',
          items: [
            { text: '基本用法', link: '/guide/basic-usage' },
            { text: '配置源选择', link: '/guide/source-selection' },
            { text: '配置合并', link: '/guide/config-merge' },
            { text: '动态重载', link: '/guide/dynamic-reload' }
          ]
        },
        {
          text: '进阶',
          items: [
            { text: '依赖注入', link: '/guide/dependency-injection' },
            { text: '加密脱敏', link: '/guide/encryption-masking' },
            { text: '编码处理', link: '/guide/encoding' },
            { text: '性能优化', link: '/guide/performance' },
            { text: '最佳实践', link: '/guide/best-practices' },
            { text: '扩展开发', link: '/guide/extension' }
          ]
        },
        {
          text: '深入',
          items: [
            { text: '架构设计', link: '/guide/architecture' },
            { text: '加密脱敏设计', link: '/guide/encryption-masking-design' },
            { text: '编码处理流程', link: '/guide/encoding-workflow' },
            { text: '动态重载设计', link: '/guide/dynamic-reload-design' }
          ]
        }
      ],
      '/config-sources/': [
        {
          text: '本地配置源',
          items: [
            { text: '概述', link: '/config-sources/' },
            { text: 'JSON', link: '/config-sources/json' },
            { text: 'YAML', link: '/config-sources/yaml' },
            { text: 'XML', link: '/config-sources/xml' },
            { text: 'INI', link: '/config-sources/ini' },
            { text: 'TOML', link: '/config-sources/toml' },
            { text: '环境变量', link: '/config-sources/env' }
          ]
        },
        {
          text: '远程配置源',
          items: [
            { text: 'Consul', link: '/config-sources/consul' },
            { text: 'Redis', link: '/config-sources/redis' },
            { text: 'Apollo', link: '/config-sources/apollo' },
            { text: 'Nacos', link: '/config-sources/nacos' },
            { text: 'Vault', link: '/config-sources/vault' },
            { text: 'Etcd', link: '/config-sources/etcd' },
            { text: 'Zookeeper', link: '/config-sources/zookeeper' }
          ]
        }
      ],
      '/api/': [
        {
          text: 'API 参考',
          items: [
            { text: '概述', link: '/api/' },
            { text: 'CfgBuilder', link: '/api/cfg-builder' },
            { text: 'ICfgRoot', link: '/api/icfg-root' },
            { text: 'ICfgSection', link: '/api/icfg-section' },
            { text: '扩展方法', link: '/api/extensions' }
          ]
        }
      ],
      '/examples/': [
        {
          text: '示例',
          items: [
            { text: '概述', link: '/examples/' },
            { text: '基础示例', link: '/examples/basic' },
            { text: '多配置源', link: '/examples/multi-source' },
            { text: '依赖注入', link: '/examples/di-integration' },
            { text: '动态重载', link: '/examples/dynamic-reload' },
            { text: '复杂场景', link: '/examples/complex-scenarios' }
          ]
        }
      ]
    },
    
    socialLinks: [
      {
        icon: {
          svg: '<svg viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg"><path d="M512 1024C229.222 1024 0 794.778 0 512S229.222 0 512 0s512 229.222 512 512-229.222 512-512 512z m259.149-568.883h-290.74a25.293 25.293 0 0 0-25.292 25.293l-0.026 63.206c0 13.952 11.315 25.293 25.267 25.293h177.024c13.978 0 25.293 11.315 25.293 25.267v12.646a75.853 75.853 0 0 1-75.853 75.853h-240.23a25.293 25.293 0 0 1-25.267-25.293V417.203a75.853 75.853 0 0 1 75.827-75.853h353.946a25.293 25.293 0 0 0 25.267-25.292l0.077-63.207a25.293 25.293 0 0 0-25.268-25.293H417.152a189.62 189.62 0 0 0-189.62 189.645V771.15c0 13.977 11.316 25.293 25.294 25.293h372.94a170.65 170.65 0 0 0 170.65-170.65V480.384a25.293 25.293 0 0 0-25.293-25.267z" fill="currentColor"/></svg>'
        },
        link: 'https://gitee.com/apq/Apq.Cfg'
      }
    ],
    
    footer: {
      message: '基于 MIT 许可发布',
      copyright: `Copyright © ${new Date().getFullYear()} Apq.Cfg`
    },
    
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
              navigateText: '切换'
            }
          }
        }
      }
    },
    
    outline: {
      label: '页面导航',
      level: [2, 3]
    },
    
    docFooter: {
      prev: '上一页',
      next: '下一页'
    },
    
    // 禁用最后更新时间（Vercel/CloudBase 部署时无法获取 git 信息）
    // lastUpdated: {
    //   text: '最后更新于',
    //   formatOptions: {
    //     dateStyle: 'short',
    //     timeStyle: 'short'
    //   }
    // },
    
    editLink: {
      pattern: 'https://gitee.com/apq/Apq.Cfg/edit/main/docs/site/:path',
      text: '在 Gitee 上编辑此页'
    }
  },
  
  markdown: {
    lineNumbers: true
  },
  
  lastUpdated: false,
  
  // 忽略 localhost 链接（用于部署文档中的本地开发服务器地址）
  ignoreDeadLinks: [
    /^http:\/\/localhost/
  ],
  
  // 排除不需要构建的文件
  srcExclude: ['**/DEPLOY.md'],

  // Sitemap 配置（SEO 优化）
  sitemap: {
    hostname: 'https://apq-cfg.gitee.io'
  }
})
