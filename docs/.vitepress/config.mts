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
          { text: 'Get Started', link: '/get-started' },
          { 
            text: "Arguments",
            collapsed: true,
            items: [
              { text: 'Argument Definition', link: '/argument-definition' },
              { text: 'Supported Types', link: '/supported-argument-types' },
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
