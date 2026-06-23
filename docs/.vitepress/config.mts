import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  title: "TypedScripts",
  description: "The cleanest way to run Shell scripts in C#",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Home', link: '/' },
      { text: 'Docs', link: '/get-started' }
    ],

    sidebar: [
      {
        text: 'Documentation',
        items: [
          { text: 'Quick Start', link: '/get-started' },
          {
            text: 'Directives',
            collapsed: false,
            items: [
              { text: 'Overview', link: '/directives/' },
              { text: 'Parameters', link: '/directives/param' },
              { text: 'Class name', link: '/directives/identifier' },
              { text: 'Interpreters', link: '/directives/interpreter' },
            ]
          },
          {
            text: 'Reference',
            collapsed: false,
            items: [
              { text: 'Supported types', link: '/reference/supported-types' },
              { text: 'Diagnostics', link: '/reference/diagnostics' },
            ]
          },
          {
            text: 'Extending',
            collapsed: false,
            items: [
              { text: 'Adding shells', link: '/development/adding-shells' },
            ]
          },
        ]
      }
    ],

    socialLinks: [
      { icon: 'github', link: 'https://github.com/Maxi10022/TypedScripts' }
    ]
  }
})
