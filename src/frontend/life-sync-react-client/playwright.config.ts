/**
 * Playwright configuration for Life Sync React frontend end-to-end tests.
 *
 * Installation:
 *   npm install --save-dev @playwright/test
 *   npx playwright install --with-deps chromium
 *
 * Running tests:
 *   npm run test:e2e           # headless (default)
 *   npm run test:e2e:headed    # run in headed mode
 *
 *   You can override the base URL by exporting PLAYWRIGHT_BASE_URL
 *   (for example when targeting a deployed preview environment).
 */
import { defineConfig, devices } from '@playwright/test';

const baseURL = process.env.PLAYWRIGHT_BASE_URL ?? 'http://localhost:5173';

export default defineConfig({
  testDir: 'tests/e2e',
  fullyParallel: true,
  retries: process.env.CI ? 2 : 0,
  use: {
    baseURL,
    headless: true,
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'on-first-retry',
  },
  webServer: {
    command: 'npm start',
    url: baseURL,
    reuseExistingServer: !process.env.CI,
    timeout: 120 * 1000,
  },
  projects: [
    {
      name: 'chromium',
      use: devices['Desktop Chrome'],
    },
  ],
});
