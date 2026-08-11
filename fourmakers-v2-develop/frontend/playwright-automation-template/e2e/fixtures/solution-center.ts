import * as fs from 'node:fs';
import * as path from 'node:path';
import { test as base, expect } from '@playwright/test';
import { SolutionCenterAuth } from '../support/auth/solution-center-otp';
import { isJwtUsable } from '../support/auth/session-reuse';

type TestFixtures = Record<string, never>;
type WorkerFixtures = { solutionCenterJwt: string };

const CACHE_FILE = path.resolve(__dirname, '../.bff-token-cache.json');

function readCachedJwt(): string | null {
  if (process.env.BFF_TOKEN && isJwtUsable(process.env.BFF_TOKEN)) {
    return process.env.BFF_TOKEN;
  }
  if (!fs.existsSync(CACHE_FILE)) return null;
  try {
    const data = JSON.parse(fs.readFileSync(CACHE_FILE, 'utf8')) as { token?: string };
    if (data.token && isJwtUsable(data.token)) return data.token;
  } catch {
    /* ignore */
  }
  return null;
}

function writeCachedJwt(token: string): void {
  fs.mkdirSync(path.dirname(CACHE_FILE), { recursive: true });
  fs.writeFileSync(
    CACHE_FILE,
    `${JSON.stringify({ token, mintedAt: new Date().toISOString() }, null, 2)}\n`,
    'utf8',
  );
}

export const test = base.extend<TestFixtures, WorkerFixtures>({
  solutionCenterJwt: [async ({ playwright }, use) => {
    const cached = readCachedJwt();
    if (cached) {
      console.log('[fixture] Reusando JWT cache/BFF_TOKEN — SEM EnviaToken');
      await use(cached);
      return;
    }

    const force = ['1', 'true', 'yes'].includes(
      String(process.env.BFF_FORCE_REFRESH || process.env.E2E_FORCE_AUTH || '').toLowerCase(),
    );
    if (!force && process.env.E2E_REUSE_SESSION !== 'false') {
      // ainda assim mintamos 1x se não há cache — smoke precisa de JWT
    }

    const api = await playwright.request.newContext();
    const auth = new SolutionCenterAuth(api);
    const jwt = await auth.autenticarViaOtp();
    writeCachedJwt(jwt);
    await use(jwt);
    await api.dispose();
  }, { scope: 'worker' }],
});
export { expect };
