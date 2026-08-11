/**
* OTP Auth Service — Solution Center (FourSys) — ambiente HML (`backoffice-rf-hom`)
*
* Preferir variáveis de ambiente (OTP_*). `OTP_SYSTEM_TOKEN` só via `.env` / CI secrets
* ou `DEMO/setup/demo-credenciais.json` (gitignored) — nunca hardcode no source.
*
* ─── Fluxo ───────────────────────────────────────────────────────────────────
*  1. POST /api/Acesso/EnviaTokenAcessoEmail   → dispara OTP (só quando necessário)
*  2. GET  /api/Acesso/ObtemCodigoAcessoEmailQA → código via Bearer QA (sem ler e-mail)
*  3. POST /api/Acesso/ValidaTokenAcessoEmail   → JWT
*
* ─── Restrições ───────────────────────────────────────────────────────────────
*  - Nunca paralelo no mesmo e-mail (Passo 1 invalida códigos anteriores).
*  - Nunca reenviar OTP em loop após rate limit.
*  - Preferir storageState / cache JWT (E2E_REUSE_SESSION) em vez de OTP a cada run.
*/
import type { APIRequestContext, Page } from '@playwright/test';
import * as fs from 'node:fs';
import * as path from 'node:path';

/** Fallbacks públicos (sem segredo) — HML Showcase. */
const FALLBACK = {
  appUrl: 'https://spw.app.foursys.com/backoffice-rf-hom',
  email: 'usuario_qa@foursys.com.br',
  orgId: 5,
  pollTimeoutMs: 8_000,
  pollIntervalMs: 500,
} as const;

/** Lê systemToken só de arquivo gitignored — nunca do source. */
function loadSystemTokenFromCredenciais(): string {
  const candidates = [
    path.resolve(__dirname, '../../../../../DEMO/setup/demo-credenciais.json'),
    path.resolve(process.cwd(), '../../DEMO/setup/demo-credenciais.json'),
    path.resolve(process.cwd(), 'DEMO/setup/demo-credenciais.json'),
  ];
  for (const credPath of candidates) {
    try {
      if (!fs.existsSync(credPath)) continue;
      const cred = JSON.parse(fs.readFileSync(credPath, 'utf8')) as {
        systemTokenBase64?: string;
      };
      if (cred.systemTokenBase64) return String(cred.systemTokenBase64);
    } catch {
      /* ignore */
    }
  }
  return '';
}

function loadConfig() {
  const systemToken =
    (process.env.OTP_SYSTEM_TOKEN || '').trim() || loadSystemTokenFromCredenciais();
  return {
    appUrl: (process.env.OTP_API_BASE_URL || FALLBACK.appUrl).replace(/\/$/, ''),
    email: process.env.OTP_EMAIL || FALLBACK.email,
    orgId: Number(process.env.OTP_ORG_ID || FALLBACK.orgId),
    systemToken,
    pollTimeoutMs: Number(process.env.OTP_POLL_TIMEOUT_MS || FALLBACK.pollTimeoutMs),
    pollIntervalMs: Number(process.env.OTP_POLL_INTERVAL_MS || FALLBACK.pollIntervalMs),
  };
}

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
  private readonly cfg: ReturnType<typeof loadConfig>;

  constructor(api: APIRequestContext) {
    this.api = api;
    this.cfg = loadConfig();
  }

  get email(): string {
    return this.cfg.email;
  }

  get orgId(): number {
    return this.cfg.orgId;
  }

  get baseUrl(): string {
    return this.cfg.appUrl;
  }

  async enviarTokenAcessoEmail(): Promise<EnviaTokenResponse> {
    const response = await this.api.post(
      `${this.cfg.appUrl}/api/Acesso/EnviaTokenAcessoEmail`,
      {
        data: { email: this.cfg.email, orgId: this.cfg.orgId },
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
    if (response.status() === 429 || /limite de tentativas/i.test(body.mensagem ?? '')) {
      throw new Error(
        `[EnviaTokenAcessoEmail] RATE LIMIT — não reenviar OTP.\n` +
        `Aguarde a janela do backend ou reutilize JWT/storageState (E2E_REUSE_SESSION).\n` +
        `Body: ${JSON.stringify(body)}`,
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
    if (!this.cfg.systemToken) {
      throw new Error(
        `[ObtemCodigoAcessoEmailQA] OTP_SYSTEM_TOKEN ausente.\n` +
          `Defina no .env / CI secret, ou rode npm run demo:preparar (grava DEMO/setup/demo-credenciais.json).`,
      );
    }
    const url =
      `${this.cfg.appUrl}/api/Acesso/ObtemCodigoAcessoEmailQA` +
      `?email=${encodeURIComponent(this.cfg.email)}&orgId=${this.cfg.orgId}`;
    const deadline = Date.now() + this.cfg.pollTimeoutMs;
    let lastError = '';
    while (Date.now() < deadline) {
      const response = await this.api.get(url, {
        headers: { Authorization: `Bearer ${this.cfg.systemToken}` },
      });
      if (response.status() === 401) {
        throw new Error(
          `[ObtemCodigoAcessoEmailQA] HTTP 401 Não autorizado.\n` +
          `Defina OTP_SYSTEM_TOKEN (secret). Body: ${await response.text()}`,
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
      await new Promise((r) => setTimeout(r, this.cfg.pollIntervalMs));
    }
    throw new Error(
      `[ObtemCodigoAcessoEmailQA] Nenhum código ativo após ${this.cfg.pollTimeoutMs}ms.\n` +
      `Última resposta: ${lastError}`,
    );
  }

  async validarTokenAcessoEmail(codigo: string): Promise<ValidaTokenResponse> {
    if (!codigo) {
      throw new Error(`[ValidaTokenAcessoEmail] O código não pode ser vazio.`);
    }
    const response = await this.api.post(
      `${this.cfg.appUrl}/api/Acesso/ValidaTokenAcessoEmail`,
      {
        data: { email: this.cfg.email, token: codigo, orgId: this.cfg.orgId },
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
   * Fluxo completo: Envia → ObtemCodigoQA → Valida. Sem retry de EnviaToken.
   */
  async autenticarViaOtp(): Promise<string> {
    await this.enviarTokenAcessoEmail();
    const codigo = await this.obterCodigoAcessoQa();
    const { token } = await this.validarTokenAcessoEmail(codigo);
    return token!;
  }

  async login(page: Page, appUrl?: string): Promise<void> {
    const jwt = await this.autenticarViaOtp();
    const target = (appUrl ?? this.cfg.appUrl).replace(/\/$/, '');
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
