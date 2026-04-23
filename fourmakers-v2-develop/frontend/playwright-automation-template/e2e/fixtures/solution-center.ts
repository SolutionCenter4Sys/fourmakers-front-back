import { test as base, expect } from '@playwright/test';
import { SolutionCenterAuth } from '../support/auth/solution-center-otp';
type TestFixtures = Record<string, never>;
type WorkerFixtures = { solutionCenterJwt: string };
export const test = base.extend<TestFixtures, WorkerFixtures>({
  solutionCenterJwt: [async ({ playwright }, use) => {
    const api  = await playwright.request.newContext();
    const auth = new SolutionCenterAuth(api);
    const jwt  = await auth.autenticarViaOtp();
    await use(jwt);
    await api.dispose();
  }, { scope: 'worker' }],
});
export { expect };
