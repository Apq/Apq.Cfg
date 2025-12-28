import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'Apq.Cfg',
  description: '强大、灵活、高性能的 .NET 配置管理库',
  lang: 'zh-CN',
  
  head: [
    ['link', { rel: 'icon', href: '/logo.svg' }]
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
            { text: '配置合并', link: '/guide/config-merge' },
            { text: '动态重载', link: '/guide/dynamic-reload' }
          ]
        },
        {
          text: '进阶',
          items: [
            { text: '依赖注入', link: '/guide/dependency-injection' },
            { text: '编码处理', link: '/guide/encoding' },
            { text: '编码处理流程', link: '/guide/encoding-workflow' },
            { text: '性能优化', link: '/guide/performance' },
            { text: '最佳实践', link: '/guide/best-practices' },
            { text: '扩展开发', link: '/guide/extension' }
          ]
        },
        {
          text: '深入',
          items: [
            { text: '架构设计', link: '/guide/architecture' }
          ]
        }
      ],
      '/config-sources/': [
        {
          text: '配置源',
          items: [
            { text: '概述', link: '/config-sources/' },
            { text: 'JSON', link: '/config-sources/json' },
            { text: 'YAML', link: '/config-sources/yaml' },
            { text: 'Consul', link: '/config-sources/consul' },
            { text: 'Redis', link: '/config-sources/redis' }
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
            { text: '动态重载', link: '/examples/dynamic-reload' }
          ]
        }
      ]
    },
    
    socialLinks: [
      { icon: 'github', link: 'https://github.com/aspect-ratio/Apq.Cfg' }
    ],
    
    footer: {
      message: '基于 MIT 许可发布',
      copyright: 'Copyright © 2024 Apq.Cfg'
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
    
    lastUpdated: {
      text: '最后更新于',
      formatOptions: {
        dateStyle: 'short',
        timeStyle: 'short'
      }
    },
    
    editLink: {
      pattern: 'https://github.com/aspect-ratio/Apq.Cfg/edit/main/docs/site/:path',
      text: '在 GitHub 上编辑此页'
    }
  },
  
  markdown: {
    lineNumbers: true
  },
  
  lastUpdated: true
})
