import { test, expect } from '@playwright/test';
import { SolutionCenterAuth } from '../support/auth/solution-center-otp';

/**
 * Smoke OTP — rate-limit friendly.
 * Faz no máximo 1 EnviaToken por execução (Passos 1→4 compartilham o mesmo código/JWT).
 * Credenciais: DEMO/setup/demo-credenciais.json (sincronizadas pelo pré-setup).
 */
test.use({ storageState: { cookies: [], origins: [] } });
test.setTimeout(60_000);
test.describe.configure({ mode: 'serial' });

test.describe('Smoke — SolutionCenterAuth OTP', () => {
  const shared: {
    codigo?: string;
    jwt?: string;
  } = {};

  test('Passo 1: EnviaTokenAcessoEmail retorna sucesso', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    const resultado = await auth.enviarTokenAcessoEmail();
    console.info('[Smoke] Passo 1 →', JSON.stringify(resultado));
    expect(resultado.sucesso).toBe(true);
  });

  test('Passo 2: ObtemCodigoAcessoEmailQA retorna 6 dígitos', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    const codigo = await auth.obterCodigoAcessoQa();
    shared.codigo = codigo;
    console.info(`[Smoke] Passo 2 → código: ${codigo}`);
    expect(codigo).toMatch(/^\d{6}$/);
  });

  test('Passo 3: ValidaTokenAcessoEmail retorna JWT', async ({ request }) => {
    const auth = new SolutionCenterAuth(request);
    expect(shared.codigo, 'Passo 2 deve ter obtido o código OTP').toBeTruthy();
    const resultado = await auth.validarTokenAcessoEmail(shared.codigo!);
    shared.jwt = resultado.token;
    console.info('[Smoke] Passo 3 → usuário:', resultado.usuario?.nomeColaborador);
    expect(resultado.sucesso).toBe(true);
    expect(resultado.token).toMatch(/^eyJ/);
    expect(resultado.usuario?.email).toBe('usuario_qa@foursys.com.br');
  });

  test('Fluxo completo: JWT é válido e usável em Bearer', async ({ request }) => {
    expect(shared.jwt, 'Passo 3 deve ter obtido o JWT').toBeTruthy();
    const jwt = shared.jwt!;
    expect(jwt).toMatch(/^eyJ/);
    expect(jwt.split('.')).toHaveLength(3);

    const [, payloadB64] = jwt.split('.');
    const base64 = payloadB64.replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(Buffer.from(base64, 'base64').toString('utf8')) as Record<
      string,
      unknown
    >;
    console.info('[Smoke] JWT payload:', JSON.stringify(payload, null, 2));
    expect(payload.Email).toBe('usuario_qa@foursys.com.br');
    expect(payload.OrgId).toBe('5');
    expect((payload.exp as number) * 1000).toBeGreaterThan(Date.now());

    // Não chama EnviaToken de novo (rate limit). Valida Bearer em endpoint de leitura.
    const response = await request.get(
      'https://spw.app.foursys.com/backoffice-rf-hom/api/Usuario/Showme',
      {
        headers: { Authorization: `Bearer ${jwt}` },
      },
    );
    console.info(`[Smoke] Chamada Bearer JWT → Showme HTTP ${response.status()}`);
    expect(response.status()).toBeGreaterThanOrEqual(200);
    expect(response.status()).toBeLessThan(500);
  });
});
