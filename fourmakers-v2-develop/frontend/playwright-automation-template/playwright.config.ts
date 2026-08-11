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
    [
      'html',
      {
        outputFolder:
          process.env.PLAYWRIGHT_HTML_OUTPUT_DIR ||
          '../../DEMO/outputs/playwright/reembolso/relatorios/playwright-html',
        open: 'never',
      },
    ],
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

  outputDir:
    process.env.PLAYWRIGHT_OUTPUT_DIR ||
    '../../DEMO/outputs/playwright/reembolso/test-results',

  projects: [
    // Setup: OTP no máximo 1x (ou 0x se E2E_REUSE_SESSION + JWT válido). Ver e2e/docs/OTP_AUTH.md
    {
      name: 'setup',
      testMatch: /_setup[\\/].*\.setup\.ts$/,
    },
    // Testes: herdam a sessão do setup — sem OTP por teste.
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        storageState: 'evidencias/.auth/demo-usuario.json',
      },
      dependencies: ['setup'],
      testIgnore: /_setup[\\/].*\.setup\.ts$/,
    },
    // Demo versionada: specs geradas pelo agente Playwright ficam centralizadas em DEMO/outputs.
    {
      name: 'demo',
      testDir: '../../DEMO/outputs/playwright/reembolso/specs',
      use: {
        ...devices['Desktop Chrome'],
        storageState: 'evidencias/.auth/demo-usuario.json',
      },
      dependencies: ['setup'],
    },
  ],
})
