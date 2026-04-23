import { defineConfig, devices } from '@playwright/test';
export default defineConfig({
  testDir:        './e2e',
  testMatch:      /debug\/solution-center.*\.spec\.ts/,
  fullyParallel:  false,
  retries:        0,
  workers:        1,
  reporter:       [['html', { outputFolder: 'playwright-report-solution-center' }], ['list']],
  use: {
    headless:          true,
    baseURL:           'https://spw.app.foursys.com/backoffice-rf-hom',
    actionTimeout:     30_000,
    navigationTimeout: 30_000,
    viewport:          { width: 1280, height: 720 },
    trace:             'on-first-retry',
    screenshot:        'only-on-failure',
  },
  projects: [
    {
      name: 'solution-center-chromium',
      use:  { ...devices['Desktop Chrome'] },
    },
  ],
});
