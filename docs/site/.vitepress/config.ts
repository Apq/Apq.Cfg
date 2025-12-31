import { defineConfig } from 'vitepress'

// 中文侧边栏配置
const zhSidebar = {
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
        { text: '环境变量', link: '/config-sources/env' },
        { text: '.env 文件', link: '/config-sources/env#env-文件支持' }
      ]
    },
    {
      text: '数据存储配置源',
      items: [
        { text: 'Redis', link: '/config-sources/redis' },
        { text: 'Database', link: '/config-sources/database' }
      ]
    },
    {
      text: '远程配置中心',
      items: [
        { text: 'Consul', link: '/config-sources/consul' },
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
    },
    {
      text: '自动生成 API (net10.0)',
      collapsed: false,
      items: [
        { text: 'Apq.Cfg', link: '/api/net10.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net10.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net10.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net10.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net10.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net10.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net10.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net10.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net10.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net10.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net10.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net10.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net10.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net10.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net10.0/database/' }
      ]
    },
    {
      text: '自动生成 API (net8.0)',
      collapsed: true,
      items: [
        { text: 'Apq.Cfg', link: '/api/net8.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net8.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net8.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net8.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net8.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net8.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net8.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net8.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net8.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net8.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net8.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net8.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net8.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net8.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net8.0/database/' }
      ]
    },
    {
      text: '自动生成 API (net6.0)',
      collapsed: true,
      items: [
        { text: 'Apq.Cfg', link: '/api/net6.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net6.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net6.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net6.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net6.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net6.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net6.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net6.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net6.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net6.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net6.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net6.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net6.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net6.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net6.0/database/' }
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
}

// 英文侧边栏配置
const enSidebar = {
  '/en/guide/': [
    {
      text: 'Getting Started',
      items: [
        { text: 'Introduction', link: '/en/guide/' },
        { text: 'Quick Start', link: '/en/guide/quick-start' },
        { text: 'Installation', link: '/en/guide/installation' }
      ]
    },
    {
      text: 'Basics',
      items: [
        { text: 'Basic Usage', link: '/en/guide/basic-usage' },
        { text: 'Source Selection', link: '/en/guide/source-selection' },
        { text: 'Config Merge', link: '/en/guide/config-merge' },
        { text: 'Dynamic Reload', link: '/en/guide/dynamic-reload' }
      ]
    },
    {
      text: 'Advanced',
      items: [
        { text: 'Dependency Injection', link: '/en/guide/dependency-injection' },
        { text: 'Encryption & Masking', link: '/en/guide/encryption-masking' },
        { text: 'Encoding', link: '/en/guide/encoding' },
        { text: 'Performance', link: '/en/guide/performance' },
        { text: 'Best Practices', link: '/en/guide/best-practices' },
        { text: 'Extension Development', link: '/en/guide/extension' }
      ]
    },
    {
      text: 'Deep Dive',
      items: [
        { text: 'Architecture', link: '/en/guide/architecture' },
        { text: 'Encryption Design', link: '/en/guide/encryption-masking-design' },
        { text: 'Encoding Workflow', link: '/en/guide/encoding-workflow' },
        { text: 'Reload Design', link: '/en/guide/dynamic-reload-design' }
      ]
    }
  ],
  '/en/config-sources/': [
    {
      text: 'Local Sources',
      items: [
        { text: 'Overview', link: '/en/config-sources/' },
        { text: 'JSON', link: '/en/config-sources/json' },
        { text: 'YAML', link: '/en/config-sources/yaml' },
        { text: 'XML', link: '/en/config-sources/xml' },
        { text: 'INI', link: '/en/config-sources/ini' },
        { text: 'TOML', link: '/en/config-sources/toml' },
        { text: 'Environment Variables', link: '/en/config-sources/env' },
        { text: '.env File', link: '/en/config-sources/env#env-file-support' }
      ]
    },
    {
      text: 'Data Storage Sources',
      items: [
        { text: 'Redis', link: '/en/config-sources/redis' },
        { text: 'Database', link: '/en/config-sources/database' }
      ]
    },
    {
      text: 'Remote Config Centers',
      items: [
        { text: 'Consul', link: '/en/config-sources/consul' },
        { text: 'Apollo', link: '/en/config-sources/apollo' },
        { text: 'Nacos', link: '/en/config-sources/nacos' },
        { text: 'Vault', link: '/en/config-sources/vault' },
        { text: 'Etcd', link: '/en/config-sources/etcd' },
        { text: 'Zookeeper', link: '/en/config-sources/zookeeper' }
      ]
    }
  ],
  '/en/api/': [
    {
      text: 'API Reference',
      items: [
        { text: 'Overview', link: '/en/api/' },
        { text: 'CfgBuilder', link: '/en/api/cfg-builder' },
        { text: 'ICfgRoot', link: '/en/api/icfg-root' },
        { text: 'ICfgSection', link: '/en/api/icfg-section' },
        { text: 'Extensions', link: '/en/api/extensions' }
      ]
    },
    {
      text: 'Auto-generated API (net10.0)',
      collapsed: false,
      items: [
        { text: 'Apq.Cfg', link: '/api/net10.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net10.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net10.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net10.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net10.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net10.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net10.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net10.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net10.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net10.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net10.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net10.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net10.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net10.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net10.0/database/' }
      ]
    },
    {
      text: 'Auto-generated API (net8.0)',
      collapsed: true,
      items: [
        { text: 'Apq.Cfg', link: '/api/net8.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net8.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net8.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net8.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net8.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net8.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net8.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net8.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net8.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net8.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net8.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net8.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net8.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net8.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net8.0/database/' }
      ]
    },
    {
      text: 'Auto-generated API (net6.0)',
      collapsed: true,
      items: [
        { text: 'Apq.Cfg', link: '/api/net6.0/core/' },
        { text: 'Apq.Cfg.Crypto', link: '/api/net6.0/crypto/' },
        { text: 'Apq.Cfg.Ini', link: '/api/net6.0/ini/' },
        { text: 'Apq.Cfg.Xml', link: '/api/net6.0/xml/' },
        { text: 'Apq.Cfg.Yaml', link: '/api/net6.0/yaml/' },
        { text: 'Apq.Cfg.Toml', link: '/api/net6.0/toml/' },
        { text: 'Apq.Cfg.Env', link: '/api/net6.0/env/' },
        { text: 'Apq.Cfg.Redis', link: '/api/net6.0/redis/' },
        { text: 'Apq.Cfg.Consul', link: '/api/net6.0/consul/' },
        { text: 'Apq.Cfg.Etcd', link: '/api/net6.0/etcd/' },
        { text: 'Apq.Cfg.Nacos', link: '/api/net6.0/nacos/' },
        { text: 'Apq.Cfg.Apollo', link: '/api/net6.0/apollo/' },
        { text: 'Apq.Cfg.Zookeeper', link: '/api/net6.0/zookeeper/' },
        { text: 'Apq.Cfg.Vault', link: '/api/net6.0/vault/' },
        { text: 'Apq.Cfg.Database', link: '/api/net6.0/database/' }
      ]
    }
  ],
  '/en/examples/': [
    {
      text: 'Examples',
      items: [
        { text: 'Overview', link: '/en/examples/' },
        { text: 'Basic Examples', link: '/en/examples/basic' },
        { text: 'Multi-Source', link: '/en/examples/multi-source' },
        { text: 'DI Integration', link: '/en/examples/di-integration' },
        { text: 'Dynamic Reload', link: '/en/examples/dynamic-reload' },
        { text: 'Complex Scenarios', link: '/en/examples/complex-scenarios' }
      ]
    }
  ]
}

// Gitee 图标 SVG
const giteeSvg = '<svg viewBox="0 0 1024 1024" xmlns="http://www.w3.org/2000/svg"><path d="M512 1024C229.222 1024 0 794.778 0 512S229.222 0 512 0s512 229.222 512 512-229.222 512-512 512z m259.149-568.883h-290.74a25.293 25.293 0 0 0-25.292 25.293l-0.026 63.206c0 13.952 11.315 25.293 25.267 25.293h177.024c13.978 0 25.293 11.315 25.293 25.267v12.646a75.853 75.853 0 0 1-75.853 75.853h-240.23a25.293 25.293 0 0 1-25.267-25.293V417.203a75.853 75.853 0 0 1 75.827-75.853h353.946a25.293 25.293 0 0 0 25.267-25.292l0.077-63.207a25.293 25.293 0 0 0-25.268-25.293H417.152a189.62 189.62 0 0 0-189.62 189.645V771.15c0 13.977 11.316 25.293 25.294 25.293h372.94a170.65 170.65 0 0 0 170.65-170.65V480.384a25.293 25.293 0 0 0-25.293-25.267z" fill="currentColor"/></svg>'

export default defineConfig({
  title: 'Apq.Cfg',

  head: [
    ['link', { rel: 'icon', href: '/logo.svg' }],
    // SEO meta 标签
    ['meta', { name: 'keywords', content: 'Apq.Cfg, .NET, 配置组件, Configuration, C#, JSON, YAML, Consul, Nacos, Redis, Vault, 热重载, 依赖注入' }],
    ['meta', { name: 'author', content: 'Apq' }],
    // Open Graph 标签（社交媒体分享）
    ['meta', { property: 'og:type', content: 'website' }],
    ['meta', { property: 'og:title', content: 'Apq.Cfg - High-Performance .NET Configuration Library' }],
    ['meta', { property: 'og:description', content: 'A .NET configuration library with multiple sources, hot reload, encryption, and DI integration' }],
    ['meta', { property: 'og:image', content: '/logo.svg' }],
    // Twitter Card 标签
    ['meta', { name: 'twitter:card', content: 'summary' }],
    ['meta', { name: 'twitter:title', content: 'Apq.Cfg - High-Performance .NET Configuration Library' }],
    ['meta', { name: 'twitter:description', content: 'A .NET configuration library with multiple sources, hot reload, encryption, and DI integration' }],
    // Sitemap 链接
    ['link', { rel: 'sitemap', type: 'application/xml', href: '/sitemap.xml' }]
  ],

  // 多语言配置
  locales: {
    root: {
      label: '简体中文',
      lang: 'zh-CN',
      description: '强大、灵活、高性能的 .NET 配置组件库',
      themeConfig: {
        nav: [
          { text: '指南', link: '/guide/', activeMatch: '/guide/' },
          { text: '配置源', link: '/config-sources/', activeMatch: '/config-sources/' },
          { text: 'API', link: '/api/', activeMatch: '/api/' },
          { text: '示例', link: '/examples/', activeMatch: '/examples/' },
          { text: '更新日志', link: '/changelog' }
        ],
        sidebar: zhSidebar,
        outline: {
          label: '页面导航',
          level: [2, 3]
        },
        docFooter: {
          prev: '上一页',
          next: '下一页'
        },
        editLink: {
          pattern: 'https://gitee.com/apq/Apq.Cfg/edit/main/docs/site/:path',
          text: '在 Gitee 上编辑此页'
        },
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
        }
      }
    },
    en: {
      label: 'English',
      lang: 'en-US',
      link: '/en/',
      description: 'Powerful, flexible, high-performance .NET configuration library',
      themeConfig: {
        nav: [
          { text: 'Guide', link: '/en/guide/', activeMatch: '/en/guide/' },
          { text: 'Config Sources', link: '/en/config-sources/', activeMatch: '/en/config-sources/' },
          { text: 'API', link: '/en/api/', activeMatch: '/en/api/' },
          { text: 'Examples', link: '/en/examples/', activeMatch: '/en/examples/' },
          { text: 'Changelog', link: '/en/changelog' }
        ],
        sidebar: enSidebar,
        outline: {
          label: 'On this page',
          level: [2, 3]
        },
        docFooter: {
          prev: 'Previous',
          next: 'Next'
        },
        editLink: {
          pattern: 'https://gitee.com/apq/Apq.Cfg/edit/main/docs/site/:path',
          text: 'Edit this page on Gitee'
        },
        footer: {
          message: 'Released under the MIT License',
          copyright: `Copyright © ${new Date().getFullYear()} Apq.Cfg`
        }
      }
    }
  },

  themeConfig: {
    logo: '/logo.svg',

    socialLinks: [
      {
        icon: { svg: giteeSvg },
        link: 'https://gitee.com/apq/Apq.Cfg'
      }
    ]
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
