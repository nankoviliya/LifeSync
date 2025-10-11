import { test, expect } from '@playwright/test';

// Run with: npx playwright test tests/e2e/example.spec.ts
// Ensure the dev server is running via the configured webServer (npm start).
// Add --headed to the command to debug the scenario in a browser window.

test.describe('Life Sync React App', () => {
  test('should display the home page', async ({ page }) => {
    await page.goto('/', { waitUntil: 'networkidle' });

    await expect(page).toHaveTitle(/life sync/i);

    const logoButton = page.getByRole('button', { name: /life sync/i });
    await expect(logoButton).toBeVisible();
  });
});
