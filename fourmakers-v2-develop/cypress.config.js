import { defineConfig } from 'cypress';

export default defineConfig({
  e2e: {
    baseUrl: 'https://app.dev.fourmakers.io',
    supportFile: 'cypress/support/e2e.js',
    specPattern: 'cypress/e2e/**/*.cy.{js,ts}',
    viewportWidth: 1280,
    viewportHeight: 720,
    defaultCommandTimeout: 10000,
    requestTimeout: 15000,
    video: false,
    screenshotOnRunFailure: true,
    chromeWebSecurity: false,
  },
});
