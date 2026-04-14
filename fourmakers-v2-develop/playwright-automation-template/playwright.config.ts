import { defineConfig, devices } from '@playwright/test'

/**
 * Configuração do Playwright — Fourmakers Automação E2E
 * Migrado de: cypress.config.ts (cypress-automation-template)
 */
export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  workers: 1,
  retries: 1,

  reporter: [
    ['list'],
    ['html', { outputFolder: 'evidencias/relatorios', open: 'never' }],
  ],

  use: {
    baseURL: 'https://app.fourmakers.io',
    viewport: { width: 1280, height: 720 },

    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'on-first-retry',

    actionTimeout: 10_000,
    navigationTimeout: 10_000,

    launchOptions: {
      args: [
        '--disable-dev-shm-usage',
        '--disable-gpu',
        '--no-sandbox',
        '--disable-background-timer-throttling',
        '--disable-renderer-backgrounding',
      ],
    },
  },

  outputDir: 'evidencias/test-results',

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
})
