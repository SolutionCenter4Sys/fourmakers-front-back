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
    // baseURL pode ser sobrescrito via env var (PLAYWRIGHT_BASE_URL)
    // - Demo FourMakers oficial → default https://app.fourmakers.io
    // - Demo local com backoffice-rf-hom → setar PLAYWRIGHT_BASE_URL=http://localhost:8080
    baseURL: process.env.PLAYWRIGHT_BASE_URL || 'https://app.fourmakers.io',
    viewport: { width: 1280, height: 720 },

    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'on-first-retry',

    actionTimeout: 15_000,
    navigationTimeout: 15_000,

    launchOptions: {
      // slowMo torna as ações lentas o bastante pra serem vistas em apresentação ao vivo
      // (só aplica em modo --headed; headless ignora).
      slowMo: process.env.PLAYWRIGHT_SLOWMO ? Number(process.env.PLAYWRIGHT_SLOWMO) : 0,
      args: [
        '--disable-dev-shm-usage',
        '--disable-background-timer-throttling',
        '--disable-renderer-backgrounding',
        '--start-maximized',
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
