import { test, expect } from '@playwright/test';
import { SolutionCenterAuth } from '../support/auth/solution-center-otp';
test.use({ storageState: { cookies: [], origins: [] } });
test.setTimeout(60_000);
test.describe.configure({ mode: 'serial' });
test.describe('Smoke — SolutionCenterAuth OTP', () => {
  test('Passo 1: EnviaTokenAcessoEmail retorna sucesso', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    const resultado = await auth.enviarTokenAcessoEmail();
    console.info('[Smoke] Passo 1 →', JSON.stringify(resultado));
    expect(resultado.sucesso).toBe(true);
  });
  test('Passo 2: ObtemCodigoAcessoEmailQA retorna 6 dígitos', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    await auth.enviarTokenAcessoEmail();
    const codigo = await auth.obterCodigoAcessoQa();
    console.info(`[Smoke] Passo 2 → código: ${codigo}`);
    expect(codigo).toMatch(/^\d{6}$/);
  });
  test('Passo 3: ValidaTokenAcessoEmail retorna JWT', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    await auth.enviarTokenAcessoEmail();
    const codigo = await auth.obterCodigoAcessoQa();
    const resultado = await auth.validarTokenAcessoEmail(codigo);
    console.info('[Smoke] Passo 3 → usuário:', resultado.usuario?.nomeColaborador);
    expect(resultado.sucesso).toBe(true);
    expect(resultado.token).toMatch(/^eyJ/);
    expect(resultado.usuario?.email).toBe('solutioncenter@foursys.com.br');
  });
  test('Fluxo completo: JWT é válido e usável em Bearer', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    const jwt  = await auth.autenticarViaOtp();
    expect(jwt).toMatch(/^eyJ/);
    expect(jwt.split('.')).toHaveLength(3);
    const [, payloadB64] = jwt.split('.');
    const base64 = payloadB64.replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(Buffer.from(base64, 'base64').toString('utf8')) as Record<string, unknown>;
    console.info('[Smoke] JWT payload:', JSON.stringify(payload, null, 2));
    expect(payload.Email).toBe('solutioncenter@foursys.com.br');
    expect(payload.OrgId).toBe('8');
    expect((payload.exp as number) * 1000).toBeGreaterThan(Date.now());
    const response = await request.post(
      'https://spw.app.foursys.com/backoffice-rf-hom/api/Acesso/EnviaTokenAcessoEmail',
      {
        headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${jwt}` },
        data:    { email: 'solutioncenter@foursys.com.br', orgId: 8 },
      },
    );
    console.info(`[Smoke] Chamada com Bearer JWT → HTTP ${response.status()}`);
    expect(response.status()).toBeGreaterThanOrEqual(200);
    expect(response.status()).toBeLessThan(500);
  });
});
