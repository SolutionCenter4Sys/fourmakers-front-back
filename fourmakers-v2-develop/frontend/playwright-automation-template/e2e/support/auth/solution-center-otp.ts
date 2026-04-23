/**
* OTP Auth Service — Solution Center (FourSys)
*
* Serviço de autenticação via código OTP pronto para uso em automações Playwright.
* Não requer variáveis de ambiente — configuração embutida para o ambiente de homologação.
*
* ─── Fluxo de autenticação ───────────────────────────────────────────────────
*  1. POST /api/Acesso/EnviaTokenAcessoEmail   → dispara e-mail com código OTP
*  2. GET  /api/Acesso/ObtemCodigoAcessoEmailQA → recupera o código via Bearer token (QA)
*  3. POST /api/Acesso/ValidaTokenAcessoEmail   → valida o código e retorna JWT
* ─────────────────────────────────────────────────────────────────────────────
*
* ─── Uso recomendado — API-only ─────────────────────────────────────────────
*
*   const auth = new SolutionCenterAuth(request);
*   const jwt  = await auth.autenticarViaOtp();
*   await request.get('...', { headers: { Authorization: `Bearer ${jwt}` } });
*
* ─── Restrições importantes ───────────────────────────────────────────────────
*  - Nunca rode testes em paralelo com o mesmo e-mail (o Passo 1 invalida códigos anteriores).
*  - OTP expira em 15 min; após Passo 3 é marcado como usado.
* ─────────────────────────────────────────────────────────────────────────────
*/
import type { APIRequestContext, Page } from '@playwright/test';
const CONFIG = {
  appUrl:      'https://spw.app.foursys.com/backoffice-rf-hom',
  email:       'solutioncenter@foursys.com.br',
  orgId:       8,
  systemToken: 'OHw1WUI0MEl6Y0I3eDgzV3NGWUswcUNpb0c2aTNsRmhQM3FsWWJDaXJ6bWM5OTVLdEI4Qg==',
  pollTimeoutMs:  8_000,
  pollIntervalMs:   500,
} as const;
type EnviaTokenResponse = {
  sucesso: boolean;
  tipoAcesso?: 'Email' | 'SSO' | number;
  mensagem?: string;
  erros?: unknown;
};
type ObtemCodigoResponse = {
  sucesso: boolean;
  codigo?: string;
  mensagem?: string;
};
type ValidaTokenResponse = {
  sucesso: boolean;
  token?: string;
  primeiroAcessoRealizado?: boolean;
  dataAceitePrimeiroAcesso?: string | null;
  usuario?: {
    nomeColaborador: string;
    usuarioId: number;
    email: string;
    orgId: number;
  };
  mensagem?: string;
};
export class SolutionCenterAuth {
  private readonly api: APIRequestContext;
  private readonly baseUrl: string;
  constructor(api: APIRequestContext) {
    this.api = api;
    this.baseUrl = CONFIG.appUrl.replace(/\/$/, '');
  }
  async enviarTokenAcessoEmail(): Promise<EnviaTokenResponse> {
    const response = await this.api.post(
      `${this.baseUrl}/api/Acesso/EnviaTokenAcessoEmail`,
      {
        data: { email: CONFIG.email, orgId: CONFIG.orgId },
        headers: { 'Content-Type': 'application/json' },
      },
    );
    let body: EnviaTokenResponse;
    try {
      body = (await response.json()) as EnviaTokenResponse;
    } catch {
      throw new Error(
        `[EnviaTokenAcessoEmail] Resposta não-JSON. ` +
        `Status: ${response.status()} ${response.statusText()}`,
      );
    }
    if (!response.ok()) {
      throw new Error(
        `[EnviaTokenAcessoEmail] HTTP ${response.status()} ${response.statusText()}\n` +
        `Body: ${JSON.stringify(body)}`,
      );
    }
    const isSso =
      body.tipoAcesso === 'SSO' ||
      (typeof body.tipoAcesso === 'number' && body.tipoAcesso !== 0);
    if (isSso) {
      throw new Error(
        `[EnviaTokenAcessoEmail] Organização usa SSO — fluxo OTP numérico não se aplica.`,
      );
    }
    if (!body.sucesso) {
      throw new Error(
        `[EnviaTokenAcessoEmail] API retornou sucesso=false: ${body.mensagem ?? '(sem mensagem)'}`,
      );
    }
    return body;
  }
  async obterCodigoAcessoQa(): Promise<string> {
    const url =
      `${this.baseUrl}/api/Acesso/ObtemCodigoAcessoEmailQA` +
      `?email=${encodeURIComponent(CONFIG.email)}&orgId=${CONFIG.orgId}`;
    const deadline = Date.now() + CONFIG.pollTimeoutMs;
    let lastError = '';
    while (Date.now() < deadline) {
      const response = await this.api.get(url, {
        headers: { Authorization: `Bearer ${CONFIG.systemToken}` },
      });
      if (response.status() === 401) {
        throw new Error(
          `[ObtemCodigoAcessoEmailQA] HTTP 401 Não autorizado.\n` +
          `Verifique se o systemToken está correto. Body: ${await response.text()}`,
        );
      }
      let body: ObtemCodigoResponse;
      try {
        body = (await response.json()) as ObtemCodigoResponse;
      } catch {
        throw new Error(
          `[ObtemCodigoAcessoEmailQA] Resposta não-JSON. ` +
          `Status: ${response.status()} ${response.statusText()}`,
        );
      }
      if (response.ok() && body.sucesso && body.codigo) {
        return body.codigo;
      }
      lastError =
        `HTTP ${response.status()} | sucesso=${body.sucesso} | ` +
        `mensagem="${body.mensagem ?? ''}"`;
      await new Promise((r) => setTimeout(r, CONFIG.pollIntervalMs));
    }
    throw new Error(
      `[ObtemCodigoAcessoEmailQA] Nenhum código ativo encontrado após ${CONFIG.pollTimeoutMs}ms.\n` +
      `Última resposta: ${lastError}`,
    );
  }
  async validarTokenAcessoEmail(codigo: string): Promise<ValidaTokenResponse> {
    if (!codigo) {
      throw new Error(`[ValidaTokenAcessoEmail] O código não pode ser vazio.`);
    }
    const response = await this.api.post(
      `${this.baseUrl}/api/Acesso/ValidaTokenAcessoEmail`,
      {
        data: { email: CONFIG.email, token: codigo, orgId: CONFIG.orgId },
        headers: { 'Content-Type': 'application/json' },
      },
    );
    let body: ValidaTokenResponse;
    try {
      body = (await response.json()) as ValidaTokenResponse;
    } catch {
      throw new Error(
        `[ValidaTokenAcessoEmail] Resposta não-JSON. ` +
        `Status: ${response.status()} ${response.statusText()}`,
      );
    }
    if (!response.ok()) {
      throw new Error(
        `[ValidaTokenAcessoEmail] HTTP ${response.status()} ${response.statusText()}\n` +
        `Body: ${JSON.stringify(body)}`,
      );
    }
    if (!body.sucesso || !body.token) {
      throw new Error(
        `[ValidaTokenAcessoEmail] Autenticação falhou: ${body.mensagem ?? '(JWT não retornado)'}\n` +
        `Body: ${JSON.stringify(body)}`,
      );
    }
    return body;
  }
  /**
   * Fluxo completo: EnviaTokenAcessoEmail → ObtemCodigoAcessoEmailQA (polling) → ValidaTokenAcessoEmail.
   * Retorna o JWT de sessão pronto para uso em `Authorization: Bearer <jwt>`.
   */
  async autenticarViaOtp(): Promise<string> {
    await this.enviarTokenAcessoEmail();
    const codigo = await this.obterCodigoAcessoQa();
    const { token } = await this.validarTokenAcessoEmail(codigo);
    return token!;
  }
  /**
   * (Opcional) Autentica e injeta o JWT no localStorage de uma page Playwright.
   * Para uso API-only, prefira `autenticarViaOtp()`.
   */
  async login(page: Page, appUrl?: string): Promise<void> {
    const jwt = await this.autenticarViaOtp();
    const target = (appUrl ?? CONFIG.appUrl).replace(/\/$/, '');
    await page.goto(target, { waitUntil: 'domcontentloaded', timeout: 20_000 });
    await page.evaluate((t: string) => {
      localStorage.setItem('authToken', t);
      localStorage.setItem('token', t);
      localStorage.setItem('@app:token', t);
    }, jwt);
    await page.reload({ waitUntil: 'domcontentloaded' });
  }
}
export type { EnviaTokenResponse, ObtemCodigoResponse, ValidaTokenResponse };
export function createSolutionCenterAuth(api: APIRequestContext): SolutionCenterAuth {
  return new SolutionCenterAuth(api);
}
